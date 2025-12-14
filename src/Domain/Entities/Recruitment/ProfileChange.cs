using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Recruitment;

public enum ProfileChangeStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    NeedsCorrection = 4
}

[Table(nameof(ProfileChange), Schema = Schemas.Hr)]
public class ProfileChange : EventEntity
{
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }

    public ProfileSection Section { get; set; }
    public ReviewTargetType TargetType { get; set; }

    public string? FieldPath { get; set; }
    public string? EntityName { get; set; }
    public Guid? EntityId { get; set; }
    public Guid? ResourceId { get; set; }
    public string? AttachmentTitle { get; set; }

    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    public ProfileChangeStatus Status { get; set; } = ProfileChangeStatus.Pending;

    public static ProfileChange Create(
        Guid userProfileId,
        ProfileSection section,
        ReviewTargetType targetType,
        string? fieldPath,
        string? entityName,
        Guid? entityId,
        Guid? resourceId,
        string? attachmentTitle,
        object? oldValue,
        object? newValue)
    {
        return new ProfileChange
        {
            UserProfileId = userProfileId,
            Section = section,
            TargetType = targetType,
            FieldPath = fieldPath,
            EntityName = entityName,
            EntityId = entityId,
            ResourceId = resourceId,
            AttachmentTitle = attachmentTitle,
            OldValue = SerializeValue(oldValue),
            NewValue = SerializeValue(newValue)
        };
    }

    public void UpdateValues(object? oldValue, object? newValue)
    {
        OldValue = SerializeValue(oldValue);
        NewValue = SerializeValue(newValue);
        Status = ProfileChangeStatus.Pending;
    }

    private static string? SerializeValue(object? value)
    {
        if (value is null) return null;

        return value switch
        {
            string s => s,
            DateOnly date => date.ToString("yyyy-MM-dd"),
            DateTime dt => dt.ToString("O"),
            DateTimeOffset dto => dto.ToString("O"),
            Enum e => e.ToString(),
            bool b => b ? "true" : "false",
            IFormattable f => f.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
            _ => JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = false })
        };
    }
}

