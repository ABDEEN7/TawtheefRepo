using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class TestSessionCandidateIdentityVerificationStatusConfiguration : LookupBaseConfiguration<TestSessionCandidateIdentityVerificationStatus>
{
    public override void Configure(EntityTypeBuilder<TestSessionCandidateIdentityVerificationStatus> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new TestSessionCandidateIdentityVerificationStatus
            {
                Id = TestSessionCandidateIdentityVerificationStatusIds.Pending,
                BackendName = nameof(TestSessionCandidateIdentityVerificationStatusIds.Pending),
                NameEn = "Pending",
                NameAr = "قيد الانتظار",
                DisplayOrder = 1
            },
            new TestSessionCandidateIdentityVerificationStatus
            {
                Id = TestSessionCandidateIdentityVerificationStatusIds.Matched,
                BackendName = nameof(TestSessionCandidateIdentityVerificationStatusIds.Matched),
                NameEn = "Matched",
                NameAr = "مطابق",
                DisplayOrder = 2
            },
            new TestSessionCandidateIdentityVerificationStatus
            {
                Id = TestSessionCandidateIdentityVerificationStatusIds.NotMatched,
                BackendName = nameof(TestSessionCandidateIdentityVerificationStatusIds.NotMatched),
                NameEn = "Not Matched",
                NameAr = "غير مطابق",
                DisplayOrder = 3
            }
        );
    }
}

