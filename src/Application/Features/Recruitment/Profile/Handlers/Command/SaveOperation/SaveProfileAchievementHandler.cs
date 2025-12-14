using System.Text.Json;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;

public sealed class SaveProfileAchievementHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileReviewService reviewService,
    IProfileStepValidationService validationService
) : IRequestHandler<SaveProfileAchievementCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IResult<Unit>> Handle(SaveProfileAchievementCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var profile = await profileRepo.DbSet
            .Include(p => p.ResidenceAddress)
            .Include(p => p.Qualifications)
            .Include(p => p.Experiences)
            .Include(p => p.TrainingCourses)
            .Include(p => p.Achievements)
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status is UserProfileStatus.Submitted or UserProfileStatus.UnderReview)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var trackChanges = profile.Status == UserProfileStatus.Approved;

        var validationResult = validationService.ValidateAchievements(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var achievementsResult = DeserializeAchievements(cmd.Request.AchievementsJson);
        if (achievementsResult.IsFailed)
            return Result.Fail<Unit>(achievementsResult.Errors);

        var lengthValidation = ValidateTextLengths(achievementsResult.Value);
        if (lengthValidation.IsFailed)
            return Result.Fail<Unit>(lengthValidation.Errors);

        var achievementFiles = cmd.Request.AchievementFiles;

        profile.Achievements ??= [];
        var newAchievements = new List<Achievement>();
        foreach (var dto in achievementsResult.Value)
        {
            var certResult = await UploadIfNeededAsync(
                dto.CertificateFileIndex,
                achievementFiles,
                ErrorsCodes.InvalidAchievementFileIndex,
                ErrorsCodes.InvalidAchievementFile,
                ErrorsCodes.AchievementFileTooLarge,
                ProfileLimits.MaxAchievementFileSizeBytes,
                "achievement",
                ct);

            if (certResult.IsFailed)
                return Result.Fail<Unit>(certResult.Errors);

            var entity = new Achievement
            {
                AchievementTypeId = dto.AchievementTypeId,
                Title = dto.Title,
                IssuingAuthority = dto.IssuingAuthority,
                CountryId = dto.CountryId,
                IssueDate = dto.IssueDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                Description = dto.Description,
                RelatedToSpecialization = dto.RelatedToSpecialization,
                AttachmentId = certResult.Value ?? dto.AttachmentId ?? Guid.Empty,
                UserProfileId = profile.Id
            };

            profile.Achievements.Add(entity);
            newAchievements.Add(entity);
        }

        if (trackChanges)
        {
            await reviewService.TouchSectionAsync(profile.Id, ProfileSection.CertificatesAndAwards, ct);
        }
        foreach (var achievement in newAchievements)
        {
            await reviewService.TouchRowAsync(profile.Id, ProfileSection.CertificatesAndAwards, nameof(Achievement), achievement.Id, ct);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);

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

            var uploadPath   = await UserProfileUploadPathFactory.CreateAsync(cmd.UserId, category, file, false, cancellationToken);
            var uploadResult = await mediator.Send(
                new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
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
}
