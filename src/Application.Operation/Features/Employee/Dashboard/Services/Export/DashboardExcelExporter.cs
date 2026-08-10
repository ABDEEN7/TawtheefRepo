using Application.Operation.Features.Employee.Dashboard.DTOs;
using Application.Operation.Features.Employee.Dashboard.DTOs.Employees;
using Application.Operation.Features.Employee.Dashboard.DTOs.Export;
using Application.Operation.Features.Employee.Dashboard.DTOs.Invitations;
using Application.Operation.Features.Employee.Dashboard.DTOs.Jobs;
using Application.Operation.Features.Employee.Dashboard.DTOs.Overview;
using Application.Operation.Features.Employee.Dashboard.Queries.Export;
using ClosedXML.Excel;
using Tawtheef.Application.Common.Interfaces.Services;

namespace Application.Operation.Features.Employee.Dashboard.Services.Export;

internal sealed class DashboardExcelExporter(ILocalizationService localizationService)
{
    public DashboardExportResult ExportCandidates(DashboardOverviewDto overview) =>
        Create(DashboardExportContext.Candidates, workbook =>
        {
            var statusSheet = workbook.Worksheets.Add(Text("حالات الملفات", "Profile Status"));
            Configure(statusSheet, Text("حالة الملف", "Profile Status"), Text("العدد", "Count"));
            var row = 2;
            foreach (var item in overview.ProfileBreakdown.ByStatus)
            {
                statusSheet.Cell(row, 1).Value = LocalizeStatus(item.Status);
                statusSheet.Cell(row++, 2).Value = item.Count;
            }
            Finish(statusSheet);

            var typeSheet = workbook.Worksheets.Add(Text("أنواع المرشحين", "Candidate Types"));
            Configure(typeSheet, Text("نوع المرشح", "Candidate Type"), Text("العدد", "Count"));
            row = 2;
            foreach (var item in overview.ProfileBreakdown.ByCandidateType)
            {
                typeSheet.Cell(row, 1).Value = item.Label;
                typeSheet.Cell(row++, 2).Value = item.Count;
            }
            Finish(typeSheet);
        });

    public DashboardExportResult ExportJobs(IReadOnlyList<LatestJobDto> jobs) =>
        Create(DashboardExportContext.Jobs, workbook =>
        {
            var sheet = workbook.Worksheets.Add(Text("أحدث الوظائف", "Latest Jobs"));
            Configure(sheet, Text("الوظيفة", "Job"), Text("الإدارة", "Management"), Text("الحالة", "Status"),
                Text("الدعوات", "Invitations"), Text("المتقدمون", "Applicants"));
            var row = 2;
            foreach (var item in jobs)
            {
                sheet.Cell(row, 1).Value = item.JobTitle;
                sheet.Cell(row, 2).Value = item.ManagementName;
                sheet.Cell(row, 3).Value = LocalizeStatus(item.Status);
                sheet.Cell(row, 4).Value = item.InvitationsSent;
                sheet.Cell(row++, 5).Value = item.CandidatesCount;
            }
            Finish(sheet);
        });

    public DashboardExportResult ExportEmployees(DashboardKpisDto summary, IReadOnlyList<TeamPerformanceRowDto> employees) =>
        Create(DashboardExportContext.Employees, workbook =>
        {
            var sheet = workbook.Worksheets.Add(Text("أداء الفريق", "Team Performance"));
            Configure(sheet, Text("الموظف", "Employee"), Text("المسند", "Assigned"), Text("المكتمل", "Completed"),
                Text("المتبقي", "Remaining"), Text("المتأخر", "Overdue"));
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
            Finish(sheet);
        });

    public DashboardExportResult ExportInvitations(IReadOnlyList<LatestInvitationDto> invitations) =>
        Create(DashboardExportContext.Invitations, workbook =>
        {
            var sheet = workbook.Worksheets.Add(Text("أحدث الدعوات", "Latest Invitations"));
            Configure(sheet, Text("الدعوة", "Invitation"), Text("الحالة", "Status"), Text("تاريخ الإرسال", "Sent Date"));
            var row = 2;
            foreach (var item in invitations)
            {
                sheet.Cell(row, 1).Value = item.Title;
                sheet.Cell(row, 2).Value = LocalizeStatus(item.Status);
                sheet.Cell(row, 3).Value = item.SentDate;
                sheet.Cell(row++, 3).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
            }
            Finish(sheet);
        });

    private DashboardExportResult Create(DashboardExportContext context, Action<XLWorkbook> populate)
    {
        using var workbook = new XLWorkbook();
        populate(workbook);
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return new DashboardExportResult
        {
            Content = stream.ToArray(),
            FileName = $"{FileName(context)}-{DateTime.UtcNow:yyyyMMdd}.xlsx"
        };
    }

    private void Configure(IXLWorksheet sheet, params string[] headers)
    {
        sheet.RightToLeft = IsArabic;
        for (var column = 0; column < headers.Length; column++) sheet.Cell(1, column + 1).Value = headers[column];
        sheet.Range(1, 1, 1, headers.Length).Style.Font.Bold = true;
    }

    private static void Finish(IXLWorksheet sheet)
    {
        sheet.SheetView.FreezeRows(1);
        sheet.Columns().AdjustToContents();
    }

    private bool IsArabic => localizationService.GetCurrentLanguage() == "ar";
    private string Text(string arabic, string english) => localizationService.GetLocalizedValue(arabic, english);
    private string FileName(DashboardExportContext context) => context switch
    {
        DashboardExportContext.Candidates => Text("بيانات-المرشحين", "dashboard-candidates"),
        DashboardExportContext.Jobs => Text("بيانات-الوظائف", "dashboard-jobs"),
        DashboardExportContext.Employees => Text("بيانات-الموظفين", "dashboard-employees"),
        _ => Text("بيانات-الدعوات", "dashboard-invitations")
    };

    private string LocalizeStatus(string status)
    {
        if (!IsArabic) return System.Text.RegularExpressions.Regex.Replace(status, "(?<!^)([A-Z])", " $1");
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
