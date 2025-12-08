using Tawtheef.Application.Common.Models;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class ResidentBreakdownResponseDto
{
    public Guid Id { get; set; }
    public Guid JobQuotaId { get; set; }
    public required Guid NationalityId { get; set; }
    public required DropdownOptions? Nationality { get; set; }
    public required decimal Percentage { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}
