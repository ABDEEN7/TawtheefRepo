using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.MinisterOfficeRegistration;

public static class MinisterOfficeRegistration
{
    public const string TemplateKey = nameof(MinisterOfficeRegistration);
}

[NotificationTemplate(MinisterOfficeRegistration.TemplateKey, "التسجيل في مكتب الوزير", "Minister Office Registration")]
public sealed record MinisterOfficeRegistrationModel(string FullName, string ClientUrl);
