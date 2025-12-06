using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobConditionRequestDto
{
    [Required(ErrorMessage = JobValidationMessages.CONDITION_TEXT_REQUIRED)]
    [MaxLength(500, ErrorMessage = JobValidationMessages.CONDITION_MAX_LENGTH)]
    public required string TextAr { get; set; }

    [Required(ErrorMessage = JobValidationMessages.CONDITION_TEXT_REQUIRED)]
    [MaxLength(500, ErrorMessage = JobValidationMessages.CONDITION_MAX_LENGTH)]
    public required string TextEn { get; set; }
}
