using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ProfileAssigned;

public static class ProfileAssigned
{
    public const string TemplateKey = nameof(ProfileAssigned);
}

[NotificationTemplate(ProfileAssigned.TemplateKey, "تحويل ملف مرشح للمراجعة", "Candidate Profile Sent for Review")]
public sealed record ProfileAssignedModel(Guid ProfileId, string CandidateName);
