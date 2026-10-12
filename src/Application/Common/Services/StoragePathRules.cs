namespace Tawtheef.Application.Common.Services;

/// <summary>Portable, relative storage keys with an explicit public/private scope.</summary>
public static class StoragePathRules
{
    public static bool TryPublicKey(string? routePath, out string key)
    {
        key = string.Empty;
        if (string.IsNullOrWhiteSpace(routePath)) return false;
        try
        {
            // Decode the route once, then validate what will reach the filesystem.
            // Remaining escape sequences are rejected by TryNormalizeKey.
            var relative = Uri.UnescapeDataString(routePath).Replace('\\', '/');
            if (relative.StartsWith('/')) return false;
            return TryNormalizeKey("public/" + relative, out key);
        }
        catch (UriFormatException)
        {
            return false;
        }
    }

    public static bool TryNormalizeKey(string? blobKey, out string key)
    {
        key = string.Empty;
        if (string.IsNullOrWhiteSpace(blobKey)) return false;
        var normalized = blobKey.Replace('\\', '/');
        if (normalized.StartsWith('/') || normalized.EndsWith('/')) return false;
        if (normalized.Any(c => char.IsControl(c) || c is ':' or '?' or '*' or '"' or '<' or '>' or '|'))
            return false;
        for (var i = 0; i + 2 < normalized.Length; i++)
            if (normalized[i] == '%' && Uri.IsHexDigit(normalized[i + 1]) && Uri.IsHexDigit(normalized[i + 2]))
                return false; // Never reinterpret a multiply encoded storage key.

        var segments = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length < 2 ||
            (!segments[0].Equals("public", StringComparison.OrdinalIgnoreCase) &&
             !segments[0].Equals("private", StringComparison.OrdinalIgnoreCase)))
            return false;

        foreach (var segment in segments)
        {
            // Windows trims trailing dots/spaces and recognizes device names even
            // with extensions; reject these aliases on every operating system.
            if (segment is "." or ".." || segment.EndsWith('.') || segment.EndsWith(' ')) return false;
            var name = segment.Split('.')[0].TrimEnd(' ');
            if (name.Equals("CON", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("PRN", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("AUX", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("NUL", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("CONIN$", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("CONOUT$", StringComparison.OrdinalIgnoreCase) ||
                (name.Length == 4 &&
                 (name.StartsWith("COM", StringComparison.OrdinalIgnoreCase) ||
                  name.StartsWith("LPT", StringComparison.OrdinalIgnoreCase)) &&
                 (name[3] is >= '0' and <= '9' or '¹' or '²' or '³')))
                return false;
        }

        segments[0] = segments[0].ToLowerInvariant();
        key = string.Join('/', segments);
        return true;
    }
}
