using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.QuestionsBank;

[Table(nameof(QuestionRevisionOption), Schema = Schemas.Hr)]
public class QuestionRevisionOption : BaseEntity
{
    public Guid QuestionRevisionId { get; set; }

    public string? OptionTextAr { get; set; }

    public string? OptionTextEn { get; set; }

    public bool IsCorrect { get; set; }

    public int DisplayOrder { get; set; }

    public Guid? ResourceId { get; set; }
    
    public QuestionRevision QuestionRevision { get; set; } = null!;
}
