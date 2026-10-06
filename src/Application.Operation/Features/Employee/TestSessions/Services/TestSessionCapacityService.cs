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
        if (end <= start) return 0;
        var events = reservations.Where(reservation => reservation.Start < end && reservation.End > start)
            .SelectMany(reservation => new[]
            {
                (Time: reservation.Start < start ? start : reservation.Start, Delta: reservation.CandidateCount),
                (Time: reservation.End > end ? end : reservation.End, Delta: -reservation.CandidateCount)
            }).OrderBy(item => item.Time).ThenBy(item => item.Delta).ToList();
        var occupancy = 0;
        var peak = 0;
        foreach (var item in events)
        {
            occupancy += item.Delta;
            peak = Math.Max(peak, occupancy);
        }
        return peak;
    }

    public static IReadOnlyList<TestSessionAvailabilityWindow> CalculateAvailabilityWindows(
        IEnumerable<TestSessionCapacityReservation> reservations,
        DateOnly slotDate,
        TimeOnly slotStart,
        TimeOnly slotEnd,
        int roomCapacity,
        int requiredDurationMinutes,
        DateTime currentDateTime)
    {
        if (requiredDurationMinutes <= 0 || slotEnd <= slotStart || roomCapacity <= 0 ||
            slotEnd - slotStart < TimeSpan.FromMinutes(requiredDurationMinutes))
            return [];

        var latestStart = slotEnd.AddMinutes(-requiredDurationMinutes);
        var earliestStart = slotStart;
        if (slotDate == DateOnly.FromDateTime(currentDateTime))
        {
            var currentTime = TimeOnly.FromDateTime(currentDateTime);
            var now = new TimeOnly(currentTime.Hour, currentTime.Minute);
            if (currentTime.Ticks % TimeSpan.TicksPerMinute != 0)
            {
                try { now = now.AddMinutes(1); }
                catch (ArgumentOutOfRangeException) { return []; }
            }
            if (now > earliestStart) earliestStart = now;
        }
        if (earliestStart > latestStart) return [];

        var reservationList = reservations.ToList();
        var boundaries = new SortedSet<TimeOnly> { earliestStart, latestStart };
        foreach (var reservation in reservationList)
        {
            AddBoundary(boundaries, reservation.Start, earliestStart, latestStart);
            AddBoundary(boundaries, reservation.End, earliestStart, latestStart);
            AddBoundary(boundaries, TryAddMinutes(reservation.Start, -requiredDurationMinutes), earliestStart, latestStart);
            AddBoundary(boundaries, TryAddMinutes(reservation.End, -requiredDurationMinutes), earliestStart, latestStart);
        }

        var points = boundaries.ToArray();
        var windows = new List<TestSessionAvailabilityWindow>();
        for (var index = 0; index < points.Length - 1; index++)
        {
            var start = points[index];
            var end = points[index + 1];
            if (end <= start) continue;
            var midpoint = new TimeOnly(start.Ticks + (end.Ticks - start.Ticks) / 2);
            AddWindow(windows, start, end, AvailableSeats(reservationList, midpoint, requiredDurationMinutes, roomCapacity));
        }

        foreach (var boundary in points)
        {
            var availableSeats = AvailableSeats(reservationList, boundary, requiredDurationMinutes, roomCapacity);
            var isRepresented = windows.Any(window => window.AvailableSeats == availableSeats &&
                boundary >= window.StartTime &&
                (boundary < window.EndTime || boundary == latestStart && boundary == window.EndTime));
            if (availableSeats > 0 && !isRepresented)
                windows.Add(new TestSessionAvailabilityWindow(boundary, boundary, availableSeats));
        }
        return windows.OrderBy(window => window.StartTime).ToList();
    }

    private static int AvailableSeats(
        IReadOnlyCollection<TestSessionCapacityReservation> reservations,
        TimeOnly start,
        int durationMinutes,
        int roomCapacity)
    {
        var end = start.AddMinutes(durationMinutes);
        var occupied = CalculatePeakOccupancy(reservations, start, end);
        return Math.Max(0, roomCapacity - occupied);
    }

    private static void AddWindow(
        List<TestSessionAvailabilityWindow> windows,
        TimeOnly start,
        TimeOnly end,
        int availableSeats)
    {
        if (availableSeats <= 0) return;
        if (windows.Count > 0 && windows[^1].AvailableSeats == availableSeats && windows[^1].EndTime == start)
            windows[^1] = windows[^1] with { EndTime = end };
        else
            windows.Add(new TestSessionAvailabilityWindow(start, end, availableSeats));
    }

    private static void AddBoundary(
        ISet<TimeOnly> boundaries,
        TimeOnly? candidate,
        TimeOnly earliestStart,
        TimeOnly latestStart)
    {
        if (candidate.HasValue && candidate.Value > earliestStart && candidate.Value < latestStart)
            boundaries.Add(candidate.Value);
    }

    private static TimeOnly? TryAddMinutes(TimeOnly time, int minutes)
    {
        try { return time.AddMinutes(minutes); }
        catch (ArgumentOutOfRangeException) { return null; }
    }
}

public sealed record TestSessionCapacityReservation(TimeOnly Start, TimeOnly End, int CandidateCount);
public sealed record TestSessionAvailabilityWindow(TimeOnly StartTime, TimeOnly EndTime, int AvailableSeats);
