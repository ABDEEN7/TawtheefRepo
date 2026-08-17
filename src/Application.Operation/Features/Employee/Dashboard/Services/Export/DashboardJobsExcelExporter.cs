using Application.Operation.Features.Employee.Common.Export;
using Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;
using ClosedXML.Excel;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Export;

namespace Application.Operation.Features.Employee.Dashboard.Services.Export;

internal sealed class DashboardJobsExcelExporter(ILocalizationService localizationService)
{
    public FileExportResult Export(IReadOnlyList<LatestJobDto> jobs)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(Text("أحدث الوظائف", "Latest Jobs"));
        ExcelExportHelper.ConfigureWorksheet(
            sheet,
            IsArabic,
            Text("الوظيفة", "Job"),
            Text("الإدارة", "Management"),
            Text("الحالة", "Status"),
            Text("الدعوات", "Invitations"),
            Text("المتقدمون", "Applicants"));
        var row = 2;
        foreach (var item in jobs)
        {
            sheet.Cell(row, 1).Value = item.JobTitle;
            sheet.Cell(row, 2).Value = item.ManagementName;
            sheet.Cell(row, 3).Value = DashboardStatusLocalizer.Localize(item.Status, IsArabic);
            sheet.Cell(row, 4).Value = item.InvitationsSent;
            sheet.Cell(row++, 5).Value = item.CandidatesCount;
        }
        ExcelExportHelper.FinalizeWorksheet(sheet);
        return ExcelExportHelper.ToFileResult(
            workbook,
            $"{Text("بيانات-الوظائف", "dashboard-jobs")}-{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    private bool IsArabic => localizationService.GetCurrentLanguage() == "ar";
    private string Text(string arabic, string english) => localizationService.GetLocalizedValue(arabic, english);
}
