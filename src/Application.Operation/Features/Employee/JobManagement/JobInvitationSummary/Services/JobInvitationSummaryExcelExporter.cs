using Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.DTOs;
using Application.Operation.Features.Employee.Common.Export;
using ClosedXML.Excel;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Export;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Services;

internal sealed class JobInvitationSummaryExcelExporter(ILocalizationService localizationService)
{
    public FileExportResult Export(IReadOnlyList<JobInvitationSummaryDto> rows)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(Text("ملخص دعوات الوظائف", "Invitation Summary"));
        var headers = new[]
        {
            Text("الوظيفة", "Job Title"), Text("فئة الوظيفة", "Job Category"),
            Text("القسم / الجهة الطالبة", "Department / Requesting Entity"),
            Text("حالة الوظيفة", "Job Status"), Text("إجمالي الدعوات", "Total Invitations"),
            Text("المتقدمون", "Applicants"), Text("المقروءة", "Read"),
            Text("المرفوضة", "Declined"), Text("غير المشاهدة", "Not Seen"),
            Text("المنتهية", "Expired"), Text("الملغاة", "Cancelled"),
            Text("بانتظار اعتماد المرفق", "Pending Attachment Approval"),
            Text("المرفقات المعادة", "Returned Attachment"),
            Text("دعوات الدفعات السابقة", "Previous Batch Invitations")
        };
        ExcelExportHelper.ConfigureWorksheet(sheet, IsArabic, headers);

        for (var index = 0; index < rows.Count; index++)
        {
            var item = rows[index];
            var row = index + 2;
            sheet.Cell(row, 1).Value = item.JobName;
            sheet.Cell(row, 2).Value = item.JobCategory;
            sheet.Cell(row, 3).Value = item.DepartmentName;
            sheet.Cell(row, 4).Value = item.JobStatus.Name;
            sheet.Cell(row, 5).Value = item.InvitationCount;
            sheet.Cell(row, 6).Value = item.ApplicantsCount;
            sheet.Cell(row, 7).Value = item.ReadCount;
            sheet.Cell(row, 8).Value = item.RefusedCount;
            sheet.Cell(row, 9).Value = item.NotSeenCount;
            sheet.Cell(row, 10).Value = item.ExpiredCount;
            sheet.Cell(row, 11).Value = item.CancelledCount;
            sheet.Cell(row, 12).Value = item.PendingAttachmentApprovalCount;
            sheet.Cell(row, 13).Value = item.ReturnedAttachmentCount;
            sheet.Cell(row, 14).Value = item.PreviousBatchInvitations;
        }

        ExcelExportHelper.FinalizeWorksheet(sheet);
        return ExcelExportHelper.ToFileResult(
            workbook,
            $"{Text("ملخص-دعوات-الوظائف", "job-invitation-summary")}-{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    private bool IsArabic => localizationService.GetCurrentLanguage() == "ar";
    private string Text(string arabic, string english) =>
        localizationService.GetLocalizedValue(arabic, english);
}
