using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using Application.Operation.Features.Employee.Common.Export;
using ClosedXML.Excel;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Export;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Services;

internal sealed class JobsExcelExporter(ILocalizationService localizationService)
{
    public FileExportResult Export(IReadOnlyList<JobExportRowDto> jobs)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(Text("الوظائف", "Jobs"));
        var headers = new[]
        {
            Text("المسمى الوظيفي بالعربية", "Job Title Arabic"),
            Text("المسمى الوظيفي بالإنجليزية", "Job Title English"),
            Text("فئة الوظيفة", "Job Category"),
            Text("الجنس", "Gender"),
            Text("حالة الوظيفة", "Job Status"),
            Text("القطاع", "Sector"),
            Text("الإدارة", "Management"),
            Text("القسم", "Department"),
            Text("عدد الشواغر", "Number of Vacancies"),
            Text("تاريخ الإغلاق", "Closing Date"),
            Text("أنشأ بواسطة", "Created By"),
            Text("تاريخ آخر إجراء", "Last Action Date")
        };

        ExcelExportHelper.ConfigureWorksheet(sheet, IsArabic, headers);

        for (var index = 0; index < jobs.Count; index++)
        {
            var job = jobs[index];
            var row = index + 2;
            sheet.Cell(row, 1).Value = job.TitleAr;
            sheet.Cell(row, 2).Value = job.TitleEn;
            sheet.Cell(row, 3).Value = Localized(job.JobCategoryAr, job.JobCategoryEn);
            sheet.Cell(row, 4).Value = Localized(job.GenderAr, job.GenderEn);
            sheet.Cell(row, 5).Value = Localized(job.StatusAr, job.StatusEn);
            sheet.Cell(row, 6).Value = Localized(job.SectorAr, job.SectorEn);
            sheet.Cell(row, 7).Value = Localized(job.ManagementAr, job.ManagementEn);
            sheet.Cell(row, 8).Value = Localized(job.DepartmentAr, job.DepartmentEn);
            sheet.Cell(row, 9).Value = job.NumberOfVacancies;
            sheet.Cell(row, 10).Value = job.ClosingDate;
            sheet.Cell(row, 11).Value = Localized(job.CreatedByAr, job.CreatedByEn);
            sheet.Cell(row, 12).Value = job.LastActionDate;
            sheet.Cell(row, 10).Style.DateFormat.Format = "yyyy-MM-dd";
            sheet.Cell(row, 12).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
        }

        ExcelExportHelper.FinalizeWorksheet(sheet);
        return ExcelExportHelper.ToFileResult(
            workbook,
            $"{Text("الوظائف", "jobs")}-{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    private bool IsArabic => localizationService.GetCurrentLanguage() == "ar";
    private string Text(string arabic, string english) =>
        localizationService.GetLocalizedValue(arabic, english);
    private string Localized(string arabic, string english) => IsArabic ? arabic : english;
}
