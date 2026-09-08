using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewMemberEvaluationConfiguration : BaseEntityConfiguration<InterviewMemberEvaluation>
{
    public override void Configure(EntityTypeBuilder<InterviewMemberEvaluation> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.InterviewAppointment)
            .WithMany()
            .HasForeignKey(x => x.InterviewAppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.InterviewCommitteeMember)
            .WithMany()
            .HasForeignKey(x => x.InterviewCommitteeMemberId)
            .OnDelete(DeleteBehavior.Restrict);

        // One evaluation per member per appointment.
        builder.HasIndex(x => new { x.InterviewAppointmentId, x.InterviewCommitteeMemberId })
            .IsUnique()
            .HasDatabaseName("UQ_MemberEvaluation");
    }
}
