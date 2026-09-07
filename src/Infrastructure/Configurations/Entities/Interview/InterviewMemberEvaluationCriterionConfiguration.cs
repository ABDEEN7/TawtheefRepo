using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewMemberEvaluationCriterionConfiguration : BaseEntityConfiguration<InterviewMemberEvaluationCriterion>
{
    public override void Configure(EntityTypeBuilder<InterviewMemberEvaluationCriterion> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.InterviewMemberEvaluation)
            .WithMany(e => e.CriterionScores)
            .HasForeignKey(x => x.InterviewMemberEvaluationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.InterviewTemplateEvaluationCriterion)
            .WithMany()
            .HasForeignKey(x => x.InterviewTemplateEvaluationCriterionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
