using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;

internal static class ReviewItemSaveHelper
{
    public static async Task CreateSolvedRowAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        string entityName,
        Guid entityId,
        CancellationToken ct)
    {
        var item = ReviewItem.Create(
            profile.Id,
            section,
            ReviewTargetType.Row,
            entityName: entityName,
            entityId: entityId,
            currentValue: ReviewItemSnapshotBuilder.GetRowSnapshot(profile, section, entityId, new ReviewItem()));

        item.Status = ReviewStatus.Solved;
        item.IsOutdated = false;
        await uow.GetEntityRepository<ReviewItem>().AddAsync(item, ct);
    }

    public static async Task CreateSolvedAttachmentAsync(
        IUnitOfWork uow,
        UserProfile profile,
        string entityName,
        Guid entityId,
        Guid resourceId,
        string title,
        CancellationToken ct)
    {
        var item = ReviewItem.Create(
            profile.Id,
            ProfileSection.Attachments,
            ReviewTargetType.Attachment,
            ProfileReviewConstants.FieldPaths.AdditionalAttachments,
            entityName,
            entityId,
            resourceId,
            new { resourceId });

        item.AttachmentTitle = title;
        item.Status = ReviewStatus.Solved;
        item.IsOutdated = false;
        await uow.GetEntityRepository<ReviewItem>().AddAsync(item, ct);
    }

    public static async Task MarkSectionChangedByAdditionAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        CancellationToken ct)
    {
        var item = await uow.GetEntityRepository<ReviewItem>().DbSet
            .FirstOrDefaultAsync(candidate =>
                candidate.UserProfileId == profile.Id &&
                candidate.ProfileChangeId == null &&
                !candidate.IsDeleted &&
                candidate.Section == section &&
                candidate.TargetType == ReviewTargetType.Section,
                ct);

        var currentValue = ReviewItemSnapshotBuilder.GetSectionSnapshot(profile.User!, profile, section);
        if (item is null)
        {
            item = ReviewItem.Create(profile.Id, section, ReviewTargetType.Section, currentValue: currentValue);
            await uow.GetEntityRepository<ReviewItem>().AddAsync(item, ct);
        }
        else
        {
            item.UpdateHash(currentValue);
        }

        item.Status = ReviewStatus.Solved;
        item.IsOutdated = false;
        item.ReviewerNote = null;
        item.ReviewedById = null;
        item.ReviewedAtUtc = null;
    }

    public static async Task ReopenSectionDataForCorrectionAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        CancellationToken ct)
    {
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var item = await reviewRepo.DbSet.FirstOrDefaultAsync(candidate =>
            candidate.UserProfileId == profile.Id &&
            candidate.Section == section &&
            candidate.TargetType == ReviewTargetType.Field &&
            candidate.FieldPath == ProfileReviewConstants.FieldPaths.SectionData &&
            candidate.ProfileChangeId == null &&
            !candidate.IsDeleted,
            ct);

        if (item is null)
        {
            item = ReviewItem.Create(
                profile.Id,
                section,
                ReviewTargetType.Field,
                ProfileReviewConstants.FieldPaths.SectionData,
                currentValue: ReviewItemSnapshotBuilder.GetSectionDataSnapshot(profile, section));
            await reviewRepo.AddAsync(item, ct);
        }
        else if (item.Status is ReviewStatus.NeedsCorrection or ReviewStatus.Rejected)
            return;

        item.Status = ReviewStatus.NeedsCorrection;
        item.IsOutdated = false;
        item.ReviewedAtUtc = null;
        item.ReviewedById = null;
        item.ReviewerNote = null;
    }

    public static Task MarkSectionDataSolvedAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        CancellationToken ct)
    {
        return MarkCandidateSavedTargetSolvedAsync(
            uow,
            profile,
            section,
            ReviewTargetType.Field,
            item => item.FieldPath == ProfileReviewConstants.FieldPaths.SectionData,
            ct);
    }

    public static async Task MarkSystemAppliedSectionDataSolvedAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        CancellationToken ct)
    {
        if (profile.Status != UserProfileStatus.RequiresUpdate)
            return;

        var item = await uow.GetEntityRepository<ReviewItem>().DbSet
            .FirstOrDefaultAsync(candidate =>
                candidate.UserProfileId == profile.Id &&
                candidate.ProfileChangeId == null &&
                !candidate.IsDeleted &&
                candidate.Section == section &&
                candidate.TargetType == ReviewTargetType.Field &&
                candidate.FieldPath == ProfileReviewConstants.FieldPaths.SectionData,
                ct);

        if (item is null)
            return;

        var previousHash = item.CurrentHash;
        var currentValue = ReviewItemSnapshotBuilder.GetCurrentValue(profile, item);
        item.UpdateHash(currentValue);

        var isSystemGeneratedCorrection =
            item.Status == ReviewStatus.NeedsCorrection &&
            item.ReviewedById is null &&
            item.ReviewedAtUtc is null &&
            string.IsNullOrWhiteSpace(item.ReviewerNote);

        if (isSystemGeneratedCorrection)
        {
            item.Status = ReviewStatus.Solved;
            item.IsOutdated = false;
            return;
        }

        var valueChanged = !string.Equals(previousHash, item.CurrentHash, StringComparison.Ordinal);
        if (!valueChanged)
            return;

        if (item.Status == ReviewStatus.Approved)
            item.Status = ReviewStatus.Solved;

        if (item.Status == ReviewStatus.Solved)
            item.IsOutdated = false;
    }

    private static Task MarkSectionSolvedAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        CancellationToken ct)
    {
        return MarkCandidateSavedTargetSolvedAsync(
            uow,
            profile,
            section,
            ReviewTargetType.Section,
            _ => true,
            ct);
    }

    public static Task MarkRowSolvedAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        Guid? entityId,
        CancellationToken ct)
    {
        if (entityId is null || entityId == Guid.Empty)
            return Task.CompletedTask;

        return MarkCandidateSavedTargetSolvedAsync(
            uow,
            profile,
            section,
            ReviewTargetType.Row,
            item => item.EntityId == entityId,
            ct);
    }

    public static Task MarkAttachmentSolvedAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        Guid? oldResourceId,
        CancellationToken ct)
    {
        if (oldResourceId is null || oldResourceId == Guid.Empty)
            return Task.CompletedTask;

        return MarkCandidateSavedTargetSolvedAsync(
            uow,
            profile,
            section,
            ReviewTargetType.Attachment,
            item => item.ResourceId == oldResourceId,
            ct);
    }

    public static async Task UpdateSectionStatusAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        CancellationToken ct)
    {
        await MarkSectionSolvedAsync(uow, profile, section, ct);
    }

    private static async Task MarkCandidateSavedTargetSolvedAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        ReviewTargetType targetType,
        Func<ReviewItem, bool> match,
        CancellationToken ct)
    {
        if (profile.Status != UserProfileStatus.RequiresUpdate)
            return;

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var candidates = await reviewRepo.DbSet
            .Where(item =>
                item.UserProfileId == profile.Id &&
                item.ProfileChangeId == null &&
                !item.IsDeleted &&
                item.Section == section &&
                item.TargetType == targetType &&
                (item.Status == ReviewStatus.NeedsCorrection ||
                 item.Status == ReviewStatus.Rejected ||
                 item.Status == ReviewStatus.Solved))
            .ToListAsync(ct);

        var item = candidates.FirstOrDefault(match);
        if (item is null)
            return;

        var currentValue = ReviewItemSnapshotBuilder.GetCurrentValue(profile, item);
        item.UpdateHash(currentValue);

        if (item.TargetType == ReviewTargetType.Attachment)
        {
            item.ResourceId = ReviewItemSnapshotBuilder.GetAttachmentResourceId(profile, item);
        }

        item.Status = ReviewStatus.Solved;
        item.IsOutdated = false;
    }
}
