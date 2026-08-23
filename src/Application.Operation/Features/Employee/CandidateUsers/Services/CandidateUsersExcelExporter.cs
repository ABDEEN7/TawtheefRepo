using Application.Operation.Features.Employee.CandidateUsers.DTOs;
using Application.Operation.Features.Employee.Common.Export;
using ClosedXML.Excel;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Export;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.CandidateUsers.Services;

internal sealed class CandidateUsersExcelExporter(ILocalizationService localizationService)
{
    public FileExportResult Export(IReadOnlyList<CandidateUserListItemDto> users)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(Text("مستخدمو المرشحين", "Candidate Users"));
        var headers = new[]
        {
            Text("الاسم بالعربية", "Arabic Name"),
            Text("الاسم بالإنجليزية", "English Name"),
            Text("البريد الإلكتروني", "Email"),
            Text("الرقم الشخصي", "QID"),
            Text("رقم الجوال", "Mobile Number"),
            Text("حالة المستخدم", "User Status"),
            Text("حالة الملف الشخصي", "Profile Status")
        };

        ExcelExportHelper.ConfigureWorksheet(sheet, IsArabic, headers);
        for (var index = 0; index < users.Count; index++)
        {
            var user = users[index];
            var row = index + 2;
            sheet.Cell(row, 1).Value = user.FullNameAr;
            sheet.Cell(row, 2).Value = user.FullNameEn;
            sheet.Cell(row, 3).Value = user.Email;
            sheet.Cell(row, 4).Value = user.Qid ?? string.Empty;
            sheet.Cell(row, 5).Value = user.MobileNumber;
            sheet.Cell(row, 6).Value = user.IsBlocked
                ? Text("محظور", "Blocked")
                : Text("نشط", "Active");
            sheet.Cell(row, 7).Value = LocalizeProfileStatus(user.ProfileStatus);
        }

        ExcelExportHelper.FinalizeWorksheet(sheet);
        return ExcelExportHelper.ToFileResult(
            workbook,
            $"{Text("مستخدمو-المرشحين", "candidate-users")}-{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    private bool IsArabic => localizationService.GetCurrentLanguage() == "ar";
    private string Text(string arabic, string english) =>
        localizationService.GetLocalizedValue(arabic, english);

    private string LocalizeProfileStatus(UserProfileStatus? status) => status switch
    {
        null => string.Empty,
        UserProfileStatus.InCreation => Text("قيد الإنشاء", "In Creation"),
        UserProfileStatus.Submitted => Text("مقدم", "Submitted"),
        UserProfileStatus.UnderReview => Text("قيد المراجعة", "Under Review"),
        UserProfileStatus.RequiresUpdate => Text("يتطلب تحديثًا", "Requires Update"),
        UserProfileStatus.Approved => Text("معتمد", "Approved"),
        _ => status.ToString()!
    };
}
