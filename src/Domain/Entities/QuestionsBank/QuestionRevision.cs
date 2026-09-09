using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.QuestionsBank;

[Table(nameof(QuestionRevision), Schema = Schemas.Hr)]
public class QuestionRevision : EventEntity
{
    public Guid QuestionId { get; set; }

    public int RevisionNo { get; set; }
    
    public Guid QuestionTypeId { get; set; }

    public Guid DifficultyLevelId { get; set; }

    public string? QuestionTextAr { get; set; }

    public string? QuestionTextEn { get; set; }

    public string? ExplanationAr { get; set; }

    public string? ExplanationEn { get; set; }

    public Guid? ResourceId { get; set; }

    public Guid? SourceRequestItemId { get; set; }
    
    public Question Question { get; set; } = null!;
    
    public QuestionType QuestionType { get; set; } = null!;

    public DifficultyLevel DifficultyLevel { get; set; } = null!;

    public ICollection<QuestionRevisionOption> Options { get; set; }
        = new List<QuestionRevisionOption>();

    public ICollection<QuestionBankVersionQuestion> BankVersionQuestions { get; set; }
        = new List<QuestionBankVersionQuestion>();

    public ICollection<QuestionBankRequestItemReview> Reviews { get; set; }
        = new List<QuestionBankRequestItemReview>();
    
    public QuestionBankRequestItem? SourceRequestItem { get; set; }
}
