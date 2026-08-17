using Application.Operation.Features.Employee.Common.Export;
using Application.Operation.Features.Employee.Dashboard.DTOs.Overview;
using ClosedXML.Excel;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Export;

namespace Application.Operation.Features.Employee.Dashboard.Services.Export;

internal sealed class DashboardCandidatesExcelExporter(ILocalizationService localizationService)
{
    public FileExportResult Export(DashboardOverviewDto overview)
    {
        using var workbook = new XLWorkbook();
        var statusSheet = workbook.Worksheets.Add(Text("حالات الملفات", "Profile Status"));
        ExcelExportHelper.ConfigureWorksheet(statusSheet, IsArabic, Text("حالة الملف", "Profile Status"), Text("العدد", "Count"));
        var row = 2;
        foreach (var item in overview.ProfileBreakdown.ByStatus)
        {
            statusSheet.Cell(row, 1).Value = DashboardStatusLocalizer.Localize(item.Status, IsArabic);
            statusSheet.Cell(row++, 2).Value = item.Count;
        }
        ExcelExportHelper.FinalizeWorksheet(statusSheet);

        var typeSheet = workbook.Worksheets.Add(Text("أنواع المرشحين", "Candidate Types"));
        ExcelExportHelper.ConfigureWorksheet(typeSheet, IsArabic, Text("نوع المرشح", "Candidate Type"), Text("العدد", "Count"));
        row = 2;
        foreach (var item in overview.ProfileBreakdown.ByCandidateType)
        {
            typeSheet.Cell(row, 1).Value = item.Label;
            typeSheet.Cell(row++, 2).Value = item.Count;
        }
        ExcelExportHelper.FinalizeWorksheet(typeSheet);
        return ExcelExportHelper.ToFileResult(
            workbook,
            $"{Text("بيانات-المرشحين", "dashboard-candidates")}-{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    private bool IsArabic => localizationService.GetCurrentLanguage() == "ar";
    private string Text(string arabic, string english) => localizationService.GetLocalizedValue(arabic, english);
}
