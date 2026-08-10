namespace Application.Operation.Features.Employee.Dashboard.DTOs.Overview;

public sealed class DashboardFiltersSnapshotDto
{
    public DateTime? FromDateUtc { get; init; }
    public DateTime? ToDateUtc { get; init; }
    public Guid? DepartmentId { get; init; }
    public Guid? EmployeeId { get; init; }
    public string? Status { get; init; }
}
