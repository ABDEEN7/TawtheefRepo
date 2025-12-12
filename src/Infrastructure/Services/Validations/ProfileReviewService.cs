using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Infrastructure.Services.Validations;

public class ProfileReviewService(IUnitOfWork uow) : IProfileReviewService
{
    public async Task<ReviewItem> TouchSectionAsync(Guid userProfileId, ProfileSection section, CancellationToken ct, object? oldValue = null, object? newValue = null)
    {
        return await TouchAsync(userProfileId, section, ReviewTargetType.Section, null, null, null, ct, null, oldValue, newValue);
    }

    public async Task<ReviewItem> TouchFieldAsync(Guid userProfileId, ProfileSection section, string fieldPath, CancellationToken ct, object? oldValue = null, object? newValue = null)
    {
        return await TouchAsync(userProfileId, section, ReviewTargetType.Field, null, null, null, ct, null, oldValue, newValue, fieldPath);
    }

    public async Task<ReviewItem> TouchRowAsync(Guid userProfileId, ProfileSection section, string entityName, Guid entityId, CancellationToken ct, object? oldValue = null, object? newValue = null, string? fieldPath = null)
    {
        return await TouchAsync(userProfileId, section, ReviewTargetType.Row, null, entityName, entityId, ct, null, oldValue, newValue, fieldPath);
    }

    public async Task<ReviewItem> TouchAttachmentAsync(Guid userProfileId, ProfileSection section, string? attachmentTitle, Guid resourceId, CancellationToken ct, object? oldValue = null, object? newValue = null)
    {
        return await TouchAsync(userProfileId, section, ReviewTargetType.Attachment, attachmentTitle, null, null, ct, resourceId, oldValue, newValue);
    }

    private async Task<ReviewItem> TouchAsync(
        Guid userProfileId,
        ProfileSection section,
        ReviewTargetType targetType,
        string? attachmentTitle,
        string? entityName,
        Guid? entityId,
        CancellationToken ct,
        Guid? resourceId = null,
        object? oldValue = null,
        object? newValue = null,
        string? fieldPath = null)
    {
        var repo = uow.GetEntityRepository<ReviewItem>();
        var changeRepo = uow.GetEntityRepository<ProfileChange>();

        var change = await changeRepo.DbSet
            .Where(c => c.UserProfileId == userProfileId
                        && c.Section == section
                        && c.TargetType == targetType
                        && c.FieldPath == fieldPath
                        && c.EntityName == entityName
                        && (entityId == null || c.EntityId == entityId)
                        && (resourceId == null || c.ResourceId == resourceId))
            .OrderByDescending(c => c.CreatedDate)
            .FirstOrDefaultAsync(ct);

        if (change is null)
        {
            change = ProfileChange.Create(userProfileId, section, targetType, fieldPath, entityName, entityId, resourceId, attachmentTitle, oldValue, newValue);
            await changeRepo.AddAsync(change);
        }
        else
        {
            change.UpdateValues(oldValue, newValue);
            change.AttachmentTitle = attachmentTitle ?? change.AttachmentTitle;
        }

        var pending = await repo.DbSet
            .Where(r => r.UserProfileId == userProfileId
                        && r.Section == section
                        && r.TargetType == targetType
                        && r.ProfileChangeId == change.Id
                        && r.FieldPath == fieldPath
                        && (entityId == null || r.EntityId == entityId)
                        && (resourceId == null || r.ResourceId == resourceId)
                        && r.Status == ReviewStatus.Pending)
            .OrderByDescending(r => r.Version)
            .FirstOrDefaultAsync(ct);

        if (pending is not null)
            return pending;

        var latestVersion = await repo.DbSet
            .Where(r => r.UserProfileId == userProfileId
                        && r.Section == section
                        && r.TargetType == targetType
                        && r.ProfileChangeId == change.Id
                        && r.FieldPath == fieldPath
                        && (entityId == null || r.EntityId == entityId)
                        && (resourceId == null || r.ResourceId == resourceId))
            .Select(r => (int?)r.Version)
            .OrderByDescending(v => v)
            .FirstOrDefaultAsync(ct) ?? 0;

        var item = ReviewItem.Create(userProfileId, section, targetType, entityName: entityName, entityId: entityId, resourceId: resourceId);
        item.Version = latestVersion + 1;
        item.AttachmentTitle = attachmentTitle;
        item.FieldPath = fieldPath;
        item.ProfileChangeId = change.Id;
        item.IsOutdated = true;

        await repo.AddAsync(item);
        return item;
    }
}
