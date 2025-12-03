using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class ResidentBreakdownResponseDto
{
    public Guid Id { get; set; }
    public DropdownOptions? Nationality { get; set; }
    public decimal Percentage { get; set; }
}
