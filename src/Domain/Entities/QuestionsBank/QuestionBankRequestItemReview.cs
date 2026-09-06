using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.QuestionsBank;

[Table(nameof(QuestionBankRequestItemReview), Schema = Schemas.Hr)]
public class QuestionBankRequestItemReview : BaseEntity
{
    public Guid RequestItemId { get; set; }

    public int ReviewRound { get; set; }

    public Guid? ReviewedRevisionId { get; set; }

    public Guid DecisionId { get; set; }

    public string? ReviewNote { get; set; }

    public Guid ReviewedById { get; set; }

    public DateTime ReviewedAt { get; set; }

    public QuestionBankRequestItem RequestItem { get; set; } = null!;

    public QuestionRevision? ReviewedRevision { get; set; }

    public QuestionReviewDecision Decision { get; set; } = null!;
    
    public EmployeeProfile ReviewedBy { get; set; } = null!;
}
