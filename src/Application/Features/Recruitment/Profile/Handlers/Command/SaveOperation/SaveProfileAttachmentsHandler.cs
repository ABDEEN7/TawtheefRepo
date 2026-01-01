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
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;


public sealed class SaveProfileAttachmentsHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService
) : IRequestHandler<SaveProfileAttachmentsCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IResult<Unit>> Handle(SaveProfileAttachmentsCommand cmd, CancellationToken ct)
    {
        var attachRepo  = uow.GetEntityRepository<ProfileAdditionalAttachment>();

        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status is not UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

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
