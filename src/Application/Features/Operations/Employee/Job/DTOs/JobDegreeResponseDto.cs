

using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobDegreeResponseDto
{
    public Guid Id { get; set; }
    public DropdownOptions Degree { get; set; } = new();
     
    public int Order { get; set; }
}
