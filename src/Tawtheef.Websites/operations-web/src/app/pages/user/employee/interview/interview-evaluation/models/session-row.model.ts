import { ScheduleListItemModel } from '../../interview-schedule/models/schedule.model';

// ScheduleListItemModel enriched, client-side, with per-schedule appointment data (see
// interview-evaluation.facade.ts enrichSessionRows) - Progress and Location aren't on
// ScheduleListItemDto (no single completed-count, no single room across appointments).
export interface SessionListRowModel extends ScheduleListItemModel {
  completedAppointmentsCount: number;
  totalAssignedAppointmentsCount: number;
  // Distinct room names (localized) across the schedule's live appointments - empty for a fully
  // remote schedule, one entry for a single shared room, several for multiple rooms.
  roomNames: string[];
}
