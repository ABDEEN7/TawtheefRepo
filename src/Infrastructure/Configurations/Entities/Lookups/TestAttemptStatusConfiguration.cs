using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class TestAttemptStatusConfiguration : LookupBaseConfiguration<TestAttemptStatus>
{
    public override void Configure(EntityTypeBuilder<TestAttemptStatus> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new TestAttemptStatus
            {
                Id = TestAttemptStatusIds.NotStarted,
                BackendName = nameof(TestAttemptStatusIds.NotStarted),
                NameEn = "Not Started",
                NameAr = "لم يبدأ",
                DisplayOrder = 1
            },
            new TestAttemptStatus
            {
                Id = TestAttemptStatusIds.InProgress,
                BackendName = nameof(TestAttemptStatusIds.InProgress),
                NameEn = "In Progress",
                NameAr = "قيد التنفيذ",
                DisplayOrder = 2
            },
            new TestAttemptStatus
            {
                Id = TestAttemptStatusIds.Interrupted,
                BackendName = nameof(TestAttemptStatusIds.Interrupted),
                NameEn = "Interrupted",
                NameAr = "متوقف",
                DisplayOrder = 3
            },
            new TestAttemptStatus
            {
                Id = TestAttemptStatusIds.Completed,
                BackendName = nameof(TestAttemptStatusIds.Completed),
                NameEn = "Completed",
                NameAr = "مكتمل",
                DisplayOrder = 4
            },
            new TestAttemptStatus
            {
                Id = TestAttemptStatusIds.TimedOut,
                BackendName = nameof(TestAttemptStatusIds.TimedOut),
                NameEn = "Timed Out",
                NameAr = "انتهى الوقت",
                DisplayOrder = 5
            },
            new TestAttemptStatus
            {
                Id = TestAttemptStatusIds.Cancelled,
                BackendName = nameof(TestAttemptStatusIds.Cancelled),
                NameEn = "Cancelled",
                NameAr = "ملغي",
                DisplayOrder = 6
            }
        );
    }
}

