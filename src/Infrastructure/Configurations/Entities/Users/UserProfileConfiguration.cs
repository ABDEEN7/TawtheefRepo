using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Configurations.Entities.Users;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public virtual void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.Property(u => u.Status)
            .HasMaxLength(50)
            .HasConversion<string>()
            .HasDefaultValue(UserProfileStatus.InCreation);
        
        builder
            .HasOne(u => u.ResidenceCountry)
            .WithMany()
            .HasForeignKey(u => u.ResidenceCountryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(nameof(UserProfile.NationalNumber), nameof(UserProfile.NationalityId)).IsUnique();
    }
}
