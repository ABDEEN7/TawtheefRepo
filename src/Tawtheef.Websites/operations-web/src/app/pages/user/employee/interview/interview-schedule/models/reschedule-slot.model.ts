// An open slot of the same schedule a candidate can be rescheduled into (RescheduleSlotDto). Every slot seats
// exactly one candidate, so each one is a single remaining seat. startAt/endAt are UTC ISO instants.
export interface RescheduleSlotModel {
  slotId: string;
  startAt: string;
  endAt: string;
  roomId?: string | null;
  roomNameAr?: string | null;
  roomNameEn?: string | null;
  remoteMeetingUrl?: string | null;
}
