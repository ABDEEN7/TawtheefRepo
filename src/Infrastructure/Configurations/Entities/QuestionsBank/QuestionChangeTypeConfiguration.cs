using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.QuestionsBank;

public class QuestionChangeTypeConfiguration
    : IEntityTypeConfiguration<QuestionChangeType>
{
    public void Configure(
        EntityTypeBuilder<QuestionChangeType> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.NameAr)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.NameEn)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.BackendName)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.BackendName)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
