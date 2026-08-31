using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals;

internal static class FullReviewSectionStateSync
{
    public static async Task SyncAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        Guid officerId,
        DateTime reviewedAtUtc,
        CancellationToken ct)
    {
        if (section is ProfileSection.Skills or ProfileSection.Languages)
            return;

        var items = await uow.GetEntityRepository<ReviewItem>().DbSet
            .Where(item =>
                item.UserProfileId == profile.Id &&
                item.ProfileChangeId == null &&
                !item.IsDeleted &&
                item.Section == section)
            .ToListAsync(ct);

        var activeItems = ActiveProfileReviewItems.ForFullReview(profile, items);
        var sectionItem = activeItems.FirstOrDefault(item => item.TargetType == ReviewTargetType.Section);
        if (sectionItem is null)
            return;

        var children = activeItems
            .Where(item => item.TargetType != ReviewTargetType.Section)
            .ToList();

        // Empty optional collections are approved. A future explicit collection-level
        // correction must use a separate actionable target rather than this aggregate item.
        var status = children.Count == 0
            ? ReviewStatus.Approved
            : children.Any(item =>
                item.Status is ReviewStatus.NeedsCorrection or ReviewStatus.Rejected)
                ? ReviewStatus.NeedsCorrection
                : children.All(item => item.Status == ReviewStatus.Approved)
                    ? ReviewStatus.Approved
                    : ReviewStatus.Pending;

        // Normal section items are aggregate state only; candidate guidance belongs
        // to the actionable child ReviewItems.
        sectionItem.ReviewerNote = null;

        if (sectionItem.Status == status)
            return;

        sectionItem.Status = status;
        sectionItem.ReviewedById = status == ReviewStatus.Pending ? null : officerId;
        sectionItem.ReviewedAtUtc = status == ReviewStatus.Pending ? null : reviewedAtUtc;
        sectionItem.IsOutdated = false;
    }
}
