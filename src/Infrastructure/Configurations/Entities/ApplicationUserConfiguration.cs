using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class ApplicationUserConfiguration<T> : IEntityTypeConfiguration<T> where T : User
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Status)
            .HasMaxLength(50)
            .HasConversion<string>()
            .HasDefaultValue(UserStatus.InCreation);

        builder.HasOne(u => u.UserType)
            .WithMany()
            .HasForeignKey(u => u.UserTypeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
