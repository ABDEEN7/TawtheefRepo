namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public record ResidentBreakdownDto
{
    public Guid NationalityId { get; init; }
    public decimal Percentage { get; init; }
}
