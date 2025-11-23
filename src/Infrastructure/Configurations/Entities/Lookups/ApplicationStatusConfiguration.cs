using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class ApplicationStatusConfiguration : LookupBaseConfiguration<InvitationStatus>
{
    public override void Configure(EntityTypeBuilder<InvitationStatus> builder)
    {
        base.Configure(builder);

        builder.HasData(
            // 1 — NEW_INVITATION
            new InvitationStatus
            {
                Id = InvitationStatusIds.NEW_INVITATION,
                BackendName = nameof(InvitationStatusIds.NEW_INVITATION),
                NameEn = "New Invitation",
                NameAr = "دعوة جديدة",
                DescriptionEn = "The candidate was invited but has not opened or viewed it yet.",
                DescriptionAr = "وظيفة تمت دعوة المرشح لها ولم يقم بقراءتها أو فتحها بعد.",
                DisplayOrder = 1
            },

            // 2 — READED
            new InvitationStatus
            {
                Id = InvitationStatusIds.READED,
                BackendName = nameof(InvitationStatusIds.READED),
                NameEn = "Read",
                NameAr = "تمت القراءة",
                DescriptionEn =
                    "The candidate opened the invitation for the first time but has not started the application.",
                DescriptionAr = "المرشح فتح الدعوة لأول مرة ولم يبدأ التقديم.",
                DisplayOrder = 2
            },

            // 3 — SUBMITTED
            new InvitationStatus
            {
                Id = InvitationStatusIds.SUBMITTED,
                BackendName = nameof(InvitationStatusIds.SUBMITTED),
                NameEn = "Submitted",
                NameAr = "تم التقديم",
                DescriptionEn = "The candidate submitted the application with all required information completed.",
                DescriptionAr = "المرشح قدم طلبه وجميع بياناته مكتملة.",
                DisplayOrder = 3
            },

            // 4 — UNDER_REVIEW
            new InvitationStatus
            {
                Id = InvitationStatusIds.UNDER_REVIEW,
                BackendName = nameof(InvitationStatusIds.UNDER_REVIEW),
                NameEn = "Under Review",
                NameAr = "قيد المراجعة",
                DescriptionEn = "The application is under initial review by the recruitment department.",
                DescriptionAr = "الطلب تحت المراجعة المبدئية لقسم التوظيف.",
                DisplayOrder = 4
            },

            // 5 — REQUIRES_UPDATE
            new InvitationStatus
            {
                Id = InvitationStatusIds.REQUIRES_UPDATE,
                BackendName = nameof(InvitationStatusIds.REQUIRES_UPDATE),
                NameEn = "Requires Update",
                NameAr = "مطلوب تعديل",
                DescriptionEn = "The application was returned to the candidate to complete missing information.",
                DescriptionAr = "الطلب تم إرجاعه للمرشح لإكمال نواقص محددة.",
                DisplayOrder = 5
            },

            // 6 — APPROVED
            new InvitationStatus
            {
                Id = InvitationStatusIds.APPROVED,
                BackendName = nameof(InvitationStatusIds.APPROVED),
                NameEn = "Approved",
                NameAr = "معتمد",
                DescriptionEn = "The application has been approved and moved to a later stage.",
                DescriptionAr = "تمت الموافقة على الطلب وانتقل لمرحلة لاحقة.",
                DisplayOrder = 6
            },

            // 7 — REJECTED
            new InvitationStatus
            {
                Id = InvitationStatusIds.REJECTED,
                BackendName = nameof(InvitationStatusIds.REJECTED),
                NameEn = "Rejected",
                NameAr = "مرفوض",
                DescriptionEn = "The application was not accepted for functional or organizational reasons.",
                DescriptionAr = "الطلب لم يتم قبوله لأسباب وظيفية أو تنظيمية.",
                DisplayOrder = 7
            },

            // 8 — CANCELLED
            new InvitationStatus
            {
                Id = InvitationStatusIds.CANCELLED,
                BackendName = nameof(InvitationStatusIds.CANCELLED),
                NameEn = "Cancelled",
                NameAr = "ملغي",
                DescriptionEn = "The candidate cancelled the application or it was cancelled procedurally.",
                DescriptionAr = "المرشح قام بإلغاء الطلب أو تم إلغاؤه وفق الإجراءات.",
                DisplayOrder = 8
            },

            // 9 — CLOSED
            new InvitationStatus
            {
                Id = InvitationStatusIds.CLOSED,
                BackendName = nameof(InvitationStatusIds.CLOSED),
                NameEn = "Closed",
                NameAr = "مغلق",
                DescriptionEn = "The job has ended or was closed by HR and no further action can be taken.",
                DescriptionAr = "الوظيفة انتهت أو أُغلقت من قبل الموارد البشرية ولا يمكن اتخاذ أي إجراء عليها.",
                DisplayOrder = 9
            }
        );
    }

}
