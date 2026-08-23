using System.Text.RegularExpressions;

namespace Application.Operation.Features.Employee.Dashboard.Services.Export;

internal static class DashboardStatusLocalizer
{
    public static string Localize(string status, bool isArabic)
    {
        if (!isArabic) return Regex.Replace(status, "(?<!^)([A-Z])", " $1");
        return status switch
        {
            "InCreation" => "قيد الإنشاء",
            "Submitted" => "مقدمة",
            "UnderReview" => "قيد المراجعة",
            "Approved" => "معتمدة",
            "RequiresUpdate" or "Returned" => "معادة",
            "Rejected" => "مرفوضة",
            "Draft" => "مسودة",
            "NeedUpdate" => "تحتاج تحديث",
            "Closed" => "مغلقة",
            "Cancelled" => "ملغاة",
            "PendingApproval" => "بانتظار الاعتماد",
            "PendingPointConfiguration" => "بانتظار توزيع النقاط",
            "NeedPointUpdate" => "نقاط تحتاج للتعديل",
            "PendingPointApproval" => "بانتظار اعتماد النقاط",
            "ReadyForAnnouncement" => "جاهزة للإعلان",
            "Published" => "منشورة",
            _ => status
        };
    }
}
