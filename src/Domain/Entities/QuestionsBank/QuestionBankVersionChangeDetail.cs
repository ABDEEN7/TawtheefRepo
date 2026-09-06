using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.QuestionsBank;

[Table(nameof(QuestionBankVersionChangeDetail), Schema = Schemas.Hr)]
public class QuestionBankVersionChangeDetail : BaseEntity
{
    public Guid VersionChangeId { get; set; }

    public string FieldPath { get; set; } = null!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public QuestionBankVersionChange VersionChange { get; set; } = null!;
}
