// Mirrors Tawtheef.Domain.Entities.Interview.ResultReportStatus.
// Declared in lifecycle order; the numbers are the stored backend values and are never renumbered
// (CommitteeReview was added later, so it keeps 6 even though it comes second in the flow).
export enum ResultReportStatus {
  Creating = 1,
  CommitteeReview = 2,
  UnderReview = 3,
  Returned = 4,
  Approved = 5,
  Closed = 6,
}

// Mirrors Tawtheef.Domain.Entities.Interview.FinalDecision.
export enum FinalDecision {
  CandidateForHiringProcess = 1,
  WaitingList = 2,
  Rejected = 3,
  NeedsAction = 4,
  NoShow = 5,
}

// Only the 3 decisions the approval dropdown offers - ApproveInterviewResultReportCommandValidator
// rejects NeedsAction/NoShow.
export const SELECTABLE_FINAL_DECISIONS: FinalDecision[] = [
  FinalDecision.CandidateForHiringProcess,
  FinalDecision.WaitingList,
  FinalDecision.Rejected,
];

// Decisions InterviewResultCandidate.Decide() refuses for a not-qualified candidate - only used to
// disable those dropdown options so the reviewer can't build a request the server will reject.
export const QUALIFIED_ONLY_DECISIONS: FinalDecision[] = [
  FinalDecision.CandidateForHiringProcess,
  FinalDecision.WaitingList,
];

// List-only pseudo status: a report soft-deleted because an appointment was rescheduled before approval.
export const DISCARDED_REPORT_FILTER = 'discarded' as const;
export type ResultReportListStatusFilter = ResultReportStatus | typeof DISCARDED_REPORT_FILTER;

export const RESULT_REPORT_STATUS_LABELS: Record<ResultReportStatus, string> = {
  [ResultReportStatus.Creating]: 'INTERVIEW_RESULT_REPORT.STATUS.CREATING',
  [ResultReportStatus.CommitteeReview]: 'INTERVIEW_RESULT_REPORT.STATUS.COMMITTEE_REVIEW',
  [ResultReportStatus.UnderReview]: 'INTERVIEW_RESULT_REPORT.STATUS.UNDER_REVIEW',
  [ResultReportStatus.Returned]: 'INTERVIEW_RESULT_REPORT.STATUS.RETURNED',
  [ResultReportStatus.Approved]: 'INTERVIEW_RESULT_REPORT.STATUS.APPROVED',
  [ResultReportStatus.Closed]: 'INTERVIEW_RESULT_REPORT.STATUS.CLOSED',
};

export const RESULT_REPORT_STATUS_PILL: Record<
  ResultReportStatus,
  'neutral' | 'warning' | 'danger' | 'success' | 'info'
> = {
  [ResultReportStatus.Creating]: 'neutral',
  [ResultReportStatus.CommitteeReview]: 'neutral',
  [ResultReportStatus.UnderReview]: 'warning',
  [ResultReportStatus.Returned]: 'danger',
  [ResultReportStatus.Approved]: 'success',
  [ResultReportStatus.Closed]: 'info',
};

export const FINAL_DECISION_LABELS: Record<FinalDecision, string> = {
  [FinalDecision.CandidateForHiringProcess]:
    'INTERVIEW_RESULT_REPORT.DECISION.CANDIDATE_FOR_HIRING_PROCESS',
  [FinalDecision.WaitingList]: 'INTERVIEW_RESULT_REPORT.DECISION.WAITING_LIST',
  [FinalDecision.Rejected]: 'INTERVIEW_RESULT_REPORT.DECISION.REJECTED',
  [FinalDecision.NeedsAction]: 'INTERVIEW_RESULT_REPORT.DECISION.NEEDS_ACTION',
  [FinalDecision.NoShow]: 'INTERVIEW_RESULT_REPORT.DECISION.NO_SHOW',
};

export const FINAL_DECISION_PILL: Record<
  FinalDecision,
  'neutral' | 'warning' | 'danger' | 'success' | 'info'
> = {
  [FinalDecision.CandidateForHiringProcess]: 'success',
  [FinalDecision.WaitingList]: 'warning',
  [FinalDecision.Rejected]: 'danger',
  [FinalDecision.NeedsAction]: 'info',
  [FinalDecision.NoShow]: 'neutral',
};
