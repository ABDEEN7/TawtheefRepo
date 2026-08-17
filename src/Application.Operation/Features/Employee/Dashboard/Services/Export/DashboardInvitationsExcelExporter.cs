using Application.Operation.Features.Employee.Common.Export;
using Application.Operation.Features.Employee.Dashboard.DTOs.Invitations;
using ClosedXML.Excel;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Export;

namespace Application.Operation.Features.Employee.Dashboard.Services.Export;

internal sealed class DashboardInvitationsExcelExporter(ILocalizationService localizationService)
{
    public FileExportResult Export(IReadOnlyList<LatestInvitationDto> invitations)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(Text("أحدث الدعوات", "Latest Invitations"));
        ExcelExportHelper.ConfigureWorksheet(
            sheet,
            IsArabic,
            Text("الدعوة", "Invitation"),
            Text("الحالة", "Status"),
            Text("تاريخ الإرسال", "Sent Date"));
        var row = 2;
        foreach (var item in invitations)
        {
            sheet.Cell(row, 1).Value = item.Title;
            sheet.Cell(row, 2).Value = DashboardStatusLocalizer.Localize(item.Status, IsArabic);
            sheet.Cell(row, 3).Value = item.SentDate;
            sheet.Cell(row++, 3).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
        }
        ExcelExportHelper.FinalizeWorksheet(sheet);
        return ExcelExportHelper.ToFileResult(
            workbook,
            $"{Text("بيانات-الدعوات", "dashboard-invitations")}-{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    private bool IsArabic => localizationService.GetCurrentLanguage() == "ar";
    private string Text(string arabic, string english) => localizationService.GetLocalizedValue(arabic, english);
}
