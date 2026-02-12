using FluentResults;

namespace Tawtheef.Application.Common.Interfaces.Services.Resources;
public sealed record StoredFileStream(
    Stream Stream,
    string ContentType,
    string? ETag,
    DateTimeOffset? LastModified,
    long? Length
);
public record FileSaved(string BlobKey, ulong Size);
public interface IFileStorageService
{
    Task<IResult<FileSaved>> SaveAsync(
        Stream stream, string blobKey, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(string blobKey, CancellationToken ct = default);
    IResult<string> ToPublicUrl(string blobKey); 
    IResult<string> MapPath(string blobKey, bool isReadOperation = false);
    
    Task<bool> ExistsAsync(string path, CancellationToken ct);
    Task<StoredFileStream?> OpenReadAsync(string path, CancellationToken ct);
}
