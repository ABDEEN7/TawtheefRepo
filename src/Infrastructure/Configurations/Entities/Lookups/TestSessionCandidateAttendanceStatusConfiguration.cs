using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class TestSessionCandidateAttendanceStatusConfiguration : LookupBaseConfiguration<TestSessionCandidateAttendanceStatus>
{
    public override void Configure(EntityTypeBuilder<TestSessionCandidateAttendanceStatus> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new TestSessionCandidateAttendanceStatus
            {
                Id = TestSessionCandidateAttendanceStatusIds.Pending,
                BackendName = nameof(TestSessionCandidateAttendanceStatusIds.Pending),
                NameEn = "Pending",
                NameAr = "قيد الانتظار",
                DisplayOrder = 1
            },
            new TestSessionCandidateAttendanceStatus
            {
                Id = TestSessionCandidateAttendanceStatusIds.Present,
                BackendName = nameof(TestSessionCandidateAttendanceStatusIds.Present),
                NameEn = "Present",
                NameAr = "حاضر",
                DisplayOrder = 2
            },
            new TestSessionCandidateAttendanceStatus
            {
                Id = TestSessionCandidateAttendanceStatusIds.NoShow,
                BackendName = nameof(TestSessionCandidateAttendanceStatusIds.NoShow),
                NameEn = "No Show",
                NameAr = "لم يحضر",
                DisplayOrder = 3
            }
        );
    }
}

