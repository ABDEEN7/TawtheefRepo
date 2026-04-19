using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.InvitationAttachmentReturned;

public static class InvitationAttachmentReturned
{
    public const string TemplateKey = nameof(InvitationAttachmentReturned);
}

[NotificationTemplate(InvitationAttachmentReturned.TemplateKey, "تمت إعادة المرفق للمراجعة", "Action Required: Job Application Attachment Returned")]
public sealed record InvitationAttachmentReturnedModel(string AttachmentTitle, string ReviewNote);
