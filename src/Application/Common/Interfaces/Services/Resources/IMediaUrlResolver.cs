namespace Tawtheef.Application.Common.Interfaces.Services.Resources;

public interface IMediaUrlResolver
{
    string ResolveAbsolute(string? storedUrlOrBlobKey);                  // auto-detect
    string ResolvePublic(string blobKey);                                // public/...
    string ResolvePrivate(string blobKey);                               // private/...  → /resource/{encoded}
    string ResolveSigned(string blobKey, TimeSpan ttl);                  // optional: HMAC-signed
}
