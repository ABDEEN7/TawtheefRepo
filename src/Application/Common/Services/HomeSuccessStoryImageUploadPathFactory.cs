using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Common.Services;

public static class HomeSuccessStoryImageUploadPathFactory
{
    public static async Task<HomeSuccessStoryImageUploadPath> CreateAsync(
        Guid storyId,
        IFormFile file,
        bool isPublic,
        CancellationToken ct)
    {
        var fileId = Guid.NewGuid();

        var ext = Path.GetExtension(file.FileName);
        ext = string.IsNullOrWhiteSpace(ext) ? "bin" : ext;

        var hash = await FileHashing.ComputeSha256Async(file, ct);

        var path = LocalPathBuilder.HomeSuccessStoryImage(
            storyId,
            fileId,
            ext,
            hash,
            isPublic);

        return new HomeSuccessStoryImageUploadPath(fileId, path, hash);
    }
}

public readonly record struct HomeSuccessStoryImageUploadPath(Guid FileId, string Path, string Hash);
