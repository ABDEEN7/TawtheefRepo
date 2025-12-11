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


public sealed class SaveProfileAttachmentsHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileReviewService reviewService,
    IProfileStepValidationService validationService
) : IRequestHandler<SaveProfileAttachmentsCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IResult<Unit>> Handle(SaveProfileAttachmentsCommand cmd, CancellationToken ct)
    {
        var profileRepo = uow.GetEntityRepository<UserProfile>();
        var attachRepo  = uow.GetEntityRepository<ProfileAdditionalAttachment>();

        var profile = await profileRepo.DbSet
            .Include(p => p.ResidenceAddress)
            .Include(p => p.Qualifications)
            .Include(p => p.Experiences)
            .Include(p => p.TrainingCourses)
            .Include(p => p.Achievements)
            .Include(p => p.Skills)
            .Include(p => p.Languages)
            .Include(p => p.AdditionalAttachments)
            .FirstOrDefaultAsync(p => p.UserId == cmd.UserId, ct);

        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        var validationResult = validationService.ValidateAttachments(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var attachmentsResult = Deserialize(cmd.Request.AttachmentsJson);
        if (attachmentsResult.IsFailed)
            return Result.Fail<Unit>(attachmentsResult.Errors);

        var attachments = attachmentsResult.Value;
        var files = cmd.Request.AttachmentFiles;

        if (profile.AdditionalAttachments is not null && profile.AdditionalAttachments.Count > 0)
        {
            attachRepo.DbSet.RemoveRange(profile.AdditionalAttachments);
        }

        profile.AdditionalAttachments = [];

        var reviewAttachments = new List<ProfileAdditionalAttachment>();

        foreach (var dto in attachments)
        {
            var uploadResult = await UploadIfNeededAsync(
                dto.FileIndex,
                files,
                ErrorsCodes.InvalidAttachmentFileIndex,
                ErrorsCodes.InvalidAttachmentFile,
                ct);

            if (uploadResult.IsFailed)
                return Result.Fail<Unit>(uploadResult.Errors);

            var attachmentId = uploadResult.Value?.ResourceId ?? dto.AttachmentId;
            if (attachmentId is null || attachmentId == Guid.Empty)
                return Result.Fail<Unit>(ErrorsCodes.InvalidAttachmentFile);

            var fileName = uploadResult.Value?.ResourceName ?? dto.FileName;
            if (string.IsNullOrWhiteSpace(fileName))
                return Result.Fail<Unit>(ErrorsCodes.InvalidAttachmentFile);

            var attachment = new ProfileAdditionalAttachment
            {
                FileName      = fileName,
                AttachmentId  = attachmentId.Value,
                UserProfileId = profile.Id
            };

            profile.AdditionalAttachments.Add(attachment);
            reviewAttachments.Add(attachment);
        }

        await reviewService.TouchSectionAsync(profile.Id, ProfileSection.Attachments, ct);
        foreach (var attachment in reviewAttachments)
        {
            await reviewService.TouchAttachmentAsync(
                profile.Id,
                ProfileSection.Attachments,
                attachment.FileName,
                attachment.AttachmentId,
                ct);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);

        static Result<List<AdditionalAttachmentUpsertDto>> Deserialize(string json)
        {
            try
            {
                var data = JsonSerializer.Deserialize<List<AdditionalAttachmentUpsertDto>>(json, JsonOptions) ?? [];
                return Result.Ok(data);
            }
            catch (JsonException)
            {
                return Result.Fail<List<AdditionalAttachmentUpsertDto>>(ErrorsCodes.InvalidAttachmentsJson);
            }
        }

        async Task<Result<UploadAttachmentRequest?>> UploadIfNeededAsync(
            int? fileIndex,
            IReadOnlyList<IFormFile> resources,
            string invalidIndexError,
            string invalidFileError,
            CancellationToken cancellationToken)
        {
            if (fileIndex is null)
                return Result.Ok<UploadAttachmentRequest?>(null);

            if (fileIndex < 0 || fileIndex >= resources.Count)
                return Result.Fail<UploadAttachmentRequest?>(invalidIndexError);

            var file = resources[fileIndex.Value];
            if (file is not { Length: > 0 })
                return Result.Fail<UploadAttachmentRequest?>(invalidFileError);

            var uploadPath   = await UserProfileUploadPathFactory.CreateAsync(cmd.UserId, "additional", file, false, cancellationToken);
            var uploadResult = await mediator.Send(
                new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
                cancellationToken);
            if (uploadResult.IsFailed)
                return Result.Fail<UploadAttachmentRequest?>(uploadResult.Errors);

            return Result.Ok<UploadAttachmentRequest?>(uploadResult.Value);
        }
    }
}
