using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class TestSessionCandidateStatusConfiguration : LookupBaseConfiguration<TestSessionCandidateStatus>
{
    public override void Configure(EntityTypeBuilder<TestSessionCandidateStatus> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new TestSessionCandidateStatus
            {
                Id = TestSessionCandidateStatusIds.Assigned,
                BackendName = nameof(TestSessionCandidateStatusIds.Assigned),
                NameEn = "Assigned",
                NameAr = "تم التعيين",
                DisplayOrder = 1
            },
            new TestSessionCandidateStatus
            {
                Id = TestSessionCandidateStatusIds.Authorized,
                BackendName = nameof(TestSessionCandidateStatusIds.Authorized),
                NameEn = "Authorized",
                NameAr = "مصرح",
                DisplayOrder = 2
            },
            new TestSessionCandidateStatus
            {
                Id = TestSessionCandidateStatusIds.Started,
                BackendName = nameof(TestSessionCandidateStatusIds.Started),
                NameEn = "Started",
                NameAr = "بدأ",
                DisplayOrder = 3
            },
            new TestSessionCandidateStatus
            {
                Id = TestSessionCandidateStatusIds.Completed,
                BackendName = nameof(TestSessionCandidateStatusIds.Completed),
                NameEn = "Completed",
                NameAr = "مكتمل",
                DisplayOrder = 4
            },
            new TestSessionCandidateStatus
            {
                Id = TestSessionCandidateStatusIds.Rescheduled,
                BackendName = nameof(TestSessionCandidateStatusIds.Rescheduled),
                NameEn = "Rescheduled",
                NameAr = "أعيدت جدولته",
                DisplayOrder = 5
            },
            new TestSessionCandidateStatus
            {
                Id = TestSessionCandidateStatusIds.Cancelled,
                BackendName = nameof(TestSessionCandidateStatusIds.Cancelled),
                NameEn = "Cancelled",
                NameAr = "ملغي",
                DisplayOrder = 6
            }
        );
    }
}

