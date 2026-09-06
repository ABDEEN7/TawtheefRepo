using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class RoomStatusConfiguration : LookupBaseConfiguration<RoomStatus>
{
    public override void Configure(EntityTypeBuilder<RoomStatus> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new RoomStatus
            {
                Id = RoomStatusIds.Active,
                BackendName = nameof(RoomStatusIds.Active),
                NameEn = "Active",
                NameAr = "نشطة",
                DisplayOrder = 1
            },
            new RoomStatus
            {
                Id = RoomStatusIds.Inactive,
                BackendName = nameof(RoomStatusIds.Inactive),
                NameEn = "Inactive",
                NameAr = "غير نشطة",
                DisplayOrder = 2
            },
            new RoomStatus
            {
                Id = RoomStatusIds.Maintenance,
                BackendName = nameof(RoomStatusIds.Maintenance),
                NameEn = "Maintenance",
                NameAr = "صيانة",
                DisplayOrder = 3
            }
        );
    }
}

