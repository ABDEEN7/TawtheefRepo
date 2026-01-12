using System.Text.Json;
using Application.Recruitment.Features.Profile.Command.RevisionOperation;
using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
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
) : ICommandHandler<ReviseProfileAttachmentsCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IResult<Unit>> Handle(ReviseProfileAttachmentsCommand cmd, CancellationToken ct)
    {
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

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

        // Only attachments with ReviewItem.Status == NeedsCorrection are editable in revision
        var allowedResourceIds = await reviewRepo.DbSet
            .AsNoTracking()
            .Where(r =>
                r.UserProfileId == profile.Id &&
                r.Section == ProfileSection.Attachments &&
                r.TargetType == ReviewTargetType.Attachment &&
                r.Status == ReviewStatus.NeedsCorrection &&
                r.ResourceId != null)
            .Select(r => r.ResourceId!.Value)
            .ToHashSetAsync(ct);

        // Only process items that are allowed; reject any attempt to touch other attachments
        foreach (var dto in incoming)
        {
            if (dto.AttachmentId is null || dto.AttachmentId == Guid.Empty)
                return Result.Fail<Unit>(ErrorsCodes.InvalidAttachmentId);

            if (!allowedResourceIds.Contains(dto.AttachmentId.Value))
                return Result.Fail<Unit>(ErrorsCodes.AttachmentNotEditableInRevision);

            if (!existingByResourceId.TryGetValue(dto.AttachmentId.Value, out var row))
                return Result.Fail<Unit>(ErrorsCodes.AttachmentNotFound);

            // Upload new file only if FileIndex provided (replacement)
            var uploadResult = await UploadIfNeededAsync(
                dto.FileIndex,
                files,
                ErrorsCodes.InvalidAttachmentFileIndex,
                ErrorsCodes.InvalidAttachmentFile,
                ct);

            if (uploadResult.IsFailed)
                return Result.Fail<Unit>(uploadResult.Errors);

            var newResourceId = uploadResult.Value?.ResourceId;

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

                // Mark the corresponding ReviewItem as solved and move it to the new resource id
                await SolveReviewItemAsync(
                    reviewRepo,
                    profile.Id,
                    oldResourceId,
                    newResourceId.Value,
                    ct);
            }
            else
            {
                // No replacement: still mark as solved if user updated allowed fields (e.g., title)
                await SolveReviewItemAsync(
                    reviewRepo,
                    profile.Id,
                    row.AttachmentId,
                    row.AttachmentId,
                    ct);
            }
        }

        await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Attachments, ct);
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

            var uploadResult = await mediator.SendCommandAsync<UploadAttachmentCommand, IResult<UploadAttachmentRequest>>(
                new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
                cancellationToken);

            if (uploadResult.IsFailed)
                return Result.Fail<UploadAttachmentRequest?>(uploadResult.Errors);

            return Result.Ok<UploadAttachmentRequest?>(uploadResult.Value);
        }

        static async Task SolveReviewItemAsync(
            IGenericRepository<ReviewItem> reviewRepo,
            Guid userProfileId,
            Guid oldResourceId,
            Guid newResourceId,
            CancellationToken cancellationToken)
        {
            // Solve the NeedsCorrection item for the old resource id
            var item = await reviewRepo.DbSet
                .FirstOrDefaultAsync(r =>
                        r.UserProfileId == userProfileId &&
                        r.Section == ProfileSection.Attachments &&
                        r.TargetType == ReviewTargetType.Attachment &&
                        r.Status == ReviewStatus.NeedsCorrection &&
                        r.ResourceId == oldResourceId,
                    cancellationToken);

            if (item is null)
                return;

            item.Status = ReviewStatus.Solved;
            item.IsOutdated = false;

            // If file replaced, move linkage to new resource id for audit/trace consistency
            if (newResourceId != Guid.Empty && newResourceId != oldResourceId)
                item.ResourceId = newResourceId;
        }
    }
}
