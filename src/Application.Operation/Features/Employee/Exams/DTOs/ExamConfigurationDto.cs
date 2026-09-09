namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed class ExamConfigurationDto
{
    public Guid JobId { get; set; }
    public string TitleAr { get; set; } = "";
    public string? TitleEn { get; set; }
    public bool AllowPreviousQuestion { get; set; }
    public Guid InterruptionPolicyId { get; set; }
    public string? Notes { get; set; }
    public int TotalQuestions { get; set; }
    public string? DecisionNotes { get; set; }
    public List<ExamPartDto> Parts { get; set; } = [];
}
