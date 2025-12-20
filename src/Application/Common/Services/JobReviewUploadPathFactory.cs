using Microsoft.AspNetCore.Http;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Common.Services;

public static class JobReviewUploadPathFactory
{
    public static async Task<JobUploadPath> CreateAsync(
        Guid jobId,
        TabType tab,
        IFormFile file,
        bool isPublic,
        CancellationToken ct)
    {
        var fileId = Guid.NewGuid();

        var ext = Path.GetExtension(file.FileName);
        ext = string.IsNullOrWhiteSpace(ext) ? "bin" : ext;

        var hash = await FileHashing.ComputeSha256Async(file, ct);

        var path = LocalPathBuilder.JobReview(
            jobId,
            tab.ToString().ToLowerInvariant(),
            fileId,
            ext,
            hash,
            isPublic
        );

        return new JobUploadPath(fileId, path, hash);
    }
}

public readonly record struct JobUploadPath(Guid FileId, string Path, string Hash);
