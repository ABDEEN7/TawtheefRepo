namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed class ExamPartDto
{
    public int PartNo { get; set; }
    public string TitleAr { get; set; } = "";
    public string? TitleEn { get; set; }
    public int DurationMinutes { get; set; }
    public decimal? QualificationScore { get; set; }
    public List<ExamCategoryDto> Categories { get; set; } = [];
}
