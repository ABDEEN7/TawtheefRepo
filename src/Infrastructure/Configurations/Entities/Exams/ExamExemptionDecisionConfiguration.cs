using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Exams;

namespace Tawtheef.Infrastructure.Configurations.Entities.Exams;

public class ExamExemptionDecisionConfiguration : BaseEntityConfiguration<ExamExemptionDecision>
{
    public override void Configure(EntityTypeBuilder<ExamExemptionDecision> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.Invitation)
            .WithMany()
            .HasForeignKey(x => x.InvitationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

