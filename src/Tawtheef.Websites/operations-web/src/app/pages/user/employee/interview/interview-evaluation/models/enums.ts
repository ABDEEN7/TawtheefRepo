import { AppointmentStatus, CommitteeRole, InterviewType, ScheduleStatus } from '../../interview-schedule/models/enums';

// Mirrors Tawtheef.Domain.Entities.Interview.MemberEvaluationStatus.
export enum MemberEvaluationStatus {
  Draft = 1,
  Submitted = 2,
  Reopened = 3,
}

// Mirrors Tawtheef.Domain.Entities.Interview.AttendanceStatus. Not previously modeled in Angular -
// interview-schedule/models/appointment.model.ts only carries it as a raw number.
export enum AttendanceStatus {
  Present = 1,
  NoShow = 2,
  Withdrew = 3,
  Late = 4,
}

// Mirrors Tawtheef.Domain.Entities.Interview.OperationalIssueType - exact existing backend enum,
// no invented issue types.
export enum OperationalIssueType {
  NoShow = 1,
  CandidateWithdrawal = 2,
  IncompleteEvaluation = 3,
  TechnicalProblem = 4,
  CouldNotBeConducted = 5,
  RescheduleRequest = 6,
}

// Mirrors Tawtheef.Domain.Entities.Interview.OperationalIssueStatus.
export enum OperationalIssueStatus {
  Open = 1,
  Resolved = 2,
  Waived = 3,
}

export const MEMBER_EVALUATION_STATUS_LABELS: Record<MemberEvaluationStatus, string> = {
  [MemberEvaluationStatus.Draft]: 'INTERVIEW_EVALUATION.MEMBER_STATUS.DRAFT',
  [MemberEvaluationStatus.Submitted]: 'INTERVIEW_EVALUATION.MEMBER_STATUS.SUBMITTED',
  [MemberEvaluationStatus.Reopened]: 'INTERVIEW_EVALUATION.MEMBER_STATUS.REOPENED',
};

export const MEMBER_EVALUATION_STATUS_PILL: Record<MemberEvaluationStatus, 'neutral' | 'warning' | 'danger' | 'success' | 'info'> = {
  [MemberEvaluationStatus.Draft]: 'warning',
  [MemberEvaluationStatus.Submitted]: 'success',
  [MemberEvaluationStatus.Reopened]: 'info',
};

export const ATTENDANCE_STATUS_LABELS: Record<AttendanceStatus, string> = {
  [AttendanceStatus.Present]: 'INTERVIEW_EVALUATION.ATTENDANCE_STATUS.PRESENT',
  [AttendanceStatus.NoShow]: 'INTERVIEW_EVALUATION.ATTENDANCE_STATUS.NO_SHOW',
  [AttendanceStatus.Withdrew]: 'INTERVIEW_EVALUATION.ATTENDANCE_STATUS.WITHDREW',
  [AttendanceStatus.Late]: 'INTERVIEW_EVALUATION.ATTENDANCE_STATUS.LATE',
};

export const ATTENDANCE_STATUS_PILL: Record<AttendanceStatus, 'neutral' | 'warning' | 'danger' | 'success' | 'info'> = {
  [AttendanceStatus.Present]: 'success',
  [AttendanceStatus.NoShow]: 'danger',
  [AttendanceStatus.Withdrew]: 'danger',
  [AttendanceStatus.Late]: 'warning',
};

export const OPERATIONAL_ISSUE_TYPE_LABELS: Record<OperationalIssueType, string> = {
  [OperationalIssueType.NoShow]: 'INTERVIEW_EVALUATION.ISSUE_TYPE.NO_SHOW',
  [OperationalIssueType.CandidateWithdrawal]: 'INTERVIEW_EVALUATION.ISSUE_TYPE.CANDIDATE_WITHDRAWAL',
  [OperationalIssueType.IncompleteEvaluation]: 'INTERVIEW_EVALUATION.ISSUE_TYPE.INCOMPLETE_EVALUATION',
  [OperationalIssueType.TechnicalProblem]: 'INTERVIEW_EVALUATION.ISSUE_TYPE.TECHNICAL_PROBLEM',
  [OperationalIssueType.CouldNotBeConducted]: 'INTERVIEW_EVALUATION.ISSUE_TYPE.COULD_NOT_BE_CONDUCTED',
  [OperationalIssueType.RescheduleRequest]: 'INTERVIEW_EVALUATION.ISSUE_TYPE.RESCHEDULE_REQUEST',
};

export const OPERATIONAL_ISSUE_STATUS_LABELS: Record<OperationalIssueStatus, string> = {
  [OperationalIssueStatus.Open]: 'INTERVIEW_EVALUATION.ISSUE_STATUS.OPEN',
  [OperationalIssueStatus.Resolved]: 'INTERVIEW_EVALUATION.ISSUE_STATUS.RESOLVED',
  [OperationalIssueStatus.Waived]: 'INTERVIEW_EVALUATION.ISSUE_STATUS.WAIVED',
};

export const OPERATIONAL_ISSUE_STATUS_PILL: Record<OperationalIssueStatus, 'neutral' | 'warning' | 'danger' | 'success' | 'info'> = {
  [OperationalIssueStatus.Open]: 'warning',
  [OperationalIssueStatus.Resolved]: 'success',
  [OperationalIssueStatus.Waived]: 'neutral',
};

// Local copy of the role label keys under this module's own INTERVIEW_EVALUATION.* namespace -
// interview-schedule/models/enums.ts's COMMITTEE_ROLE_LABELS point at INTERVIEW_SCHEDULE.* keys,
// which this page doesn't load (only its own i18nNamespace JSON, see candidate-evaluation.ts).
export const MEMBER_ROLE_LABELS: Record<CommitteeRole, string> = {
  [CommitteeRole.Chair]: 'INTERVIEW_EVALUATION.COMMITTEE_ROLE.CHAIR',
  [CommitteeRole.Evaluator]: 'INTERVIEW_EVALUATION.COMMITTEE_ROLE.EVALUATOR',
  [CommitteeRole.Observer]: 'INTERVIEW_EVALUATION.COMMITTEE_ROLE.OBSERVER',
  [CommitteeRole.Admin]: 'INTERVIEW_EVALUATION.COMMITTEE_ROLE.ADMIN',
};

// Same local-copy reasoning as MEMBER_ROLE_LABELS above, for the enums borrowed from
// interview-schedule/models/enums.ts (their label maps point at INTERVIEW_SCHEDULE.* keys).
export const EVAL_SCHEDULE_STATUS_LABELS: Record<ScheduleStatus, string> = {
  [ScheduleStatus.Draft]: 'INTERVIEW_EVALUATION.SCHEDULE_STATUS.DRAFT',
  [ScheduleStatus.Proposed]: 'INTERVIEW_EVALUATION.SCHEDULE_STATUS.DRAFT',
  [ScheduleStatus.PendingApproval]: 'INTERVIEW_EVALUATION.SCHEDULE_STATUS.PENDING_APPROVAL',
  [ScheduleStatus.Returned]: 'INTERVIEW_EVALUATION.SCHEDULE_STATUS.RETURNED',
  [ScheduleStatus.Approved]: 'INTERVIEW_EVALUATION.SCHEDULE_STATUS.APPROVED',
  [ScheduleStatus.ReadyForExecution]: 'INTERVIEW_EVALUATION.SCHEDULE_STATUS.READY_FOR_EXECUTION',
  [ScheduleStatus.InProgress]: 'INTERVIEW_EVALUATION.SCHEDULE_STATUS.IN_PROGRESS',
  [ScheduleStatus.Closed]: 'INTERVIEW_EVALUATION.SCHEDULE_STATUS.CLOSED',
  [ScheduleStatus.Cancelled]: 'INTERVIEW_EVALUATION.SCHEDULE_STATUS.CANCELLED',
};

export const EVAL_SCHEDULE_STATUS_PILL: Record<ScheduleStatus, 'neutral' | 'warning' | 'danger' | 'success' | 'info'> = {
  [ScheduleStatus.Draft]: 'neutral',
  [ScheduleStatus.Proposed]: 'neutral',
  [ScheduleStatus.PendingApproval]: 'warning',
  [ScheduleStatus.Returned]: 'danger',
  [ScheduleStatus.Approved]: 'success',
  [ScheduleStatus.ReadyForExecution]: 'info',
  [ScheduleStatus.InProgress]: 'info',
  [ScheduleStatus.Closed]: 'info',
  [ScheduleStatus.Cancelled]: 'danger',
};

export const EVAL_APPOINTMENT_STATUS_LABELS: Record<AppointmentStatus, string> = {
  [AppointmentStatus.Held]: 'INTERVIEW_EVALUATION.APPOINTMENT_STATUS.HELD',
  [AppointmentStatus.Scheduled]: 'INTERVIEW_EVALUATION.APPOINTMENT_STATUS.SCHEDULED',
  [AppointmentStatus.InInterview]: 'INTERVIEW_EVALUATION.APPOINTMENT_STATUS.IN_INTERVIEW',
  [AppointmentStatus.UnderEvaluation]: 'INTERVIEW_EVALUATION.APPOINTMENT_STATUS.UNDER_EVALUATION',
  [AppointmentStatus.Completed]: 'INTERVIEW_EVALUATION.APPOINTMENT_STATUS.COMPLETED',
  [AppointmentStatus.Closed]: 'INTERVIEW_EVALUATION.APPOINTMENT_STATUS.CLOSED',
  [AppointmentStatus.Rescheduled]: 'INTERVIEW_EVALUATION.APPOINTMENT_STATUS.RESCHEDULED',
  [AppointmentStatus.Cancelled]: 'INTERVIEW_EVALUATION.APPOINTMENT_STATUS.CANCELLED',
};

export const EVAL_APPOINTMENT_STATUS_PILL: Record<AppointmentStatus, 'neutral' | 'warning' | 'danger' | 'success' | 'info'> = {
  [AppointmentStatus.Held]: 'neutral',
  [AppointmentStatus.Scheduled]: 'info',
  [AppointmentStatus.InInterview]: 'warning',
  [AppointmentStatus.UnderEvaluation]: 'warning',
  [AppointmentStatus.Completed]: 'success',
  [AppointmentStatus.Closed]: 'info',
  [AppointmentStatus.Rescheduled]: 'warning',
  [AppointmentStatus.Cancelled]: 'danger',
};

export const EVAL_INTERVIEW_TYPE_LABELS: Record<InterviewType, string> = {
  [InterviewType.InPerson]: 'INTERVIEW_EVALUATION.INTERVIEW_TYPE.IN_PERSON',
  [InterviewType.Remote]: 'INTERVIEW_EVALUATION.INTERVIEW_TYPE.ONLINE',
};
