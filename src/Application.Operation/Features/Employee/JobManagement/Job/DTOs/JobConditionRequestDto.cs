using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.Job.DTOs;

public class JobConditionRequestDto
{
    [Required(ErrorMessage = JobMessages.ConditionTextRequired)]
    [MaxLength(500, ErrorMessage = JobMessages.ConditionMaxLength)]
    public required string TextAr { get; set; }

    [Required(ErrorMessage = JobMessages.ConditionTextRequired)]
    [MaxLength(500, ErrorMessage = JobMessages.ConditionMaxLength)]
    public required string TextEn { get; set; }
}
