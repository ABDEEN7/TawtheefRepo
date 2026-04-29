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
                Id = InvitationStatusIds.Read,
                BackendName = nameof(InvitationStatusIds.Read),
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
                Id = InvitationStatusIds.ExamEligible,
                BackendName = nameof(InvitationStatusIds.ExamEligible),
                NameEn = "Exam Eligible",
                NameAr = "مرشح للاختبار",
                DescriptionEn = "The application is submitted and the candidate is eligible for the exam.",
                DescriptionAr = "المرشح قدم طلبه وجميع بياناته مكتملة وهو مؤهل للاختبار.",
                DisplayOrder = 3
            },

            // 4 — PENDING_ATTACHMENT_APPROVAL
            new InvitationStatus
            {
                Id = InvitationStatusIds.PendingAttachmentApproval,
                BackendName = nameof(InvitationStatusIds.PendingAttachmentApproval),
                NameEn = "Pending Attachment Approval",
                NameAr = "بانتظار اعتماد المرفقات",
                DescriptionEn = "The candidate successfully uploaded the mandatory attachments and is pending HR approval.",
                DescriptionAr = "قام المرشح برفع المرفقات الإلزامية وهو بانتظار اعتماد الموارد البشرية.",
                DisplayOrder = 4
            },

            // 6 — RETURNED_ATTACHMENT
            new InvitationStatus
            {
                Id = InvitationStatusIds.ReturnedAttachment,
                BackendName = nameof(InvitationStatusIds.ReturnedAttachment),
                NameEn = "Returned Attachment",
                NameAr = "مرفقات معادة",
                DescriptionEn = "The HR returned an attachment to candidate for fix.",
                DescriptionAr = "قامت الموارد البشرية بإعادة مرفق للمرشح للتعديل.",
                DisplayOrder = 5
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
                DisplayOrder = 6
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
                DisplayOrder = 7
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
                DisplayOrder = 8
            }
        );
    }

}
