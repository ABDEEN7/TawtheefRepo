using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Tawtheef.Application.Common.Interfaces.Services;


public interface IFileStorageService
{
    Task<Result<FileSaved>> SaveAsync(
        Stream stream, string blobKey, CancellationToken ct = default);

    Task<Result<bool>> DeleteAsync(string blobKey, CancellationToken ct = default);

    Task<Result<int>> DeletePrefixAsync(string prefix, CancellationToken ct = default);

    Result<string> ToPublicUrl(string blobKey);  // Ok(url) or Fail(NotPublicResource/ConfigMissing)

    Result<string> MapPath(string blobKey);      // Ok(fullPath) or Fail(InvalidBlobKey)
}

public record FileSaved(string BlobKey, long Size);