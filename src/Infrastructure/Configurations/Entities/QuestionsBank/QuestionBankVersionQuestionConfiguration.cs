using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

public class QuestionBankVersionQuestionConfiguration
    : IEntityTypeConfiguration<QuestionBankVersionQuestion>
{
    public void Configure(
        EntityTypeBuilder<QuestionBankVersionQuestion> builder)
    {
        builder.HasIndex(x => new
            {
                x.QuestionBankVersionId,
                x.QuestionId
            })
            .IsUnique();

        builder.HasOne(x => x.QuestionBankVersion)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.QuestionBankVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Question)
            .WithMany(x => x.BankVersionQuestions)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.QuestionRevision)
            .WithMany(x => x.BankVersionQuestions)
            .HasForeignKey(x => x.QuestionRevisionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SourceRequestItem)
            .WithMany()
            .HasForeignKey(x => x.SourceRequestItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
