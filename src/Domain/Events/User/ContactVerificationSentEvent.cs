using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Auth;

namespace Tawtheef.Domain.Events.User;

public sealed record ContactVerificationSentEvent(Guid UserId,
    ContactVerificationType Type, string Destination, string Code, DateTimeOffset OccurredOn = default
) : BaseEvent(OccurredOn);
