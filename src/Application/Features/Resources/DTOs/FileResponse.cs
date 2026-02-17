namespace Tawtheef.Application.Features.Resources.DTOs;
public enum FileSourceKind
{
    LocalPath = 1,
    RedirectUrl = 2
}
public class FileResponse
{
    public required FileSourceKind SourceKind { get; init; } // LocalPath | RedirectUrl | Stream
    public string? LocalPath { get; init; }
    public string? RedirectUrl { get; init; }   // SAS URL
    public string ContentType { get; init; } = "application/octet-stream";
    public string DownloadName { get; init; } = "download";
    public bool EnableRangeProcessing { get; init; }
}
