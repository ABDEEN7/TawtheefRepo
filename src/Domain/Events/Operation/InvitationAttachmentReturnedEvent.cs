using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation;

public record InvitationAttachmentReturnedEvent(
    Guid ApplicantId,
    Guid InvitationId,
    string AttachmentTitle,
    string? ReviewNote,
    DateTimeOffset OccurredOn = default
) : BaseEvent(OccurredOn);
