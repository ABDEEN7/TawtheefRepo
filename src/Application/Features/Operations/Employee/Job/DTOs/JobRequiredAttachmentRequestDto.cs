using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobRequiredAttachmentRequestDto
{
    [Required(ErrorMessage = JobValidationMessages.ATTACHMENT_TITLE_REQUIRED)]
    [MaxLength(200, ErrorMessage = JobValidationMessages.ATTACHMENT_TITLE_MAX_LENGTH)]
    public required string TitleAr { get; set; }

    [Required(ErrorMessage = JobValidationMessages.ATTACHMENT_TITLE_REQUIRED)]
    [MaxLength(200, ErrorMessage = JobValidationMessages.ATTACHMENT_TITLE_MAX_LENGTH)]
    public required string TitleEn { get; set; }

    public bool IsMandatory { get; set; } = true;
}
