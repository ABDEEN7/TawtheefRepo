using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Dashboard.Services.Access;

internal sealed record DashboardAccessContext(
    Guid CurrentUserId,
    User CurrentUser,
    bool CanManageDashboard,
    DashboardScope Scope);
