using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

public class QuestionBankVersionConfiguration
    : IEntityTypeConfiguration<QuestionBankVersion>
{
    public void Configure(
        EntityTypeBuilder<QuestionBankVersion> builder)
    {
        builder.HasIndex(x => new
            {
                x.QuestionBankId,
                x.VersionNo
            })
            .IsUnique();

        builder.HasOne(x => x.QuestionBank)
            .WithMany(x => x.Versions)
            .HasForeignKey(x => x.QuestionBankId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PreviousVersion)
            .WithMany(x => x.NextVersions)
            .HasForeignKey(x => x.PreviousVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedFromRequest)
            .WithMany()
            .HasForeignKey(x => x.CreatedFromRequestId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.CreatedFromRequestId)
            .IsUnique();
    }
}
