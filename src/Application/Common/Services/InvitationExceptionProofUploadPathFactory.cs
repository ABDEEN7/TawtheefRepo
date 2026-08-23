using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Common.Services;

public static class InvitationExceptionProofUploadPathFactory
{
    public static async Task<InvitationExceptionProofUploadPath> CreateAsync(
        Guid exceptionId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var fileId = Guid.NewGuid();
        var extension = Path.GetExtension(file.FileName);
        extension = string.IsNullOrWhiteSpace(extension) ? "bin" : extension;

        var hash = await FileHashing.ComputeSha256Async(file, cancellationToken);
        var path = LocalPathBuilder.InvitationExceptionProof(
            exceptionId,
            fileId,
            extension,
            hash);

        return new InvitationExceptionProofUploadPath(fileId, path, hash);
    }
}

public readonly record struct InvitationExceptionProofUploadPath(
    Guid FileId,
    string Path,
    string Hash);
