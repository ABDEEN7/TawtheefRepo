using System.Text.Json;
using Application.Recruitment.Features.Profile.Command.ChangeRequestOperation;
using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.ChangeRequestOperation;

public sealed class RequestProfileAttachmentsChangeHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService,
    IProfileReviewService reviewService
) : ICommandHandler<RequestProfileAttachmentsChangeCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IResult<Unit>> Handle(RequestProfileAttachmentsChangeCommand cmd, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, ct: ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status == UserProfileStatus.InCreation)
            return Result.Fail<Unit>(ErrorsCodes.NotSubmitted);

        var validationResult = validationService.ValidateAttachments(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var attachmentsResult = Deserialize(cmd.Request.AttachmentsJson);
        if (attachmentsResult.IsFailed)
            return Result.Fail<Unit>(attachmentsResult.Errors);

        var attachments = attachmentsResult.Value;
        var files = cmd.Request.AttachmentFiles;

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

            var resource = uploadResult.Value;
            if (resource is null && dto.AttachmentId is null)
                return Result.Fail<Unit>(ErrorsCodes.InvalidAttachmentFile);

            if (dto.Id.HasValue)
                return Result.Fail<Unit>(ErrorsCodes.CanNotModifiedApprovedDocument);

            var pending = new PendingAttachmentSnapshot
            {
                AttachmentResourceId = resource?.ResourceId ?? dto.AttachmentId,
                Title = dto.Title,
                FileName = resource?.ResourceName ?? dto.FileName
            };

            await reviewService.TouchRowAsync(
                profile.Id,
                ProfileSection.Attachments,
                ProfileReviewConstants.EntityNames.Attachment,
                Guid.NewGuid(),
                cmd.UserId,
                ct,
                null,
                pending);
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

            var uploadPath = await UserProfileUploadPathFactory.CreateAsync(cmd.UserId, ProfileFileCategories.Additional, file, false, cancellationToken);
            var uploadResult = await mediator.SendCommandAsync<UploadAttachmentCommand, IResult<UploadAttachmentRequest>>(
                new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
                cancellationToken);
            if (uploadResult.IsFailed)
                return Result.Fail<UploadAttachmentRequest?>(uploadResult.Errors);

            return Result.Ok<UploadAttachmentRequest?>(uploadResult.Value);
        }
    }
}

file sealed record PendingAttachmentSnapshot
{
    public Guid? AttachmentResourceId { get; init; }
    public string? Title { get; init; }
    public string? FileName { get; init; }
}
