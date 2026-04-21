namespace Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;

public class JobTabReviewNoteResponseDto
{
    public Guid Id { get; set; }
    public Guid ReviewCycleId { get; set; }
    public string? Tab { get; set; }
    public string? Note { get; set; } 
    public string? TabStatus { get; set; }
    public bool IsResolved { get; set; } = false;
}
