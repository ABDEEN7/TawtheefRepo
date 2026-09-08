using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewEvaluationCriterionConfiguration : BaseEntityConfiguration<InterviewEvaluationCriterion>
{
    public override void Configure(EntityTypeBuilder<InterviewEvaluationCriterion> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
        builder.Property(x => x.NameEn).HasMaxLength(200);
        builder.Property(x => x.IsActive).HasDefaultValue(true);

        builder.HasOne(x => x.Axis)
            .WithMany()
            .HasForeignKey(x => x.InterviewEvaluationAxisId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
