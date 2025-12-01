using System.Text.Json;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;

public sealed class SaveProfileExperienceHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileReviewService reviewService
) : IRequestHandler<SaveProfileExperienceCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IResult<Unit>> Handle(SaveProfileExperienceCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var profile = await profileRepo.DbSet
            .Include(p => p.Experiences)
            .Include(p => p.TrainingCourses)
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        var experiencesResult = DeserializeExperiences(cmd.Request.ExperiencesJson);
        if (experiencesResult.IsFailed)
            return Result.Fail<Unit>(experiencesResult.Errors);

        var trainingsResult = DeserializeTrainings(cmd.Request.TrainingCoursesJson);
        if (trainingsResult.IsFailed)
            return Result.Fail<Unit>(trainingsResult.Errors);

        var experiences = experiencesResult.Value;
        var trainings = trainingsResult.Value;
        var experienceFiles = cmd.Request.ExperienceFiles;
        var trainingFiles   = cmd.Request.TrainingCourseFiles;

        profile.Experiences ??= [];
        var newExperiences = new List<Experience>();
        foreach (var dto in experiences)
        {
            var certResult = await UploadIfNeededAsync(
                dto.CertificateFileIndex,
                experienceFiles,
                ErrorsCodes.InvalidExperienceFileIndex,
                ErrorsCodes.InvalidExperienceFile,
                ct);

            if (certResult.IsFailed)
                return Result.Fail<Unit>(certResult.Errors);

            var entity = new Experience
            {
                Organization  = dto.Organization,
                Position      = dto.Position,
                StartDate     = dto.StartDate,
                EndDate       = dto.EndDate,
                CertificateId = certResult.Value ?? dto.CertificateId ?? Guid.Empty,
                UserProfileId = profile.Id,
                Achievements  = dto.Achievements
            };

            profile.Experiences.Add(entity);
            newExperiences.Add(entity);
        }

        profile.TrainingCourses ??= [];
        var newTrainings = new List<TrainingCourse>();
        foreach (var dto in trainings)
        {
            var certResult = await UploadIfNeededAsync(
                dto.CertificateFileIndex,
                trainingFiles,
                ErrorsCodes.InvalidTrainingCourseFileIndex,
                ErrorsCodes.InvalidTrainingCourseFile,
                ct);

            if (certResult.IsFailed)
                return Result.Fail<Unit>(certResult.Errors);

            var entity = new TrainingCourse
            {
                Organization  = dto.Organization,
                Position      = dto.Position,
                StartDate     = dto.StartDate,
                EndDate       = dto.EndDate,
                CertificateId = certResult.Value ?? dto.CertificateId ?? Guid.Empty,
                UserProfileId = profile.Id
            };

            profile.TrainingCourses.Add(entity);
            newTrainings.Add(entity);
        }

        profile.IsDraft = !cmd.Request.Submit;

        await reviewService.TouchSectionAsync(profile.Id, Domain.Entities.Recruitment.ProfileSection.Experience, ct);
        foreach (var experience in newExperiences)
        {
            await reviewService.TouchRowAsync(profile.Id, Domain.Entities.Recruitment.ProfileSection.Experience, nameof(Experience), experience.Id, ct);
        }

        foreach (var training in newTrainings)
        {
            await reviewService.TouchRowAsync(profile.Id, Domain.Entities.Recruitment.ProfileSection.Experience, nameof(TrainingCourse), training.Id, ct);
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
            CancellationToken cancellationToken)
        {
            if (fileIndex is null)
                return Result.Ok<Guid?>(null);

            if (fileIndex < 0 || fileIndex >= files.Count)
                return Result.Fail<Guid?>(invalidIndexError);

            var file = files[fileIndex.Value];
            if (file is not { Length: > 0 })
                return Result.Fail<Guid?>(invalidFileError);

            var uploadResult = await mediator.Send(new UploadAttachmentCommand(file), cancellationToken);
            if (uploadResult.IsFailed)
                return Result.Fail<Guid?>(uploadResult.Errors);

            return Result.Ok<Guid?>(uploadResult.Value.ResourceId);
        }
    }
}
