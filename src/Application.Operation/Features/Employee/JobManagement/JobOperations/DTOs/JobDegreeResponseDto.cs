using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;

public class JobDegreeResponseDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public required Guid DegreeId { get; set; }
    public required DropdownOptions? Degree { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}
