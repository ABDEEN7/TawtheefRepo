using Tawtheef.Notifications.Attributes;

namespace Tawtheef.Notifications.Templates.CandidateExamRescheduledNotification;

[NotificationTemplate(CandidateExamRescheduledNotification.TemplateKey,
    "تمت إعادة جدولة موعد اختبارك الوظيفي", "Your exam appointment has been rescheduled")]
public sealed record CandidateExamRescheduledNotificationModel(
    string ExamTitleAr, string? ExamTitleEn, DateOnly SlotDate, TimeOnly StartTime,
    string LocationAr, string? LocationEn, string RoomAr, string? RoomEn, int SessionNo);
