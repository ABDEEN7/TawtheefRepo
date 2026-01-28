using FluentResults;

namespace Tawtheef.Application.Common.Interfaces.Services.Resources;

public record FileSaved(string BlobKey, ulong Size);
public interface IFileStorageService
{
    Task<IResult<FileSaved>> SaveAsync(
        Stream stream, string blobKey, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(string blobKey, CancellationToken ct = default);
    IResult<string> ToPublicUrl(string blobKey); 
    IResult<string> MapPath(string blobKey);
}
