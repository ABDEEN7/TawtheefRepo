export interface CandidateExamSessionState {
  readonly testAttemptId: string;
  readonly testAttemptPartId: string;
  readonly activeSessionId: string;
  readonly effectiveDeadlineAt: string;
  readonly allowPreviousQuestion: boolean;
  readonly currentQuestionNumber: number;
}

export interface StoredCandidateExamSession extends CandidateExamSessionState {
  readonly version: 1;
}
