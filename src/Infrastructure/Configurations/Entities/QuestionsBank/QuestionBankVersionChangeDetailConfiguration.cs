using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

public class QuestionBankVersionChangeDetailConfiguration
    : IEntityTypeConfiguration<QuestionBankVersionChangeDetail>
{
    public void Configure(
        EntityTypeBuilder<QuestionBankVersionChangeDetail> builder)
    {
        builder.Property(x => x.FieldPath)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(x => x.VersionChangeId);

        builder.HasOne(x => x.VersionChange)
            .WithMany(x => x.Details)
            .HasForeignKey(x => x.VersionChangeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
