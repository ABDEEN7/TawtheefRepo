using System.Text.Json;
using Application.Recruitment.Features.Profile.Command.ChangeRequestOperation;
using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Application.Recruitment.Features.Profile.Validators;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.ChangeRequestOperation;

public sealed class RequestProfileEducationChangeHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService,
    IProfileReviewService reviewService)
    : IRequestHandler<RequestProfileEducationChangeCommand, IResult<Unit>>
{
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

    public async Task<IResult<Unit>> Handle(RequestProfileEducationChangeCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, ct: ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status == UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.NotSubmitted);

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
        if (degrees.Count == 0)
            return Result.Fail<Unit>(ErrorsCodes.InvalidDegreesJson);

        var degreesValidation = ValidateDegrees(degrees);
        if (degreesValidation.IsFailed)
            return Result.Fail<Unit>(degreesValidation.Errors);

        var files = cmd.Request.DegreeFiles;
        var filesValidation = ValidateDegreeFiles(degrees, files);
        if (filesValidation.IsFailed)
            return Result.Fail<Unit>(filesValidation.Errors);

        if (degrees.Any(d => d.Id.HasValue))
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        foreach (var dto in degrees)
        {
            var file = ResolveFile(dto, files);
            var attachmentIdResult = await UploadIfNeededAsync(cmd.UserId, file, ct);
            if (attachmentIdResult.IsFailed)
                return Result.Fail<Unit>(attachmentIdResult.Errors);

            var pending = PendingQualificationSnapshot.From(dto, attachmentIdResult.Value);
            await reviewService.TouchRowAsync(
                profile.Id,
                ProfileSection.Qualifications,
                ProfileReviewConstants.EntityNames.Qualification,
                Guid.NewGuid(),
                cmd.UserId,
                ct,
                null,
                pending);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }

    private static Result<List<SaveProfileEducationDegreeDto>> DeserializeDegrees(string json)
    {
        try
        {
            var degrees = JsonSerializer.Deserialize<List<SaveProfileEducationDegreeDto>>(json, JsonOptions)
                          ?? [];

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

    private async Task<Result<Guid?>> UploadIfNeededAsync(Guid userId, IFormFile? file, CancellationToken ct)
    {
        if (!FileValidationHelpers.HasFile(file))
            return Result.Ok<Guid?>(null);

        var uploadPath = await UserProfileUploadPathFactory.CreateAsync(userId, ProfileFileCategories.Education, file!, false, ct);
        var uploadResult = await mediator.Send(
            new UploadAttachmentCommand(userId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file!),
            ct);

        if (uploadResult.IsFailed)
            return Result.Fail<Guid?>(uploadResult.Errors);

        return Result.Ok<Guid?>(uploadResult.Value.ResourceId);
    }
}

file sealed record PendingQualificationSnapshot
{
    public Guid? DegreeId { get; init; }
    public Guid? GradCountryId { get; init; }
    public Guid? UniversityId { get; init; }
    public Guid? MajorId { get; init; }
    public Guid? SubMajorId { get; init; }
    public Guid? StudyTypeId { get; init; }
    public Guid? GradeId { get; init; }
    public int? GradYear { get; init; }
    public decimal? Gpa { get; init; }
    public Guid? AttachmentResourceId { get; init; }

    public static PendingQualificationSnapshot From(SaveProfileEducationDegreeDto dto, Guid? attachmentResourceId) => new()
    {
        DegreeId = dto.DegreeId,
        GradCountryId = dto.GradCountryId,
        UniversityId = dto.UniversityId,
        MajorId = dto.MajorId,
        SubMajorId = dto.SubMajorId,
        StudyTypeId = dto.StudyTypeId,
        GradeId = dto.GradeId,
        GradYear = dto.GradYear,
        Gpa = dto.Gpa,
        AttachmentResourceId = attachmentResourceId
    };
}


