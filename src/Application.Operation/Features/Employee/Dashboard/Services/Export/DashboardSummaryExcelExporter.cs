using Application.Operation.Features.Employee.Common.Export;
using Application.Operation.Features.Employee.Dashboard.DTOs.Overview;
using ClosedXML.Excel;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Export;

namespace Application.Operation.Features.Employee.Dashboard.Services.Export;

internal sealed class DashboardSummaryExcelExporter(ILocalizationService localizationService)
{
    public FileExportResult Export(DashboardOverviewDto overview, int selectedYear)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(Text("ملخص لوحة المعلومات", "Dashboard Summary"));
        var previousYear = selectedYear - 1;
        ExcelExportHelper.ConfigureWorksheet(
            sheet,
            IsArabic,
            Text("المؤشر", "Metric"),
            Text($"السنة الحالية ({selectedYear})", $"Current Year ({selectedYear})"),
            Text($"السنة السابقة ({previousYear})", $"Previous Year ({previousYear})"),
            Text("نسبة التغيير", "Change %"));
        var metrics = new (string Label, int Value, DashboardMetricTrendDto Trend)[]
        {
            (Text("إجمالي الملفات", "Total Profiles"), overview.Kpis.TotalProfiles, overview.KpiTrends.TotalProfiles),
            (Text("الملفات المعتمدة", "Approved Profiles"), overview.Kpis.ApprovedProfiles, overview.KpiTrends.ApprovedProfiles),
            (Text("الملفات قيد المراجعة", "Profiles Under Review"), overview.Kpis.UnderReviewProfiles, overview.KpiTrends.UnderReviewProfiles),
            (Text("الملفات بانتظار التوزيع", "Profiles Waiting for Distribution"), overview.Kpis.UnassignedProfiles, overview.KpiTrends.UnassignedProfiles),
            (Text("الوظائف المنشورة", "Published Jobs"), overview.JobKpis.PublishedJobs, overview.KpiTrends.PublishedJobs),
            (Text("إجمالي الدعوات", "Total Invitations"), overview.InvitationKpis.TotalInvitations, overview.KpiTrends.TotalInvitations),
            (Text("الدعوات المقبولة", "Accepted Invitations"), overview.InvitationKpis.AcceptedInvitations, overview.KpiTrends.AcceptedInvitations)
        };
        for (var index = 0; index < metrics.Length; index++)
        {
            var row = index + 2;
            var trend = metrics[index].Trend;
            sheet.Cell(row, 1).Value = metrics[index].Label;
            sheet.Cell(row, 2).Value = metrics[index].Value;
            sheet.Cell(row, 3).Value = trend.PreviousValue;
            if (trend.ChangePercentage.HasValue)
            {
                sheet.Cell(row, 4).Value = trend.ChangePercentage.Value / 100m;
                sheet.Cell(row, 4).Style.NumberFormat.Format = "+0.0%;-0.0%;0%";
            }
            else
            {
                sheet.Cell(row, 4).Value = Text("غير متاح", "N/A");
            }
        }
        ExcelExportHelper.FinalizeWorksheet(sheet);
        return ExcelExportHelper.ToFileResult(
            workbook,
            $"{Text("ملخص-لوحة-المعلومات", "dashboard-summary")}-{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    private bool IsArabic => localizationService.GetCurrentLanguage() == "ar";
    private string Text(string arabic, string english) => localizationService.GetLocalizedValue(arabic, english);
}
