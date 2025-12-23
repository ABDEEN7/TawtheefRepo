using System.Text.Json;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.Command.ChangeRequestOperation;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.ChangeRequestOperation;

public sealed class RequestProfileExperienceChangeHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService,
    IProfileReviewService reviewService
) : IRequestHandler<RequestProfileExperienceChangeCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IResult<Unit>> Handle(RequestProfileExperienceChangeCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfile(uow, cmd.UserId, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status == UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.NotSubmitted);

        var validationResult = validationService.ValidateExperience(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var experiencesResult = DeserializeExperiences(cmd.Request.ExperiencesJson);
        if (experiencesResult.IsFailed)
            return Result.Fail<Unit>(experiencesResult.Errors);

        var trainingsResult = DeserializeTrainings(cmd.Request.TrainingCoursesJson);
        if (trainingsResult.IsFailed)
            return Result.Fail<Unit>(trainingsResult.Errors);

        var experiences = experiencesResult.Value;
        var trainings = trainingsResult.Value;

        var lengthValidationResult = ValidateTextLengths(experiences, trainings);
        if (lengthValidationResult.IsFailed)
            return Result.Fail<Unit>(lengthValidationResult.Errors);

        var qualificationValidation = ValidateQualifications(experiences, profile.Qualifications ?? []);
        if (qualificationValidation.IsFailed)
            return Result.Fail<Unit>(qualificationValidation.Errors);

        if (experiences.Any(e => e.Id.HasValue) || trainings.Any(t => t.Id.HasValue))
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var experienceFiles = cmd.Request.ExperienceFiles;
        var trainingFiles = cmd.Request.TrainingCourseFiles;

        foreach (var dto in experiences)
        {
            var certResult = await UploadIfNeededAsync(
                dto.CertificateFileIndex,
                experienceFiles,
                ErrorsCodes.InvalidExperienceFileIndex,
                ErrorsCodes.InvalidExperienceFile,
                ErrorsCodes.ExperienceFileTooLarge,
                ProfileLimits.MaxExperienceFileSizeBytes,
                "experience",
                ct);

            if (certResult.IsFailed)
                return Result.Fail<Unit>(certResult.Errors);

            var pending = PendingExperienceSnapshot.From(dto, certResult.Value ?? dto.CertificateId);
            await reviewService.TouchRowAsync(profile.Id, ProfileSection.Experience, "Experience", Guid.NewGuid(), cmd.UserId, ct, null, pending);
        }

        foreach (var dto in trainings)
        {
            var certResult = await UploadIfNeededAsync(
                dto.CertificateFileIndex,
                trainingFiles,
                ErrorsCodes.InvalidTrainingCourseFileIndex,
                ErrorsCodes.InvalidTrainingCourseFile,
                ErrorsCodes.TrainingCourseFileTooLarge,
                ProfileLimits.MaxTrainingFileSizeBytes,
                "training",
                ct);

            if (certResult.IsFailed)
                return Result.Fail<Unit>(certResult.Errors);

            var pending = PendingTrainingSnapshot.From(dto, certResult.Value ?? dto.CertificateId);
            await reviewService.TouchRowAsync(profile.Id, ProfileSection.TrainingCourses, "TrainingCourse", Guid.NewGuid(), cmd.UserId, ct, null, pending);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);

        static Result<List<ExperienceUpsertDto>> DeserializeExperiences(string json)
        {
            try
            {
                var data = JsonSerializer.Deserialize<List<ExperienceUpsertDto>>(json, JsonOptions) ?? [];
                return Result.Ok(data);
            }
            catch (JsonException)
            {
                return Result.Fail<List<ExperienceUpsertDto>>(ErrorsCodes.InvalidExperiencesJson);
            }
        }

        static Result<List<TrainingCourseUpsertDto>> DeserializeTrainings(string json)
        {
            try
            {
                var data = JsonSerializer.Deserialize<List<TrainingCourseUpsertDto>>(json, JsonOptions) ?? [];
                return Result.Ok(data);
            }
            catch (JsonException)
            {
                return Result.Fail<List<TrainingCourseUpsertDto>>(ErrorsCodes.InvalidTrainingCoursesJson);
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

        static Result ValidateTextLengths(
            IEnumerable<ExperienceUpsertDto> experiencesToValidate,
            IEnumerable<TrainingCourseUpsertDto> trainingsToValidate)
        {
            foreach (var experience in experiencesToValidate)
            {
                if (!string.IsNullOrEmpty(experience.Description) &&
                    experience.Description.Length > ProfileLimits.ExperienceDescriptionMaxLength)
                {
                    return Result.Fail(ErrorsCodes.ExperienceDescriptionTooLong);
                }
            }

            foreach (var training in trainingsToValidate)
            {
                if (!string.IsNullOrEmpty(training.Description) &&
                    training.Description.Length > ProfileLimits.TrainingDescriptionMaxLength)
                {
                    return Result.Fail(ErrorsCodes.TrainingDescriptionTooLong);
                }
            }

            return Result.Ok();
        }

        static Result ValidateQualifications(
            IEnumerable<ExperienceUpsertDto> experiencesToValidate,
            IEnumerable<Qualification> qualifications)
        {
            var qualificationLookup = qualifications.ToDictionary(q => q.Id);

            foreach (var experience in experiencesToValidate)
            {
                if (experience.QualificationId is null)
                    continue;

                if (!qualificationLookup.TryGetValue(experience.QualificationId.Value, out var qualification))
                {
                    return Result.Fail(ErrorsCodes.InvalidExperienceQualification);
                }

                if (qualification.GraduationYear is not null && experience.StartDate.Year < qualification.GraduationYear)
                {
                    return Result.Fail(ErrorsCodes.ExperienceBeforeGraduation);
                }
            }

            return Result.Ok();
        }
    }
}

file sealed record PendingExperienceSnapshot
{
    public string? EmployerName { get; init; }
    public string? JobTitle { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public Guid? CountryId { get; init; }
    public Guid? CertificateId { get; init; }
    public string? Description { get; init; }
    public Guid? QualificationId { get; init; }

    public static PendingExperienceSnapshot From(ExperienceUpsertDto dto, Guid? certificateId) => new()
    {
        EmployerName = dto.EmployerName,
        JobTitle = dto.JobTitle,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        CountryId = dto.CountryId,
        CertificateId = certificateId,
        Description = dto.Description,
        QualificationId = dto.QualificationId
    };
}

file sealed record PendingTrainingSnapshot
{
    public string? Title { get; init; }
    public string? Provider { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public Guid? CountryId { get; init; }
    public string? Description { get; init; }
    public Guid? CertificateId { get; init; }

    public static PendingTrainingSnapshot From(TrainingCourseUpsertDto dto, Guid? certificateId) => new()
    {
        Title = dto.Title,
        Provider = dto.Provider,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        CountryId = dto.CountryId,
        Description = dto.Description,
        CertificateId = certificateId
    };
}
