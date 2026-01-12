using System.Text.Json;
using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.RevisionOperation.Save;

public sealed class ReviseProfileAchievementHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService
) : ICommandHandler<ReviseProfileAchievementCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
public async Task<IResult<Unit>> Handle(ReviseProfileAchievementCommand cmd, CancellationToken ct)
{
    var achievementRepo = uow.GetEntityRepository<Achievement>();

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

            var row = dto.Id.HasValue && dto.Id.Value != Guid.Empty
                ? existing.FirstOrDefault(x => x.Id == dto.Id.Value)
                : null;

            var finalAttachmentId = certResult.Value ?? dto.AttachmentId;

            if (row is null)
            {
                // INSERT
                var entity = new Achievement
                {
                    UserProfileId = profile.Id,

                    AchievementTypeId        = dto.AchievementTypeId,
                    Title                    = dto.Title,
                    IssuingAuthority         = dto.IssuingAuthority,
                    CountryId                = dto.CountryId,
                    IssueDate                = dto.IssueDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                    Description              = dto.Description,
                    RelatedToSpecialization  = dto.RelatedToSpecialization,
                    AttachmentId             = finalAttachmentId ?? Guid.Empty // or null if your column is nullable
                };

                await achievementRepo.DbSet.AddAsync(entity, ct);
            }
            else
            {
                // UPDATE
                row.AchievementTypeId       = dto.AchievementTypeId;
                row.Title                   = dto.Title;
                row.IssuingAuthority        = dto.IssuingAuthority;
                row.CountryId               = dto.CountryId;
                row.IssueDate               = dto.IssueDate ?? row.IssueDate; // keep existing if not provided
                row.Description             = dto.Description;
                row.RelatedToSpecialization = dto.RelatedToSpecialization;

                // Only overwrite attachment if a new upload happened OR dto sends a new attachment id
                if (certResult.Value is not null)
                    row.AttachmentId = certResult.Value.Value;
                else if (dto.AttachmentId is not null && dto.AttachmentId != Guid.Empty)
                    row.AttachmentId = dto.AttachmentId.Value;
            }
        }

        await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.CertificatesAndAwards, ct);
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

            var uploadPath   = await UserProfileUploadPathFactory.CreateAsync(userId, category, file, false, cancellationToken);
            var uploadResult = await mediator.SendCommandAsync<UploadAttachmentCommand, IResult<UploadAttachmentRequest>>(
                new UploadAttachmentCommand(userId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
                cancellationToken);
            if (uploadResult.IsFailed)
                return Result.Fail<Guid?>(uploadResult.Errors);

            return Result.Ok<Guid?>(uploadResult.Value.ResourceId);
        }

        static Result ValidateTextLengths(IEnumerable<AchievementUpsertDto> achievements)
        {
            foreach (var achievement in achievements)
            {
                if (!string.IsNullOrEmpty(achievement.Description) && achievement.Description.Length > ProfileLimits.AchievementDescriptionMaxLength)
                {
                    return Result.Fail(ErrorsCodes.AchievementDescriptionTooLong);
                }
            }

            return Result.Ok();
        }
}
