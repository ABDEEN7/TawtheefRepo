using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Infrastructure.Services.Validations;

public class ProfileReviewService(IUnitOfWork uow) : IProfileReviewService
{
    public async Task<ReviewItem> TouchSectionAsync(Guid userProfileId, ProfileSection section, CancellationToken ct)
    {
        return await TouchAsync(userProfileId, section, ReviewTargetType.Section, null, null, null, ct);
    }

    public async Task<ReviewItem> TouchRowAsync(Guid userProfileId, ProfileSection section, string entityName, Guid entityId, CancellationToken ct)
    {
        return await TouchAsync(userProfileId, section, ReviewTargetType.Row, null, entityName, entityId, ct);
    }

    public async Task<ReviewItem> TouchAttachmentAsync(Guid userProfileId, ProfileSection section, string? attachmentTitle, Guid resourceId, CancellationToken ct)
    {
        return await TouchAsync(userProfileId, section, ReviewTargetType.Attachment, attachmentTitle, null, null, ct, resourceId);
    }

    private async Task<ReviewItem> TouchAsync(
        Guid userProfileId,
        ProfileSection section,
        ReviewTargetType targetType,
        string? attachmentTitle,
        string? entityName,
        Guid? entityId,
        CancellationToken ct,
        Guid? resourceId = null)
    {
        var repo = uow.GetEntityRepository<ReviewItem>();

        var pending = await repo.DbSet
            .Where(r => r.UserProfileId == userProfileId
                        && r.Section == section
                        && r.TargetType == targetType
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
                        && (entityId == null || r.EntityId == entityId)
                        && (resourceId == null || r.ResourceId == resourceId))
            .Select(r => (int?)r.Version)
            .OrderByDescending(v => v)
            .FirstOrDefaultAsync(ct) ?? 0;

        var item = ReviewItem.Create(userProfileId, section, targetType, entityName: entityName, entityId: entityId, resourceId: resourceId);
        item.Version = latestVersion + 1;
        item.AttachmentTitle = attachmentTitle;
        item.IsOutdated = true;

        await repo.AddAsync(item);
        return item;
    }
}
