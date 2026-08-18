using Tawtheef.Application.Common.Models.Filters;

namespace Application.Operation.Features.Employee.Dashboard.Services.Time;

internal readonly record struct DashboardDateRange(DateTime FromUtc, DateTime ToExclusiveUtc);

internal static class DashboardTemporalResolver
{
    private const int DefaultLookbackDays = 30;

    public static int ResolveSelectedYear(int? year, DateTime? legacyFromDateUtc)
    {
        if (YearRange.TryCreate(year, out _)) return year!.Value;
        return legacyFromDateUtc?.Year ?? DateTime.UtcNow.Year;
    }

    public static DashboardDateRange ResolveRequestRange(
        int? year,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        DateTime utcNow)
    {
        if (fromDateUtc.HasValue || toDateUtc.HasValue)
        {
            var from = fromDateUtc ?? utcNow.AddDays(-DefaultLookbackDays);
            var to = toDateUtc ?? utcNow;
            return from <= to
                ? new DashboardDateRange(from, to)
                : new DashboardDateRange(to, from);
        }

        if (YearRange.TryCreate(year, out var yearRange))
            return new DashboardDateRange(yearRange.FromUtc, yearRange.ToExclusiveUtc);

        return new DashboardDateRange(utcNow.AddDays(-DefaultLookbackDays), utcNow);
    }
}
