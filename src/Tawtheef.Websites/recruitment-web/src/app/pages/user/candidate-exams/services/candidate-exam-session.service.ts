import { computed, Injectable, signal } from '@angular/core';
import {
  CandidateExamCurrentQuestionResponse,
  CandidateExamSubmissionOutcome,
  StartedCandidateExamPart,
  SubmitCandidateExamPartResponse,
  TimeoutCandidateExamPartResponse,
} from '../models/candidate-exam-execution.model';
import { StartCandidateExamResponse } from '../models/candidate-exam.model';
import { CandidateExamResumeResult } from '../models/candidate-exam-realtime.model';
import { CandidateExamSessionState, StoredCandidateExamSession } from '../models/candidate-exam-session.model';
import {
  CANDIDATE_EXAM_SESSION_STORAGE_KEY,
  CANDIDATE_EXAM_SESSION_STORAGE_VERSION,
} from '../constants/candidate-exam-storage.constants';

@Injectable({ providedIn: 'root' })
export class CandidateExamSessionService {
  private readonly sessionState = signal<CandidateExamSessionState | null>(null);
  private readonly outcomeState = signal<CandidateExamSubmissionOutcome | null>(null);

  readonly session = this.sessionState.asReadonly();
  readonly outcome = this.outcomeState.asReadonly();
  readonly hasActiveSession = computed(() => this.sessionState() !== null);

  initializeFromStart(response: StartCandidateExamResponse): void {
    this.outcomeState.set(null);
    this.setAndPersist({
      testAttemptId: response.testAttemptId,
      testAttemptPartId: response.testAttemptPartId,
      activeSessionId: response.activeSessionId,
      effectiveDeadlineAt: response.effectiveDeadlineAt,
      allowPreviousQuestion: response.allowPreviousQuestion,
      currentQuestionNumber: response.currentQuestionNumber,
    });
  }

  initializeFromResume(response: CandidateExamResumeResult): void {
    this.outcomeState.set(null);
    this.setAndPersist({
      testAttemptId: response.testAttemptId,
      testAttemptPartId: response.testAttemptPartId,
      activeSessionId: response.activeSessionId,
      effectiveDeadlineAt: response.effectiveDeadlineAt,
      allowPreviousQuestion: response.allowPreviousQuestion,
      currentQuestionNumber: response.currentQuestionNumber,
    });
  }

  restore(): boolean {
    this.outcomeState.set(null);

    try {
      const rawSession = sessionStorage.getItem(CANDIDATE_EXAM_SESSION_STORAGE_KEY);
      if (!rawSession) {
        this.sessionState.set(null);
        return false;
      }

      const parsedSession: unknown = JSON.parse(rawSession);
      if (!this.isStoredSession(parsedSession)) {
        this.clearExecution();
        return false;
      }

      this.sessionState.set({
        testAttemptId: parsedSession.testAttemptId,
        testAttemptPartId: parsedSession.testAttemptPartId,
        activeSessionId: parsedSession.activeSessionId,
        effectiveDeadlineAt: parsedSession.effectiveDeadlineAt,
        allowPreviousQuestion: parsedSession.allowPreviousQuestion,
        currentQuestionNumber: parsedSession.currentQuestionNumber,
      });
      return true;
    } catch {
      this.clearExecution();
      return false;
    }
  }

  updateFromCurrentQuestion(response: CandidateExamCurrentQuestionResponse): void {
    const currentSession = this.sessionState();
    if (!currentSession) return;

    if (currentSession.testAttemptId !== response.testAttemptId) {
      this.clear();
      return;
    }

    this.setAndPersist({
      ...currentSession,
      testAttemptPartId: response.testAttemptPartId,
      effectiveDeadlineAt: response.effectiveDeadlineAt,
      allowPreviousQuestion: response.allowPreviousQuestion,
      currentQuestionNumber: response.currentQuestionNumber,
    });
  }

  transitionToNextPart(nextPart: StartedCandidateExamPart): void {
    const currentSession = this.sessionState();
    if (!currentSession) return;

    this.setAndPersist({
      ...currentSession,
      testAttemptPartId: nextPart.testAttemptPartId,
      effectiveDeadlineAt: nextPart.effectiveDeadlineAt,
      currentQuestionNumber: nextPart.currentQuestionNumber,
    });
  }

  replaceAfterResume(
    testAttemptId: string,
    testAttemptPartId: string,
    activeSessionId: string,
    effectiveDeadlineAt: string,
  ): boolean {
    const currentSession = this.sessionState();
    if (!currentSession || currentSession.testAttemptId !== testAttemptId) {
      this.clear();
      return false;
    }

    this.outcomeState.set(null);
    this.setAndPersist({
      ...currentSession,
      testAttemptPartId,
      activeSessionId,
      effectiveDeadlineAt,
    });
    return true;
  }

  applyPartResult(
    response: SubmitCandidateExamPartResponse | TimeoutCandidateExamPartResponse,
  ): void {
    const currentSession = this.sessionState();
    if (!currentSession || currentSession.testAttemptId !== response.testAttemptId) {
      this.clear();
      return;
    }

    this.outcomeState.set(response.outcome);

    if (response.hasNextPart && response.nextPart) {
      this.transitionToNextPart(response.nextPart);
      return;
    }

    this.clearExecution();
  }

  clear(): void {
    this.outcomeState.set(null);
    this.clearExecution();
  }

  clearOutcome(): void {
    this.outcomeState.set(null);
  }

  private setAndPersist(session: CandidateExamSessionState): void {
    this.sessionState.set(session);

    const storedSession: StoredCandidateExamSession = {
      version: CANDIDATE_EXAM_SESSION_STORAGE_VERSION,
      ...session,
    };

    try {
      sessionStorage.setItem(CANDIDATE_EXAM_SESSION_STORAGE_KEY, JSON.stringify(storedSession));
    } catch {
      // In-memory execution can continue when browser storage is unavailable.
    }
  }

  private clearExecution(): void {
    this.sessionState.set(null);

    try {
      sessionStorage.removeItem(CANDIDATE_EXAM_SESSION_STORAGE_KEY);
    } catch {
      // The in-memory session is still cleared when browser storage is unavailable.
    }
  }

  private isStoredSession(value: unknown): value is StoredCandidateExamSession {
    if (!this.isRecord(value)) return false;

    return (
      value['version'] === CANDIDATE_EXAM_SESSION_STORAGE_VERSION &&
      this.isNonEmptyString(value['testAttemptId']) &&
      this.isNonEmptyString(value['testAttemptPartId']) &&
      this.isNonEmptyString(value['activeSessionId']) &&
      this.isValidDateString(value['effectiveDeadlineAt']) &&
      typeof value['allowPreviousQuestion'] === 'boolean' &&
      typeof value['currentQuestionNumber'] === 'number' &&
      Number.isInteger(value['currentQuestionNumber']) &&
      value['currentQuestionNumber'] > 0
    );
  }

  private isRecord(value: unknown): value is Record<string, unknown> {
    return typeof value === 'object' && value !== null && !Array.isArray(value);
  }

  private isNonEmptyString(value: unknown): value is string {
    return typeof value === 'string' && value.trim().length > 0;
  }

  private isValidDateString(value: unknown): value is string {
    return this.isNonEmptyString(value) && !Number.isNaN(Date.parse(value));
  }
}
