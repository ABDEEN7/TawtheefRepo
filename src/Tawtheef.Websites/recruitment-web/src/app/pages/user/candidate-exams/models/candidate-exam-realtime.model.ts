export type CandidateExamConnectionStatus =
  | 'disconnected'
  | 'connecting'
  | 'connected'
  | 'reconnecting';

export interface CandidateExamResumeResult {
  readonly testAttemptId: string;
  readonly testAttemptPartId: string;
  readonly activeSessionId: string;
  readonly effectiveDeadlineAt: string;
  readonly currentQuestionNumber: number;
  readonly allowPreviousQuestion: boolean;
  readonly wasChanged: boolean;
}
