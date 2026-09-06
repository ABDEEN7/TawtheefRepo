using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class QuestionBankRequestItemStatusConfiguration : LookupBaseConfiguration<QuestionBankRequestItemStatus>
{
    public override void Configure(EntityTypeBuilder<QuestionBankRequestItemStatus> builder)
    {
        base.Configure(builder);

        builder.HasData(
            new QuestionBankRequestItemStatus
            {
                Id = QuestionBankRequestItemStatusIds.DRAFT,
                BackendName = "DRAFT",
                NameEn = "Draft",
                NameAr = "مسودة",
                DisplayOrder = 1
            },
            new QuestionBankRequestItemStatus
            {
                Id = QuestionBankRequestItemStatusIds.PENDING_REVIEW,
                BackendName = "PENDING_REVIEW",
                NameEn = "Pending Review",
                NameAr = "بانتظار المراجعة",
                DisplayOrder = 2
            },
            new QuestionBankRequestItemStatus
            {
                Id = QuestionBankRequestItemStatusIds.APPROVED,
                BackendName = "APPROVED",
                NameEn = "Approved",
                NameAr = "معتمد",
                DisplayOrder = 3
            },
            new QuestionBankRequestItemStatus
            {
                Id = QuestionBankRequestItemStatusIds.NEEDS_MODIFICATION,
                BackendName = "NEEDS_MODIFICATION",
                NameEn = "Needs Modification",
                NameAr = "يحتاج إلى تعديل",
                DisplayOrder = 4
            },
            new QuestionBankRequestItemStatus
            {
                Id = QuestionBankRequestItemStatusIds.REJECTED,
                BackendName = "REJECTED",
                NameEn = "Rejected",
                NameAr = "مرفوض",
                DisplayOrder = 5
            },
            new QuestionBankRequestItemStatus
            {
                Id = QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST,
                BackendName = "REMOVED_FROM_REQUEST",
                NameEn = "Removed from Request",
                NameAr = "تمت إزالته من الطلب",
                DisplayOrder = 6
            }
        );
    }
}
