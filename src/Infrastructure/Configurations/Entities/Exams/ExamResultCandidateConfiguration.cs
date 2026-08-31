using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Exams;

namespace Tawtheef.Infrastructure.Configurations.Entities.Exams;

public class ExamResultCandidateConfiguration : BaseEntityConfiguration<ExamResultCandidate>
{
    public override void Configure(EntityTypeBuilder<ExamResultCandidate> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.ExamResultReport)
            .WithMany()
            .HasForeignKey(x => x.ExamResultReportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Invitation)
            .WithMany()
            .HasForeignKey(x => x.InvitationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TestAttempt)
            .WithMany()
            .HasForeignKey(x => x.TestAttemptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ResultStatus)
            .WithMany()
            .HasForeignKey(x => x.ResultStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

