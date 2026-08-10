namespace Seeds.SeedData;

internal static class SeedDataPath
{
    public static string Get(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        return Path.Combine(
            AppContext.BaseDirectory,
            "SeedData",
            fileName);
    }
}
