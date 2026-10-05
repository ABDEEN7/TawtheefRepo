import { localToUtcIso, summarizeSessions, utcIsoToLocalParts } from './schedule-time';

// Written against whatever timezone the test browser runs in: local picks are built with localToUtcIso, so the
// assertions hold in Qatar, UTC or anywhere else.
describe('schedule-time', () => {
  it('round-trips a local day + time through a UTC instant', () => {
    const iso = localToUtcIso('2026-10-05', '09:30');

    expect(iso).toMatch(/Z$/);
    expect(new Date(iso).getTime()).toBe(new Date(2026, 9, 5, 9, 30).getTime());
    expect(utcIsoToLocalParts(iso)).toEqual({ date: '2026-10-05', time: '09:30' });
  });

  it('summarizes sessions into local days and the daily hours envelope', () => {
    const summary = summarizeSessions([
      { startAt: localToUtcIso('2026-10-06', '10:00'), endAt: localToUtcIso('2026-10-06', '14:00') },
      { startAt: localToUtcIso('2026-10-05', '09:00'), endAt: localToUtcIso('2026-10-05', '12:00') },
    ])!;

    expect(summary.firstDate.getDate()).toBe(5);
    expect(summary.lastDate.getDate()).toBe(6);
    expect(summary.multiDay).toBeTrue();
    expect([summary.earliestStart.getHours(), summary.earliestStart.getMinutes()]).toEqual([9, 0]);
    expect([summary.latestEnd.getHours(), summary.latestEnd.getMinutes()]).toEqual([14, 0]);
  });

  it('reports a single-day schedule and no sessions', () => {
    const summary = summarizeSessions([
      { startAt: localToUtcIso('2026-10-05', '09:00'), endAt: localToUtcIso('2026-10-05', '10:00') },
      { startAt: localToUtcIso('2026-10-05', '13:00'), endAt: localToUtcIso('2026-10-05', '15:00') },
    ])!;

    expect(summary.multiDay).toBeFalse();
    expect(summarizeSessions([])).toBeNull();
    expect(summarizeSessions(null)).toBeNull();
  });
});
