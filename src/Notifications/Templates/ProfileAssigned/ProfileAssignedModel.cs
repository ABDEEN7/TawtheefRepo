using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ProfileAssigned;

public static class ProfileAssigned
{
    public const string TemplateKey = nameof(ProfileAssigned);
}

[NotificationTemplate(ProfileAssigned.TemplateKey, "إسناد ملفات مرشحين للمراجعة", "Candidate Profiles Assigned for Review")]
public sealed record ProfileAssignedModel(int AssignedProfileCount);
