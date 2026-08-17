using Application.Operation.Features.Employee.Dashboard.Services.Time;
using Tawtheef.Application.Common.Models.Filters;

namespace Application.Operation.Features.Employee.Dashboard.Services.Read;

internal sealed record DashboardYearPeriod(
    int SelectedYear,
    DateTime PreviousFrom,
    DateTime CurrentFrom,
    DateTime CurrentToExclusive)
{
    public static DashboardYearPeriod FromRequest(int? year, DateTime? legacyFromDateUtc)
    {
        var selectedYear = DashboardTemporalResolver.ResolveSelectedYear(year, legacyFromDateUtc);
        var currentRange = YearRange.TryCreate(selectedYear, out var range)
            ? range
            : new YearRange(
                new DateTime(selectedYear, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(selectedYear + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        return new DashboardYearPeriod(
            selectedYear,
            new DateTime(selectedYear - 1, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            currentRange.FromUtc,
            currentRange.ToExclusiveUtc);
    }
}
