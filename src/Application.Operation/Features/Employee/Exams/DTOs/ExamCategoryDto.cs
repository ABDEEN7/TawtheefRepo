namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed class ExamCategoryDto
{
    public Guid QuestionBankTypeId { get; set; }
    public Guid QuestionBankVersionId { get; set; }
    public int QuestionCount { get; set; }
    public decimal WeightPercent { get; set; }
    public int EasyQuestionCount { get; set; }
    public int MediumQuestionCount { get; set; }
    public int HardQuestionCount { get; set; }
}
