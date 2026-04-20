using System.Text;
using System.Text.Json;
using Application.Operation.Features.Employee.Kawader.Commands;
using Application.Operation.Features.Employee.Kawader.DTOs;
using MediatR;
using ExcelDataReader;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Utils;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Kawader;
using Tawtheef.Domain.Entities.Notification;
using Tawtheef.Notifications.Templates.KawaderInvitation;

namespace Application.Operation.Features.Employee.Kawader.Handlers;

public sealed class UploadKawaderUserCommandHandler(
    IUnitOfWork uow,
    IAppLogger logger)
    : IRequestHandler<UploadKawaderUserCommand, IResult<KawaderUploadResultDto>>
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".xlsx", ".xls" };

    static UploadKawaderUserCommandHandler()
        => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    public async Task<IResult<KawaderUploadResultDto>> Handle(
        UploadKawaderUserCommand request,
        CancellationToken ct)
    {
        var validationResult = ValidateFile(request.File);
        if (validationResult.IsFailed)
            return Result.Fail<KawaderUploadResultDto>(validationResult.Errors);

        var rows = await ReadRowsAsync(request.File, ct);
        if (!rows.Any())
            return Result.Fail<KawaderUploadResultDto>(ErrorsCodes.EmptyFile);

        var (validRows, errors) = ValidateRows(rows);

        if (!validRows.Any())
            return Result.Ok(BuildResult(0, rows.Count, errors));

        var existingQids = await GetExistingQids(validRows, ct);

        var importedCount = await PersistAndNotify(validRows, existingQids, errors, ct);

        if (importedCount > 0)
            await uow.SaveChangesAsync(ct);

        return Result.Ok(BuildResult(importedCount, rows.Count, errors));
    }

    #region Pipeline Steps

    private static Result ValidateFile(IFormFile file)
    {
        if (file.Length == 0)
            return Result.Fail(ErrorsCodes.EmptyFile);

        var ext = Path.GetExtension(file.FileName);
        return AllowedExtensions.Contains(ext)
            ? Result.Ok()
            : Result.Fail(ErrorsCodes.InvalidRequest);
    }

    private static (List<RowEntry> Valid, List<KawaderUploadErrorDto> Errors)
        ValidateRows(List<RowEntry> rows)
    {
        var errors = new List<KawaderUploadErrorDto>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var valid = new List<RowEntry>();

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.Qid))
                continue;

            if (!QidUtilities.IsValid(row.Qid))
            {
                errors.Add(new(row.RowNumber, row.Qid, "invalid"));
                continue;
            }

            if (!seen.Add(row.Qid))
            {
                errors.Add(new(row.RowNumber, row.Qid, "duplicate"));
                continue;
            }

            valid.Add(row);
        }

        return (valid, errors);
    }

    private async Task<HashSet<string>> GetExistingQids(
        List<RowEntry> rows,
        CancellationToken ct)
    {
        var qids = rows.Select(r => r.Qid).ToHashSet();

        return await uow.GetEntityRepository<KawaderQid>()
            .DbSet.AsNoTracking()
            .Where(x => qids.Contains(x.Qid))
            .Select(x => x.Qid)
            .ToHashSetAsync(ct);
    }

    private async Task<int> PersistAndNotify(
        List<RowEntry> rows,
        HashSet<string> existing,
        List<KawaderUploadErrorDto> errors,
        CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<KawaderQid>();
        var count = 0;

        foreach (var row in rows)
        {
            if (existing.Contains(row.Qid))
            {
                errors.Add(new(row.RowNumber, row.Qid, "exists"));
                continue;
            }

            await repo.AddAsync(MapEntity(row), ct);
            await SendInvitationSafe(row, ct);

            count++;
        }

        return count;
    }

    #endregion

    #region Mapping

    private static KawaderQid MapEntity(RowEntry row) => new()
    {
        Qid = row.Qid,
        FullName = row.Name,
        Email = row.Email,
        PhoneNumber = row.Phone,
        IsInvited = true,
        InvitedAt = DateTime.UtcNow
    };

    #endregion

    #region Notifications

    private async Task SendInvitationSafe(RowEntry row, CancellationToken ct)
    {
        try
        {
            await SendEmail(row, ct);
            await QueueSms(row, ct);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Invitation failed for QID {Qid}", row.Qid);
        }
    }

    private async Task SendEmail(RowEntry row, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(row.Email)) return;

        var model = new KawaderInvitationModel
        {
            FullName = row.Name ?? "User",
            Qid = row.Qid
        };

        var notification = Notification.Create(
            NotificationChannel.Email,
            KawaderInvitation.TemplateKey,
            null,
            row.Email,
            null,
            null, null, JsonSerializer.Serialize(model),
            $"KawaderInvite_Email_{row.Qid}");
        await uow.GetEntityRepository<Notification>().AddAsync(notification, ct);
        await uow.SaveChangesAsync(ct);
    }

    private async Task QueueSms(RowEntry row, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(row.Phone)) return;
        
        var phone = row.Phone.Trim();

        // Normalize phone number
        if (phone.StartsWith("00974"))
            phone = "+974" + phone[5..];
        else if (phone.StartsWith("974") && phone.Length == 11)
            phone = "+" + phone;
        else if (phone.Length == 8)
            phone = "+974" + phone;
        
        // Validate final format
        if (!MoiUtils.IsQatarMobileNumber(phone))
            return;

        var payload = JsonSerializer.Serialize(new
        {
            FullName = row.Name ?? "User",
            Qid = row.Qid
        });

        var sms = Notification.Create(
            NotificationChannel.Sms,
            KawaderInvitation.TemplateKey,
            null,
            phone,
            null,
            null,
            null,
            payload,
            $"KawaderInvite_Sms_{row.Qid}",
            3,
            "ar");

        await uow.GetEntityRepository<Notification>().AddAsync(sms, ct);
    }

    #endregion

    #region Excel Parsing

    private static async Task<List<RowEntry>> ReadRowsAsync(IFormFile file, CancellationToken ct)
    {
        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream, ct);
        stream.Position = 0;

        using var reader = ExcelReaderFactory.CreateReader(stream);
        if (!reader.NextResult()) return [];

        var header = new HeaderMap(reader);
        return header.ReadRows(reader);
    }

    private sealed class HeaderMap
    {
        private readonly Dictionary<string, int> _map = new(StringComparer.OrdinalIgnoreCase);

        public HeaderMap(IExcelDataReader reader)
        {
            DetectHeaders(reader);
        }

        private void DetectHeaders(IExcelDataReader reader)
        {
            // Skip first 2 rows → start at row 3
            for (int i = 0; i < 3; i++)
            {
                if (!reader.Read())
                    break;
            }
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var val = reader.GetValue(i)?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(val)) continue;

                if (IsHeader(val))
                    _map[val] = i;
            }
        }

        public List<RowEntry> ReadRows(IExcelDataReader reader)
        {
            var rows = new List<RowEntry>();
            var rowNum = 0;

            // Skip first 3 rows → start at row 4
            while (reader.Read())
            {
                rowNum++;

                var qid = Get(reader, ["QID", "الرقم الشخصي", "البطاقة"], 0);
                if (string.IsNullOrWhiteSpace(qid)) continue;

                rows.Add(new RowEntry(
                    rowNum,
                    QidUtilities.Normalize(qid),
                    Get(reader, ["Full Name", "الاسم"]),
                    Get(reader, ["Email", "البريد"]),
                    Get(reader, ["Phone", "Mobile", "الهاتف", "الجوال"])
                ));
            }

            return rows;
        }

        private string? Get(IExcelDataReader reader, string[] keys, int defaultIndex = -1)
        {
            var index = ResolveIndex(keys, defaultIndex);
            return index >= 0 ? reader.GetValue(index)?.ToString()?.Trim() : null;
        }

        private int ResolveIndex(string[] keys, int fallback)
        {
            foreach (var key in keys)
            {
                var match = _map.Keys.FirstOrDefault(k =>
                    k.Contains(key, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                    return _map[match];
            }

            return fallback;
        }

        private static bool IsHeader(string val)
        {
            return val.Contains("QID", StringComparison.OrdinalIgnoreCase) ||
                   val.Contains("Email", StringComparison.OrdinalIgnoreCase) ||
                   val.Contains("Phone", StringComparison.OrdinalIgnoreCase) ||
                   val.Contains("Name", StringComparison.OrdinalIgnoreCase) ||
                   val.Contains("الاسم") ||
                   val.Contains("البريد");
        }
    }

    #endregion

    private static KawaderUploadResultDto BuildResult(
        int imported,
        int processed,
        List<KawaderUploadErrorDto> errors)
        => new()
        {
            ImportedCount = imported,
            ProcessedRows = processed,
            Errors = errors
        };

    private sealed record RowEntry(
        int RowNumber,
        string Qid,
        string? Name,
        string? Email,
        string? Phone);
}
