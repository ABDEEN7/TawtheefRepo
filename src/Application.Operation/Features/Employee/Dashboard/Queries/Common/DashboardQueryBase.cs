namespace Application.Operation.Features.Employee.Dashboard.Queries.Common;

public abstract record DashboardQueryBase
{
    public int? Year { get; init; }
    public DateTime? FromDateUtc { get; init; }
    public DateTime? ToDateUtc { get; init; }
    public Guid? DepartmentId { get; init; }
    public Guid? EmployeeId { get; init; }
    public string? Status { get; init; }
}
