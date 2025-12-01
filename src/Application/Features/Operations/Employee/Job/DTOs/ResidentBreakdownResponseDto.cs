using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public record ResidentBreakdownResponseDto
{
    public DropdownOptions? Nationality { get; init; } // Now returns DropdownOptions
    public decimal Percentage { get; init; }
}
