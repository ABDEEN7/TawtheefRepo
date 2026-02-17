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
    Task<IResult<FileSaved>> SaveAsync(Stream stream, string blobKey, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(string blobKey, CancellationToken ct = default);

    IResult<string> ToPublicUrl(string blobKey);

    /// <summary>
    /// Local: maps blobKey to full local file path.
    /// Azure: maps blobKey to normalized blob name (NOT URL).
    /// </summary>
    IResult<string> MapPath(string blobKey, bool isReadOperation = false);

    Task<bool> ExistsAsync(string pathOrBlobName, CancellationToken ct);
    Task<StoredFileStream?> OpenReadAsync(string pathOrBlobName, CancellationToken ct);

    /// <summary>
    /// Azure only (Connection String / Shared Key): returns SAS URL for READ.
    /// Local: can return Fail (not supported).
    /// </summary>
    Task<IResult<string>> GetSignedReadUrlAsync(string blobKey, TimeSpan ttl, string? downloadName = null, CancellationToken ct = default);
}
