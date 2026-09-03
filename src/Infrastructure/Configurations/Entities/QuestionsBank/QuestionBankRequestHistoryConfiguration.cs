using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

public class QuestionBankRequestHistoryConfiguration
    : IEntityTypeConfiguration<QuestionBankRequestHistory>
{
    public void Configure(
        EntityTypeBuilder<QuestionBankRequestHistory> builder)
    {
        builder.Property(x => x.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.RequestId,
            x.PerformedAt
        });

        builder.HasOne(x => x.Request)
            .WithMany(x => x.History)
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FromStatus)
            .WithMany(x => x.FromHistories)
            .HasForeignKey(x => x.FromStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ToStatus)
            .WithMany(x => x.ToHistories)
            .HasForeignKey(x => x.ToStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
