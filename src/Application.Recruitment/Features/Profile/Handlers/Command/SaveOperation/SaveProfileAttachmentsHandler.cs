using System.Text.Json;
using Application.Recruitment.Features.Profile.Command.SaveOperation;
using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Common.Validations;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;

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
        var attachRepo = uow.GetEntityRepository<ProfileAdditionalAttachment>();

        var profile = await UserProfileLoader.GetFullProfileByUserId(uow, cmd.UserId, true, ct);
        if (profile is null)
            return Result.Fail<Unit>(ErrorsCodes.UserProfileNotFound);

        if (profile.Status != UserProfileStatus.InCreation)
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

        // Build lookup of existing by AttachmentId (resource id). This assumes AttachmentId is stable identity.
        // If your row has its own PK Id and AttachmentId is not unique, tell me and Iâ€™ll adjust.
        var existingByAttachmentId = profile.AdditionalAttachments
            .Where(x => x.AttachmentId != Guid.Empty)
            .ToDictionary(x => x.AttachmentId);

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

            var hasNewFile = uploadResult.Value is not null;

            // في حالة التعديل: ابحث عن الصف القديم بالـ AttachmentId القادم من الـ DTO
            ProfileAdditionalAttachment? row = null;

            if (dto.AttachmentId is not null && dto.AttachmentId != Guid.Empty)
            {
                existingByAttachmentId.TryGetValue(dto.AttachmentId.Value, out row);
            }

            if (row is not null)
            {
                // تحديث العنوان
                if (!string.Equals(row.FileName, dto.Title, StringComparison.Ordinal))
                    row.FileName = dto.Title;

                // تحديث الملف فقط إذا تم رفع ملف جديد
                if (hasNewFile)
                    row.AttachmentId = uploadResult.Value!.ResourceId;
            }
            else
            {
                // إضافة عنصر جديد
                var finalAttachmentId = uploadResult.Value?.ResourceId ?? dto.AttachmentId;

                if (finalAttachmentId is null || finalAttachmentId == Guid.Empty)
                    return Result.Fail<Unit>(ErrorsCodes.InvalidAttachmentFile);

                var newRow = new ProfileAdditionalAttachment
                {
                    FileName      = dto.Title,
                    AttachmentId  = finalAttachmentId.Value,
                    UserProfileId = profile.Id
                };

                await attachRepo.DbSet.AddAsync(newRow, ct);
                profile.AdditionalAttachments.Add(newRow);
            }
        }

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
                cmd.UserId, ProfileFileCategories.Additional, file, false, cancellationToken);

            var uploadResult = await mediator.Send(
                new UploadAttachmentCommand(cmd.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
                cancellationToken);

            if (uploadResult.IsFailed)
                return Result.Fail<UploadAttachmentRequest?>(uploadResult.Errors);

            return Result.Ok<UploadAttachmentRequest?>(uploadResult.Value);
        }
    }
}


