using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Common.Services;

public interface IProfileReviewService
{
    Task<ReviewItem> TouchSectionAsync(Guid userProfileId, ProfileSection section, CancellationToken ct);
    Task<ReviewItem> TouchRowAsync(Guid userProfileId, ProfileSection section, string entityName, Guid entityId, CancellationToken ct);
    Task<ReviewItem> TouchAttachmentAsync(Guid userProfileId, ProfileSection section, string? attachmentTitle, Guid resourceId, CancellationToken ct);
}
