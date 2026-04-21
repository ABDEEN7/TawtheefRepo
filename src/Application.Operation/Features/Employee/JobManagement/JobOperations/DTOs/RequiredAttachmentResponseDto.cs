namespace Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;

public class RequiredAttachmentResponseDto
{
    public string Title { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }
    public int Order { get; set; }
}
