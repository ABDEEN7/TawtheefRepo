using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.KawaderInvitation;

[NotificationTemplate("KawaderInvitation")]
public class KawaderInvitationModel
{
    public string FullName { get; set; } = string.Empty;
    public string Qid { get; set; } = string.Empty;
}
