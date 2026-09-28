using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Common.Services;

public static class QuestionImageUploadPathFactory
{
    public static async Task<QuestionImageUploadPath> CreateAsync(
        Guid userId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var fileId = Guid.NewGuid();
        var extension = Path.GetExtension(file.FileName);
        extension = string.IsNullOrWhiteSpace(extension) ? "bin" : extension;

        var hash = await FileHashing.ComputeSha256Async(file, cancellationToken);
        var path = LocalPathBuilder.QuestionImage(userId, fileId, extension, hash);

        return new QuestionImageUploadPath(fileId, path, hash);
    }
}

public readonly record struct QuestionImageUploadPath(Guid FileId, string Path, string Hash);
