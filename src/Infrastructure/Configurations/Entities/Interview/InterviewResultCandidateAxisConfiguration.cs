using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewResultCandidateAxisConfiguration : BaseEntityConfiguration<InterviewResultCandidateAxis>
{
    public override void Configure(EntityTypeBuilder<InterviewResultCandidateAxis> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.InterviewResultCandidate)
            .WithMany(c => c.Axes)
            .HasForeignKey(x => x.InterviewResultCandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.InterviewTemplateEvaluationAxis)
            .WithMany()
            .HasForeignKey(x => x.InterviewTemplateEvaluationAxisId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
