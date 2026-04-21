namespace Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;

public class UpdateJobAttachmentsDto
{
    public List<JobRequiredAttachmentRequestDto> RequiredAttachments { get; set; } = [];
}
