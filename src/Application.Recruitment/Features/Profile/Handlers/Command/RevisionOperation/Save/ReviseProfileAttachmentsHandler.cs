using System.Text.Json;
using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.RevisionOperation.Save;

public sealed class ReviseProfileAttachmentsHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService
) : IRequestHandler<ReviseProfileAttachmentsCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IResult<Unit>> Handle(ReviseProfileAttachmentsCommand cmd, CancellationToken ct)
    {
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var attachmentRepo = uow.GetEntityRepository<ProfileAdditionalAttachment>();

        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var validationResult = validationService.ValidateAttachments(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var attachmentsResult = Deserialize(cmd.Request.AttachmentsJson);
        if (attachmentsResult.IsFailed)
            return Result.Fail<Unit>(attachmentsResult.Errors);

        var incoming = attachmentsResult.Value;
        var files = cmd.Request.AttachmentFiles;

        profile.AdditionalAttachments ??= new List<ProfileAdditionalAttachment>();

        // Existing rows keyed by current resource id (AttachmentId)
        var existingByResourceId = profile.AdditionalAttachments
            .Where(x => x.AttachmentId != Guid.Empty)
            .ToDictionary(x => x.AttachmentId);

        // Corrected attachments remain editable until the candidate resubmits the profile.
        var allowedResourceIds = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(r =>
                r.UserProfileId == profile.Id &&
                r.ProfileChangeId == null &&
                !r.IsDeleted &&
                r.Section == ProfileSection.Attachments &&
                r.TargetType == ReviewTargetType.Attachment &&
                (r.Status == ReviewStatus.NeedsCorrection || r.Status == ReviewStatus.Rejected ||
                 r.Status == ReviewStatus.Solved) &&
                r.ResourceId != null)
            .Select(r => r.ResourceId!.Value)
            .ToHashSetAsync(ct);

        // Only process items that are allowed; reject any attempt to touch other attachments
        foreach (var dto in incoming)
        {
            var uploadResult = await UploadIfNeededAsync(
                dto.FileIndex,
                files,
                ErrorsCodes.InvalidAttachmentFileIndex,
                ErrorsCodes.InvalidAttachmentFile,
                ct);

            if (uploadResult.IsFailed)
                return Result.Fail<Unit>(uploadResult.Errors);

            var newResourceId = uploadResult.Value?.ResourceId;
            var isNew = !dto.AttachmentId.HasValue || dto.AttachmentId == Guid.Empty;
            if (isNew)
            {
                if (!newResourceId.HasValue || newResourceId == Guid.Empty)
                    return Result.Fail<Unit>(ErrorsCodes.InvalidAttachmentFile);

                var newRow = new ProfileAdditionalAttachment
                {
                    Id = Guid.NewGuid(),
                    UserProfileId = profile.Id,
                    FileName = dto.Title,
                    AttachmentId = newResourceId.Value
                };
                await attachmentRepo.DbSet.AddAsync(newRow, ct);
                profile.AdditionalAttachments.Add(newRow);
                existingByResourceId.Add(newRow.AttachmentId, newRow);
                await ReviewItemSaveHelper.CreateSolvedAttachmentAsync(
                    uow, profile,
                    ProfileReviewConstants.EntityNames.ProfileAdditionalAttachment,
                    newRow.Id, newRow.AttachmentId, newRow.FileName, ct);
                continue;
            }

            if (!allowedResourceIds.Contains(dto.AttachmentId!.Value))
                return Result.Fail<Unit>(ErrorsCodes.AttachmentNotEditableInRevision);

            if (!existingByResourceId.TryGetValue(dto.AttachmentId.Value, out var row))
                return Result.Fail<Unit>(ErrorsCodes.AttachmentNotFound);

            // Title update allowed only for corrected items
            if (!string.Equals(row.FileName, dto.Title, StringComparison.Ordinal))
                row.FileName = dto.Title;

            // Replace resource if new file uploaded
            if (newResourceId is not null && newResourceId != Guid.Empty && newResourceId != row.AttachmentId)
            {
                var oldResourceId = row.AttachmentId;

                // Prevent duplicate resource id collisions with other rows
                if (existingByResourceId.ContainsKey(newResourceId.Value))
                    return Result.Fail<Unit>(ErrorsCodes.DuplicateAttachmentResource);

                // Update row
                row.AttachmentId = newResourceId.Value;

                // Keep dictionary consistent
                existingByResourceId.Remove(oldResourceId);
                existingByResourceId[row.AttachmentId] = row;

                await ReviewItemSaveHelper.MarkAttachmentSolvedAsync(
                    uow,
                    profile,
                    ProfileSection.Attachments,
                    oldResourceId,
                    ct);
            }
            else
            {
                // No replacement: still mark as solved if user updated allowed fields (e.g., title)
                await ReviewItemSaveHelper.MarkAttachmentSolvedAsync(
                    uow,
                    profile,
                    ProfileSection.Attachments,
                    row.AttachmentId,
                    ct);
            }
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);

        // ----------------- local helpers -----------------

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

            var uploadPath = await UserProfileUploadPathFactory.CreateAsync(
                cmd.UserId, ProfileFileCategories.Additional, file, false, cancellationToken);

            var uploadResult = await mediator.Send(
                new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
                cancellationToken);

            return uploadResult.IsFailed
                ? Result.Fail<UploadAttachmentRequest?>(uploadResult.Errors)
                : Result.Ok<UploadAttachmentRequest?>(uploadResult.Value);
        }
    }
}
