// Interview slot times travel as UTC instants. The user picks a day and times of day in their own (browser)
// timezone, so conversion happens here, at the edges: local picks -> ISO instant on the way out, instant ->
// local day/time on the way back in. Calendar days and times of day are never computed server-side.

/** "yyyy-mm-dd" + "HH:mm" in the browser's timezone -> UTC ISO instant. */
export function localToUtcIso(date: string, time: string): string {
  return new Date(`${date}T${time}:00`).toISOString();
}

/** UTC ISO instant -> the browser-local "yyyy-mm-dd" and "HH:mm" it falls on. */
export function utcIsoToLocalParts(iso: string): { date: string; time: string } {
  const d = new Date(iso);
  const pad = (n: number) => String(n).padStart(2, '0');
  return {
    date: `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`,
    time: `${pad(d.getHours())}:${pad(d.getMinutes())}`,
  };
}

export interface SessionRange {
  startAt: string;
  endAt: string;
}

/**
 * What a schedules list row shows: the local days of the first and last session, and the daily hours envelope
 * across them (earliest local start .. latest local end, as Dates on an arbitrary day - only their time is shown).
 */
export interface SessionSummary {
  firstDate: Date;
  lastDate: Date;
  multiDay: boolean;
  earliestStart: Date;
  latestEnd: Date;
}

export function summarizeSessions(sessions: SessionRange[] | null | undefined): SessionSummary | null {
  if (!sessions?.length) return null;

  const starts = sessions.map((s) => new Date(s.startAt)).sort((a, b) => a.getTime() - b.getTime());
  const ends = sessions.map((s) => new Date(s.endAt));
  const minutesOfDay = (d: Date) => d.getHours() * 60 + d.getMinutes();
  const timeOnly = (minutes: number) => new Date(2000, 0, 1, Math.floor(minutes / 60), minutes % 60);

  const firstDate = starts[0];
  const lastDate = starts[starts.length - 1];

  return {
    firstDate,
    lastDate,
    multiDay: firstDate.toDateString() !== lastDate.toDateString(),
    earliestStart: timeOnly(Math.min(...starts.map(minutesOfDay))),
    latestEnd: timeOnly(Math.max(...ends.map(minutesOfDay))),
  };
}
