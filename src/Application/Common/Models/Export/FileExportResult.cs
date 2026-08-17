namespace Tawtheef.Application.Common.Models.Export;

public sealed class FileExportResult
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }
    public string ContentType { get; init; } =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}
