using Application.Recruitment.Features.Profile.DTOs.ReviseOperation;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.RevisionOperation.Save;

public sealed class ProfileBasicAttachmentSaver(IUnitOfWork uow, IMediator mediator)
{
    public async Task<Result<Guid>> SaveOrReplaceAsync(
        UserProfile profile,
        ProfileSection section,
        ReviseProfileAttachmentRequest meta,
        IFormFile? file,
        Guid? currentProfileResourceId,
        string folder,
        CancellationToken ct)
    {
        // meta.Id = "old resource id"
        if (meta.Id == Guid.Empty)
            return Result.Fail<Guid>(ErrorsCodes.InvalidAttachmentId);

        // In RequiresUpdate, must match what's on profile (no tampering)
        if (profile.Status == UserProfileStatus.RequiresUpdate || profile.Status == UserProfileStatus.Submitted)
        {
            if (currentProfileResourceId is null || currentProfileResourceId.Value == Guid.Empty)
                return Result.Fail<Guid>(ErrorsCodes.AttachmentNotFound);

            if (currentProfileResourceId.Value != meta.Id)
                return Result.Fail<Guid>(ErrorsCodes.AttachmentNotEditableInRevision);

            // Must have NeedsCorrection or Solved review item
            var allowed = await IsAllowedByReviewAsync(profile.Id, section, meta.Id, ct);
            if (!allowed)
                return Result.Fail<Guid>(ErrorsCodes.AttachmentNotEditableInRevision);
        }

        // In InCreation: allow upload (or keep existing id if no file)
        // In RequiresUpdate: replacement requires file; if no file, treat as "metadata-only update" if you want.
        if (file is null || file.Length == 0)
        {
            // If you do NOT allow title-only change for basic resources, return error here instead.
            // return Result.Fail<Guid>(ErrorsCodes.InvalidAttachmentFile);

            // Keep same resource id if no upload
            return Result.Ok(meta.Id);
        }

        var uploadPath = await UserProfileUploadPathFactory.CreateAsync(profile.UserId, folder, file, false, ct);

        var uploadResult = await mediator.Send(
            new UploadAttachmentCommand(profile.UserId, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file),
            ct);

        if (uploadResult.IsFailed)
            return Result.Fail<Guid>(uploadResult.Errors);

        var newId = uploadResult.Value.ResourceId;

        return Result.Ok(newId);
    }

    private async Task<bool> IsAllowedByReviewAsync(Guid userProfileId, ProfileSection section, Guid resourceId,
        CancellationToken ct)
    {
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        return await reviewRepo.DbSet
            .AsNoTracking()
            .AnyAsync(r =>
                r.UserProfileId == userProfileId &&
                r.ProfileChangeId == null &&
                !r.IsDeleted &&
                r.Section == section &&
                r.TargetType == ReviewTargetType.Attachment &&
                (r.Status == ReviewStatus.NeedsCorrection || r.Status == ReviewStatus.Rejected ||
                 r.Status == ReviewStatus.Solved) &&
                r.ResourceId == resourceId, ct);
    }
}
