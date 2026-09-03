using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

public class QuestionBankRequestItemConfiguration
    : IEntityTypeConfiguration<Domain.Entities.QuestionsBank.QuestionBankRequestItem>
{
    public void Configure(
        EntityTypeBuilder<Domain.Entities.QuestionsBank.QuestionBankRequestItem> builder)
    {
        builder.HasIndex(x => new
            {
                x.RequestId,
                x.QuestionId
            })
            .IsUnique();

        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Question)
            .WithMany(x => x.RequestItems)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ChangeType)
            .WithMany(x => x.RequestItems)
            .HasForeignKey(x => x.ChangeTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OriginalRevision)
            .WithMany()
            .HasForeignKey(x => x.OriginalRevisionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CurrentProposedRevision)
            .WithMany()
            .HasForeignKey(x => x.CurrentProposedRevisionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany(x => x.RequestItems)
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.QuestionBankAssignment)
            .WithMany(x => x.RequestItems)
            .HasForeignKey(x => x.QuestionBankAssignmentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.RemovedBy)
            .WithMany()
            .HasForeignKey(x => x.RemovedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
