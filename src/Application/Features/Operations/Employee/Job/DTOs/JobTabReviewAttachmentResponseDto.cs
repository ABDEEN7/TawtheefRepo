using System;
using System.Collections.Generic;
using System.Text;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobTabReviewAttachmentResponseDto
{
    public Guid Id { get; set; }
    public Guid AttachmentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? Url { get; set; }
}
