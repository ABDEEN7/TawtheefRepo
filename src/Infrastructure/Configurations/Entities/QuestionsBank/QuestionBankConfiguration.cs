using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

public class QuestionBankConfiguration
    : IEntityTypeConfiguration<QuestionBank>
{
    public void Configure(
        EntityTypeBuilder<QuestionBank> builder)
    {
        // Specialized:
        // Only one active bank for the same Management + JobTitle.
        builder.HasIndex(x => new
            {
                x.QuestionBankTypeId,
                x.ManagementId,
                x.JobTitleId
            })
            .IsUnique()
            .HasFilter(
                $"[IsDeleted] = 0 AND " +
                $"[QuestionBankTypeId] = '{QuestionBankTypeIds.SPECIALIZED}'");

        // Skills / Educational:
        // Only one active bank from each type in the whole system.
        builder.HasIndex(x => x.QuestionBankTypeId)
            .IsUnique()
            .HasFilter(
                $"[IsDeleted] = 0 AND " +
                $"[QuestionBankTypeId] IN " +
                $"('{QuestionBankTypeIds.SKILLS}', '{QuestionBankTypeIds.EDUCATIONAL}')");

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasOne(x => x.QuestionBankType)
            .WithMany()
            .HasForeignKey(x => x.QuestionBankTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Management)
            .WithMany()
            .HasForeignKey(x => x.ManagementId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.JobTitle)
            .WithMany()
            .HasForeignKey(x => x.JobTitleId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Stage)
            .WithMany(x => x.QuestionBanks)
            .HasForeignKey(x => x.StageId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CurrentApprovedVersion)
            .WithMany()
            .HasForeignKey(x => x.CurrentApprovedVersionId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
