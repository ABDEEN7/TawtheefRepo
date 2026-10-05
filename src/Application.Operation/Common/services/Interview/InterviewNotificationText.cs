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
