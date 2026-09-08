using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewAppointmentConfiguration : BaseEntityConfiguration<InterviewAppointment>
{
    public override void Configure(EntityTypeBuilder<InterviewAppointment> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.RemoteMeetingUrl).HasMaxLength(1000);

        builder.HasOne(x => x.InterviewSchedule)
            .WithMany(s => s.Appointments)
            .HasForeignKey(x => x.InterviewScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Invitation)
            .WithMany()
            .HasForeignKey(x => x.InvitationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.InterviewCommittee)
            .WithMany()
            .HasForeignKey(x => x.InterviewCommitteeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RescheduledFromAppointment)
            .WithMany()
            .HasForeignKey(x => x.RescheduledFromAppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t => t.HasCheckConstraint("CK_Appointment_Time", "[EndAt] > [StartAt]"));

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Appointment_Venue",
            "([InterviewType] = 1 AND [RoomId] IS NOT NULL) OR ([InterviewType] = 2 AND [RemoteMeetingUrl] IS NOT NULL)"));

        // Conflict detection support: committee/room/candidate overlap is a domain-service
        // range check (05-invariants.md #6), not expressible as a unique index — this index
        // just makes that check fast.
        builder.HasIndex(x => new { x.InterviewCommitteeId, x.StartAt, x.EndAt })
            .IncludeProperties(x => new { x.RoomId, x.InvitationId })
            .HasFilter("[Status] IN (1, 2, 3, 4, 5)")
            .HasDatabaseName("IX_Appointment_Conflict");
    }
}
