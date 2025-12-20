using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Infrastructure.Configurations.Entities;

public sealed class ProfileChangeRequestConfiguration : IEntityTypeConfiguration<ProfileChangeRequest>
{
    public void Configure(EntityTypeBuilder<ProfileChangeRequest> b)
    {
        b.Property(x => x.RowVersion).IsRowVersion();

        b.HasIndex(x => new { x.UserProfileId, x.Section, x.Status });

        b.HasIndex(x => new { x.UserProfileId, x.TargetKey })
            .IsUnique()
            .HasFilter($"[{nameof(ProfileChangeRequest.Status)}] IN (1,2)"); // Pending=1, UnderReview=2
    }
}
