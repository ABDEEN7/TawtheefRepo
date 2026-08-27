using System.Text.Json;
using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using Application.Recruitment.Features.Profile.Validators;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.RevisionOperation.Save;

public sealed class ReviseProfileAchievementHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService
) : IRequestHandler<ReviseProfileAchievementCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IResult<Unit>> Handle(ReviseProfileAchievementCommand cmd, CancellationToken ct)
    {
        var achievementRepo = uow.GetEntityRepository<Achievement>();
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var validationResult = validationService.ValidateAchievements(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var achievementsResult = DeserializeAchievements(cmd.Request.AchievementsJson);
        if (achievementsResult.IsFailed)
            return Result.Fail<Unit>(achievementsResult.Errors);

        var dtos = achievementsResult.Value;

        var lengthValidation = ValidateTextLengths(dtos);
        if (lengthValidation.IsFailed)
            return Result.Fail<Unit>(lengthValidation.Errors);

        var files = cmd.Request.AchievementFiles;

        // Load existing from DB
        var existing = await achievementRepo.DbSet
            .Where(x => x.UserProfileId == profile.Id)
            .ToListAsync(ct);

        var duplicateValidation = ProfileDuplicateValidation.ValidateAchievements(dtos, existing);
        if (duplicateValidation.IsFailed)
            return Result.Fail<Unit>(duplicateValidation.Errors);

        var allowedEntityIds = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(r =>
                r.UserProfileId == profile.Id &&
                r.ProfileChangeId == null &&
                !r.IsDeleted &&
                r.Section == ProfileSection.CertificatesAndAwards &&
                r.TargetType == ReviewTargetType.Row &&
                (r.Status == ReviewStatus.NeedsCorrection ||
                 r.Status == ReviewStatus.Rejected ||
                 r.Status == ReviewStatus.Solved) &&
                r.EntityId != null)
            .Select(r => r.EntityId!.Value)
            .ToHashSetAsync(ct);

        // Upsert
        foreach (var dto in dtos)
        {
            var certResult = await UploadIfNeededAsync(
                cmd.UserId,
                dto.CertificateFileIndex,
                files,
                ErrorsCodes.InvalidAchievementFileIndex,
                ErrorsCodes.InvalidAchievementFile,
                ErrorsCodes.AchievementFileTooLarge,
                ProfileLimits.MaxAchievementFileSizeBytes,
                ProfileFileCategories.Achievement,
                ct);

            if (certResult.IsFailed)
                return Result.Fail<Unit>(certResult.Errors);

            var isNew = !dto.Id.HasValue || dto.Id == Guid.Empty;
            if (!isNew && !allowedEntityIds.Contains(dto.Id!.Value))
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var row = isNew ? null : existing.FirstOrDefault(x => x.Id == dto.Id!.Value);

            if (row is null)
            {
                if (!isNew)
                    return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

                var entity = new Achievement
                {
                    Id = Guid.NewGuid(),
                    UserProfileId = profile.Id,
                    AchievementTypeId = dto.AchievementTypeId,
                    Title = dto.Title,
                    IssuingAuthority = dto.IssuingAuthority,
                    CountryId = dto.CountryId,
                    IssueDate = dto.IssueDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                    Description = dto.Description,
                    RelatedToSpecialization = dto.RelatedToSpecialization,
                    AttachmentId = certResult.Value ?? dto.AttachmentId ?? Guid.Empty
                };
                await achievementRepo.DbSet.AddAsync(entity, ct);
                profile.Achievements ??= [];
                profile.Achievements.Add(entity);
                await ReviewItemSaveHelper.CreateSolvedRowAsync(
                    uow, profile, ProfileSection.CertificatesAndAwards,
                    ProfileReviewConstants.EntityNames.Achievement, entity.Id, ct);
                continue;
            }


            // UPDATE
            row.AchievementTypeId = dto.AchievementTypeId;
            row.Title = dto.Title;
            row.IssuingAuthority = dto.IssuingAuthority;
            row.CountryId = dto.CountryId;
            row.IssueDate = dto.IssueDate ?? row.IssueDate; // keep existing if not provided
            row.Description = dto.Description;
            row.RelatedToSpecialization = dto.RelatedToSpecialization;

            // Only overwrite attachment if a new upload happened OR dto sends a new attachment id
            if (certResult.Value is not null)
                row.AttachmentId = certResult.Value.Value;
            else if (dto.AttachmentId is not null && dto.AttachmentId != Guid.Empty)
                row.AttachmentId = dto.AttachmentId.Value;
        }

        foreach (var dto in dtos)
        {
            if (!dto.Id.HasValue || dto.Id == Guid.Empty)
                continue;

            await ReviewItemSaveHelper.MarkRowSolvedAsync(
                uow,
                profile,
                ProfileSection.CertificatesAndAwards,
                dto.Id,
                ct);
        }

        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }


    static Result<List<AchievementUpsertDto>> DeserializeAchievements(string json)
    {
        try
        {
            var data = JsonSerializer.Deserialize<List<AchievementUpsertDto>>(json, JsonOptions) ?? [];
            return Result.Ok(data);
        }
        catch (JsonException)
        {
            return Result.Fail<List<AchievementUpsertDto>>(ErrorsCodes.InvalidAchievementsJson);
        }
    }

    async Task<Result<Guid?>> UploadIfNeededAsync(
        Guid userId,
        int? fileIndex,
        IReadOnlyList<IFormFile> files,
        string invalidIndexError,
        string invalidFileError,
        string fileTooLargeError,
        long maxFileSizeBytes,
        string category,
        CancellationToken cancellationToken)
    {
        if (fileIndex is null)
            return Result.Ok<Guid?>(null);

        if (fileIndex < 0 || fileIndex >= files.Count)
            return Result.Fail<Guid?>(invalidIndexError);

        var file = files[fileIndex.Value];
        if (file is not { Length: > 0 })
            return Result.Fail<Guid?>(invalidFileError);

        if (file.Length > maxFileSizeBytes)
            return Result.Fail<Guid?>(fileTooLargeError);

        var uploadPath =
            await UserProfileUploadPathFactory.CreateAsync(userId, category, file, false, cancellationToken);
        var uploadResult = await mediator.Send(
            new UploadAttachmentCommand(userId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
            cancellationToken);
        return uploadResult.IsFailed
            ? Result.Fail<Guid?>(uploadResult.Errors)
            : Result.Ok<Guid?>(uploadResult.Value.ResourceId);
    }

    static Result ValidateTextLengths(IEnumerable<AchievementUpsertDto> achievements)
    {
        return achievements.Any(achievement => !string.IsNullOrEmpty(achievement.Description) &&
                                               achievement.Description.Length >
                                               ProfileLimits.AchievementDescriptionMaxLength)
            ? Result.Fail(ErrorsCodes.AchievementDescriptionTooLong)
            : Result.Ok();
    }
}
