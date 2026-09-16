using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Domain.Entities.Exams;

[Table(nameof(ExamCategory), Schema = Schemas.Hr)]
public class ExamCategory : EventEntity
{
    public Guid ExamPartId { get; set; }
    public Guid QuestionBankTypeId { get; set; }
    public Guid QuestionBankVersionId { get; set; }
    public int QuestionCount { get; set; }
    public decimal WeightPercent { get; set; }
    public int EasyQuestionCount { get; set; }
    public int MediumQuestionCount { get; set; }
    public int HardQuestionCount { get; set; }

    public ExamPart? ExamPart { get; set; }
    public QuestionBankType? QuestionBankType { get; set; }
    public QuestionBankVersion? QuestionBankVersion { get; set; }
}
