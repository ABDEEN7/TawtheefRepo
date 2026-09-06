using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class QuestionReviewDecisionConfiguration : LookupBaseConfiguration<QuestionReviewDecision>
{
    public override void Configure(EntityTypeBuilder<QuestionReviewDecision> builder)
    {
        base.Configure(builder);

        builder.HasData(
            new QuestionReviewDecision
            {
                Id = QuestionReviewDecisionIds.APPROVED,
                BackendName = "APPROVED",
                NameEn = "Approved",
                NameAr = "معتمد",
                DisplayOrder = 1
            },
            new QuestionReviewDecision
            {
                Id = QuestionReviewDecisionIds.NEEDS_MODIFICATION,
                BackendName = "NEEDS_MODIFICATION",
                NameEn = "Needs Modification",
                NameAr = "يحتاج إلى تعديل",
                DisplayOrder = 2
            },
            new QuestionReviewDecision
            {
                Id = QuestionReviewDecisionIds.REJECTED,
                BackendName = "REJECTED",
                NameEn = "Rejected",
                NameAr = "مرفوض",
                DisplayOrder = 3
            }
        );
    }
}
