using ClosedXML.Excel;
using Tawtheef.Application.Common.Models.Export;

namespace Application.Operation.Features.Employee.Common.Export;

internal static class ExcelExportHelper
{
    public static void ConfigureWorksheet(
        IXLWorksheet sheet,
        bool rightToLeft,
        params string[] headers)
    {
        sheet.RightToLeft = rightToLeft;
        for (var column = 0; column < headers.Length; column++)
            sheet.Cell(1, column + 1).Value = headers[column];
        sheet.Range(1, 1, 1, headers.Length).Style.Font.Bold = true;
    }

    public static void FinalizeWorksheet(IXLWorksheet sheet)
    {
        sheet.SheetView.FreezeRows(1);
        sheet.Columns().AdjustToContents();
    }

    public static FileExportResult ToFileResult(XLWorkbook workbook, string fileName)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return new FileExportResult
        {
            Content = stream.ToArray(),
            FileName = fileName
        };
    }
}
