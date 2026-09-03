using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

public class QuestionBankVersionChangeConfiguration
    : IEntityTypeConfiguration<QuestionBankVersionChange>
{
    public void Configure(
        EntityTypeBuilder<QuestionBankVersionChange> builder)
    {
        builder.HasIndex(x => new
            {
                x.ToBankVersionId,
                x.QuestionId
            })
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.QuestionBankId,
            x.ChangedAt
        });

        builder.HasOne(x => x.QuestionBank)
            .WithMany(x => x.VersionChanges)
            .HasForeignKey(x => x.QuestionBankId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ChangeRequest)
            .WithMany()
            .HasForeignKey(x => x.ChangeRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Question)
            .WithMany(x => x.VersionChanges)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ChangeType)
            .WithMany(x => x.VersionChanges)
            .HasForeignKey(x => x.ChangeTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FromBankVersion)
            .WithMany()
            .HasForeignKey(x => x.FromBankVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ToBankVersion)
            .WithMany()
            .HasForeignKey(x => x.ToBankVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OldQuestionRevision)
            .WithMany()
            .HasForeignKey(x => x.OldQuestionRevisionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.NewQuestionRevision)
            .WithMany()
            .HasForeignKey(x => x.NewQuestionRevisionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
