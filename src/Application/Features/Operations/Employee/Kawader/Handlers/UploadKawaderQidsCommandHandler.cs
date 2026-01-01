using System.Text;
using System.IO;
using ExcelDataReader;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Utilities;
using Tawtheef.Application.Features.Operations.Employee.Kawader.Commands;
using Tawtheef.Application.Features.Operations.Employee.Kawader.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Kawader;

namespace Tawtheef.Application.Features.Operations.Employee.Kawader.Handlers;

public sealed class UploadKawaderQidsCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UploadKawaderQidsCommand, IResult<KawaderUploadResultDto>>
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".xlsx", ".xls" };

    static UploadKawaderQidsCommandHandler()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public async Task<IResult<KawaderUploadResultDto>> Handle(UploadKawaderQidsCommand request, CancellationToken ct)
    {
        if (request.File is null || request.File.Length == 0)
            return Result.Fail<KawaderUploadResultDto>(ErrorsCodes.EmptyFile);

        var extension = Path.GetExtension(request.File.FileName);
        if (!AllowedExtensions.Contains(extension))
            return Result.Fail<KawaderUploadResultDto>(ErrorsCodes.InvalidRequest);

        var rows = await ReadRowsAsync(request.File, ct);
        var processedRows = rows.Count(r => !string.IsNullOrWhiteSpace(r.RawValue));

        if (processedRows == 0)
        {
            return Result.Fail<KawaderUploadResultDto>(ErrorsCodes.EmptyFile);
        }

        var errors = new List<KawaderUploadErrorDto>();
        var uniqueRows = new List<RowEntry>();
        var seenQids = new HashSet<string>(StringComparer.Ordinal);

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.RawValue)) continue;

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
            var result = new KawaderUploadResultDto
            {
                ImportedCount = 0,
                ProcessedRows = processedRows,
                Errors = errors
            };

            return Result.Ok(result);
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
            await repo.AddRangeAsync(toInsert);
            await uow.SaveChangesAsync(ct);
        }

        var response = new KawaderUploadResultDto
        {
            ImportedCount = toInsert.Count,
            ProcessedRows = processedRows,
            Errors = errors
        };

        return Result.Ok(response);
    }

    private static async Task<List<RowEntry>> ReadRowsAsync(IFormFile file, CancellationToken ct)
    {
        var rows = new List<RowEntry>();

        await using var memory = new MemoryStream();
        await file.CopyToAsync(memory, ct);
        memory.Position = 0;

        using var reader = ExcelReaderFactory.CreateReader(memory);

        var rowNumber = 0;

        do
        {
            while (reader.Read())
            {
                rowNumber++;
                var rawValue = reader.GetValue(0)?.ToString()?.Trim() ?? string.Empty;
                var normalized = QidUtilities.Normalize(rawValue);

                rows.Add(new RowEntry(rowNumber, rawValue, normalized));
            }
        } while (reader.NextResult());

        return rows;
    }

    private sealed record RowEntry(int RowNumber, string RawValue, string Normalized);
}
