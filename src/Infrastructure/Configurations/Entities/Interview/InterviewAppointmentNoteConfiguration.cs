using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewAppointmentNoteConfiguration : BaseEntityConfiguration<InterviewAppointmentNote>
{
    public override void Configure(EntityTypeBuilder<InterviewAppointmentNote> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.InterviewAppointment)
            .WithMany()
            .HasForeignKey(x => x.InterviewAppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AuthorCommitteeMember)
            .WithMany()
            .HasForeignKey(x => x.AuthorCommitteeMemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.NoteType)
            .WithMany()
            .HasForeignKey(x => x.NoteTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
