namespace Seeds.SeedData;

internal static class SeedValueNormalizer
{
    public static string ToBackendName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var cleaned = new string(
            value
                .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                .ToArray());

        var words = cleaned
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(word =>
                char.ToUpperInvariant(word[0]) +
                word[1..].ToLowerInvariant())
            .ToArray();

        var backendName = string.Join(string.Empty, words);

        return backendName.Length > 50
            ? backendName[..50]
            : backendName;
    }
}
