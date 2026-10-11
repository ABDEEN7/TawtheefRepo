export const TestSlotCandidateSessionStatus = {
  Approved: 'Approved',
  Ready: 'Ready',
  InProgress: 'InProgress',
} as const;

export type TestSlotCandidateSessionStatus =
  (typeof TestSlotCandidateSessionStatus)[keyof typeof TestSlotCandidateSessionStatus];

export const TestSlotCandidateAttemptStatus = {
  InProgress: 'InProgress',
  Interrupted: 'Interrupted',
  Completed: 'Completed',
  TimedOut: 'TimedOut',
  Cancelled: 'Cancelled',
} as const;

export type TestSlotCandidateAttemptStatus =
  (typeof TestSlotCandidateAttemptStatus)[keyof typeof TestSlotCandidateAttemptStatus];

export const TestSlotCandidateStatus = {
  Assigned: 'Assigned',
  Authorized: 'Authorized',
  Started: 'Started',
  Completed: 'Completed',
  Rescheduled: 'Rescheduled',
  Cancelled: 'Cancelled',
} as const;

export type TestSlotCandidateStatus =
  (typeof TestSlotCandidateStatus)[keyof typeof TestSlotCandidateStatus];

export const TestSlotCandidateAttendanceStatus = {
  Pending: 'Pending',
  Present: 'Present',
  NoShow: 'NoShow',
} as const;

export type TestSlotCandidateAttendanceStatus =
  (typeof TestSlotCandidateAttendanceStatus)[keyof typeof TestSlotCandidateAttendanceStatus];

export const TestSlotCandidateIdentityStatus = {
  Pending: 'Pending',
  Matched: 'Matched',
  NotMatched: 'NotMatched',
} as const;

export type TestSlotCandidateIdentityStatus =
  (typeof TestSlotCandidateIdentityStatus)[keyof typeof TestSlotCandidateIdentityStatus];
