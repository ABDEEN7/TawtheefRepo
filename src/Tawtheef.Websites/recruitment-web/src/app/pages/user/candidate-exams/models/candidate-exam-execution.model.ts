import { CandidateExamLookupOption } from './candidate-exam.model';

export type CandidateExamSubmissionOutcome = 'PASSED' | 'FAILED' | 'COMPLETED';

export interface CandidateExamFileReference {
  readonly resourceId: string;
  readonly fileName: string;
  readonly url: string;
}

export interface CandidateExamQuestionOption {
  readonly optionId: string;
  readonly optionText: string;
  readonly displayOrder: number;
  readonly resource: CandidateExamFileReference | null;
}

export interface CandidateExamCurrentQuestionResponse {
  readonly testAttemptId: string;
  readonly testAttemptPartId: string;
  readonly examId: string;
  readonly partNo: number;
  readonly partTitle: string;
  readonly currentPartNumber: number;
  readonly totalParts: number;
  readonly durationMinutes: number;
  readonly startedAt: string;
  readonly effectiveDeadlineAt: string;
  readonly serverTimeUtc: string;
  readonly remainingSeconds: number;
  readonly isTimeExpired: boolean;
  readonly currentQuestionNumber: number;
  readonly totalQuestionsInPart: number;
  readonly allowPreviousQuestion: boolean;
  readonly hasPrevious: boolean;
  readonly hasNext: boolean;
  readonly testAttemptQuestionId: string;
  readonly questionType: CandidateExamLookupOption;
  readonly questionText: string;
  readonly resource: CandidateExamFileReference | null;
  readonly selectedOptionId: string | null;
  readonly savedAt: string | null;
  readonly options: readonly CandidateExamQuestionOption[];
  readonly jobTitle: string;
}

export interface SaveCandidateExamAnswerResponse {
  readonly testAttemptQuestionId: string;
  readonly selectedOptionId: string;
  readonly savedAt: string;
  readonly wasChanged: boolean;
}

export interface NavigateCandidateExamResponse {
  readonly testAttemptId: string;
  readonly testAttemptPartId: string;
  readonly currentQuestionNumber: number;
  readonly totalQuestionsInPart: number;
  readonly hasPrevious: boolean;
  readonly hasNext: boolean;
  readonly allowPreviousQuestion: boolean;
  readonly wasChanged: boolean;
}

export interface StartedCandidateExamPart {
  readonly testAttemptPartId: string;
  readonly partNo: number;
  readonly startedAt: string;
  readonly durationMinutes: number;
  readonly effectiveDeadlineAt: string;
  readonly currentQuestionNumber: number;
  readonly totalQuestionsInPart: number;
}

export interface CompletedCandidateExamPart {
  readonly testAttemptPartId: string;
  readonly partNo: number;
  readonly completedAt: string;
}

export interface TimedOutCandidateExamPart {
  readonly testAttemptPartId: string;
  readonly partNo: number;
  readonly timedOutAt: string;
}

export interface SubmitCandidateExamPartResponse {
  readonly testAttemptId: string;
  readonly completedPart: CompletedCandidateExamPart;
  readonly outcome: CandidateExamSubmissionOutcome;
  readonly hasNextPart: boolean;
  readonly nextPart: StartedCandidateExamPart | null;
  readonly attemptStatusId: string;
  readonly isExamEnded: boolean;
  readonly allowPreviousQuestion: boolean;
  readonly wasChanged: boolean;
}

export interface TimeoutCandidateExamPartResponse {
  readonly testAttemptId: string;
  readonly timedOutPart: TimedOutCandidateExamPart;
  readonly outcome: CandidateExamSubmissionOutcome;
  readonly hasNextPart: boolean;
  readonly nextPart: StartedCandidateExamPart | null;
  readonly attemptStatusId: string;
  readonly isExamEnded: boolean;
  readonly wasChanged: boolean;
}
