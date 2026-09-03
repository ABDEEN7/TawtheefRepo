using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

public class QuestionRevisionConfiguration
    : IEntityTypeConfiguration<QuestionRevision>
{
    public void Configure(
        EntityTypeBuilder<QuestionRevision> builder)
    {
        builder.HasIndex(x => new
            {
                x.QuestionId,
                x.RevisionNo
            })
            .IsUnique();

        builder.HasOne(x => x.Question)
            .WithMany(x => x.Revisions)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.QuestionType)
            .WithMany(x => x.QuestionRevisions)
            .HasForeignKey(x => x.QuestionTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DifficultyLevel)
            .WithMany(x => x.QuestionRevisions)
            .HasForeignKey(x => x.DifficultyLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SourceRequestItem)
            .WithMany()
            .HasForeignKey(x => x.SourceRequestItemId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
