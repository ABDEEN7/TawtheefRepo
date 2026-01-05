using System.Text.Json;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Application.Features.Recruitment.Profile.Validators;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;

public sealed class SaveProfileEducationHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService)
    : ICommandHandler<SaveProfileEducationCommand, IResult<Unit>>
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
        var educationRepo = uow.GetEntityRepository<Qualification>();

        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status is not UserProfileStatus.InCreation && profile.Status is not UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var validationResult = validationService.ValidateEducation(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

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

        var degreesValidation = ValidateDegrees(degrees);
        if (degreesValidation.IsFailed)
            return Result.Fail<Unit>(degreesValidation.Errors);

        var filesValidation = ValidateDegreeFiles(degrees, files);
        if (filesValidation.IsFailed)
            return Result.Fail<Unit>(filesValidation.Errors);

        var newQualifications = new List<Qualification>();
        var updatedQualifications = new List<Qualification>();

        var existingQualifications = await educationRepo.DbSet
            .Include(q => q.Certificate)
            .Where(q => q.UserProfileId == profile.Id)
            .ToListAsync(ct);

        foreach (var dto in degrees)
        {
            var file = ResolveFile(dto, files);
            var existingQualification = dto.Id.HasValue
                ? existingQualifications.FirstOrDefault(q => q.Id == dto.Id.Value)
                : null;

            var hasNewFile = FileValidationHelpers.HasFile(file);
            var hasExistingFile = FileValidationHelpers.HasExisting(dto.ExistingFileName) ||
                                  dto.CertificateId is not null ||
                                  (existingQualification?.CertificateId is not null);

            if (!hasNewFile && !hasExistingFile)
                return Result.Fail<Unit>(ErrorsCodes.DegreeFileRequired);

            var attachmentIdResult = await UploadIfNeededAsync(cmd, file, ct);
            if (attachmentIdResult.IsFailed)
                return Result.Fail<Unit>(attachmentIdResult.Errors);

            var attachmentId = attachmentIdResult.Value ?? existingQualification?.CertificateId ?? dto.CertificateId;

            if (existingQualification is null)
            {
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
            else
            {
                existingQualification.DegreeId       = dto.DegreeId;
                existingQualification.CountryId      = dto.GradCountryId;
                existingQualification.UniversityId   = dto.UniversityId;
                existingQualification.MajorId        = dto.MajorId;
                existingQualification.SubMajorId     = dto.SubMajorId;
                existingQualification.StudyTypeId    = dto.StudyTypeId;
                existingQualification.RatingId       = dto.GradeId;
                existingQualification.GraduationYear = dto.GradYear;
                existingQualification.GPA            = dto.Gpa;
                existingQualification.CertificateId  = attachmentId;
                updatedQualifications.Add(existingQualification);
            }
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

    private static IFormFile? ResolveFile(SaveProfileEducationDegreeDto dto, IReadOnlyList<IFormFile?> files)
    {
        if (dto.FileIndex is null)
            return null;

        return dto.FileIndex.Value >= 0 && dto.FileIndex.Value < files.Count
            ? files[dto.FileIndex.Value]
            : null;
    }

    private static Result ValidateDegreeFiles(
        IReadOnlyList<SaveProfileEducationDegreeDto> degrees,
        IReadOnlyList<IFormFile?> degreeFiles)
    {
        foreach (var degree in degrees)
        {
            if (degree.FileIndex is null)
                continue;

            if (degree.FileIndex.Value < 0 || degree.FileIndex.Value >= degreeFiles.Count)
                return Result.Fail(ErrorsCodes.InvalidDegreeFile);

            var file = degreeFiles[degree.FileIndex.Value];
            if (!FileValidationHelpers.HasFile(file))
                return Result.Fail(ErrorsCodes.InvalidDegreeFile);
        }

        return Result.Ok();
    }

    private async Task<Result<Guid?>> UploadIfNeededAsync(
        SaveProfileEducationCommand cmd,
        IFormFile? file,
        CancellationToken ct)
    {
        if (!FileValidationHelpers.HasFile(file))
            return Result.Ok<Guid?>(null);

        var uploadPath   = await UserProfileUploadPathFactory.CreateAsync(cmd.UserId, "education", file!, false, ct);
        var uploadResult = await mediator.SendCommandAsync<UploadAttachmentCommand, IResult<UploadAttachmentRequest>>(
            new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file!),
            ct);

        if (uploadResult.IsFailed)
            return Result.Fail<Guid?>(uploadResult.Errors);

        return Result.Ok<Guid?>(uploadResult.Value.ResourceId);
    }
}
