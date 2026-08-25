using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public virtual void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.Property(invitation => invitation.Source)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>()
            .HasDefaultValue(InvitationSource.Normal)
            .HasSentinel(0);

        // Keep this filter aligned with CandidateEligibilityRules.ActiveInvitationStatuses.
        builder.HasIndex(invitation => new { invitation.ApplicantId, invitation.JobId })
            .IsUnique()
            .HasDatabaseName("IX_Invitation_ApplicantId_JobId_Active")
            .HasFilter(
                "[IsDeleted] = 0 AND [InvitationStatusId] IN (" +
                "'F0BC801D-F54C-4A0E-8AAE-00694E4FC80D'," +
                "'64236C6A-167A-4213-B1D6-80C2C8C86DDE'," +
                "'6608F560-4DC0-4F2A-A190-6743A9A8C5CB'," +
                "'9B86FA2C-D295-46D7-9092-23AD8AB7B10C'," +
                "'22EF7E86-28CB-4A30-98BC-7D45F9B44DE3')");

        builder
            .HasOne(i => i.InvitationStatus)
            .WithMany()
            .HasForeignKey(i => i.InvitationStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(i => i.History)
            .WithOne(h => h.Invitation)
            .HasForeignKey(h => h.InvitationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
