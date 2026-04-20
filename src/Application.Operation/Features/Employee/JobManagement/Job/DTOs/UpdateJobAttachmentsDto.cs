namespace Application.Operation.Features.Employee.JobManagement.Job.DTOs;

public class UpdateJobAttachmentsDto
{
    public List<JobRequiredAttachmentRequestDto> RequiredAttachments { get; set; } = [];
}
