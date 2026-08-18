using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Common.Access;

internal sealed record EmployeeProfileAccessContext(
    Guid CurrentUserId,
    User CurrentUser,
    bool CanViewProfileDistribution,
    bool CanViewAssignedProfiles);
