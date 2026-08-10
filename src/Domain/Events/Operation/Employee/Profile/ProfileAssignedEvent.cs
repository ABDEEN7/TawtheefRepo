using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.Profile;

public sealed record ProfileAssignedEvent(
    Guid UserProfileId,
    Guid EmployeeId,
    DateTimeOffset OnDateOccurred,
    int AssignedProfileCount = 1)
    : BaseEvent(OnDateOccurred);
