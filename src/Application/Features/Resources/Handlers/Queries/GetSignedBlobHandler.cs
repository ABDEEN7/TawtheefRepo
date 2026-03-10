using System.Security.Cryptography;
using System.Text;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Application.Features.Resources.Queries;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Resources.Handlers.Queries;


public class GetSignedBlobHandler(
    IFileStorageService storage,
    IAppLogger logger,
    IOptions<StorageSettings> storageSettings,
    IOptions<AppConfigSettings> cfg)
    : IRequestHandler<GetSignedBlobQuery, Result<FileResponse>>
{
    public async Task<Result<FileResponse>> Handle(GetSignedBlobQuery request, CancellationToken cancellationToken)
    {
        // 1) Secret
        var secret = cfg.Value.BlobSignKey;
        if (string.IsNullOrWhiteSpace(secret))
        {
            logger.Error("Blob sign key is missing from appsettings.json");
            return Result.Fail<FileResponse>(ErrorsCodes.BlobSignKeyConfigMissing);
        }

        // 2) Verify signature + expiry
        var payload = $"{request.B}.{request.Exp}";
        var expected = ComputeHmacSha256Base64Url(secret, payload);

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (!CryptographicEquals(expected, request.Sig) || now > request.Exp)
        {
            logger.Error("User tried to access blob with invalid signature or expired");
            return Result.Fail<FileResponse>(ErrorsCodes.UrlFileExpired);
        }

        // 3) Decode and validate key
        var blobKey = DecodeBase64UrlUtf8(request.B);
        if (!blobKey.StartsWith("private/", StringComparison.OrdinalIgnoreCase))
        {
            logger.Error("User tried to access blob with invalid key. Must start with 'private/': {Key}", blobKey);
            return Result.Fail<FileResponse>(ErrorsCodes.OnlyPrivateBlobKeysAllowed);
        }

        // 4) Provider-specific response
        if (storageSettings.Value.Provider == nameof(StorageProvider.AzureBlobStorage))
        {
            // TTL bounded by request.Exp (min 30s)
            var ttlSeconds = Math.Max(30, request.Exp - now);
            var downloadName = Path.GetFileName(blobKey);

            var sas = await storage.GetSignedReadUrlAsync(blobKey, TimeSpan.FromSeconds(ttlSeconds), downloadName, cancellationToken);
            if (sas.IsFailed)
            {
                logger.Error("Failed to generate SAS URL for blob: {BlobKey}. Errors: {Errors}", blobKey, sas.Errors);
                return Result.Fail<FileResponse>(ErrorsCodes.UnExpectedError);
            }
            
            var contentType = TryGetMimeType(downloadName, out var mt) ? mt : "application/octet-stream";

            return Result.Ok(new FileResponse
            {
                SourceKind = FileSourceKind.RedirectUrl,
                RedirectUrl = sas.Value,
                ContentType = contentType,
                DownloadName = downloadName,
                EnableRangeProcessing = false
            });
        }

        // Local storage: map to absolute path + verify exists
        var map = storage.MapPath(blobKey, isReadOperation: true);
        if (map.IsFailed)
        {
            logger.Error("Failed to map blob key to local path: {BlobKey}. Errors: {Errors}", blobKey, map.Errors);
            return Result.Fail<FileResponse>(ErrorsCodes.UnExpectedError);
        }

        var path = map.Value!;
        var contentTypeFile = TryGetMimeType(path, out var mtPath) ? mtPath : "application/octet-stream";
        var dlName = Path.GetFileName(path);

        return Result.Ok(new FileResponse
        {
            SourceKind = FileSourceKind.LocalPath,
            LocalPath = path,
            ContentType = contentTypeFile,
            DownloadName = dlName,
            EnableRangeProcessing = true
        });
    }

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

    private static string DecodeBase64UrlUtf8(string input)
    {
        try
        {
            var bytes = WebEncoders.Base64UrlDecode(input);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            // if invalid, return raw to fail validation quickly
            return input;
        }
    }

    private static bool TryGetMimeType(string path, out string mime)
    {
        mime = null!;
        try
        {
            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext)) return false;

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

