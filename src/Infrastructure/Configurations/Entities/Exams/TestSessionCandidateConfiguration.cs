using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Exams;

namespace Tawtheef.Infrastructure.Configurations.Entities.Exams;

public class TestSessionCandidateConfiguration : BaseEntityConfiguration<TestSessionCandidate>
{
    public override void Configure(EntityTypeBuilder<TestSessionCandidate> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => new
            {
                x.TestSessionId,
                x.InvitationId
            })
            .IsUnique();

        builder.HasIndex(x => x.RescheduledFromTestSessionCandidateId)
            .IsUnique()
            .HasFilter("[RescheduledFromTestSessionCandidateId] IS NOT NULL");

        builder.HasOne(x => x.RescheduledFromTestSessionCandidate)
            .WithMany()
            .HasForeignKey(x => x.RescheduledFromTestSessionCandidateId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TestSession)
            .WithMany(x => x.TestSessionCandidates)
            .HasForeignKey(x => x.TestSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Invitation)
            .WithMany()
            .HasForeignKey(x => x.InvitationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AttendanceStatus)
            .WithMany()
            .HasForeignKey(x => x.AttendanceStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.IdentityVerificationStatus)
            .WithMany()
            .HasForeignKey(x => x.IdentityVerificationStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
