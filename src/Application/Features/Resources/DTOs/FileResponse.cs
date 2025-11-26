namespace Tawtheef.Application.Features.Resources.DTOs;

public class FileResponse
{
    public string Path { get; init; } = null!;
    public string ContentType { get; init; } = "application/octet-stream";
    public string DownloadName { get; init; } = null!;
    public bool EnableRangeProcessing { get; init; } = true;
}
