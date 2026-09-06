using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class TestAttemptPartStatusConfiguration : LookupBaseConfiguration<TestAttemptPartStatus>
{
    public override void Configure(EntityTypeBuilder<TestAttemptPartStatus> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new TestAttemptPartStatus
            {
                Id = TestAttemptPartStatusIds.NotStarted,
                BackendName = nameof(TestAttemptPartStatusIds.NotStarted),
                NameEn = "Not Started",
                NameAr = "لم يبدأ",
                DisplayOrder = 1
            },
            new TestAttemptPartStatus
            {
                Id = TestAttemptPartStatusIds.InProgress,
                BackendName = nameof(TestAttemptPartStatusIds.InProgress),
                NameEn = "In Progress",
                NameAr = "قيد التنفيذ",
                DisplayOrder = 2
            },
            new TestAttemptPartStatus
            {
                Id = TestAttemptPartStatusIds.Completed,
                BackendName = nameof(TestAttemptPartStatusIds.Completed),
                NameEn = "Completed",
                NameAr = "مكتمل",
                DisplayOrder = 3
            },
            new TestAttemptPartStatus
            {
                Id = TestAttemptPartStatusIds.TimedOut,
                BackendName = nameof(TestAttemptPartStatusIds.TimedOut),
                NameEn = "Timed Out",
                NameAr = "انتهى الوقت",
                DisplayOrder = 4
            },
            new TestAttemptPartStatus
            {
                Id = TestAttemptPartStatusIds.Interrupted,
                BackendName = nameof(TestAttemptPartStatusIds.Interrupted),
                NameEn = "Interrupted",
                NameAr = "متوقف",
                DisplayOrder = 5
            }
        );
    }
}

