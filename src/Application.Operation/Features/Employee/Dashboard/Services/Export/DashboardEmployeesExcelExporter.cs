using Application.Operation.Features.Employee.Common.Export;
using Application.Operation.Features.Employee.Dashboard.DTOs.Employees;
using Application.Operation.Features.Employee.Dashboard.DTOs.Overview;
using ClosedXML.Excel;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Export;

namespace Application.Operation.Features.Employee.Dashboard.Services.Export;

internal sealed class DashboardEmployeesExcelExporter(ILocalizationService localizationService)
{
    public FileExportResult Export(
        DashboardKpisDto summary,
        IReadOnlyList<TeamPerformanceRowDto> employees)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(Text("أداء الفريق", "Team Performance"));
        ExcelExportHelper.ConfigureWorksheet(
            sheet,
            IsArabic,
            Text("الموظف", "Employee"),
            Text("المسند", "Assigned"),
            Text("المكتمل", "Completed"),
            Text("المتبقي", "Remaining"),
            Text("المتأخر", "Overdue"));
        sheet.Cell(2, 7).Value = Text("إجمالي الموظفين", "Total Employees");
        sheet.Cell(2, 8).Value = summary.TotalEmployees;
        sheet.Cell(3, 7).Value = Text("إجمالي الملفات المسندة", "Total Assigned Profiles");
        sheet.Cell(3, 8).Value = summary.TotalAssignedTasks;
        sheet.Cell(4, 7).Value = Text("ملفات بانتظار التوزيع", "Profiles Awaiting Distribution");
        sheet.Cell(4, 8).Value = summary.UnassignedProfiles;
        var row = 2;
        foreach (var item in employees)
        {
            sheet.Cell(row, 1).Value = item.Name;
            sheet.Cell(row, 2).Value = item.AssignedTasks;
            sheet.Cell(row, 3).Value = item.CompletedTasks;
            sheet.Cell(row, 4).Value = item.RemainingTasks;
            sheet.Cell(row++, 5).Value = item.OverdueTasks;
        }
        ExcelExportHelper.FinalizeWorksheet(sheet);
        return ExcelExportHelper.ToFileResult(
            workbook,
            $"{Text("بيانات-الموظفين", "dashboard-employees")}-{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    private bool IsArabic => localizationService.GetCurrentLanguage() == "ar";
    private string Text(string arabic, string english) => localizationService.GetLocalizedValue(arabic, english);
}
