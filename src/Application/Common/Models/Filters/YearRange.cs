namespace Tawtheef.Application.Common.Models.Filters;

public readonly record struct YearRange(DateTime FromUtc, DateTime ToExclusiveUtc)
{
    public const int MinimumYear = 1900;
    public const int MaximumYear = 9998;

    public static bool TryCreate(int? year, out YearRange range)
    {
        if (year is < MinimumYear or > MaximumYear or null)
        {
            range = default;
            return false;
        }

        range = new YearRange(
            new DateTime(year.Value, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(year.Value + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        return true;
    }
}
