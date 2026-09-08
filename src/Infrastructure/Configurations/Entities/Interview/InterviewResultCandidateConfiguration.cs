using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewResultCandidateConfiguration : BaseEntityConfiguration<InterviewResultCandidate>
{
    public override void Configure(EntityTypeBuilder<InterviewResultCandidate> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.InterviewResultReport)
            .WithMany(r => r.Candidates)
            .HasForeignKey(x => x.InterviewResultReportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.InterviewAppointment)
            .WithMany()
            .HasForeignKey(x => x.InterviewAppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DecidedBy)
            .WithMany()
            .HasForeignKey(x => x.DecidedById)
            .OnDelete(DeleteBehavior.Restrict);

        // One result per conducted appointment.
        builder.HasIndex(x => x.InterviewAppointmentId)
            .IsUnique()
            .HasDatabaseName("UQ_ResultCandidate");
    }
}
