using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class QuestionBankRequestStatusConfiguration : LookupBaseConfiguration<QuestionBankRequestStatus>
{
    public override void Configure(EntityTypeBuilder<QuestionBankRequestStatus> builder)
    {
        base.Configure(builder);

        builder.HasData(
            new QuestionBankRequestStatus
            {
                Id = QuestionBankRequestStatusIds.PendingAssignment,
                BackendName = "PENDING_ASSIGNMENT",
                NameEn = "Pending Assignment",
                NameAr = "بانتظار الإسناد",
                DisplayOrder = 1
            },
            new QuestionBankRequestStatus
            {
                Id = QuestionBankRequestStatusIds.QuestionEntryInProgress,
                BackendName = "QUESTION_ENTRY_IN_PROGRESS",
                NameEn = "Question Entry In Progress",
                NameAr = "قيد إدخال الأسئلة",
                DisplayOrder = 2
            },
            new QuestionBankRequestStatus
            {
                Id = QuestionBankRequestStatusIds.PendingReview,
                BackendName = "PENDING_REVIEW",
                NameEn = "Pending Review",
                NameAr = "بانتظار المراجعة",
                DisplayOrder = 3
            },
            new QuestionBankRequestStatus
            {
                Id = QuestionBankRequestStatusIds.ModificationInProgress,
                BackendName = "MODIFICATION_IN_PROGRESS",
                NameEn = "Modification In Progress",
                NameAr = "قيد التعديل",
                DisplayOrder = 4
            },
            new QuestionBankRequestStatus
            {
                Id = QuestionBankRequestStatusIds.Issued,
                BackendName = "ISSUED",
                NameEn = "Issued",
                NameAr = "تم الإصدار",
                DisplayOrder = 5
            },
            new QuestionBankRequestStatus
            {
                Id = QuestionBankRequestStatusIds.Cancelled,
                BackendName = "CANCELLED",
                NameEn = "Cancelled",
                NameAr = "ملغي",
                DisplayOrder = 6
            },
            new QuestionBankRequestStatus
            {
                Id = QuestionBankRequestStatusIds.Rejected,
                BackendName = "REJECTED",
                NameEn = "Rejected",
                NameAr = "مرفوض",
                DisplayOrder = 7
            }
        );
    }
}
