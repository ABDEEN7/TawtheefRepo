using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.QuestionsBank;

[Table(nameof(QuestionBankVersionChange), Schema = Schemas.Hr)]
public class QuestionBankVersionChange : EventEntity
{
    public Guid QuestionBankId { get; set; }

    public Guid ChangeRequestId { get; set; }

    public Guid QuestionId { get; set; }

    public Guid ChangeTypeId { get; set; }

    public Guid? FromBankVersionId { get; set; }

    public Guid ToBankVersionId { get; set; }

    public Guid? OldQuestionRevisionId { get; set; }

    public Guid? NewQuestionRevisionId { get; set; }

    public string? OldSnapshotJson { get; set; }

    public string? NewSnapshotJson { get; set; }

    public Guid ChangedById { get; set; }

    public DateTime ChangedAt { get; set; }

    public QuestionBank QuestionBank { get; set; } = null!;

    public QuestionBankRequest ChangeRequest { get; set; } = null!;

    public Question Question { get; set; } = null!;

    public QuestionChangeType ChangeType { get; set; } = null!;

    public QuestionBankVersion? FromBankVersion { get; set; }

    public QuestionBankVersion ToBankVersion { get; set; } = null!;

    public QuestionRevision? OldQuestionRevision { get; set; }

    public QuestionRevision? NewQuestionRevision { get; set; }

    public ICollection<QuestionBankVersionChangeDetail> Details { get; set; }
        = new List<QuestionBankVersionChangeDetail>();
}
