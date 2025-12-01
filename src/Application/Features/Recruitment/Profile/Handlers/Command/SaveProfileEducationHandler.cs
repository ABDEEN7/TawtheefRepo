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
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;

public sealed class SaveProfileEducationHandler(IUnitOfWork uow, IMediator mediator, IProfileReviewService reviewService)
    : IRequestHandler<SaveProfileEducationCommand, IResult<Unit>>
{
    // JSON options مرة واحدة بدل ما نعيد إنشائها
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly HashSet<Guid> DegreesWithoutQualificationInfo =
    [
        DegreeIds.Preparatory,
        DegreeIds.Primary,
        DegreeIds.Secondary
    ];

    public async Task<IResult<Unit>> Handle(SaveProfileEducationCommand cmd, CancellationToken ct)
    {
        var profileRepo   = uow.GetEntityRepository<UserProfile>();
        var educationRepo = uow.GetEntityRepository<Qualification>();

        var profile = await profileRepo.DbSet
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        var json = cmd.Request.DegreesJson;
        if (string.IsNullOrWhiteSpace(json))
            return Result.Fail<Unit>(ErrorsCodes.InvalidDegreesJson);

        var deserializeResult = DeserializeDegrees(json);
        if (deserializeResult.IsFailed)
            return Result.Fail<Unit>(deserializeResult.Errors);

        var degrees = deserializeResult.Value;

        var files = cmd.Request.DegreeFiles;
        if (degrees.Count == 0)
            return Result.Fail<Unit>(ErrorsCodes.InvalidDegreesJson);

        if (degrees.Count != files.Count)
            return Result.Fail<Unit>(ErrorsCodes.InvalidDegreesCount);

        var degreesValidation = ValidateDegrees(degrees);
        if (degreesValidation.IsFailed)
            return Result.Fail<Unit>(degreesValidation.Errors);

        var filesValidation = ValidateDegreeFiles(files);
        if (filesValidation.IsFailed)
            return Result.Fail<Unit>(filesValidation.Errors);

        var newQualifications = new List<Qualification>();

        for (var i = 0; i < degrees.Count; i++)
        {
            var dto  = degrees[i];
            var file = files[i];

            var uploadResult = await mediator.Send(new UploadAttachmentCommand(file), ct);
            if (uploadResult.IsFailed)
                return Result.Fail<Unit>(uploadResult.Errors);

            var attachmentId = uploadResult.Value.ResourceId;

            var edu = new Qualification
            {
                UserProfileId  = profile.Id,
                DegreeId       = dto.DegreeId,
                CountryId      = dto.GradCountryId,
                UniversityId   = dto.UniversityId,
                MajorId        = dto.MajorId,
                SubMajorId     = dto.SubMajorId,
                StudyTypeId    = dto.StudyTypeId,
                RatingId       = dto.GradeId,
                GraduationYear = dto.GradYear,
                GPA            = dto.Gpa,
                CertificateId  = attachmentId,
            };

            newQualifications.Add(edu);
            await educationRepo.AddAsync(edu);
        }

        profile.IsDraft = true;

        await reviewService.TouchSectionAsync(profile.Id, Domain.Entities.Recruitment.ProfileSection.Qualifications, ct);
        foreach (var qualification in newQualifications)
        {
            await reviewService.TouchRowAsync(
                profile.Id,
                Domain.Entities.Recruitment.ProfileSection.Qualifications,
                nameof(Qualification),
                qualification.Id,
                ct);
        }

        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }

    // ===== Helpers =====

    private static Result<List<SaveProfileEducationDegreeDto>> DeserializeDegrees(string json)
    {
        try
        {
            var degrees = JsonSerializer.Deserialize<List<SaveProfileEducationDegreeDto>>(json, JsonOptions) 
                          ?? new List<SaveProfileEducationDegreeDto>();

            return Result.Ok(degrees);
        }
        catch (JsonException)
        {
            return Result.Fail<List<SaveProfileEducationDegreeDto>>(ErrorsCodes.InvalidDegreesJson);
        }
    }

    private static Result ValidateDegrees(IReadOnlyList<SaveProfileEducationDegreeDto> degrees)
    {
        const int minYear = 1970;
        var maxYear = DateTime.UtcNow.Year;

        foreach (var d in degrees)
        {
            if (d.DegreeId == Guid.Empty)
                return Result.Fail(ErrorsCodes.InvalidDegreeId);

            if (d.GradCountryId == Guid.Empty)
                return Result.Fail(ErrorsCodes.InvalidDegreeCountryId);

            var skipQualificationDetails = DegreesWithoutQualificationInfo.Contains(d.DegreeId);
            if (skipQualificationDetails)
            {
                // Check if the degree is in the list of degrees that don't require qualification details'
                continue;
            }

            if (!d.UniversityId.HasValue || d.UniversityId == Guid.Empty)
                return Result.Fail(ErrorsCodes.InvalidDegreeUniversityId);

            if (!d.MajorId.HasValue || d.MajorId == Guid.Empty)
                return Result.Fail(ErrorsCodes.InvalidDegreeMajorId);

            if (!d.SubMajorId.HasValue || d.SubMajorId == Guid.Empty)
                return Result.Fail(ErrorsCodes.InvalidDegreeSubMajorId);

            if (!d.StudyTypeId.HasValue || d.StudyTypeId == Guid.Empty)
                return Result.Fail(ErrorsCodes.InvalidDegreeStudyTypeId);

            if (!d.GradeId.HasValue || d.GradeId == Guid.Empty)
                return Result.Fail(ErrorsCodes.InvalidDegreeGradeId);

            if (d.GradYear < minYear || d.GradYear > maxYear)
                return Result.Fail(ErrorsCodes.InvalidGradYear);

            if (d.Gpa is < 0 or > 100)
                return Result.Fail(ErrorsCodes.InvalidGpa);
        }

        return Result.Ok();
    }

    private static Result ValidateDegreeFiles(IReadOnlyList<IFormFile?>? degreeFiles)
    {
        if (degreeFiles is null || degreeFiles.Count == 0 || degreeFiles.Any(file => file is null || file.Length == 0))
            return Result.Fail(ErrorsCodes.InvalidDegreeFile);

        return Result.Ok();
    }
}
