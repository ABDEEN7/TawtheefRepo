using System.ComponentModel.DataAnnotations;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobRequiredAttachmentRequestDto
{
    [Required(ErrorMessage = JobMessages.AttachmentTitleRequired)]
    [MaxLength(200, ErrorMessage = JobMessages.AttachmentTitleMaxLength)]
    public required string TitleAr { get; set; }

    [Required(ErrorMessage = JobMessages.AttachmentTitleRequired)]
    [MaxLength(200, ErrorMessage = JobMessages.AttachmentTitleMaxLength)]
    public required string TitleEn { get; set; }

    public bool IsMandatory { get; set; } = true;
}
