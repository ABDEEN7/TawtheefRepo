import { AppointmentModel } from './appointment.model';
import { AppointmentStatus, InterviewType } from './enums';
import { RoomOptionModel, SchedulePeriodModel } from './schedule.model';

export type WizardMode = 'create' | 'edit';

export interface WizardInfoDraft {
  jobId: string;
  interviewType: InterviewType | null;
}

export function emptyWizardInfoDraft(): WizardInfoDraft {
  return { jobId: '', interviewType: null };
}

/** The job facts step 1's picker shows - only what it renders, not the whole job record. */
export interface WizardJobContext {
  id: string;
  titleAr: string;
  titleEn: string | null;
  jobNumber: string | null;
}

/** One row of the Step 2 periods table. Never sent as-is: the facade maps it to PeriodInputDto. */
export interface PeriodDraft {
  localId: string;
  date: string; // yyyy-mm-dd
  startTime: string; // HH:mm
  endTime: string; // HH:mm
  room: RoomOptionModel | null; // set when the schedule's interview type is In-Person
  remoteMeetingUrl: string | null; // set when the schedule's interview type is Remote
  remoteMeetingInstructions: string | null;
}

export function newPeriodDraft(): PeriodDraft {
  return {
    localId: crypto.randomUUID(),
    date: '',
    startTime: '',
    endTime: '',
    room: null,
    remoteMeetingUrl: null,
    remoteMeetingInstructions: null,
  };
}

// Rebuilds a step-2 row from a saved schedule's period. Only the room's id and name are known here, which is all
// the payload (id) and the table/dialog (name) use.
export function periodDraftFromSchedule(period: SchedulePeriodModel): PeriodDraft {
  return {
    localId: crypto.randomUUID(),
    date: period.date,
    startTime: period.startTime.slice(0, 5),
    endTime: period.endTime.slice(0, 5),
    room: period.roomId
      ? {
          id: period.roomId,
          nameAr: period.roomNameAr ?? '',
          nameEn: period.roomNameEn ?? null,
          locationNameAr: '',
          locationNameEn: null,
          capacity: 0,
        }
      : null,
    remoteMeetingUrl: period.remoteMeetingUrl ?? null,
    remoteMeetingInstructions: period.remoteMeetingInstructions ?? null,
  };
}

// A saved distribution is restored by pinning every live assignment, so the server reproduces exactly the
// candidate -> slot pairs that were saved (any hand edits included) instead of re-running its automatic fill.
export function manualAssignmentsFromAppointments(appointments: AppointmentModel[]): ManualAssignmentDraft[] {
  return appointments
    .filter(
      (a) =>
        !!a.invitationId && a.status !== AppointmentStatus.Rescheduled && a.status !== AppointmentStatus.Cancelled,
    )
    .map((a) => ({ slotStartAt: a.startAt, invitationId: a.invitationId! }));
}

/**
 * One user-pinned candidate -> slot override (Step 3's "reassign"). Slots left out of this list are
 * auto-filled from the remaining eligible pool by the server - mirrors SlotAssignmentDto exactly.
 */
export interface ManualAssignmentDraft {
  slotStartAt: string; // ISO, matches a SlotPreviewModel.slot.startAt exactly
  invitationId: string;
}
