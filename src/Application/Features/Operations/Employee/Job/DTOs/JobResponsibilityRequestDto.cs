using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobResponsibilityRequestDto
{
    [Required(ErrorMessage = JobMessages.RESPONSIBILITY_TEXT_REQUIRED)]
    [MaxLength(500, ErrorMessage = JobMessages.RESPONSIBILITY_MAX_LENGTH)]
    public required string TextAr { get; set; }

    [Required(ErrorMessage = JobMessages.RESPONSIBILITY_TEXT_REQUIRED)]
    [MaxLength(500, ErrorMessage = JobMessages.RESPONSIBILITY_MAX_LENGTH)]
    public required string TextEn { get; set; }
}
