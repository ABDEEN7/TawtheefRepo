using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class TestSlotStaffRoleConfiguration : LookupBaseConfiguration<TestSlotStaffRole>
{
    public override void Configure(EntityTypeBuilder<TestSlotStaffRole> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new TestSlotStaffRole
            {
                Id = TestSlotStaffRoleIds.HallSupervisor,
                BackendName = nameof(TestSlotStaffRoleIds.HallSupervisor),
                NameEn = "Hall Supervisor",
                NameAr = "مشرف القاعة",
                DisplayOrder = 1
            },
            new TestSlotStaffRole
            {
                Id = TestSlotStaffRoleIds.Monitor,
                BackendName = nameof(TestSlotStaffRoleIds.Monitor),
                NameEn = "Monitor",
                NameAr = "مراقب",
                DisplayOrder = 2
            },
            new TestSlotStaffRole
            {
                Id = TestSlotStaffRoleIds.Support,
                BackendName = nameof(TestSlotStaffRoleIds.Support),
                NameEn = "Support",
                NameAr = "دعم",
                DisplayOrder = 3
            }
        );
    }
}

