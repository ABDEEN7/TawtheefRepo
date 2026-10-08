using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.CandidateExamOperationalIssueReportedNotification;

[NotificationTemplate(CandidateExamOperationalIssueReportedNotification.TemplateKey,
    "بلاغ مشكلة تشغيلية في اختبار", "Exam operational issue reported")]
public sealed record CandidateExamOperationalIssueReportedNotificationModel(
    string ExamNo,
    string ExamTitleAr,
    string? ExamTitleEn,
    string SessionNo,
    Guid TestAttemptId,
    Guid InterruptionId,
    DateTimeOffset ReportedAt);
