using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewCommitteeMemberEvaluationAxisConfiguration : BaseEntityConfiguration<InterviewCommitteeMemberEvaluationAxis>
{
    public override void Configure(EntityTypeBuilder<InterviewCommitteeMemberEvaluationAxis> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.InterviewCommitteeMember)
            .WithMany(m => m.EvaluationAxes)
            .HasForeignKey(x => x.InterviewCommitteeMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.InterviewTemplateEvaluationAxis)
            .WithMany()
            .HasForeignKey(x => x.InterviewTemplateEvaluationAxisId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
