export enum ScheduleStatus {
  Draft = 1,
  Proposed = 2,
  PendingApproval = 3,
  Returned = 4,
  Approved = 5,
  ReadyForExecution = 6,
  InProgress = 7,
  Closed = 8,
  Cancelled = 9,
}

export enum AppointmentStatus {
  Held = 1,
  Scheduled = 2,
  InInterview = 3,
  UnderEvaluation = 4,
  Completed = 5,
  Closed = 6,
  Rescheduled = 7,
  Cancelled = 8,
}

// Backend calls this Remote; the product spec and UI call it "Online".
export enum InterviewType {
  InPerson = 1,
  Remote = 2,
}

export const SCHEDULE_STATUS_LABELS: Record<ScheduleStatus, string> = {
  [ScheduleStatus.Draft]: 'INTERVIEW_SCHEDULE.STATUS.DRAFT',
  [ScheduleStatus.Proposed]: 'INTERVIEW_SCHEDULE.STATUS.DRAFT',
  [ScheduleStatus.PendingApproval]: 'INTERVIEW_SCHEDULE.STATUS.PENDING_APPROVAL',
  [ScheduleStatus.Returned]: 'INTERVIEW_SCHEDULE.STATUS.RETURNED',
  [ScheduleStatus.Approved]: 'INTERVIEW_SCHEDULE.STATUS.APPROVED',
  [ScheduleStatus.ReadyForExecution]: 'INTERVIEW_SCHEDULE.STATUS.READY_FOR_EXECUTION',
  [ScheduleStatus.InProgress]: 'INTERVIEW_SCHEDULE.STATUS.IN_PROGRESS',
  [ScheduleStatus.Closed]: 'INTERVIEW_SCHEDULE.STATUS.CLOSED',
  [ScheduleStatus.Cancelled]: 'INTERVIEW_SCHEDULE.STATUS.CANCELLED',
};

export const SCHEDULE_STATUS_PILL: Record<ScheduleStatus, 'neutral' | 'warning' | 'danger' | 'success' | 'info'> = {
  [ScheduleStatus.Draft]: 'neutral',
  // Proposed is the real persisted state behind the wizard's "Save as Draft" (see facade save()) - shown to
  // the user as Draft, so it gets the same pill as Draft rather than a status they never asked for.
  [ScheduleStatus.Proposed]: 'neutral',
  [ScheduleStatus.PendingApproval]: 'warning',
  [ScheduleStatus.Returned]: 'danger',
  [ScheduleStatus.Approved]: 'success',
  [ScheduleStatus.ReadyForExecution]: 'info',
  [ScheduleStatus.InProgress]: 'info',
  [ScheduleStatus.Closed]: 'info',
  [ScheduleStatus.Cancelled]: 'danger',
};

export const APPOINTMENT_STATUS_LABELS: Record<AppointmentStatus, string> = {
  [AppointmentStatus.Held]: 'INTERVIEW_SCHEDULE.APPOINTMENT_STATUS.HELD',
  [AppointmentStatus.Scheduled]: 'INTERVIEW_SCHEDULE.APPOINTMENT_STATUS.SCHEDULED',
  [AppointmentStatus.InInterview]: 'INTERVIEW_SCHEDULE.APPOINTMENT_STATUS.IN_INTERVIEW',
  [AppointmentStatus.UnderEvaluation]: 'INTERVIEW_SCHEDULE.APPOINTMENT_STATUS.UNDER_EVALUATION',
  [AppointmentStatus.Completed]: 'INTERVIEW_SCHEDULE.APPOINTMENT_STATUS.COMPLETED',
  [AppointmentStatus.Closed]: 'INTERVIEW_SCHEDULE.APPOINTMENT_STATUS.CLOSED',
  [AppointmentStatus.Rescheduled]: 'INTERVIEW_SCHEDULE.APPOINTMENT_STATUS.RESCHEDULED',
  [AppointmentStatus.Cancelled]: 'INTERVIEW_SCHEDULE.APPOINTMENT_STATUS.CANCELLED',
};

export const APPOINTMENT_STATUS_PILL: Record<AppointmentStatus, 'neutral' | 'warning' | 'danger' | 'success' | 'info'> = {
  [AppointmentStatus.Held]: 'neutral',
  [AppointmentStatus.Scheduled]: 'info',
  [AppointmentStatus.InInterview]: 'warning',
  [AppointmentStatus.UnderEvaluation]: 'warning',
  [AppointmentStatus.Completed]: 'success',
  [AppointmentStatus.Closed]: 'info',
  [AppointmentStatus.Rescheduled]: 'warning',
  [AppointmentStatus.Cancelled]: 'danger',
};

export const INTERVIEW_TYPE_LABELS: Record<InterviewType, string> = {
  [InterviewType.InPerson]: 'INTERVIEW_SCHEDULE.INTERVIEW_TYPE.IN_PERSON',
  [InterviewType.Remote]: 'INTERVIEW_SCHEDULE.INTERVIEW_TYPE.ONLINE',
};

// Mirrors ScheduleConstants on the backend.
export const MINIMUM_DURATION_MINUTES = 5;
export const MINIMUM_BUFFER_MINUTES = 0;

// Read-only display concepts borrowed from the Committee stage (step 1's committee card). Kept as a
// local copy rather than importing across feature modules, matching the lazy-select precedent.
export enum CommitteeStatus {
  Draft = 1,
  PendingApproval = 2,
  Returned = 3,
  Approved = 4,
  Stopped = 5,
  Closed = 6,
  Cancelled = 7,
}

export enum CommitteeRole {
  Chair = 1,
  Evaluator = 2,
  Observer = 3,
  Admin = 4,
}

// Translation keys live under this module's own INTERVIEW_SCHEDULE.* namespace, not
// INTERVIEW_COMMITTEES.* - the page only loads its own i18nNamespace JSON (see interview-schedule.page.html),
// so reusing the committee stage's keys here would render untranslated.
export const COMMITTEE_STATUS_LABELS: Record<CommitteeStatus, string> = {
  [CommitteeStatus.Draft]: 'INTERVIEW_SCHEDULE.COMMITTEE_STATUS.DRAFT',
  [CommitteeStatus.PendingApproval]: 'INTERVIEW_SCHEDULE.COMMITTEE_STATUS.PENDING_APPROVAL',
  [CommitteeStatus.Returned]: 'INTERVIEW_SCHEDULE.COMMITTEE_STATUS.RETURNED',
  [CommitteeStatus.Approved]: 'INTERVIEW_SCHEDULE.COMMITTEE_STATUS.APPROVED',
  [CommitteeStatus.Stopped]: 'INTERVIEW_SCHEDULE.COMMITTEE_STATUS.STOPPED',
  [CommitteeStatus.Closed]: 'INTERVIEW_SCHEDULE.COMMITTEE_STATUS.CLOSED',
  [CommitteeStatus.Cancelled]: 'INTERVIEW_SCHEDULE.COMMITTEE_STATUS.CANCELLED',
};

export const COMMITTEE_STATUS_PILL: Record<CommitteeStatus, 'neutral' | 'warning' | 'danger' | 'success' | 'info'> = {
  [CommitteeStatus.Draft]: 'neutral',
  [CommitteeStatus.PendingApproval]: 'warning',
  [CommitteeStatus.Returned]: 'danger',
  [CommitteeStatus.Approved]: 'success',
  [CommitteeStatus.Stopped]: 'warning',
  [CommitteeStatus.Closed]: 'info',
  [CommitteeStatus.Cancelled]: 'danger',
};

export const COMMITTEE_ROLE_LABELS: Record<CommitteeRole, string> = {
  [CommitteeRole.Chair]: 'INTERVIEW_SCHEDULE.COMMITTEE_ROLE.CHAIR',
  [CommitteeRole.Evaluator]: 'INTERVIEW_SCHEDULE.COMMITTEE_ROLE.EVALUATOR',
  [CommitteeRole.Observer]: 'INTERVIEW_SCHEDULE.COMMITTEE_ROLE.OBSERVER',
  [CommitteeRole.Admin]: 'INTERVIEW_SCHEDULE.COMMITTEE_ROLE.ADMIN',
};
