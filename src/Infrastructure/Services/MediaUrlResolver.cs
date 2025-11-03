using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Configurations;
using Tawtheef.Domain.Configurations.Settings;

namespace Tawtheef.Infrastructure.Services;

public sealed class MediaUrlResolver(IOptions<AppConfigSettings> cfg, IFileStorageService storage) : IMediaUrlResolver
{
    private readonly string _apiBase = $"{cfg.Value.BackendUrl.TrimEnd('/')}/api";

    public string ResolveAbsolute(string? stored)
    {
        if (string.IsNullOrWhiteSpace(stored)) return ""; // caller can fallback to placeholder

        // Absolute URL already?
        if (Uri.TryCreate(stored, UriKind.Absolute, out _)) return stored;

        // BlobKey?
        if (stored.StartsWith("public/", StringComparison.OrdinalIgnoreCase))
        {
            var pub = storage.ToPublicUrl(stored);
            return pub.IsSuccess ? pub.Value! : "";
        }
        if (stored.StartsWith("private/", StringComparison.OrdinalIgnoreCase))
        {
            // Unsigned variant:
            //var enc = EncodeBlobKey(stored);
            //return $"{_apiBase}/resource/{enc}";
            // Or signed variant below via ResolveSigned()
            var ttl = TimeSpan.FromMinutes(cfg.Value.DefaultSignedUrlMinutes);
            return ResolveSigned(stored, ttl);
        }

        // Unknown format; treat as relative and anchor it on API
        return $"{_apiBase}/{stored.TrimStart('/')}";
    }

    public string ResolvePublic(string blobKey)
    {
        var pub = storage.ToPublicUrl(blobKey);
        return pub.IsSuccess ? pub.Value! : "";
    }

    public string ResolvePrivate(string blobKey)
    {
        var enc = EncodeBlobKey(blobKey);
        return $"{_apiBase}/resource/{enc}";
    }

    // OPTIONAL: signed private links to prevent tampering (recommended)
    public string ResolveSigned(string blobKey, TimeSpan ttl)
    {
        var key = cfg.Value.BlobSignKey ?? throw new InvalidOperationException("AppConfig:BlobSignKey missing");
        var encBlob = EncodeBlobKey(blobKey);
        var exp = DateTimeOffset.UtcNow.Add(ttl).ToUnixTimeSeconds();
        var payload = $"{encBlob}.{exp}";
        var sig = ComputeHmacSha256Base64Url(key, payload);
        return $"{_apiBase}/resource/dl?b={encBlob}&exp={exp}&sig={sig}";
    }

    private static string EncodeBlobKey(string blobKey)
        => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(blobKey));

    // HMAC helpers
    private static string ComputeHmacSha256Base64Url(string secret, string data)
    {
        using var h = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var bytes = h.ComputeHash(Encoding.UTF8.GetBytes(data));
        return WebEncoders.Base64UrlEncode(bytes);
    }
}
