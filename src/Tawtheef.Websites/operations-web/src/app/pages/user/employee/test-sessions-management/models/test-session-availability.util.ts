import {
  ReadyTestSlotListItemDto,
  TestSessionAvailabilityWindowDto,
} from './ready-test-slot.dto';

export function calculateAvailableSeatsForInterval(
  slot: ReadyTestSlotListItemDto,
  startTime: string,
  endTime: string,
  examDurationMinutes: number,
): number {
  const start = timeValue(startTime);
  const end = timeValue(endTime);
  const lastDurationStart = end - examDurationMinutes;
  const latestSlotStart = timeValue(slot.endTime) - examDurationMinutes;
  if (
    examDurationMinutes <= 0 ||
    start < timeValue(slot.startTime) ||
    end > timeValue(slot.endTime) ||
    lastDurationStart < start
  ) {
    return 0;
  }

  const points = new Set<number>([start, lastDurationStart]);
  for (const window of slot.availabilityWindows) {
    addPoint(points, timeValue(window.startTime), start, lastDurationStart);
    addPoint(points, timeValue(window.endTime), start, lastDurationStart);
  }
  const boundaries = [...points].sort((left, right) => left - right);
  const capacities = boundaries.map((point) => seatsAt(slot.availabilityWindows, point, latestSlotStart));
  for (let index = 0; index < boundaries.length - 1; index++) {
    const midpoint = (boundaries[index] + boundaries[index + 1]) / 2;
    capacities.push(seatsAt(slot.availabilityWindows, midpoint, latestSlotStart));
  }
  return capacities.length > 0 ? Math.min(...capacities) : 0;
}

function addPoint(points: Set<number>, point: number, start: number, end: number): void {
  if (point > start && point < end) points.add(point);
}

function seatsAt(
  windows: TestSessionAvailabilityWindowDto[],
  start: number,
  latestSlotStart: number,
): number {
  return windows.reduce((highest, window) => {
    const windowStart = timeValue(window.startTime);
    const windowEnd = timeValue(window.endTime);
    const containsStart =
      start >= windowStart &&
      (start < windowEnd ||
        (start === windowEnd && start === latestSlotStart) ||
        (start === windowStart && start === windowEnd));
    return containsStart ? Math.max(highest, window.availableSeats) : highest;
  }, 0);
}

function timeValue(time: string): number {
  const [hour = '0', minute = '0'] = time.split(':');
  return Number(hour) * 60 + Number(minute.slice(0, 2));
}
