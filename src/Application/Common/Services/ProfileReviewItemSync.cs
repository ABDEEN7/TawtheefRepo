using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Utils;

namespace Tawtheef.Application.Common.Services;

public static class ProfileReviewItemSync
{
    public static async Task EnsureConditionalAttachmentItemsAsync(
        IUnitOfWork uow,
        UserProfile profile,
        CancellationToken ct)
    {
        await EnsurePrerequisiteAttachmentItemsAsync(uow, profile, ct);
        await EnsureSponsorAttachmentItemAsync(uow, profile, ct);
        await EnsureNationalAddressAttachmentItemAsync(uow, profile, ct);
    }

    public static async Task EnsurePrerequisiteAttachmentItemsAsync(IUnitOfWork uow, UserProfile profile, CancellationToken ct)
    {
        if (ProfileValidatorUtils.RequiresBirthCertificate(profile.CandidateTypeId))
        {
            await EnsureProfileAttachmentItemAsync(
                uow, profile,
                ProfileSection.Prerequisites,
                ProfileReviewConstants.FieldPaths.BirthdayCertificateId,
                profile.BirthdayCertificateId,
                ProfileReviewConstants.AttachmentTitles.BirthCertificate,
                ct);
        }

        if (ProfileValidatorUtils.RequiresMarriageCertificate(profile.CandidateTypeId))
        {
            await EnsureProfileAttachmentItemAsync(
                uow,
                profile,
                ProfileSection.Prerequisites,
                ProfileReviewConstants.FieldPaths.MarriageCertificateId,
                profile.MarriageCertificateId,
                ProfileReviewConstants.AttachmentTitles.MarriageCertificate,
                ct);
        }
    }

    public static Task EnsureSponsorAttachmentItemAsync(IUnitOfWork uow, UserProfile profile, CancellationToken ct)
    {
        if (!ProfileValidatorUtils.RequiresSponsor(profile.CandidateTypeId, profile.Provider))
            return Task.CompletedTask;

        return EnsureProfileAttachmentItemAsync(
            uow,
            profile,
            ProfileSection.Personal,
            ProfileReviewConstants.FieldPaths.SponsorCardResourceId,
            profile.SponsorProfile?.SponsorCardId,
            ProfileReviewConstants.AttachmentTitles.SponsorCard,
            ct);
    }

    public static Task EnsureNationalAddressAttachmentItemAsync(IUnitOfWork uow, UserProfile profile, CancellationToken ct)
    {
        if (!ProfileValidatorUtils.RequiresNationalAddress(profile.CandidateTypeId, profile.Provider))
            return Task.CompletedTask;

        return EnsureProfileAttachmentItemAsync(
            uow,
            profile,
            ProfileSection.Contact,
            ProfileReviewConstants.FieldPaths.NationalAddressCertificateId,
            profile.ResidenceAddress?.CertificateId,
            ProfileReviewConstants.AttachmentTitles.NationalAddressCertificate,
            ct);
    }

    private static async Task EnsureProfileAttachmentItemAsync(
        IUnitOfWork uow,
        UserProfile profile,
        ProfileSection section,
        string fieldPath,
        Guid? currentResourceId,
        string title,
        CancellationToken ct)
    {
        if (currentResourceId is null || currentResourceId == Guid.Empty)
            return;

        var resourceId = currentResourceId.Value;
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var existingItems = await reviewRepo.DbSet
            .Where(item =>
                item.UserProfileId == profile.Id &&
                item.ProfileChangeId == null &&
                !item.IsDeleted &&
                item.Section == section &&
                item.TargetType == ReviewTargetType.Attachment &&
                item.FieldPath == fieldPath)
            .ToListAsync(ct);

        var currentItem = existingItems.FirstOrDefault(item => item.ResourceId == resourceId);
        if (currentItem is not null)
        {
            currentItem.AttachmentTitle = title;
            currentItem.UpdateHash(new { resourceId });
            return;
        }

        var reusableItem = existingItems
            .OrderBy(item => item.Status switch
            {
                ReviewStatus.NeedsCorrection => 0,
                ReviewStatus.Rejected => 1,
                ReviewStatus.Solved => 2,
                ReviewStatus.Pending => 3,
                ReviewStatus.NotReviewed => 4,
                ReviewStatus.Approved => 5,
                _ => 6
            })
            .FirstOrDefault();

        if (reusableItem is not null)
        {
            reusableItem.ResourceId = resourceId;
            reusableItem.AttachmentTitle = title;
            reusableItem.UpdateHash(new { resourceId });
            ReopenForCurrentReview(reusableItem);
            return;
        }

        var item = ReviewItem.Create(
            profile.Id,
            section,
            ReviewTargetType.Attachment,
            fieldPath,
            resourceId: resourceId,
            currentValue: new { resourceId });

        item.AttachmentTitle = title;
        item.Status = ReviewStatus.Pending;
        item.IsOutdated = true;

        await reviewRepo.AddAsync(item, ct);
    }

    private static void ReopenForCurrentReview(ReviewItem item)
    {
        item.Status = ReviewStatus.Pending;
        item.IsOutdated = true;
        item.ReviewedAtUtc = null;
        item.ReviewedById = null;
    }
}
