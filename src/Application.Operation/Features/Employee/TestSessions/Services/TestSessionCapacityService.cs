using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSessions.Services;

public static class TestSessionCapacityService
{
    public static readonly Guid[] NonReservingStatusIds =
    [TestSessionStatusIds.Draft, TestSessionStatusIds.Cancelled, TestSessionStatusIds.Rejected];

    public static bool ReservesCapacity(Guid statusId) =>
        !NonReservingStatusIds.Contains(statusId);

    public static bool HasFutureUsableWindow(
        DateOnly slotDate,
        TimeOnly slotStart,
        TimeOnly slotEnd,
        int requiredDurationMinutes,
        DateTime currentDateTime)
    {
        if (requiredDurationMinutes <= 0) return false;

        var start = slotDate.ToDateTime(slotStart);
        var end = slotDate.ToDateTime(slotEnd);
        var earliestStart = start > currentDateTime ? start : currentDateTime;
        return earliestStart.AddMinutes(requiredDurationMinutes) <= end;
    }

    public static bool IsSessionStartCurrentOrFuture(
        DateOnly slotDate,
        TimeOnly sessionStart,
        DateTime currentDateTime) =>
        slotDate.ToDateTime(sessionStart) >= currentDateTime;

    public static int CalculatePeakOccupancy(
        IEnumerable<TestSessionCapacityReservation> reservations,
        TimeOnly start,
        TimeOnly end)
    {
        var events = reservations.Where(reservation => reservation.Start < end && reservation.End > start)
            .SelectMany(reservation => new[]
            {
                (Time: reservation.Start < start ? start : reservation.Start, Delta: reservation.CandidateCount),
                (Time: reservation.End > end ? end : reservation.End, Delta: -reservation.CandidateCount)
            }).OrderBy(item => item.Time).ThenBy(item => item.Delta).ToList();
        var occupancy = 0;
        var peak = 0;
        foreach (var item in events) { occupancy += item.Delta; peak = Math.Max(peak, occupancy); }
        return peak;
    }
}

public sealed record TestSessionCapacityReservation(TimeOnly Start, TimeOnly End, int CandidateCount);
