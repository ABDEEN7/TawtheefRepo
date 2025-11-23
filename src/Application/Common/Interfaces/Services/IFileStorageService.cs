using FluentResults;

namespace Tawtheef.Application.Common.Interfaces.Services;


public interface IFileStorageService
{
    Task<IResult<FileSaved>> SaveAsync(
        Stream stream, string blobKey, CancellationToken ct = default);

    Task<Result<bool>> DeleteAsync(string blobKey, CancellationToken ct = default);

    Task<Result<int>> DeletePrefixAsync(string prefix, CancellationToken ct = default);

    IResult<string> ToPublicUrl(string blobKey);  // Ok(url) or Fail(NotPublicResource/ConfigMissing)

    IResult<string> MapPath(string blobKey);      // Ok(fullPath) or Fail(InvalidBlobKey)
}

public record FileSaved(string BlobKey, long Size);
