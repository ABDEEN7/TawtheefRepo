using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Recruitment;

public enum ProfileChangeRequestStatus
{
    Pending = 1,
    UnderReview = 2,
    Approved = 3,
    Rejected = 4,
    Canceled = 5
}

public enum ProfileChangeAction
{
    UpdateField = 1,
    ReplaceAttachment = 2,
    AddListItem = 3
}

[Table(nameof(ProfileChangeRequest), Schema = Schemas.Hr)]
public class ProfileChangeRequest : EventEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }

    public ProfileSection Section { get; set; }

    public ProfileChangeAction Action { get; set; }

    /// <summary>
    /// Unique identity of target inside one profile.
    /// Examples:
    /// Field|Contact|ResidenceCountryId
    /// Attachment|Personal|NationalCard
    /// Add|Qualifications|{RequestId}
    /// </summary>
    [MaxLength(256)]
    public string TargetKey { get; set; } = default!;

    /// <summary>
    /// For Field/Attachment targets (optional but helpful for reporting/UI).
    /// Example: "ResidenceCountryId" or "NationalCardId"
    /// </summary>
    [MaxLength(256)]
    public string? FieldPath { get; set; }

    /// <summary>
    /// For list adds (Qualification/Experience/...).
    /// Example: "Qualification"
    /// </summary>
    [MaxLength(128)]
    public string? EntityName { get; set; }

    /// <summary>
    /// Old/New values for fields (store as string/JSON).
    /// OldValue can be optional because you can compute from approved profile.
    /// But storing it helps audit and stable review.
    /// </summary>
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    /// <summary>
    /// Attachments: store ids explicitly.
    /// </summary>
    public Guid? OldResourceId { get; set; }
    public Guid? NewResourceId { get; set; }
    [MaxLength(256)]
    public string? AttachmentTitle { get; set; }

    public ProfileChangeRequestStatus Status { get; set; } = ProfileChangeRequestStatus.Pending;

    /// <summary>
    /// User who created the request.
    /// </summary>
    public Guid RequestedById { get; set; }
    public DateTimeOffset RequestedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Reviewer decisions.
    /// Reject reason is required when status = Rejected.
    /// </summary>
    public Guid? ReviewedById { get; set; }
    public DateTimeOffset? ReviewedAtUtc { get; set; }
    [MaxLength(2000)]
    public string? ReviewerNote { get; set; }

    /// <summary>
    /// Cancel by user while Pending/UnderReview only.
    /// </summary>
    public Guid? CanceledById { get; set; }
    public DateTime? CanceledAtUtc { get; set; }

    /// <summary>
    /// Concurrency token to avoid races.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = default!;
    
    public static string BuildTargetKey(
        ProfileSection section,
        ReviewTargetType targetType,
        string? fieldPath,
        string? entityName,
        Guid? entityId,
        Guid? resourceId)
    {
        return targetType switch
        {
            ReviewTargetType.Section => $"Section|{section}",
            ReviewTargetType.Field => fieldPath is null
                ? throw new ArgumentNullException(nameof(fieldPath))
                : $"Field|{section}|{fieldPath}",
            ReviewTargetType.Row => entityId is null
                ? throw new ArgumentNullException(nameof(entityId))
                : $"Row|{section}|{entityName ?? "Entity"}|{entityId}",
            ReviewTargetType.Attachment => (fieldPath ?? entityName) is null
                ? throw new ArgumentNullException(nameof(fieldPath))
                : $"Attachment|{section}|{fieldPath ?? entityName}|{resourceId?.ToString() ?? Guid.NewGuid().ToString()}",
            _ => throw new ArgumentOutOfRangeException(nameof(targetType), "Unsupported target type for change request")
        };
    }

    public static ProfileChangeRequest Create(
        Guid userProfileId,
        ProfileSection section,
        ReviewTargetType targetType,
        Guid requestedById,
        string targetKey,
        string? fieldPath,
        string? entityName,
        Guid? entityId,
        Guid? resourceId,
        string? attachmentTitle,
        object? oldValue,
        object? newValue)
    {
        var action = targetType switch
        {
            ReviewTargetType.Section => ProfileChangeAction.UpdateField,
            ReviewTargetType.Field => ProfileChangeAction.UpdateField,
            ReviewTargetType.Row => ProfileChangeAction.AddListItem,
            ReviewTargetType.Attachment => ProfileChangeAction.AddListItem,
            _ => throw new ArgumentOutOfRangeException(nameof(targetType), "Invalid target type for profile change request")
        };

        var request = new ProfileChangeRequest
        {
            UserProfileId = userProfileId,
            Section = section,
            Action = action,
            TargetKey = targetKey,
            FieldPath = fieldPath,
            EntityName = entityName,
            OldValue = oldValue != null ? JsonSerializer.Serialize(oldValue) : null,
            NewValue = newValue != null ? JsonSerializer.Serialize(newValue) : null,
            OldResourceId = targetType == ReviewTargetType.Attachment ? resourceId : null,
            NewResourceId = targetType == ReviewTargetType.Attachment ? resourceId : null,
            AttachmentTitle = attachmentTitle,
            RequestedById = requestedById,
            RequestedAtUtc = DateTime.UtcNow
        };

        return request;
    }

    public void UpdateValues(object? oldValue, object? newValue)
    {
        OldValue = oldValue != null ? JsonSerializer.Serialize(oldValue) : null;
        NewValue = newValue != null ? JsonSerializer.Serialize(newValue) : null;
        RequestedAtUtc = DateTime.UtcNow;
    }
}
