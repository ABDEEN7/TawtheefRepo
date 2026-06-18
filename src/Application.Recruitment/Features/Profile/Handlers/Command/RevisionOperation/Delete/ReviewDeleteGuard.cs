using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.Profile.Handlers.Command.RevisionOperation.Delete;

internal static class ReviewDeleteGuard
{
    public static Task<bool> CanDeleteRowAsync(
        IUnitOfWork uow,
        Guid userProfileId,
        ProfileSection section,
        Guid entityId,
        CancellationToken ct)
    {
        return HasActionableReviewItemAsync(
            uow,
            userProfileId,
            section,
            ReviewTargetType.Row,
            entityId,
            ct);
    }

    public static async Task<bool> CanDeleteRowOrSectionAsync(
        IUnitOfWork uow,
        Guid userProfileId,
        ProfileSection section,
        Guid entityId,
        CancellationToken ct)
    {
        if (await CanDeleteRowAsync(uow, userProfileId, section, entityId, ct))
            return true;

        return await HasActionableSectionReviewItemAsync(uow, userProfileId, section, ct);
    }

    public static Task<bool> CanDeleteAttachmentAsync(
        IUnitOfWork uow,
        Guid userProfileId,
        ProfileSection section,
        Guid resourceId,
        CancellationToken ct)
    {
        return HasActionableReviewItemAsync(
            uow,
            userProfileId,
            section,
            ReviewTargetType.Attachment,
            resourceId,
            ct);
    }

    private static Task<bool> HasActionableReviewItemAsync(
        IUnitOfWork uow,
        Guid userProfileId,
        ProfileSection section,
        ReviewTargetType targetType,
        Guid targetId,
        CancellationToken ct)
    {
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        return reviewRepo.DbSet
            .AsNoTracking()
            .AnyAsync(item =>
                item.UserProfileId == userProfileId &&
                item.Section == section &&
                item.TargetType == targetType &&
                item.ReviewedAtUtc != null &&
                !string.IsNullOrWhiteSpace(item.ReviewerNote) &&
                (item.Status == ReviewStatus.NeedsCorrection || item.Status == ReviewStatus.Rejected || item.Status == ReviewStatus.Solved) &&
                (targetType == ReviewTargetType.Attachment
                    ? item.ResourceId == targetId
                    : item.EntityId == targetId),
                ct);
    }

    private static Task<bool> HasActionableSectionReviewItemAsync(
        IUnitOfWork uow,
        Guid userProfileId,
        ProfileSection section,
        CancellationToken ct)
    {
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();

        return reviewRepo.DbSet
            .AsNoTracking()
            .AnyAsync(item =>
                item.UserProfileId == userProfileId &&
                item.Section == section &&
                item.TargetType == ReviewTargetType.Section &&
                item.ReviewedAtUtc != null &&
                !string.IsNullOrWhiteSpace(item.ReviewerNote) &&
                (item.Status == ReviewStatus.NeedsCorrection || item.Status == ReviewStatus.Rejected || item.Status == ReviewStatus.Solved),
                ct);
    }
}
