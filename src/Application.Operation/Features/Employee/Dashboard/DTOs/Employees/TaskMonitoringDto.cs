using Application.Operation.Features.Employee.Dashboard.DTOs.Common;

namespace Application.Operation.Features.Employee.Dashboard.DTOs.Employees;

public sealed class TaskMonitoringDto
{
    public IReadOnlyList<GroupCountDto> TaskStatusStacked { get; init; } = [];
}
