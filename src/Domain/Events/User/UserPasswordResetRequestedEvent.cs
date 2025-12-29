using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.User;

public sealed record UserPasswordResetRequestedEvent(
    Guid UserId,
    string Email,
    DateTimeOffset OccurredOn
) : BaseEvent(OccurredOn);
