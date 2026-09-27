using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Exams;

namespace Tawtheef.Infrastructure.Configurations.Entities.Exams;

public class CandidateExamConnectionConfiguration : IEntityTypeConfiguration<CandidateExamConnection>
{
    public void Configure(EntityTypeBuilder<CandidateExamConnection> builder)
    {
        builder.HasKey(x => x.ConnectionId);

        builder.Property(x => x.ConnectionId)
            .HasMaxLength(256);

        builder.HasIndex(x => new
        {
            x.TestAttemptId,
            x.ExpiresAt
        });
        
        builder.HasOne(x => x.TestAttempt)
            .WithMany(x => x.Connections)
            .HasForeignKey(x => x.TestAttemptId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
