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
        
        builder.HasOne(x => x.UserProfile)
            .WithMany(up => up.UserProfileLoggers)
            .HasForeignKey(x => x.UserProfileId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.PerformedBy)
            .WithMany(u => u.UserProfileLoggers)
            .HasForeignKey(x => x.PerformedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
