using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class ExamResultCandidateResultStatusConfiguration
    : LookupBaseConfiguration<ExamResultCandidateResultStatus>
{
    public override void Configure(EntityTypeBuilder<ExamResultCandidateResultStatus> builder)
    {
        base.Configure(builder);

        builder.HasData(
            new ExamResultCandidateResultStatus
            {
                Id = ExamResultCandidateResultStatusIds.QualifiedForInterview,
                BackendName = nameof(ExamResultCandidateResultStatusIds.QualifiedForInterview),
                NameEn = "Qualified for Interview",
                NameAr = "مؤهل للمقابلة",
                DisplayOrder = 1
            },
            new ExamResultCandidateResultStatus
            {
                Id = ExamResultCandidateResultStatusIds.NotQualifiedForInterview,
                BackendName = nameof(ExamResultCandidateResultStatusIds.NotQualifiedForInterview),
                NameEn = "Not Qualified for Interview",
                NameAr = "غير مؤهل للمقابلة",
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
                NameAr = "لم يكتمل الاختبار",
                DisplayOrder = 4
            },
            new ExamResultCandidateResultStatus
            {
                Id = ExamResultCandidateResultStatusIds.RequiresAction,
                BackendName = nameof(ExamResultCandidateResultStatusIds.RequiresAction),
                NameEn = "Requires Action",
                NameAr = "بحاجة إلى معالجة",
                DisplayOrder = 5
            }
        );
    }
}
