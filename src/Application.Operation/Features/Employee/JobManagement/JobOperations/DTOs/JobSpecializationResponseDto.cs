using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;

public class JobSpecializationResponseDto
{
    public DropdownOptions Major { get; set; } = null!;
    public DropdownOptions? SubMajor { get; set; }
}
