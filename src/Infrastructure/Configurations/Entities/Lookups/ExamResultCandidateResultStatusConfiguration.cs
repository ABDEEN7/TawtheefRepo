using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class ExamResultCandidateResultStatusConfiguration : LookupBaseConfiguration<ExamResultCandidateResultStatus>
{
    public override void Configure(EntityTypeBuilder<ExamResultCandidateResultStatus> builder)
    {
        base.Configure(builder);
        builder.HasData(
            new ExamResultCandidateResultStatus
            {
                Id = ExamResultCandidateResultStatusIds.Passed,
                BackendName = nameof(ExamResultCandidateResultStatusIds.Passed),
                NameEn = "Passed",
                NameAr = "ناجح",
                DisplayOrder = 1
            },
            new ExamResultCandidateResultStatus
            {
                Id = ExamResultCandidateResultStatusIds.Failed,
                BackendName = nameof(ExamResultCandidateResultStatusIds.Failed),
                NameEn = "Failed",
                NameAr = "راسب",
                DisplayOrder = 2
            },
            new ExamResultCandidateResultStatus
            {
                Id = ExamResultCandidateResultStatusIds.NoShow,
                BackendName = nameof(ExamResultCandidateResultStatusIds.NoShow),
                NameEn = "No Show",
                NameAr = "لم يحضر",
                DisplayOrder = 3
            },
            new ExamResultCandidateResultStatus
            {
                Id = ExamResultCandidateResultStatusIds.NotCompleted,
                BackendName = nameof(ExamResultCandidateResultStatusIds.NotCompleted),
                NameEn = "Not Completed",
                NameAr = "غير مكتمل",
                DisplayOrder = 4
            }
        );
    }
}

