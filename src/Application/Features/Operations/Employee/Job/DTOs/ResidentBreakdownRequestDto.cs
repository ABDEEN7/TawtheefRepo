
using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class ResidentBreakdownRequestDto
{
    public required Guid NationalityId { get; set; }

    [Range(0, 100, ErrorMessage = JobValidationMessages.QUOTA_PERCENTAGE_RANGE)]
    public required decimal Percentage { get; set; }
}
