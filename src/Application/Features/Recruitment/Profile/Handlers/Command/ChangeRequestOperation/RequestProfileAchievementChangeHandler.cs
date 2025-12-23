using System.Text.Json;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.Command.ChangeRequestOperation;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.ChangeRequestOperation;

public sealed class RequestProfileAchievementChangeHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService,
    IProfileReviewService reviewService
) : IRequestHandler<RequestProfileAchievementChangeCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IResult<Unit>> Handle(RequestProfileAchievementChangeCommand cmd, CancellationToken ct)
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

        if (profile.Status == UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.NotSubmitted);

        var validationResult = validationService.ValidateAchievements(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var achievementsResult = DeserializeAchievements(cmd.Request.AchievementsJson);
        if (achievementsResult.IsFailed)
            return Result.Fail<Unit>(achievementsResult.Errors);

        var lengthValidation = ValidateTextLengths(achievementsResult.Value);
        if (lengthValidation.IsFailed)
            return Result.Fail<Unit>(lengthValidation.Errors);

        if (achievementsResult.Value.Any(a => a.Id.HasValue))
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var achievementFiles = cmd.Request.AchievementFiles;

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

            var pending = new PendingAchievementSnapshot
            {
                AchievementTypeId = dto.AchievementTypeId,
                Title = dto.Title,
                IssuingAuthority = dto.IssuingAuthority,
                CountryId = dto.CountryId,
                IssueDate = dto.IssueDate,
                Description = dto.Description,
                RelatedToSpecialization = dto.RelatedToSpecialization,
                AttachmentResourceId = certResult.Value ?? dto.AttachmentId
            };

            await reviewService.TouchRowAsync(profile.Id, ProfileSection.CertificatesAndAwards, "Achievement", Guid.NewGuid(), cmd.UserId, ct, null, pending);
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

            var uploadPath = await UserProfileUploadPathFactory.CreateAsync(cmd.UserId, category, file, false, cancellationToken);
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
                if (!string.IsNullOrEmpty(achievement.Description) &&
                    achievement.Description.Length > ProfileLimits.AchievementDescriptionMaxLength)
                {
                    return Result.Fail(ErrorsCodes.AchievementDescriptionTooLong);
                }
            }

            return Result.Ok();
        }
    }
}

file sealed record PendingAchievementSnapshot
{
    public Guid AchievementTypeId { get; init; }
    public string? Title { get; init; }
    public string? IssuingAuthority { get; init; }
    public Guid? CountryId { get; init; }
    public DateOnly? IssueDate { get; init; }
    public string? Description { get; init; }
    public bool? RelatedToSpecialization { get; init; }
    public Guid? AttachmentResourceId { get; init; }
}

