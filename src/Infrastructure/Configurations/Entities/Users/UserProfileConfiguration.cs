using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Configurations.Entities.Users;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public virtual void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.Property(p => p.Status)
            .HasMaxLength(50)
            .HasConversion<string>()
            .HasDefaultValue(UserStatus.InCreation);

        builder
            .HasOne(u => u.ResidenceCountry)
            .WithMany()
            .HasForeignKey(u => u.ResidenceCountryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
