using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class LookupBaseConfiguration<T> : IEntityTypeConfiguration<T> 
    where T : LookupBase
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
        builder.Property(x => x.BackendName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(100).IsRequired();
        builder.Property(x => x.NameEn).HasMaxLength(100).IsRequired();
            
        builder.HasIndex(x => x.BackendName).IsUnique();
        builder.HasIndex(x => x.DisplayOrder);
    }
}
