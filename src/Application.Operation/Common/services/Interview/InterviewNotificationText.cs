using System.Globalization;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Notifications.Templates.InterviewShared;

namespace Application.Operation.Common.Services.Interview;

/// <summary>Per-language strings for interview notifications, kept in one place (subjects and picked names).</summary>
public static class InterviewNotificationText
{
    public static bool IsArabic(string lang) => lang.Equals("ar", StringComparison.OrdinalIgnoreCase);

    /// <summary>Arabic value for "ar", otherwise English, falling back to Arabic when no English value exists.</summary>
    public static string Pick(string lang, string valueAr, string? valueEn) =>
        IsArabic(lang) || string.IsNullOrWhiteSpace(valueEn) ? valueAr : valueEn;

    public static string? FullName(User? user, string lang) =>
        user is null ? null : Pick(lang, user.FullNameAr, user.FullNameEn);

    // Qatar has no daylight saving time, so a fixed offset is exact. Emails must never show UTC times.
    private static readonly TimeSpan QatarOffset = TimeSpan.FromHours(3);

    /// <summary>A UTC timestamp as a Qatar-local date (dd/MM/yyyy, Latin digits in both languages).</summary>
    public static string QatarDate(DateTime utc) =>
        ToQatar(utc).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

    /// <summary>The Qatar-local weekday name in the recipient's language.</summary>
    public static string QatarDayName(DateTime utc, string lang) =>
        ToQatar(utc).ToString("dddd", Culture(lang));

    /// <summary>Qatar-local 12-hour time, e.g. "9:30 AM" / "9:30 ص" (Latin digits).</summary>
    public static string QatarTime(DateTime utc, string lang) =>
        ToQatar(utc).ToString("h:mm tt", Culture(lang));

    private static DateTime ToQatar(DateTime utc) => DateTime.SpecifyKind(utc, DateTimeKind.Utc).Add(QatarOffset);

    // ar-QA formats with Latin digits by default and gives Arabic day names / ص-م; en-US gives AM/PM.
    private static CultureInfo Culture(string lang) =>
        CultureInfo.GetCultureInfo(IsArabic(lang) ? "ar-QA" : "en-US");

    public static string InvitationSubject(bool isReminder, string jobTitle, string lang) => (isReminder, IsArabic(lang)) switch
    {
        (false, true) => $"دعوة لحضور مقابلة: {jobTitle}",
        (true, true) => $"تذكير بموعد المقابلة: {jobTitle}",
        (false, false) => $"Interview invitation: {jobTitle}",
        (true, false) => $"Interview reminder: {jobTitle}"
    };

    public static string ResultReviewSubject(string scheduleName, string lang) =>
        IsArabic(lang)
            ? $"نتائج المقابلات بانتظار مراجعتكم: {scheduleName}"
            : $"Interview results awaiting your review: {scheduleName}";

    public static string AppointmentChangeSubject(InterviewAppointmentChange change, string jobTitle, string lang) =>
        (change, IsArabic(lang)) switch
        {
            (InterviewAppointmentChange.Rescheduled, true) => $"تغيير موعد المقابلة: {jobTitle}",
            (InterviewAppointmentChange.Cancelled, true) => $"إلغاء المقابلة: {jobTitle}",
            (InterviewAppointmentChange.Rescheduled, false) => $"Interview rescheduled: {jobTitle}",
            _ => $"Interview cancelled: {jobTitle}"
        };

    /// <summary>Same wording as the committee screen (INTERVIEW_COMMITTEES.ROLE.*).</summary>
    public static string RoleName(CommitteeRole role, string lang) => (role, IsArabic(lang)) switch
    {
        (CommitteeRole.Chair, true) => "رئيس اللجنة",
        (CommitteeRole.Evaluator, true) => "عضو مُقيّم",
        (CommitteeRole.Observer, true) => "مراقب",
        (CommitteeRole.Admin, true) => "إداري",
        (CommitteeRole.Chair, false) => "Committee Chair",
        (CommitteeRole.Evaluator, false) => "Evaluator",
        (CommitteeRole.Observer, false) => "Observer",
        (CommitteeRole.Admin, false) => "Administrator",
        _ => role.ToString()
    };

    public static string AssignmentSubject(InterviewAssignmentChange change, string committeeName, string lang) =>
        (change, IsArabic(lang)) switch
        {
            (InterviewAssignmentChange.Assigned, true) => $"تعيين في لجنة المقابلة: {committeeName}",
            (InterviewAssignmentChange.Removed, true) => $"إلغاء العضوية في لجنة المقابلة: {committeeName}",
            (InterviewAssignmentChange.ScheduleApproved, true) => $"اعتماد جدول المقابلات للجنة: {committeeName}",
            (InterviewAssignmentChange.ScheduleCancelled, true) => $"إلغاء جدول المقابلات للجنة: {committeeName}",
            (InterviewAssignmentChange.Assigned, false) => $"Assigned to interview committee: {committeeName}",
            (InterviewAssignmentChange.Removed, false) => $"Removed from interview committee: {committeeName}",
            (InterviewAssignmentChange.ScheduleCancelled, false) => $"Interview schedule cancelled for committee: {committeeName}",
            _ => $"Interview schedule approved for committee: {committeeName}"
        };

    public static string ApprovalRequestedSubject(InterviewEntityKind kind, string entityName, string lang) =>
        IsArabic(lang)
            ? $"طلب اعتماد {KindAr(kind)}: {entityName}"
            : $"{KindEn(kind)} approval request: {entityName}";

    public static string DecisionSubject(InterviewEntityKind kind, InterviewDecision decision, string entityName, string lang)
    {
        if (IsArabic(lang))
        {
            var verb = decision switch
            {
                InterviewDecision.Approved => "تم اعتماد",
                InterviewDecision.Returned => "تمت إعادة",
                _ => "تم إلغاء"
            };
            var suffix = decision == InterviewDecision.Returned ? " للتعديل" : "";
            return $"{verb} {KindAr(kind)}{suffix}: {entityName}";
        }

        var outcome = decision switch
        {
            InterviewDecision.Approved => "approved",
            InterviewDecision.Returned => "returned for update",
            _ => "cancelled"
        };
        return $"{KindEn(kind)} {outcome}: {entityName}";
    }

    private static string KindAr(InterviewEntityKind kind) => kind switch
    {
        InterviewEntityKind.Template => "نموذج التقييم",
        InterviewEntityKind.Committee => "لجنة المقابلة",
        InterviewEntityKind.Schedule => "جدول المقابلات",
        InterviewEntityKind.ResultReport => "تقرير نتائج المقابلة",
        _ => ""
    };

    private static string KindEn(InterviewEntityKind kind) => kind switch
    {
        InterviewEntityKind.Template => "Evaluation template",
        InterviewEntityKind.Committee => "Interview committee",
        InterviewEntityKind.Schedule => "Interview schedule",
        InterviewEntityKind.ResultReport => "Interview result report",
        _ => ""
    };
}
