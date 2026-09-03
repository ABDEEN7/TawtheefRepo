using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

public class QuestionBankRequestItemReviewConfiguration
    : IEntityTypeConfiguration<QuestionBankRequestItemReview>
{
    public void Configure(
        EntityTypeBuilder<QuestionBankRequestItemReview> builder)
    {
        builder.HasIndex(x => new
            {
                x.RequestItemId,
                x.ReviewRound
            })
            .IsUnique();
        
        builder.HasOne(x => x.ReviewedBy)
            .WithMany()
            .HasForeignKey(x => x.ReviewedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RequestItem)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.RequestItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReviewedRevision)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.ReviewedRevisionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Decision)
            .WithMany(x => x.ItemReviews)
            .HasForeignKey(x => x.DecisionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
