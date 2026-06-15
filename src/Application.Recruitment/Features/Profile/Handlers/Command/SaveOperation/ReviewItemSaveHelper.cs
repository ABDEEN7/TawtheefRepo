using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.Handlers.Command.SaveOperation;

internal static class ReviewItemSaveHelper
{
    public static Task MarkSectionDataSolvedAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        CancellationToken ct)
    {
        return MarkTargetSolvedAsync(
            uow,
            profile,
            section,
            ReviewTargetType.Field,
            item => item.FieldPath == ProfileReviewConstants.FieldPaths.SectionData,
            ct);
    }

    public static Task MarkSectionSolvedAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        CancellationToken ct)
    {
        return MarkTargetSolvedAsync(
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
        CancellationToken ct,
        bool force = false)
    {
        if (entityId is null || entityId == Guid.Empty)
            return Task.CompletedTask;

        return MarkTargetSolvedAsync(
            uow,
            profile,
            section,
            ReviewTargetType.Row,
            item => item.EntityId == entityId,
            ct,
            force);
    }

    public static Task MarkAttachmentSolvedAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        Guid? oldResourceId,
        CancellationToken ct,
        bool force = false)
    {
        if (oldResourceId is null || oldResourceId == Guid.Empty)
            return Task.CompletedTask;

        return MarkTargetSolvedAsync(
            uow,
            profile,
            section,
            ReviewTargetType.Attachment,
            item => item.ResourceId == oldResourceId,
            ct,
            force);
    }

    public static async Task UpdateSectionStatusAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        CancellationToken ct)
    {
        await MarkSectionSolvedAsync(uow, profile, section, ct);
    }

    private static async Task MarkTargetSolvedAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        ReviewTargetType targetType,
        Func<ReviewItem, bool> match,
        CancellationToken ct,
        bool force = false)
    {
        if (profile.Status != UserProfileStatus.RequiresUpdate && profile.Status != UserProfileStatus.Submitted)
            return;

        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var candidates = await reviewRepo.DbSet
            .Where(item =>
                item.UserProfileId == profile.Id &&
                item.Section == section &&
                item.TargetType == targetType &&
                (item.Status == ReviewStatus.NeedsCorrection || item.Status == ReviewStatus.Solved))
            .ToListAsync(ct);

        var item = candidates.FirstOrDefault(match);
        if (item is null)
            return;

        var previousHash = item.CurrentHash;
        var currentValue = ReviewItemSnapshotBuilder.GetCurrentValue(profile, item);
        item.UpdateHash(currentValue);

        if (item.TargetType == ReviewTargetType.Attachment)
        {
            item.ResourceId = ReviewItemSnapshotBuilder.GetAttachmentResourceId(profile, item);
        }

        var valueChanged = !string.Equals(previousHash, item.CurrentHash, StringComparison.Ordinal);
        if (!force && !valueChanged)
            return;

        item.Status = ReviewStatus.Solved;
        item.IsOutdated = false;
    }
}
