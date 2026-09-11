using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ExamSubmittedForApprovalNotification;

public static class ExamSubmittedForApprovalNotification
{
    public const string TemplateKey = nameof(ExamSubmittedForApprovalNotification);
}

[NotificationTemplate(
    ExamSubmittedForApprovalNotification.TemplateKey,
    "تم إرسال الاختبار للاعتماد",
    "Exam submitted for approval")]
public sealed record ExamSubmittedForApprovalNotificationModel(
    string ExamNumber,
    string ExamTitleAr,
    string? ExamTitleEn,
    string JobTitleAr,
    string JobTitleEn);
