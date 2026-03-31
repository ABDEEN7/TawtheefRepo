using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.InvitationAttachmentReturned;

public static class InvitationAttachmentReturned
{
    public const string TemplateKey = nameof(InvitationAttachmentReturned);
}

[NotificationTemplate(InvitationAttachmentReturned.TemplateKey)]
public sealed record InvitationAttachmentReturnedModel(string AttachmentTitle, string ReviewNote);
