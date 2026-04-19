using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Auth;

namespace Tawtheef.Domain.Events.User;

public sealed record ContactVerificationSentEvent(Guid UserId,
    ContactVerificationType Type, string Destination, string Code, string Language = "en", DateTimeOffset OccurredOn = default
) : BaseEvent(OccurredOn);
