using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Common.Services;

public static class UserProfileUploadPathFactory
{
    public static async Task<UserProfileUploadPath> CreateAsync(
        Guid userId,
        string category,
        IFormFile file,
        bool isPublic,
        CancellationToken ct)
    {
        var fileId = Guid.NewGuid();
        var ext    = Path.GetExtension(file.FileName);
        ext        = string.IsNullOrWhiteSpace(ext) ? "bin" : ext;

        var hash = await FileHashing.ComputeSha256Async(file, ct);
        var path = LocalPathBuilder.UserProfile(userId, category, fileId, ext, hash, isPublic);

        return new UserProfileUploadPath(fileId, path, hash);
    }
}

public readonly record struct UserProfileUploadPath(Guid FileId, string Path, string Hash);
