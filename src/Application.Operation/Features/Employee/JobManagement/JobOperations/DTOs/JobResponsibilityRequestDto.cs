using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;

public class JobResponsibilityRequestDto
{
    [Required(ErrorMessage = JobMessages.ResponsibilityTextRequired)]
    [MaxLength(500, ErrorMessage = JobMessages.ResponsibilityMaxLength)]
    public required string TextAr { get; set; }

    [Required(ErrorMessage = JobMessages.ResponsibilityTextRequired)]
    [MaxLength(500, ErrorMessage = JobMessages.ResponsibilityMaxLength)]
    public required string TextEn { get; set; }
}
