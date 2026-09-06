using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class TestSessionStatusConfiguration : LookupBaseConfiguration<TestSessionStatus>
{
    public override void Configure(EntityTypeBuilder<TestSessionStatus> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new TestSessionStatus
            {
                Id = TestSessionStatusIds.Draft,
                BackendName = nameof(TestSessionStatusIds.Draft),
                NameEn = "Draft",
                NameAr = "مسودة",
                DisplayOrder = 1
            },
            new TestSessionStatus
            {
                Id = TestSessionStatusIds.Approved,
                BackendName = nameof(TestSessionStatusIds.Approved),
                NameEn = "Approved",
                NameAr = "معتمدة",
                DisplayOrder = 2
            },
            new TestSessionStatus
            {
                Id = TestSessionStatusIds.Ready,
                BackendName = nameof(TestSessionStatusIds.Ready),
                NameEn = "Ready",
                NameAr = "جاهزة",
                DisplayOrder = 3
            },
            new TestSessionStatus
            {
                Id = TestSessionStatusIds.InProgress,
                BackendName = nameof(TestSessionStatusIds.InProgress),
                NameEn = "In Progress",
                NameAr = "قيد التنفيذ",
                DisplayOrder = 4
            },
            new TestSessionStatus
            {
                Id = TestSessionStatusIds.Closed,
                BackendName = nameof(TestSessionStatusIds.Closed),
                NameEn = "Closed",
                NameAr = "مغلقة",
                DisplayOrder = 5
            },
            new TestSessionStatus
            {
                Id = TestSessionStatusIds.Cancelled,
                BackendName = nameof(TestSessionStatusIds.Cancelled),
                NameEn = "Cancelled",
                NameAr = "ملغاة",
                DisplayOrder = 6
            }
        );
    }
}

