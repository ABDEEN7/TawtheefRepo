using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ProfileAssigned;

public static class ProfileAssigned
{
    public const string TemplateKey = nameof(ProfileAssigned);
}

[NotificationTemplate(ProfileAssigned.TemplateKey, "تم إسناد ملف شخصي للمراجعة", "New User Profile Assigned for Review")]
public sealed record ProfileAssignedModel(Guid ProfileId, string CandidateName);
