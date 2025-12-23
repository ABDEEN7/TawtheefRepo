using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Infrastructure.Services.Validations;

public class ProfileReviewService(IUnitOfWork uow) : IProfileReviewService
{
    public async Task<ReviewItem> TouchSectionAsync(Guid userProfileId, ProfileSection section, Guid requestedByUserId, CancellationToken ct, object? oldValue = null, object? newValue = null)
    {
        return await TouchAsync(userProfileId, section, ReviewTargetType.Section, requestedByUserId, null, null, null, ct, null, oldValue, newValue);
    }

    public async Task<ReviewItem> TouchFieldAsync(Guid userProfileId, ProfileSection section, string fieldPath, Guid requestedByUserId, CancellationToken ct, object? oldValue = null, object? newValue = null)
    {
        return await TouchAsync(userProfileId, section, ReviewTargetType.Field, requestedByUserId, null, null, null, ct, null, oldValue, newValue, fieldPath);
    }

    public async Task<ReviewItem> TouchRowAsync(Guid userProfileId, ProfileSection section, string entityName, Guid entityId, Guid requestedByUserId, CancellationToken ct, object? oldValue = null, object? newValue = null, string? fieldPath = null)
    {
        return await TouchAsync(userProfileId, section, ReviewTargetType.Row, requestedByUserId, null, entityName, entityId, ct, null, oldValue, newValue, fieldPath);
    }

    public async Task<ReviewItem> TouchAttachmentAsync(Guid userProfileId, ProfileSection section, string? attachmentTitle, Guid resourceId, Guid requestedByUserId, CancellationToken ct, object? oldValue = null, object? newValue = null)
    {
        return await TouchAsync(userProfileId, section, ReviewTargetType.Attachment, requestedByUserId, attachmentTitle, null, null, ct, resourceId, oldValue, newValue);
    }

    private async Task<ReviewItem> TouchAsync(
        Guid userProfileId,
        ProfileSection section,
        ReviewTargetType targetType,
        Guid requestedByUserId,
        string? attachmentTitle,
        string? entityName,
        Guid? entityId,
        CancellationToken ct,
        Guid? resourceId = null,
        object? oldValue = null,
        object? newValue = null,
        string? fieldPath = null)
    {
        var reviewRepo = uow.GetEntityRepository<ReviewItem>();
        var changeRepo = uow.GetEntityRepository<ProfileChangeRequest>();

        var targetKey = ProfileChangeRequest.BuildTargetKey(section, targetType, fieldPath, entityName, entityId, resourceId);
        var change = await changeRepo.DbSet
            .Where(c => c.UserProfileId == userProfileId
                        && c.TargetKey == targetKey
                        && (c.Status == ProfileChangeRequestStatus.Pending || c.Status == ProfileChangeRequestStatus.UnderReview))
            .OrderByDescending(c => c.CreatedDate)
            .FirstOrDefaultAsync(ct);

        if (change is null)
        {
            change = ProfileChangeRequest.Create(userProfileId, section, targetType, requestedByUserId, targetKey, fieldPath, 
                entityName, entityId, resourceId, attachmentTitle, oldValue, newValue);
            await changeRepo.AddAsync(change);
        }
        else
        {
            change.UpdateValues(oldValue, newValue);
            change.AttachmentTitle = attachmentTitle ?? change.AttachmentTitle;
        }

        var pending = await reviewRepo.DbSet
            .Where(r => r.UserProfileId == userProfileId
                        && r.Section == section
                        && r.TargetType == targetType
                        && r.ProfileChangeId == change.Id
                        && r.FieldPath == fieldPath
                        && (entityId == null || r.EntityId == entityId)
                        && (resourceId == null || r.ResourceId == resourceId)
                        && r.Status == ReviewStatus.Pending)
            .FirstOrDefaultAsync(ct);

        if (pending is not null)
            return pending;


        var item = ReviewItem.Create(userProfileId, section, targetType, entityName: entityName, entityId: entityId, resourceId: resourceId);
        item.AttachmentTitle = attachmentTitle;
        item.FieldPath = fieldPath;
        item.ProfileChangeId = change.Id;
        item.IsOutdated = true;

        await reviewRepo.AddAsync(item);
        return item;
    }
}
