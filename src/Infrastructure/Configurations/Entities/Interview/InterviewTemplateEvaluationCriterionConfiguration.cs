using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewTemplateEvaluationCriterionConfiguration : BaseEntityConfiguration<InterviewTemplateEvaluationCriterion>
{
    public override void Configure(EntityTypeBuilder<InterviewTemplateEvaluationCriterion> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.NameAr).HasMaxLength(200);
        builder.Property(x => x.NameEn).HasMaxLength(200);

        builder.HasOne(x => x.InterviewTemplateEvaluationAxis)
            .WithMany(a => a.Criteria)
            .HasForeignKey(x => x.InterviewTemplateEvaluationAxisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.InterviewEvaluationCriterion)
            .WithMany()
            .HasForeignKey(x => x.InterviewEvaluationCriterionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_TemplateCriterion_Source",
            "([InterviewEvaluationCriterionId] IS NOT NULL AND [NameAr] IS NULL) " +
            "OR ([InterviewEvaluationCriterionId] IS NULL AND [NameAr] IS NOT NULL)"));
    }
}
