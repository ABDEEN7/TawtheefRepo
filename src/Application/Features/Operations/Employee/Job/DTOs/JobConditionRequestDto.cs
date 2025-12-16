using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobConditionRequestDto
{
    [Required(ErrorMessage = JobMessages.CONDITION_TEXT_REQUIRED)]
    [MaxLength(500, ErrorMessage = JobMessages.CONDITION_MAX_LENGTH)]
    public required string TextAr { get; set; }

    [Required(ErrorMessage = JobMessages.CONDITION_TEXT_REQUIRED)]
    [MaxLength(500, ErrorMessage = JobMessages.CONDITION_MAX_LENGTH)]
    public required string TextEn { get; set; }
}
