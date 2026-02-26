using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Events.Operation.Employee.Profile;

public record FinalizeReviewProfileEvent(Guid UserId, UserProfileStatus Status, DateTimeOffset OccurredOn = default) : BaseEvent(OccurredOn);
