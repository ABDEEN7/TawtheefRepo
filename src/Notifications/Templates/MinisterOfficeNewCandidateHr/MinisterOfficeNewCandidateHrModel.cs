using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.MinisterOfficeNewCandidateHr;

public static class MinisterOfficeNewCandidateHr
{
    public const string TemplateKey = nameof(MinisterOfficeNewCandidateHr);
}

[NotificationTemplate(MinisterOfficeNewCandidateHr.TemplateKey, "مرشح جديد لمكتب الوزير", "New candidate added to the Minister's Office")]
public class MinisterOfficeNewCandidateHrModel
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
}
