using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.QuestionsBank;

[Table(nameof(QuestionBankVersionQuestion), Schema = Schemas.Hr)]
public class QuestionBankVersionQuestion : BaseEntity
{
    public Guid QuestionBankVersionId { get; set; }

    public Guid QuestionId { get; set; }

    public Guid QuestionRevisionId { get; set; }

    public Guid? SourceRequestItemId { get; set; }
    
    public QuestionBankVersion QuestionBankVersion { get; set; } = null!;

    public Question Question { get; set; } = null!;

    public QuestionRevision QuestionRevision { get; set; } = null!;

    public QuestionBankRequestItem? SourceRequestItem { get; set; }
}
