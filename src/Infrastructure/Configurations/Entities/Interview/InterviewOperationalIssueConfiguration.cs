using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Infrastructure.Configurations.Entities.Interview;

public sealed class InterviewOperationalIssueConfiguration : BaseEntityConfiguration<InterviewOperationalIssue>
{
    public override void Configure(EntityTypeBuilder<InterviewOperationalIssue> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.InterviewAppointment)
            .WithMany()
            .HasForeignKey(x => x.InterviewAppointmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
