using System.Text.Json;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command.SaveOperation;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;

public sealed class SaveProfileAttachmentsHandler(
    IUnitOfWork uow,
    IMediator mediator,
    IProfileStepValidationService validationService
) : ICommandHandler<SaveProfileAttachmentsCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IResult<Unit>> Handle(SaveProfileAttachmentsCommand cmd, CancellationToken ct)
    {
        var attachRepo = uow.GetEntityRepository<ProfileAdditionalAttachment>();

        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status is not UserProfileStatus.InCreation &&
            profile.Status is not UserProfileStatus.RequiresUpdate)
            return Result.Fail<Unit>(ErrorsCodes.ProfileLockedUnderReview);

        var validationResult = validationService.ValidateAttachments(profile);
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        var attachmentsResult = Deserialize(cmd.Request.AttachmentsJson);
        if (attachmentsResult.IsFailed)
            return Result.Fail<Unit>(attachmentsResult.Errors);

        var incoming = attachmentsResult.Value;
        var files = cmd.Request.AttachmentFiles;

        // Ensure non-null collection
        profile.AdditionalAttachments ??= new List<ProfileAdditionalAttachment>();

        // If user removed everything => delete all existing (edit mode clear)
        if (incoming.Count == 0)
        {
            if (profile.AdditionalAttachments.Count > 0)
                attachRepo.DbSet.RemoveRange(profile.AdditionalAttachments);

            await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Attachments, ct);
            await uow.SaveChangesAsync(ct);
            return Result.Ok(Unit.Value);
        }

        // Build lookup of existing by AttachmentId (resource id). This assumes AttachmentId is stable identity.
        // If your row has its own PK Id and AttachmentId is not unique, tell me and I’ll adjust.
        var existingByAttachmentId = profile.AdditionalAttachments
            .Where(x => x.AttachmentId != Guid.Empty)
            .ToDictionary(x => x.AttachmentId);

        // Track incoming ids to know what to delete
        var incomingAttachmentIds = new HashSet<Guid>();

        foreach (var dto in incoming)
        {
            // Upload new file only if FileIndex is provided
            var uploadResult = await UploadIfNeededAsync(
                dto.FileIndex,
                files,
                ErrorsCodes.InvalidAttachmentFileIndex,
                ErrorsCodes.InvalidAttachmentFile,
                ct);

            if (uploadResult.IsFailed)
                return Result.Fail<Unit>(uploadResult.Errors);

            var finalAttachmentId = uploadResult.Value?.ResourceId ?? dto.AttachmentId;

            if (finalAttachmentId is null || finalAttachmentId == Guid.Empty)
                return Result.Fail<Unit>(ErrorsCodes.InvalidAttachmentFile);

            incomingAttachmentIds.Add(finalAttachmentId.Value);

            // Upsert
            if (existingByAttachmentId.TryGetValue(finalAttachmentId.Value, out var row))
            {
                // Update editable fields (e.g., title / file name)
                // Note: your entity uses FileName to store dto.Title
                if (!string.Equals(row.FileName, dto.Title, StringComparison.Ordinal))
                    row.FileName = dto.Title;
            }
            else
            {
                var newRow = new ProfileAdditionalAttachment
                {
                    FileName      = dto.Title,
                    AttachmentId  = finalAttachmentId.Value,
                    UserProfileId = profile.Id
                };

                // Option A (recommended): EF Core supports CT
                await attachRepo.DbSet.AddAsync(newRow, ct);

                // Option B (if you prefer sync add):
                // attachRepo.DbSet.Add(newRow);

                profile.AdditionalAttachments.Add(newRow);
            }
        }

        // Delete removed attachments (existing not in incoming)
        var toRemove = profile.AdditionalAttachments
            .Where(x => !incomingAttachmentIds.Contains(x.AttachmentId))
            .ToList();

        if (toRemove.Count > 0)
            attachRepo.DbSet.RemoveRange(toRemove);

        await ReviewItemSaveHelper.UpdateSectionStatusAsync(uow, profile, ProfileSection.Attachments, ct);
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

            var uploadPath = await UserProfileUploadPathFactory.CreateAsync(
                cmd.UserId, "additional", file, false, cancellationToken);

            var uploadResult = await mediator.SendCommandAsync<UploadAttachmentCommand, IResult<UploadAttachmentRequest>>(
                new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
                cancellationToken);

            if (uploadResult.IsFailed)
                return Result.Fail<UploadAttachmentRequest?>(uploadResult.Errors);

            return Result.Ok<UploadAttachmentRequest?>(uploadResult.Value);
        }
    }
}
