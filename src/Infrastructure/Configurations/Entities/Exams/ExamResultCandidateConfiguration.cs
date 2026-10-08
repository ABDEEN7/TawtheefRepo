using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Exams;

namespace Tawtheef.Infrastructure.Configurations.Entities.Exams;

public class ExamResultCandidateConfiguration : BaseEntityConfiguration<ExamResultCandidate>
{
    public override void Configure(EntityTypeBuilder<ExamResultCandidate> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.SpecializedScore)
            .HasPrecision(18, 2);

        builder.Property(x => x.EducationalScore)
            .HasPrecision(18, 2);

        builder.Property(x => x.SkillsScore)
            .HasPrecision(18, 2);

        builder.Property(x => x.FinalScore)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.ExamResultReport)
            .WithMany(x => x.Candidates)
            .HasForeignKey(x => x.ExamResultReportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Invitation)
            .WithMany()
            .HasForeignKey(x => x.InvitationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TestSessionCandidate)
            .WithMany()
            .HasForeignKey(x => x.TestSessionCandidateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TestAttempt)
            .WithMany()
            .HasForeignKey(x => x.TestAttemptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ResultStatus)
            .WithMany()
            .HasForeignKey(x => x.ResultStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
            {
                x.ExamResultReportId,
                x.InvitationId
            })
            .IsUnique();
        
        builder.HasIndex(x => x.TestSessionCandidateId)
            .IsUnique()
            .HasFilter("[TestSessionCandidateId] IS NOT NULL");
    }
}
