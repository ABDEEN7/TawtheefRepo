using System;
using System.Collections.Generic;
using System.Text;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobTabReviewNoteResponseDto
{
    public Guid Id { get; set; }
    public TabType Tab { get; set; }
    public string Note { get; set; } = string.Empty;
    public TabStatus TabStatus { get; set; }

    public List<JobTabReviewAttachmentResponseDto>? Attachments { get; set; }
}
