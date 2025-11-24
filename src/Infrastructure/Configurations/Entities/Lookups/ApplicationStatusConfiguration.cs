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
                Id = InvitationStatusIds.NewInvitation,
                BackendName = nameof(InvitationStatusIds.NewInvitation),
                NameEn = "New Invitation",
                NameAr = "دعوة جديدة",
                DescriptionEn = "The candidate was invited but has not opened or viewed it yet.",
                DescriptionAr = "وظيفة تمت دعوة المرشح لها ولم يقم بقراءتها أو فتحها بعد.",
                DisplayOrder = 1
            },

            // 2 — READED
            new InvitationStatus
            {
                Id = InvitationStatusIds.Readed,
                BackendName = nameof(InvitationStatusIds.Readed),
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
                Id = InvitationStatusIds.Submitted,
                BackendName = nameof(InvitationStatusIds.Submitted),
                NameEn = "Submitted",
                NameAr = "تم التقديم",
                DescriptionEn = "The candidate submitted the application with all required information completed.",
                DescriptionAr = "المرشح قدم طلبه وجميع بياناته مكتملة.",
                DisplayOrder = 3
            },

            // 4 — UNDER_REVIEW
            new InvitationStatus
            {
                Id = InvitationStatusIds.UnderReview,
                BackendName = nameof(InvitationStatusIds.UnderReview),
                NameEn = "Under Review",
                NameAr = "قيد المراجعة",
                DescriptionEn = "The application is under initial review by the recruitment department.",
                DescriptionAr = "الطلب تحت المراجعة المبدئية لقسم التوظيف.",
                DisplayOrder = 4
            },

            // 5 — REQUIRES_UPDATE
            new InvitationStatus
            {
                Id = InvitationStatusIds.RequiresUpdate,
                BackendName = nameof(InvitationStatusIds.RequiresUpdate),
                NameEn = "Requires Update",
                NameAr = "مطلوب تعديل",
                DescriptionEn = "The application was returned to the candidate to complete missing information.",
                DescriptionAr = "الطلب تم إرجاعه للمرشح لإكمال نواقص محددة.",
                DisplayOrder = 5
            },

            // 6 — APPROVED
            new InvitationStatus
            {
                Id = InvitationStatusIds.Approved,
                BackendName = nameof(InvitationStatusIds.Approved),
                NameEn = "Approved",
                NameAr = "معتمد",
                DescriptionEn = "The application has been approved and moved to a later stage.",
                DescriptionAr = "تمت الموافقة على الطلب وانتقل لمرحلة لاحقة.",
                DisplayOrder = 6
            },

            // 7 — REJECTED
            new InvitationStatus
            {
                Id = InvitationStatusIds.Rejected,
                BackendName = nameof(InvitationStatusIds.Rejected),
                NameEn = "Rejected",
                NameAr = "مرفوض",
                DescriptionEn = "The application was not accepted for functional or organizational reasons.",
                DescriptionAr = "الطلب لم يتم قبوله لأسباب وظيفية أو تنظيمية.",
                DisplayOrder = 7
            },

            // 8 — CANCELLED
            new InvitationStatus
            {
                Id = InvitationStatusIds.Cancelled,
                BackendName = nameof(InvitationStatusIds.Cancelled),
                NameEn = "Cancelled",
                NameAr = "ملغي",
                DescriptionEn = "The candidate cancelled the application or it was cancelled procedurally.",
                DescriptionAr = "المرشح قام بإلغاء الطلب أو تم إلغاؤه وفق الإجراءات.",
                DisplayOrder = 8
            },

            // 9 — CLOSED
            new InvitationStatus
            {
                Id = InvitationStatusIds.Closed,
                BackendName = nameof(InvitationStatusIds.Closed),
                NameEn = "Closed",
                NameAr = "مغلق",
                DescriptionEn = "The job has ended or was closed by HR and no further action can be taken.",
                DescriptionAr = "الوظيفة انتهت أو أُغلقت من قبل الموارد البشرية ولا يمكن اتخاذ أي إجراء عليها.",
                DisplayOrder = 9
            }
        );
    }

}
