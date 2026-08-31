using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class TestAttemptInterruptionResolutionActionConfiguration
    : LookupBaseConfiguration<TestAttemptInterruptionResolutionAction>
{
    public override void Configure(
        EntityTypeBuilder<TestAttemptInterruptionResolutionAction> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new TestAttemptInterruptionResolutionAction
            {
                Id = TestAttemptInterruptionResolutionActionIds.Resume,
                BackendName = nameof(TestAttemptInterruptionResolutionActionIds.Resume),
                NameEn = "Resume",
                NameAr = "استئناف",
                DisplayOrder = 1,
                IsActive = true
            },
            new TestAttemptInterruptionResolutionAction
            {
                Id = TestAttemptInterruptionResolutionActionIds.Reschedule,
                BackendName = nameof(TestAttemptInterruptionResolutionActionIds.Reschedule),
                NameEn = "Reschedule",
                NameAr = "إعادة الجدولة",
                DisplayOrder = 2,
                IsActive = true
            },
            new TestAttemptInterruptionResolutionAction
            {
                Id = TestAttemptInterruptionResolutionActionIds.Cancel,
                BackendName = nameof(TestAttemptInterruptionResolutionActionIds.Cancel),
                NameEn = "Cancel",
                NameAr = "إلغاء",
                DisplayOrder = 3,
                IsActive = true
            }
        );
    }
}
