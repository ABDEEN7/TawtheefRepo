using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewTemplateEvaluationAxisConfiguration : BaseEntityConfiguration<InterviewTemplateEvaluationAxis>
{
    public override void Configure(EntityTypeBuilder<InterviewTemplateEvaluationAxis> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.InterviewTemplateVersion)
            .WithMany(v => v.Axes)
            .HasForeignKey(x => x.InterviewTemplateVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.InterviewEvaluationAxis)
            .WithMany()
            .HasForeignKey(x => x.InterviewEvaluationAxisId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
