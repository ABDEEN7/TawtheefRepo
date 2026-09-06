using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

public class QuestionBankRequestReviewConfiguration
    : IEntityTypeConfiguration<QuestionBankRequestReview>
{
    public void Configure(
        EntityTypeBuilder<QuestionBankRequestReview> builder)
    {
        builder.HasIndex(x => new
            {
                x.RequestId,
                x.ReviewRound
            })
            .IsUnique();

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.ReviewedBy)
            .WithMany()
            .HasForeignKey(x => x.ReviewedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Note)
            .HasMaxLength(2000);
    }
}
