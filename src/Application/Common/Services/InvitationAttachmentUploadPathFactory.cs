using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Common.Services;

public static class InvitationAttachmentUploadPathFactory
{
    public static async Task<InvitationUploadPath> CreateAsync(
        Guid invitationId,
        IFormFile file,
        bool isPublic,
        CancellationToken ct)
    {
        var fileId = Guid.NewGuid();

        var ext = Path.GetExtension(file.FileName);
        ext = string.IsNullOrWhiteSpace(ext) ? "bin" : ext;

        var hash = await FileHashing.ComputeSha256Async(file, ct);

        var path = LocalPathBuilder.InvitationAttachment(
            invitationId,
            fileId,
            ext,
            hash,
            isPublic
        );

        return new InvitationUploadPath(fileId, path, hash);
    }
}

public readonly record struct InvitationUploadPath(Guid FileId, string Path, string Hash);
