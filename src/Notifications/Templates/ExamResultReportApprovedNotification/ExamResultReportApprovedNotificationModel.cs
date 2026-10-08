using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ExamResultReportApprovedNotification;

[NotificationTemplate(ExamResultReportApprovedNotification.TemplateKey,
    "تم اعتماد تقرير نتائج الاختبار", "Exam result report approved")]
public sealed record ExamResultReportApprovedNotificationModel(
    string ExamNumber, string ExamTitleAr, string? ExamTitleEn,
    string JobTitleAr, string JobTitleEn, int QualifiedCandidateCount);
