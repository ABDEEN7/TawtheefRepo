using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Common.Services;

public static class UniversityLogoUploadPathFactory
{
    public static async Task<UniversityLogoUploadPath> CreateAsync(
        Guid universityId,
        string logoType,
        IFormFile file,
        bool isPublic,
        CancellationToken ct)
    {
        var fileId = Guid.NewGuid();

        var ext = Path.GetExtension(file.FileName);
        ext = string.IsNullOrWhiteSpace(ext) ? "bin" : ext;

        var hash = await FileHashing.ComputeSha256Async(file, ct);

        var path = LocalPathBuilder.UniversityLogo(
            universityId,
            logoType,
            fileId,
            ext,
            hash,
            isPublic);

        return new UniversityLogoUploadPath(fileId, path, hash);
    }
}

public readonly record struct UniversityLogoUploadPath(Guid FileId, string Path, string Hash);
