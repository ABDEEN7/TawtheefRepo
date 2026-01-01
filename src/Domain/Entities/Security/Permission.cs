using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Security;

public static class PermissionIds
{
    // Do NOT change this after first deployment.
    private static readonly Guid Namespace = new("2D7A7C2E-1E17-4A5B-9B58-4C5E2A7D6D10");

    public static Guid FromKey(PermissionKey key) => FromKey(key.Value);

    public static Guid FromKey(string key)
    {
        using var sha1 = SHA1.Create();

        var nsBytes = Namespace.ToByteArray();
        var nameBytes = Encoding.UTF8.GetBytes(key.ToLowerInvariant());

        var data = new byte[nsBytes.Length + nameBytes.Length];
        Buffer.BlockCopy(nsBytes, 0, data, 0, nsBytes.Length);
        Buffer.BlockCopy(nameBytes, 0, data, nsBytes.Length, nameBytes.Length);

        var hash = sha1.ComputeHash(data);
        var bytes = hash[..16];

        // Make it GUID v5-like (stable)
        bytes[6] = (byte)((bytes[6] & 0x0F) | 0x50);
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);

        return new Guid(bytes);
    }
}

[Table(nameof(Permission), Schema = Schemas.Lookup)]
public class Permission : LookupBase
{
}
