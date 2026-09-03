using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

public class QuestionBankRequestConfiguration
    : IEntityTypeConfiguration<QuestionBankRequest>
{
    public void Configure(
        EntityTypeBuilder<QuestionBankRequest> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasOne(x => x.QuestionBank)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.QuestionBankId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BaseVersion)
            .WithMany()
            .HasForeignKey(x => x.BaseVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RequestType)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.RequestTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.SubmittedBy)
            .WithMany()
            .HasForeignKey(x => x.SubmittedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FinalDecisionBy)
            .WithMany()
            .HasForeignKey(x => x.FinalDecisionById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
