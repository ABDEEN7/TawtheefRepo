using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Configurations.Entities;

public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> 
    where T : EventEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        builder.HasQueryFilter(x => !x.IsDeleted);
            
        builder.Property(x => x.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
