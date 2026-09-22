using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class TestSlotStatusConfiguration : LookupBaseConfiguration<TestSlotStatus>
{
    public override void Configure(EntityTypeBuilder<TestSlotStatus> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new TestSlotStatus
            {
                Id = TestSlotStatusIds.Ready,
                BackendName = nameof(TestSlotStatusIds.Ready),
                NameEn = "Ready",
                NameAr = "جاهز",
                DisplayOrder = 1
            },
            new TestSlotStatus
            {
                Id = TestSlotStatusIds.Started,
                BackendName = nameof(TestSlotStatusIds.Started),
                NameEn = "Started",
                NameAr = "بدأ",
                DisplayOrder = 2
            },
            new TestSlotStatus
            {
                Id = TestSlotStatusIds.Closed,
                BackendName = nameof(TestSlotStatusIds.Closed),
                NameEn = "Closed",
                NameAr = "مغلق",
                DisplayOrder = 3
            },
            new TestSlotStatus
            {
                Id = TestSlotStatusIds.Rescheduled,
                BackendName = nameof(TestSlotStatusIds.Rescheduled),
                NameEn = "Rescheduled",
                NameAr = "أعيدت جدولته",
                DisplayOrder = 4
            }
        );
    }
}

