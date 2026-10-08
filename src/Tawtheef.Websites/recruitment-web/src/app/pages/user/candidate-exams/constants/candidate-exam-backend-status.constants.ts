export const CandidateExamSlotStatus = {
  Started: 'Started',
  Closed: 'Closed',
} as const;

export type CandidateExamSlotStatus =
  (typeof CandidateExamSlotStatus)[keyof typeof CandidateExamSlotStatus];

export const CandidateExamAttemptStatus = {
  InProgress: 'InProgress',
  Interrupted: 'Interrupted',
  Completed: 'Completed',
  TimedOut: 'TimedOut',
} as const;

export type CandidateExamAttemptStatus =
  (typeof CandidateExamAttemptStatus)[keyof typeof CandidateExamAttemptStatus];
