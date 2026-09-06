using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Domain.Entities.Lookups;

public static class QuestionReviewDecisionIds
{
    public static Guid APPROVED = Guid.Parse("73df4c1d-05a0-4bb4-9f19-5889a791aa27");
    public static Guid NEEDS_MODIFICATION = Guid.Parse("a29e6157-401f-49ac-ba46-98ecd8ca73dd");
    public static Guid REJECTED = Guid.Parse("a0ce4f21-955b-4290-bf88-fc1775a21410");
}

[Table(nameof(QuestionReviewDecision), Schema = Schemas.Lookup)]
public class QuestionReviewDecision : LookupBase
{
    public ICollection<QuestionBankRequestItemReview> ItemReviews { get; set; } = new List<QuestionBankRequestItemReview>();
}
