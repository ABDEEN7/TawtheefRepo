using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Configurations.Entities.Profile;

public sealed class UserProfileLoggerConfiguration : IEntityTypeConfiguration<UserProfileLogger>
{
    public void Configure(EntityTypeBuilder<UserProfileLogger> builder)
    {
        builder.HasIndex(x => x.UserProfileId);
        builder.HasIndex(x => x.PerformedById);
        builder.HasIndex(x => x.ActionType);
        builder.HasIndex(x => x.ReviewStatus);

        builder.HasOne<UserProfile>()
            .WithMany()
            .HasForeignKey(x => x.UserProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.PerformedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
