using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {

        builder.Property(u => u.Status)
            .HasMaxLength(50)
            .HasConversion<string>()
            .HasDefaultValue(UserProfileStatus.InCreation);
    }
}
