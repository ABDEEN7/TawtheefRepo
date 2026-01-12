using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Common.Validations;

public interface IProfileReviewService
{
    Task<ReviewItem> TouchSectionAsync(Guid userProfileId, ProfileSection section, Guid requestedByUserId, CancellationToken ct, object? oldValue = null, object? newValue = null);
    Task<ReviewItem> TouchFieldAsync(Guid userProfileId, ProfileSection section, string fieldPath, Guid requestedByUserId, CancellationToken ct, object? oldValue = null, object? newValue = null);
    Task<ReviewItem> TouchRowAsync(Guid userProfileId, ProfileSection section, string entityName, Guid entityId, Guid requestedByUserId, CancellationToken ct, object? oldValue = null, object? newValue = null, string? fieldPath = null);
    Task<ReviewItem> TouchAttachmentAsync(Guid userProfileId, ProfileSection section, string? attachmentTitle, Guid resourceId, Guid requestedByUserId, CancellationToken ct, object? oldValue = null, object? newValue = null);
}
