using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Exams;

namespace Tawtheef.Infrastructure.Configurations.Entities.Exams;

public class TestAttemptQuestionConfiguration : BaseEntityConfiguration<TestAttemptQuestion>
{
    public override void Configure(EntityTypeBuilder<TestAttemptQuestion> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => new
            {
                x.TestAttemptPartId,
                x.OrderNo
            })
            .IsUnique();

        builder.HasIndex(x => new
            {
                x.TestAttemptId,
                x.QuestionId
            })
            .IsUnique();

        builder.HasOne(x => x.TestAttempt)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.TestAttemptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TestAttemptPart)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.TestAttemptPartId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ExamCategory)
            .WithMany()
            .HasForeignKey(x => x.ExamCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Question)
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.QuestionRevision)
            .WithMany(x => x.TestAttemptQuestions)
            .HasForeignKey(x => x.QuestionRevisionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SelectedQuestionRevisionOption)
            .WithMany()
            .HasForeignKey(x => x.SelectedQuestionRevisionOptionId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
