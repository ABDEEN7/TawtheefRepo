export type CandidateExamEntryState =
  | 'Assigned'
  | 'NotEligible'
  | 'WaitingForSession'
  | 'WaitingForAttendance'
  | 'EntryWindowClosed'
  | 'WaitingForIdentityVerification'
  | 'WaitingForAuthorization'
  | 'ReadyToEnter'
  | 'Started'
  | 'NoShow'
  | 'Completed'
  | 'Rescheduled'
  | 'Cancelled'
  | 'Closed';

export interface CandidateExamLookupOption {
  readonly id: string;
  readonly backendName: string;
  readonly name: string;
  readonly description: string | null;
  readonly additionalData: unknown | null;
}

export interface CandidateExamAssignment {
  readonly testSessionCandidateId: string;
  readonly testSessionId: string;
  readonly examId: string;
  readonly testAttemptId: string | null;
  readonly examNo: string;
  readonly examTitleAr: string;
  readonly examTitleEn: string | null;
  readonly jobTitleAr: string;
  readonly jobTitleEn: string;
  readonly totalQuestions: number;
  readonly allowPreviousQuestion: boolean;
  readonly sessionNo: string;
  readonly slotDate: string;
  readonly startTime: string;
  readonly endTime: string;
  readonly roomNameAr: string;
  readonly roomNameEn: string | null;
  readonly locationNameAr: string;
  readonly locationNameEn: string | null;
  readonly examStatus: CandidateExamLookupOption;
  readonly slotStatus: CandidateExamLookupOption;
  readonly sessionStatus: CandidateExamLookupOption;
  readonly candidateStatus: CandidateExamLookupOption;
  readonly attendanceStatus: CandidateExamLookupOption;
  readonly identityVerificationStatus: CandidateExamLookupOption;
  readonly attemptStatus: CandidateExamLookupOption | null;
  readonly resumeAuthorized: boolean;
  readonly authorizedAt: string | null;
  readonly entryState: CandidateExamEntryState;
}

export interface ValidateCandidateExamAccessCodeResponse {
  readonly testSessionCandidateId: string;
  readonly testSessionId: string;
  readonly examId: string;
  readonly entryState: CandidateExamEntryState;
}

export interface StartCandidateExamResponse {
  readonly testSessionCandidateId: string;
  readonly testAttemptId: string;
  readonly testAttemptPartId: string;
  readonly examId: string;
  readonly activeSessionId: string;
  readonly currentPartNumber: number;
  readonly currentQuestionNumber: number;
  readonly startedAt: string;
  readonly effectiveDeadlineAt: string;
  readonly durationMinutes: number;
  readonly allowPreviousQuestion: boolean;
}

export interface CandidateExamActiveExecutionResponse {
  readonly testAttemptId: string;
  readonly testAttemptPartId: string;
  readonly activeSessionId: string;
  readonly effectiveDeadlineAt: string;
  readonly currentQuestionNumber: number;
  readonly allowPreviousQuestion: boolean;
}
