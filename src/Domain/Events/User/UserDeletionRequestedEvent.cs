using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.User;

public sealed record UserDeletionRequestedEvent(
    Guid UserId, string Email, DateTime OccurredOn
) : BaseEvent(OccurredOn);
