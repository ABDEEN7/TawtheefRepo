namespace Application.Operation.Features.Employee.Dashboard.DTOs.Employees;

public sealed class TeamPerformanceRowDto
{
    public Guid EmployeeId { get; init; }
    public required string Name { get; init; }
    public string? EmployeeNumber { get; init; }
    public string? DepartmentName { get; init; }
    public string? JobDescription { get; init; }
    public int AssignedTasks { get; init; }
    public int CompletedTasks { get; init; }
    public int RemainingTasks { get; init; }
    public int OverdueTasks { get; init; }
}
