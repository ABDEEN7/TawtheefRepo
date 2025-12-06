using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobDegreeResponseDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public required Guid DegreeId { get; set; }
    public required DropdownOptions? Degree { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
