using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.KawaderInvitation;

public static class KawaderInvitation
{
    public const string TemplateKey = nameof(KawaderInvitation);
}


[NotificationTemplate(KawaderInvitation.TemplateKey, "دعوة للانضمام إلى نظام الاستقطاب والتوظيف وتخطيط القوى العاملة", "Invitation to Join Careers Platform")]
public class KawaderInvitationModel
{
    public string FullName { get; set; } = string.Empty;
    public string Qid { get; set; } = string.Empty;
}
