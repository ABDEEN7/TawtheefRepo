using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Configurations.Entities;

public class ApplicationUserConfiguration<T> : IEntityTypeConfiguration<T> where T : User
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(u => u.GivenNameEn).HasMaxLength(100).IsRequired();
        builder.Property(u => u.FamilyNameEn).HasMaxLength(100).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
                  
        builder.HasOne(u => u.UserType)
            .WithMany()
            .HasForeignKey(u => u.UserTypeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
