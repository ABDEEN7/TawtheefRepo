using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.MinisterOfficeNewCandidateHr;

public static class MinisterOfficeNewCandidateHr
{
    public const string TemplateKey = nameof(MinisterOfficeNewCandidateHr);
}

[NotificationTemplate(MinisterOfficeNewCandidateHr.TemplateKey)]
public class MinisterOfficeNewCandidateHrModel
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
}
