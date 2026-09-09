using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.QuestionsBank;

[Table(nameof(QuestionBankRequestHistory), Schema = Schemas.Hr)]
public class QuestionBankRequestHistory : EventEntity
{

    public Guid RequestId { get; set; }

    public Guid? FromStatusId { get; set; }

    public Guid ToStatusId { get; set; }

    public string Action { get; set; } = null!;

    public Guid PerformedById { get; set; }

    public DateTime PerformedAt { get; set; }

    public string? Note { get; set; }

    public QuestionBankRequest Request { get; set; } = null!;

    public QuestionBankRequestStatus? FromStatus { get; set; }

    public QuestionBankRequestStatus ToStatus { get; set; } = null!;
}
