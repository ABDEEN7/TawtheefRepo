using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.SaveOperation;

internal static class ReviewItemSaveHelper
{
    public static async Task UpdateSectionStatusAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        CancellationToken ct)
    {
        if (profile.Status != UserProfileStatus.RequiresUpdate)
            return;

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var items = await reviewRepo.DbSet
            .Where(item => item.UserProfileId == profile.Id && item.Status != ReviewStatus.Solved)
            .ToListAsync(ct);

        if (items.Count == 0)
        {
            var handler = new ResubmitUserProfileHandler(uow);
            await handler.Handle(new ResubmitUserProfileCommand(profile.UserId, new SubmitUserProfileRequest()), ct);
            return;
        }

        foreach (var item in items.Where(item => item.Section == section))
        {
            var previousHash = item.CurrentHash;
            var previousStatus = item.Status;
            var previousOutdated = item.IsOutdated;
            var wasReviewerIssue = item.Status is ReviewStatus.NeedsCorrection;
            var currentValue = GetCurrentValue(profile, item);
            item.UpdateHash(currentValue);

            var valueChanged = previousHash != item.CurrentHash;
            var attachmentReplaced = item.TargetType != ReviewTargetType.Attachment
                || IsAttachmentReplaced(profile, item);

            if (!valueChanged)
            {
                item.Status = previousStatus;
                item.IsOutdated = previousOutdated;
                continue;
            }

            if (wasReviewerIssue && attachmentReplaced)
            {
                item.Status = ReviewStatus.Solved;
                item.IsOutdated = false;
            }
        }

        if (items.All(item => item.Status != ReviewStatus.NeedsCorrection))
        {
            var handler = new ResubmitUserProfileHandler(uow);
            await handler.Handle(new ResubmitUserProfileCommand(profile.UserId, new SubmitUserProfileRequest()), ct);
        }
    }

    private static object? GetCurrentValue(UserProfile profile, ReviewItem item)
    {
        return item.TargetType switch
        {
            ReviewTargetType.Section => ReviewItemSnapshotBuilder.GetSectionSnapshot(profile, item.Section),
            ReviewTargetType.Field => ReviewItemSnapshotBuilder.GetFieldValue(profile, item.FieldPath),
            ReviewTargetType.Row => ReviewItemSnapshotBuilder.GetRowSnapshot(profile, item.Section, item.EntityId, item),
            ReviewTargetType.Attachment => ReviewItemSnapshotBuilder.GetAttachmentSnapshot(profile, item),
            _ => null
        };
    }

    private static bool IsAttachmentReplaced(UserProfile profile, ReviewItem item)
    {
        var currentResourceId = ReviewItemSnapshotBuilder.GetAttachmentResourceId(profile, item);
        if (item.ResourceId is null || currentResourceId is null)
            return false;

        return currentResourceId.Value != item.ResourceId.Value;
    }
}
