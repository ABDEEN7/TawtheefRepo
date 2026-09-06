using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

public class QuestionBankAssignmentConfiguration
    : IEntityTypeConfiguration<QuestionBankAssignment>
{
    public void Configure(
        EntityTypeBuilder<QuestionBankAssignment> builder)
    {
        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.MinimumQuestionCount)
            .IsRequired();

        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasOne(x => x.QuestionBankRequest)
            .WithMany(x => x.Assignments)
            .HasForeignKey(x => x.QuestionBankRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AssignedByUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
            {
                x.QuestionBankRequestId,
                x.EmployeeId
            })
            .IsUnique();
    }
}
