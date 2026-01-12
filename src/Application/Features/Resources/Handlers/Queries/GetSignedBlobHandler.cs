using System.Security.Cryptography;
using System.Text;
using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Serilog;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Application.Features.Resources.Queries;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Resources.Handlers.Queries;

public class GetSignedBlobHandler(IFileStorageService storage, ILogger logger, IOptions<AppConfigSettings> cfg)
    : IQueryHandler<GetSignedBlobQuery, Result<FileResponse>>
{
    public Task<Result<FileResponse>> Handle(GetSignedBlobQuery request, CancellationToken cancellationToken)
    {
        // 1. secret
        var secret = cfg.Value.BlobSignKey;
        if (string.IsNullOrEmpty(secret))
        {
            logger.Error("Blob sign key is missing from appsettings.json");
            return Task.FromResult(Result.Fail<FileResponse>(ErrorsCodes.BlobSignKeyConfigMissing));
        }

        // 2. verify signature + expiry
        var payload = $"{request.B}.{request.Exp}";
        var expected = ComputeHmacSha256Base64Url(secret, payload);
        if (!CryptographicEquals(expected, request.Sig) || DateTimeOffset.UtcNow.ToUnixTimeSeconds() > request.Exp)
        {
            logger.Error("User try to access blob with invalid signature or expired");
            return Task.FromResult(Result.Fail<FileResponse>(ErrorsCodes.UrlFileExpired));
        }

        // 3. decode and validate key
        var blobKey = Decode(request.B);
        if (!blobKey.StartsWith("private/", StringComparison.OrdinalIgnoreCase))
        {
            logger.Error("User try to access blob with invalid key. Key must start with 'private/': {Key}", blobKey);
            return Task.FromResult(Result.Fail<FileResponse>(ErrorsCodes.OnlyPrivateBlobKeysAllowed));
        }

        // 4. map to path using storage service (preserve original behavior)
        var map = storage.MapPath(blobKey);
        if (!map.IsSuccess)
        {
            logger.Error("Failed to map blob key to path: {Error}", map.Errors);
            return Task.FromResult(Result.Fail<FileResponse>(ErrorsCodes.UnExpectedError));
        }

        var path = map.Value!;
        if (!File.Exists(path))
        {
            logger.Error("File not found: {Path}", path);
            return Task.FromResult(Result.Fail<FileResponse>(ErrorsCodes.FileNotFound));
        }

        // 5. mime
        var contentType = TryGetMimeType(path, out var mt) ? mt : "application/octet-stream";
        var dlName = Path.GetFileName(path);

        var file = new FileResponse
        {
            Path = path,
            ContentType = contentType,
            DownloadName = dlName,
            EnableRangeProcessing = true
        };

        return Task.FromResult(Result.Ok(file));
    }

    // Helpers (kept internal to handler)
    private static bool CryptographicEquals(string a, string b)
    {
        try
        {
            var ba = WebEncoders.Base64UrlDecode(a);
            var bb = WebEncoders.Base64UrlDecode(b);
            if (ba.Length != bb.Length) return false;
            var diff = 0;
            for (var i = 0; i < ba.Length; i++) diff |= ba[i] ^ bb[i];
            return diff == 0;
        }
        catch
        {
            return false;
        }
    }

    private static string ComputeHmacSha256Base64Url(string secret, string data)
    {
        using var h = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var bytes = h.ComputeHash(Encoding.UTF8.GetBytes(data));
        return WebEncoders.Base64UrlEncode(bytes);
    }

    private static string Decode(string input)
    {
        // original Decode implementation ? replace with your actual decoding logic if different
        // assuming URL-safe base64 that was used to encode the blob key
        try
        {
            var bytes = WebEncoders.Base64UrlDecode(input);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            // fallback: return raw input to allow MapPath to reject if it's invalid
            return input;
        }
    }

    private static bool TryGetMimeType(string path, out string mime)
    {
        // try to detect content type; if you have MimeKit or other lib use it.
        mime = null!;
        try
        {
            // Prefer the OS-provided mapping (Windows) or a library; simple fallback:
            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext)) return false;
            // minimal mapping; expand if you want
            switch (ext.ToLowerInvariant())
            {
                case ".pdf": mime = "application/pdf"; return true;
                case ".txt": mime = "text/plain"; return true;
                case ".jpg":
                case ".jpeg": mime = "image/jpeg"; return true;
                case ".png": mime = "image/png"; return true;
                case ".mp4": mime = "video/mp4"; return true;
                default: return false;
            }
        }
        catch
        {
            return false;
        }
    }
}
