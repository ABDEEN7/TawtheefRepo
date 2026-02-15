using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class JobTitleConfiguration : IEntityTypeConfiguration<JobTitle>
{
    public void Configure(EntityTypeBuilder<JobTitle> builder)
    {
        builder.ToTable(nameof(JobTitle), Schemas.Lookup);

        builder.HasIndex(x => x.JobNumber).IsUnique();

        builder.Property(x => x.JobNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.JobNameAr)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.JobNameEn)
            .HasMaxLength(200)
            .IsRequired();
    }
}
