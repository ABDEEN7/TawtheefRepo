using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(TestAttemptQuestion), Schema = Schemas.Hr)]
public class TestAttemptQuestion : EventEntity
{
    public Guid TestAttemptId { get; set; }
    public Guid ExamCategoryId { get; set; }
    // public Guid QuestionId { get; set; }
    public int OrderNo { get; set; }
    // public Guid? SelectedQuestionOptionId { get; set; }
    public DateTime? SavedAt { get; set; }

    public TestAttempt? TestAttempt { get; set; }
    public ExamCategory? ExamCategory { get; set; }
    // public Question? Question { get; set; }
    // public QuestionOption? SelectedQuestionOption { get; set; }
}
