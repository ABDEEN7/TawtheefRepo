using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.QuestionsBank;

[Table(nameof(QuestionBankRequestReview), Schema = Schemas.Hr)]
public class QuestionBankRequestReview : EventEntity
{
    public Guid RequestId { get; set; }

    public int ReviewRound { get; set; }

    public string? Note { get; set; }

    public Guid ReviewedById { get; set; }

    public DateTime ReviewedAt { get; set; }


    public QuestionBankRequest Request { get; set; } = null!;

    public EmployeeProfile ReviewedBy { get; set; } = null!;
}
