using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class TestAttemptInterruptionStatusConfiguration
    : LookupBaseConfiguration<TestAttemptInterruptionStatus>
{
    public override void Configure(EntityTypeBuilder<TestAttemptInterruptionStatus> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new TestAttemptInterruptionStatus
            {
                Id = TestAttemptInterruptionStatusIds.Open,
                BackendName = nameof(TestAttemptInterruptionStatusIds.Open),
                NameEn = "Open",
                NameAr = "مفتوح",
                DisplayOrder = 1,
                IsActive = true
            },
            new TestAttemptInterruptionStatus
            {
                Id = TestAttemptInterruptionStatusIds.Resolved,
                BackendName = nameof(TestAttemptInterruptionStatusIds.Resolved),
                NameEn = "Resolved",
                NameAr = "تم الحل",
                DisplayOrder = 2,
                IsActive = true
            }
        );
    }
}
