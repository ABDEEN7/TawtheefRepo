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

public sealed class ReviseProfileExperienceHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService
) : IRequestHandler<ReviseProfileExperienceCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IResult<Unit>> Handle(ReviseProfileExperienceCommand cmd, CancellationToken ct)
    {
        var experienceRepo = uow.GetEntityRepository<Experience>();
        var trainingRepo = uow.GetEntityRepository<TrainingCourse>();
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.RequiresUpdate)
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
        var trainingFiles = cmd.Request.TrainingCourseFiles;

        // Load existing rows from DB (recommended, do not rely on profile navigation state)
        var existingExperiences = await experienceRepo.DbSet
            .Where(x => x.UserProfileId == profile.Id)
            .ToListAsync(ct);

        var existingTrainings = await trainingRepo.DbSet
            .Where(x => x.UserProfileId == profile.Id)
            .ToListAsync(ct);

        var experienceDuplicateValidation =
            ProfileDuplicateValidation.ValidateExperiences(experiences, existingExperiences);
        if (experienceDuplicateValidation.IsFailed)
            return Result.Fail<Unit>(experienceDuplicateValidation.Errors);

        var trainingDuplicateValidation =
            ProfileDuplicateValidation.ValidateTrainingCourses(trainings, existingTrainings);
        if (trainingDuplicateValidation.IsFailed)
            return Result.Fail<Unit>(trainingDuplicateValidation.Errors);

        var allowedExperienceIds = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(r =>
                r.UserProfileId == profile.Id &&
                r.ProfileChangeId == null &&
                !r.IsDeleted &&
                r.Section == ProfileSection.Experience &&
                r.TargetType == ReviewTargetType.Row &&
                (r.Status == ReviewStatus.NeedsCorrection ||
                 r.Status == ReviewStatus.Rejected ||
                 r.Status == ReviewStatus.Solved) &&
                r.EntityId != null)
            .Select(r => r.EntityId!.Value)
            .ToHashSetAsync(ct);

        var allowedTrainingIds = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(r =>
                r.UserProfileId == profile.Id &&
                r.ProfileChangeId == null &&
                !r.IsDeleted &&
                r.Section == ProfileSection.TrainingCourses &&
                r.TargetType == ReviewTargetType.Row &&
                (r.Status == ReviewStatus.NeedsCorrection ||
                 r.Status == ReviewStatus.Rejected ||
                 r.Status == ReviewStatus.Solved) &&
                r.EntityId != null)
            .Select(r => r.EntityId!.Value)
            .ToHashSetAsync(ct);


        // ===== Experiences UPSERT =====
        foreach (var dto in experiences)
        {
            var certResult = await UploadIfNeededAsync(
                cmd.UserId,
                dto.CertificateFileIndex,
                experienceFiles,
                ErrorsCodes.InvalidExperienceFileIndex,
                ErrorsCodes.InvalidExperienceFile,
                ErrorsCodes.ExperienceFileTooLarge,
                ProfileLimits.MaxExperienceFileSizeBytes,
                ProfileFileCategories.Experience,
                ct);

            if (certResult.IsFailed)
                return Result.Fail<Unit>(certResult.Errors);

            var isNew = !dto.Id.HasValue || dto.Id == Guid.Empty;
            if (!isNew && !allowedExperienceIds.Contains(dto.Id!.Value))
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var existing = isNew
                ? null
                : existingExperiences.FirstOrDefault(x => x.Id == dto.Id!.Value);

            var finalCertId = certResult.Value ?? dto.CertificateId; // keep null if no file/no existing
            if (existing is null)
            {
                if (!isNew)
                    return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

                var entity = new Experience
                {
                    Id = Guid.NewGuid(),
                    UserProfileId = profile.Id,
                    EmployerName = dto.EmployerName,
                    JobTitle = dto.JobTitle,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    CountryId = dto.CountryId,
                    Description = dto.Description,
                    QualificationId = dto.QualificationId,
                    CertificateId = finalCertId ?? Guid.Empty
                };
                await experienceRepo.DbSet.AddAsync(entity, ct);
                profile.Experiences ??= [];
                profile.Experiences.Add(entity);
                await ReviewItemSaveHelper.CreateSolvedRowAsync(
                    uow, profile, ProfileSection.Experience,
                    ProfileReviewConstants.EntityNames.Experience, entity.Id, ct);
            }
            else
            {
                existing.EmployerName = dto.EmployerName;
                existing.JobTitle = dto.JobTitle;
                existing.StartDate = dto.StartDate;
                existing.EndDate = dto.EndDate;
                existing.CountryId = dto.CountryId;
                existing.Description = dto.Description;
                existing.QualificationId = dto.QualificationId;

                // Only overwrite certificate if:
                // - new file uploaded OR dto sends a certificateId explicitly
                if (certResult.Value is not null)
                    existing.CertificateId = certResult.Value.Value;
                else if (dto.CertificateId is not null && dto.CertificateId != Guid.Empty)
                    existing.CertificateId = dto.CertificateId.Value;
            }
        }

        // ===== Trainings UPSERT =====
        foreach (var dto in trainings)
        {
            var certResult = await UploadIfNeededAsync(
                cmd.UserId,
                dto.CertificateFileIndex,
                trainingFiles,
                ErrorsCodes.InvalidTrainingCourseFileIndex,
                ErrorsCodes.InvalidTrainingCourseFile,
                ErrorsCodes.TrainingCourseFileTooLarge,
                ProfileLimits.MaxTrainingFileSizeBytes,
                ProfileFileCategories.Training,
                ct);

            if (certResult.IsFailed)
                return Result.Fail<Unit>(certResult.Errors);

            var isNew = !dto.Id.HasValue || dto.Id == Guid.Empty;
            if (!isNew && !allowedTrainingIds.Contains(dto.Id!.Value))
                return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

            var existing = isNew
                ? null
                : existingTrainings.FirstOrDefault(x => x.Id == dto.Id!.Value);

            if (existing is null)
            {
                if (!isNew)
                    return Result.Fail<Unit>(ErrorsCodes.InvalidRequest);

                var entity = new TrainingCourse
                {
                    Id = Guid.NewGuid(),
                    UserProfileId = profile.Id,
                    Title = dto.Title,
                    Provider = dto.Provider,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    CountryId = dto.CountryId,
                    Description = dto.Description,
                    CertificateId = certResult.Value ?? dto.CertificateId ?? Guid.Empty
                };
                await trainingRepo.DbSet.AddAsync(entity, ct);
                profile.TrainingCourses ??= [];
                profile.TrainingCourses.Add(entity);
                await ReviewItemSaveHelper.CreateSolvedRowAsync(
                    uow, profile, ProfileSection.TrainingCourses,
                    ProfileReviewConstants.EntityNames.TrainingCourse, entity.Id, ct);
            }
            else
            {
                existing.Title = dto.Title;
                existing.Provider = dto.Provider;
                existing.StartDate = dto.StartDate;
                existing.EndDate = dto.EndDate;
                existing.CountryId = dto.CountryId;
                existing.Description = dto.Description;

                if (certResult.Value is not null)
                    existing.CertificateId = certResult.Value.Value;
                else if (dto.CertificateId is not null && dto.CertificateId != Guid.Empty)
                    existing.CertificateId = dto.CertificateId.Value;
            }
        }

        foreach (var dto in experiences)
        {
            if (!dto.Id.HasValue || dto.Id == Guid.Empty)
                continue;

            await ReviewItemSaveHelper.MarkRowSolvedAsync(
                uow,
                profile,
                ProfileSection.Experience,
                dto.Id,
                ct);
        }

        foreach (var dto in trainings)
        {
            if (!dto.Id.HasValue || dto.Id == Guid.Empty)
                continue;

            await ReviewItemSaveHelper.MarkRowSolvedAsync(
                uow,
                profile,
                ProfileSection.TrainingCourses,
                dto.Id,
                ct);
        }

        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }


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

    static Result ValidateTextLengths(
        IEnumerable<ExperienceUpsertDto> experiencesToValidate,
        IEnumerable<TrainingCourseUpsertDto> trainingsToValidate)
    {
        if (experiencesToValidate.Any(experience => !string.IsNullOrEmpty(experience.Description) &&
                                                    experience.Description.Length >
                                                    ProfileLimits.ExperienceDescriptionMaxLength))
        {
            return Result.Fail(ErrorsCodes.ExperienceDescriptionTooLong);
        }

        return trainingsToValidate.Any(training => !string.IsNullOrEmpty(training.Description) &&
                                                   training.Description.Length >
                                                   ProfileLimits.TrainingDescriptionMaxLength)
            ? Result.Fail(ErrorsCodes.TrainingDescriptionTooLong)
            : Result.Ok();
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
