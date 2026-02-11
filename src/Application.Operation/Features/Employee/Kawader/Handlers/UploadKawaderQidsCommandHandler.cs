using System.Text;
using Application.Operation.Features.Employee.Kawader.Commands;
using Application.Operation.Features.Employee.Kawader.DTOs;
using Cortex.Mediator.Commands;
using ExcelDataReader;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Utils;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Kawader;

namespace Application.Operation.Features.Employee.Kawader.Handlers;

public sealed class UploadKawaderQidsCommandHandler(IUnitOfWork uow)
    : ICommandHandler<UploadKawaderQidsCommand, IResult<KawaderUploadResultDto>>
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".xlsx", ".xls" };

    static UploadKawaderQidsCommandHandler()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public async Task<IResult<KawaderUploadResultDto>> Handle(UploadKawaderQidsCommand request, CancellationToken ct)
    {
        if (request.File.Length == 0)
            return Result.Fail<KawaderUploadResultDto>(ErrorsCodes.EmptyFile);

        var extension = Path.GetExtension(request.File.FileName);
        if (!AllowedExtensions.Contains(extension))
            return Result.Fail<KawaderUploadResultDto>(ErrorsCodes.InvalidRequest);

        // Reads ONLY sheet 2 + skips header rows
        var rows = await ReadRowsFromSecondSheetAsync(request.File, ct);

        var processedRows = rows.Count(r => !string.IsNullOrWhiteSpace(r.RawValue));
        if (processedRows == 0)
            return Result.Fail<KawaderUploadResultDto>(ErrorsCodes.EmptyFile);

        var errors = new List<KawaderUploadErrorDto>();
        var uniqueRows = new List<RowEntry>();
        var seenQids = new HashSet<string>(StringComparer.Ordinal);

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.RawValue))
                continue;

            if (!QidUtilities.IsValid(row.Normalized))
            {
                errors.Add(new KawaderUploadErrorDto(row.RowNumber, row.RawValue, "invalid"));
                continue;
            }

            if (!seenQids.Add(row.Normalized))
            {
                errors.Add(new KawaderUploadErrorDto(row.RowNumber, row.RawValue, "duplicate"));
                continue;
            }

            uniqueRows.Add(row);
        }

        if (uniqueRows.Count == 0)
        {
            return Result.Ok(new KawaderUploadResultDto
            {
                ImportedCount = 0,
                ProcessedRows = processedRows,
                Errors = errors
            });
        }

        var normalizedValues = uniqueRows.Select(r => r.Normalized).ToHashSet(StringComparer.Ordinal);
        var repo = uow.GetEntityRepository<KawaderQid>();

        var existing = await repo.DbSet.AsNoTracking()
            .Where(x => normalizedValues.Contains(x.Qid))
            .Select(x => x.Qid)
            .ToListAsync(ct);

        var existingSet = new HashSet<string>(existing, StringComparer.Ordinal);

        var toInsert = new List<KawaderQid>();

        foreach (var row in uniqueRows)
        {
            if (existingSet.Contains(row.Normalized))
            {
                errors.Add(new KawaderUploadErrorDto(row.RowNumber, row.RawValue, "exists"));
                continue;
            }

            toInsert.Add(new KawaderQid { Qid = row.Normalized });
        }

        if (toInsert.Count > 0)
        {
            await repo.AddRangeAsync(toInsert, ct);
            await uow.SaveChangesAsync(ct);
        }

        return Result.Ok(new KawaderUploadResultDto
        {
            ImportedCount = toInsert.Count,
            ProcessedRows = processedRows,
            Errors = errors
        });
    }

    /// <summary>
    /// Reads ONLY the second worksheet (sheet index 1), and starts after the row whose first cell equals "QID".
    /// If "QID" header is not found, it reads from the first row (no header skip).
    /// </summary>
    private static async Task<List<RowEntry>> ReadRowsFromSecondSheetAsync(IFormFile file, CancellationToken ct)
    {
        await using var memory = new MemoryStream();
        await file.CopyToAsync(memory, ct);
        memory.Position = 0;

        using var reader = ExcelReaderFactory.CreateReader(memory);

        // Move to sheet #2 (index 1)
        // If there is no second sheet, return empty list (so caller will treat as EmptyFile).
        var movedToSecond = reader.NextResult();
        if (!movedToSecond)
            return new List<RowEntry>();

        // First pass: find header row index (where col A == "QID")
        var excelRow = 0;
        var headerRowIndex = -1;

        while (reader.Read())
        {
            excelRow++;
            
            var value = (object?)reader.GetValue(0);
            if(value is null)
                continue;
            
            var a = value.ToString()?.Trim() ?? string.Empty;
            if (a.Equals("QID", StringComparison.OrdinalIgnoreCase))
            {
                headerRowIndex = excelRow;
                break;
            }
        }

        // Reset back to beginning of sheet #2 by recreating the reader (ExcelDataReader is forward-only).
        memory.Position = 0;
        using var reader2 = ExcelReaderFactory.CreateReader(memory);

        // Move again to sheet #2
        if (!reader2.NextResult())
            return new List<RowEntry>();

        var rows = new List<RowEntry>();
        excelRow = 0;

        while (reader2.Read())
        {
            excelRow++;

            // Skip until after header row if found
            if (headerRowIndex > 0 && excelRow <= headerRowIndex)
                continue;

            var value = (object?)reader2.GetValue(0);
            if(value is null)
                continue;
            
            var rawValue = value.ToString()?.Trim() ?? string.Empty;
            var normalized = QidUtilities.Normalize(rawValue);

            rows.Add(new RowEntry(excelRow, rawValue, normalized));
        }

        return rows;
    }

    private sealed record RowEntry(int RowNumber, string RawValue, string Normalized);
}
