using System.Text.Json;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;

public sealed class SaveProfileExperienceHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService
) : IRequestHandler<SaveProfileExperienceCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IResult<Unit>> Handle(SaveProfileExperienceCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status is not UserProfileStatus.InCreation && profile.Status is not UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

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
        var experienceFiles = cmd.Request.ExperienceFiles;
        var trainingFiles   = cmd.Request.TrainingCourseFiles;

        profile.Experiences ??= [];
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

            var entity = new Experience
            {
                EmployerName  = dto.EmployerName,
                JobTitle      = dto.JobTitle,
                StartDate     = dto.StartDate,
                EndDate       = dto.EndDate,
                CountryId     = dto.CountryId,
                CertificateId = certResult.Value ?? dto.CertificateId ?? Guid.Empty,
                UserProfileId = profile.Id,
                Description   = dto.Description,
                QualificationId = dto.QualificationId
            };

            profile.Experiences.Add(entity);
        }

        profile.TrainingCourses ??= [];
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

            var entity = new TrainingCourse
            {
                Title = dto.Title,
                Provider = dto.Provider,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CountryId = dto.CountryId,
                Description = dto.Description,
                CertificateId = certResult.Value ?? dto.CertificateId ?? Guid.Empty,
                UserProfileId = profile.Id
            };

            profile.TrainingCourses.Add(entity);
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

            var uploadPath   = await UserProfileUploadPathFactory.CreateAsync(cmd.UserId, category, file, false, cancellationToken);
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
