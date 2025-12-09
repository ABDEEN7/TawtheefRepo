using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Common.Services;

public static class FileHashing
{
    public static async Task<string> ComputeSha256Async(IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        using var sha = SHA256.Create();

        var buffer = new byte[8192];
        int read;

        while ((read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), ct).ConfigureAwait(false)) > 0)
        {
            sha.TransformBlock(buffer, 0, read, null, 0);
        }

        sha.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        return Convert.ToHexString(sha.Hash ?? Array.Empty<byte>());
    }
}
