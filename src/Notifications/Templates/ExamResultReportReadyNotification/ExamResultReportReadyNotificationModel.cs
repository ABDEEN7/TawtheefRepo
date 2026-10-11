using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.ExamResultReportReadyNotification;

[NotificationTemplate(ExamResultReportReadyNotification.TemplateKey,
    "تقرير نتائج الاختبار جاهز للمراجعة", "Exam result report ready for review")]
public sealed record ExamResultReportReadyNotificationModel(
    string ExamNumber, string ExamTitleAr, string? ExamTitleEn,
    string JobTitleAr, string JobTitleEn, int CandidateCount);
