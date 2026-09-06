using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(ExamCategory), Schema = Schemas.Hr)]
public class ExamCategory : EventEntity
{
    public Guid ExamPartId { get; set; }
    public Guid CategoryId { get; set; }
    // public Guid QuestionBankVersionId { get; set; }
    public int QuestionCount { get; set; }
    public decimal WeightPercent { get; set; }
    public int EasyQuestionCount { get; set; }
    public int MediumQuestionCount { get; set; }
    public int HardQuestionCount { get; set; }

    public ExamPart? ExamPart { get; set; }
    public ExamCategoryType? Category { get; set; }
    // public QuestionBankVersion? QuestionBankVersion { get; set; }
}
