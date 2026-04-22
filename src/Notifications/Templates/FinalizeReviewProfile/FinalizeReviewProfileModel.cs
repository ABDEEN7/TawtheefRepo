using Tawtheef.Domain.Entities.Users;
using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.FinalizeReviewProfile;

public static class FinalizeReviewProfile
{
    public const string TemplateKey = nameof(FinalizeReviewProfile);
}

[NotificationTemplate(FinalizeReviewProfile.TemplateKey,  "تم الانتهاء من مراجعة ملفك الشخصي", "Finalize Review Profile")]
public sealed record FinalizeReviewProfileModel(UserProfileStatus Status);
