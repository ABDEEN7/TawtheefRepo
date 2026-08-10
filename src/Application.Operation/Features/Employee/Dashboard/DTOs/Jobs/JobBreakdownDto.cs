using Application.Operation.Features.Employee.Dashboard.DTOs.Common;

namespace Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;

public sealed class JobBreakdownDto
{
    public IReadOnlyList<StatusCountDto> ByStatus { get; init; } = [];
}
