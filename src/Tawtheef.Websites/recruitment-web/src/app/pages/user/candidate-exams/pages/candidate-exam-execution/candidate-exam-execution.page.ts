import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { firstValueFrom } from 'rxjs';
import { DialogHelperService } from '../../../../../core/services/dialog-helper.service';
import { routes } from '../../../../../routes/routes';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import {
  CandidateExamCurrentQuestionResponse,
  CandidateExamQuestionOption,
  CandidateExamSubmissionOutcome,
  SubmitCandidateExamPartResponse,
  TimeoutCandidateExamPartResponse,
} from '../../models/candidate-exam-execution.model';
import { CandidateExamRealtimeService } from '../../services/candidate-exam-realtime.service';
import { CandidateExamService } from '../../services/candidate-exam.service';
import { CandidateExamSessionService } from '../../services/candidate-exam-session.service';
import { HttpErrorResponse } from '@angular/common/http';
import {
  CandidateExamAnswerSaveState,
  CandidateExamExecutionPreparationState,
} from '../../models/candidate-exam-execution-state.model';
import { CANDIDATE_EXAM_TIMER_SERVER_SYNC_INTERVAL_MS } from '../../constants/candidate-exam-execution-timer.constants';
import { CandidateExamExecutionTimerService } from '../../services/candidate-exam-execution-timer.service';

@Component({
  selector: 'app-candidate-exam-execution',
  standalone: true,
  imports: [CommonModule, TranslatePipe, I18nNamespaceDirective],
  templateUrl: './candidate-exam-execution.page.html',
  styleUrl: './candidate-exam-execution.page.scss',
  providers: [CandidateExamExecutionTimerService],
})
export class CandidateExamExecutionPage implements OnInit, OnDestroy {
  private readonly candidateExamService = inject(CandidateExamService);
  private readonly candidateExamSession = inject(CandidateExamSessionService);
  private readonly realtime = inject(CandidateExamRealtimeService);
  private readonly router = inject(Router);
  private readonly dialogHelper = inject(DialogHelperService);
  private readonly executionTimer = inject(CandidateExamExecutionTimerService);

  readonly preparationState = signal<CandidateExamExecutionPreparationState>('preparing');
  readonly executionOutcome = this.candidateExamSession.outcome;
  readonly currentQuestion = signal<CandidateExamCurrentQuestionResponse | null>(null);
  readonly selectedOptionId = signal<string | null>(null);
  readonly answerSaveState = signal<CandidateExamAnswerSaveState>('idle');
  readonly navigating = signal(false);
  readonly navigationError = signal(false);
  readonly connectionStatus = this.realtime.connectionStatus;
  readonly reconnectRegistrationPending = signal(false);
  readonly reconnectRegistering = signal(false);
  readonly reconnectRegistrationFailed = signal(false);
  readonly remainingSeconds = this.executionTimer.remainingSeconds;
  readonly deadlineReached = signal(false);
  readonly timerSynchronizing = signal(false);
  readonly timeoutProcessing = signal(false);
  readonly timeoutFailed = signal(false);
  readonly submissionProcessing = signal(false);
  readonly submissionFailed = signal(false);
  readonly navigationRequiresReload = signal(false);
  readonly terminalOutcome = signal<Extract<
    CandidateExamSubmissionOutcome,
    'FAILED' | 'COMPLETED'
  > | null>(null);
  readonly formattedRemainingTime = this.executionTimer.formattedRemainingTime;
  readonly realtimeMutationsBlocked = computed(
    () =>
      this.connectionStatus() !== 'connected' ||
      this.reconnectRegistrationPending() ||
      this.reconnectRegistering() ||
      this.reconnectRegistrationFailed(),
  );
  readonly questionMutationsBlocked = computed(
    () =>
      this.realtimeMutationsBlocked() ||
      this.deadlineReached() ||
      this.timeoutProcessing() ||
      this.submissionProcessing() ||
      this.submissionFailed() ||
      this.navigationRequiresReload() ||
      this.terminalOutcome() !== null,
  );
  readonly orderedOptions = computed<readonly CandidateExamQuestionOption[]>(() =>
    [...(this.currentQuestion()?.options ?? [])].sort(
      (first, second) => first.displayOrder - second.displayOrder,
    ),
  );

  private readonly preparing = signal(false);
  private resyncInterval: ReturnType<typeof setInterval> | null = null;
  private syncInFlight = false;
  private operationVersion = 0;
  private submissionConfirmationRef: DynamicDialogRef | null = null;

  constructor() {
    effect(() => {
      const status = this.connectionStatus();

      if (status === 'reconnecting') {
        this.reconnectRegistrationPending.set(true);
        this.reconnectRegistrationFailed.set(false);
        return;
      }

      if (
        status === 'connected' &&
        this.reconnectRegistrationPending() &&
        !this.reconnectRegistering()
      ) {
        void this.registerAfterReconnect();
        return;
      }

      if (status === 'disconnected' && this.reconnectRegistrationPending()) {
        this.reconnectRegistrationPending.set(false);
        this.reconnectRegistrationFailed.set(true);
      }
    });
  }

  ngOnInit(): void {
    document.addEventListener('visibilitychange', this.handleVisibilityChange);
    void this.prepareExecution();
  }

  ngOnDestroy(): void {
    this.closeSubmissionConfirmation();
    this.stopServerSynchronization();
    document.removeEventListener('visibilitychange', this.handleVisibilityChange);
    void this.realtime.disconnect();
  }

  async prepareExecution(): Promise<void> {
    if (this.preparing()) return;

    const session = this.getSession();
    if (!session) return;

    this.preparing.set(true);
    this.preparationState.set('preparing');

    try {
      await this.realtime.connect();
      await this.realtime.registerAttempt(session.testAttemptId, session.activeSessionId);
    } catch {
      this.preparationState.set('connectionError');
      this.preparing.set(false);
      return;
    }

    this.preparing.set(false);
    await this.loadCurrentQuestion();
  }

  async retryQuestionLoad(): Promise<void> {
    await this.loadCurrentQuestion();
  }

  async selectAnswer(optionId: string): Promise<void> {
    const question = this.currentQuestion();
    const session = this.candidateExamSession.session();
    if (
      !question ||
      !session ||
      this.questionMutationsBlocked() ||
      this.answerSaveState() === 'saving'
    ) {
      return;
    }
    if (question.selectedOptionId === optionId) return;

    this.selectedOptionId.set(optionId);
    this.answerSaveState.set('saving');
    this.operationVersion += 1;

    try {
      const response = await firstValueFrom(
        this.candidateExamService.saveAnswer(
          session.testAttemptId,
          question.testAttemptQuestionId,
          session.activeSessionId,
          { selectedOptionId: optionId },
        ),
      );

      this.currentQuestion.update((current) =>
        current && current.testAttemptQuestionId === response.testAttemptQuestionId
          ? { ...current, selectedOptionId: response.selectedOptionId, savedAt: response.savedAt }
          : current,
      );
      this.selectedOptionId.set(response.selectedOptionId);
      this.answerSaveState.set('saved');
    } catch {
      this.selectedOptionId.set(question.selectedOptionId);
      this.answerSaveState.set('error');
    } finally {
      if (this.deadlineReached()) {
        void this.handleDeadlineReached();
      }
    }
  }

  async goToPrevious(): Promise<void> {
    const question = this.currentQuestion();
    if (
      !question ||
      this.questionMutationsBlocked() ||
      !question.allowPreviousQuestion ||
      !question.hasPrevious
    ) {
      return;
    }

    await this.navigateTo(question.currentQuestionNumber - 1);
  }

  async goToNext(): Promise<void> {
    const question = this.currentQuestion();
    if (!question || this.questionMutationsBlocked() || !question.hasNext) return;

    await this.navigateTo(question.currentQuestionNumber + 1);
  }

  async retryNavigation(): Promise<void> {
    if (!this.navigationRequiresReload()) return;
    await this.reloadAfterNavigation();
  }

  async confirmSubmit(): Promise<void> {
    if (!this.canSendManualSubmit() || this.submissionConfirmationRef) return;

    const dialogRef = this.dialogHelper.openConfirmDialog({
      type: 'submit',
      title: 'candidate_exams.execution.submitConfirmationTitle',
      description: 'candidate_exams.execution.submitConfirmationDescription',
      confirmText: 'candidate_exams.execution.submitPart',
      cancelText: 'candidate_exams.execution.cancelSubmission',
    });
    if (!dialogRef) return;
    this.submissionConfirmationRef = dialogRef;

    try {
      const confirmed: unknown = await firstValueFrom(dialogRef.onClose);
      if (confirmed === true && !this.deadlineReached()) {
        await this.submitCurrentPart();
      }
    } finally {
      if (this.submissionConfirmationRef === dialogRef) {
        this.submissionConfirmationRef = null;
      }
    }
  }

  async retrySubmit(): Promise<void> {
    await this.submitCurrentPart(true);
  }

  backToExams(): Promise<boolean> {
    return this.router.navigateByUrl(routes.user.candidateExams);
  }

  private async loadCurrentQuestion(): Promise<void> {
    const session = this.getSession();
    if (!session) return;

    this.preparationState.set('loadingQuestion');

    try {
      const response = await firstValueFrom(
        this.candidateExamService.getCurrentQuestion(
          session.testAttemptId,
          session.activeSessionId,
        ),
      );
      if (!this.applyCurrentQuestion(response)) return;

      this.preparationState.set('ready');
    } catch {
      this.preparationState.set('questionError');
    }
  }

  private async navigateTo(targetQuestionNumber: number): Promise<void> {
    const session = this.getSession();
    if (
      !session ||
      this.questionMutationsBlocked() ||
      this.navigating() ||
      this.answerSaveState() === 'saving' ||
      this.navigationRequiresReload()
    ) {
      return;
    }

    this.navigating.set(true);
    this.operationVersion += 1;
    this.navigationError.set(false);
    this.answerSaveState.set('idle');

    try {
      await firstValueFrom(
        this.candidateExamService.navigate(session.testAttemptId, session.activeSessionId, {
          targetQuestionNumber,
        }),
      );
      this.navigationRequiresReload.set(true);
      await this.reloadAfterNavigation();
    } catch {
      this.navigationError.set(true);
    } finally {
      this.navigating.set(false);
      if (this.deadlineReached()) {
        void this.handleDeadlineReached();
      }
    }
  }

  private async reloadAfterNavigation(): Promise<void> {
    const session = this.getSession();
    if (!session) return;

    this.navigationError.set(false);

    try {
      const response = await firstValueFrom(
        this.candidateExamService.getCurrentQuestion(
          session.testAttemptId,
          session.activeSessionId,
        ),
      );
      if (!this.applyCurrentQuestion(response)) return;

      this.navigationRequiresReload.set(false);
    } catch {
      this.navigationError.set(true);
    }
  }

  private applyCurrentQuestion(response: CandidateExamCurrentQuestionResponse): boolean {
    this.candidateExamSession.updateFromCurrentQuestion(response);
    if (!this.candidateExamSession.session()) {
      void this.backToExams();
      return false;
    }

    this.currentQuestion.set(response);
    this.selectedOptionId.set(response.selectedOptionId);
    this.answerSaveState.set('idle');
    this.navigationError.set(false);
    this.anchorTimer(response);
    return true;
  }

  private async registerAfterReconnect(): Promise<void> {
    if (this.reconnectRegistering()) return;

    const session = this.candidateExamSession.session();
    if (!session) {
      this.reconnectRegistrationPending.set(false);
      this.reconnectRegistrationFailed.set(true);
      return;
    }

    this.reconnectRegistering.set(true);
    this.reconnectRegistrationFailed.set(false);

    try {
      await this.realtime.registerAttempt(session.testAttemptId, session.activeSessionId);
      this.reconnectRegistrationPending.set(false);
      this.reconnectRegistering.set(false);
      if (this.deadlineReached()) {
        await this.synchronizeCurrentQuestion();
      }
    } catch {
      this.reconnectRegistrationPending.set(false);
      this.reconnectRegistrationFailed.set(true);
    } finally {
      this.reconnectRegistering.set(false);
    }
  }

  private getSession() {
    let session = this.candidateExamSession.session();
    if (!session) {
      this.candidateExamSession.restore();
      session = this.candidateExamSession.session();
    }

    if (!session) void this.backToExams();
    return session;
  }

  async retryTimeout(): Promise<void> {
    this.timeoutFailed.set(false);
    await this.synchronizeCurrentQuestion();
    if (this.deadlineReached()) await this.finalizeTimeout();
  }

  dismissPassedNotice(): void {
    this.candidateExamSession.clearOutcome();
  }

  private anchorTimer(response: CandidateExamCurrentQuestionResponse): void {
    const deadlineReached = this.executionTimer.synchronize(
      response.serverTimeUtc,
      response.effectiveDeadlineAt,
    );
    if (deadlineReached === null) return;

    this.deadlineReached.set(response.isTimeExpired || deadlineReached);
    this.startTimer();

    if (this.deadlineReached()) {
      this.closeSubmissionConfirmation();
      void this.handleDeadlineReached();
    }
  }

  private startTimer(): void {
    this.executionTimer.start(this.handleTimerDeadlineReached);
    if (!this.resyncInterval) {
      this.resyncInterval = setInterval(() => {
        if (!document.hidden) void this.synchronizeCurrentQuestion();
      }, CANDIDATE_EXAM_TIMER_SERVER_SYNC_INTERVAL_MS);
    }
  }

  private stopServerSynchronization(): void {
    if (this.resyncInterval) clearInterval(this.resyncInterval);
    this.resyncInterval = null;
  }

  private async handleDeadlineReached(): Promise<void> {
    if (this.realtimeMutationsBlocked() || this.timeoutFailed()) return;
    await this.finalizeTimeout();
  }

  private async synchronizeCurrentQuestion(): Promise<void> {
    const session = this.candidateExamSession.session();
    const question = this.currentQuestion();
    if (
      !session ||
      !question ||
      this.syncInFlight ||
      this.answerSaveState() === 'saving' ||
      this.navigating() ||
      this.realtimeMutationsBlocked() ||
      this.timeoutProcessing() ||
      this.timeoutFailed() ||
      this.submissionProcessing() ||
      this.submissionFailed() ||
      this.terminalOutcome()
    ) {
      return;
    }

    const version = this.operationVersion;
    const attemptId = session.testAttemptId;
    const partId = question.testAttemptPartId;
    this.syncInFlight = true;
    this.timerSynchronizing.set(true);

    try {
      const response = await firstValueFrom(
        this.candidateExamService.getCurrentQuestion(attemptId, session.activeSessionId),
      );
      const currentSession = this.candidateExamSession.session();
      if (
        version !== this.operationVersion ||
        currentSession?.testAttemptId !== attemptId ||
        currentSession.testAttemptPartId !== partId ||
        response.testAttemptId !== attemptId ||
        response.testAttemptPartId !== partId
      ) {
        return;
      }
      this.applyCurrentQuestion(response);
    } catch {
      // A display synchronization failure does not replace the existing authoritative anchor.
    } finally {
      this.syncInFlight = false;
      this.timerSynchronizing.set(false);
    }
  }

  private async finalizeTimeout(): Promise<void> {
    const session = this.candidateExamSession.session();
    const question = this.currentQuestion();
    if (
      !session ||
      !question ||
      !this.deadlineReached() ||
      this.realtimeMutationsBlocked() ||
      this.answerSaveState() === 'saving' ||
      this.navigating() ||
      this.timeoutProcessing() ||
      this.submissionProcessing() ||
      this.submissionFailed() ||
      this.terminalOutcome()
    ) {
      return;
    }

    this.timeoutProcessing.set(true);
    this.timeoutFailed.set(false);
    try {
      this.operationVersion += 1;
      const response = await firstValueFrom(
        this.candidateExamService.timeoutPart(
          session.testAttemptId,
          question.testAttemptPartId,
          session.activeSessionId,
        ),
      );
      await this.handlePartResult(response);
    } catch {
      this.timeoutFailed.set(true);
    } finally {
      this.timeoutProcessing.set(false);
      if (this.deadlineReached() && this.currentQuestion() && !this.timeoutFailed()) {
        void this.handleDeadlineReached();
      }
    }
  }

  private canSendManualSubmit(allowFailedRetry = false): boolean {
    const realtimeReady = allowFailedRetry
      ? this.connectionStatus() === 'connected' &&
        !this.reconnectRegistrationPending() &&
        !this.reconnectRegistering()
      : !this.realtimeMutationsBlocked();

    return (
      this.candidateExamSession.session() !== null &&
      this.currentQuestion() !== null &&
      (!this.deadlineReached() || (allowFailedRetry && this.submissionFailed())) &&
      realtimeReady &&
      this.answerSaveState() !== 'saving' &&
      !this.navigating() &&
      !this.navigationRequiresReload() &&
      !this.timeoutProcessing() &&
      !this.submissionProcessing() &&
      (allowFailedRetry ? this.submissionFailed() : !this.submissionFailed()) &&
      !this.terminalOutcome()
    );
  }

  private async submitCurrentPart(isRetry = false): Promise<void> {
    if (!this.canSendManualSubmit(isRetry)) return;

    const session = this.candidateExamSession.session();
    const question = this.currentQuestion();
    if (!session || !question) return;

    this.submissionProcessing.set(true);
    this.submissionFailed.set(false);
    this.operationVersion += 1;

    try {
      const response = await firstValueFrom(
        this.candidateExamService.submitPart(
          session.testAttemptId,
          question.testAttemptPartId,
          session.activeSessionId,
        ),
      );
      await this.handlePartResult(response);
    } catch (error: unknown) {
      if (this.isPartTimeExpired(error)) {
        this.deadlineReached.set(true);
        this.closeSubmissionConfirmation();
        this.submissionFailed.set(false);
      } else {
        this.submissionFailed.set(true);
      }
    } finally {
      this.submissionProcessing.set(false);

      if (this.deadlineReached() && this.currentQuestion() && !this.submissionFailed()) {
        void this.handleDeadlineReached();
      }
    }
  }

  private async handlePartResult(
    response: SubmitCandidateExamPartResponse | TimeoutCandidateExamPartResponse,
  ): Promise<void> {
    this.candidateExamSession.applyPartResult(response);

    if (response.outcome === 'PASSED' && response.hasNextPart && response.nextPart) {
      this.currentQuestion.set(null);
      this.deadlineReached.set(false);
      this.executionTimer.resetRemainingTime();
      this.timeoutFailed.set(false);
      this.submissionFailed.set(false);
      await this.loadCurrentQuestion();
      return;
    }

    if (response.outcome === 'FAILED' || response.outcome === 'COMPLETED') {
      this.terminalOutcome.set(response.outcome);
      this.currentQuestion.set(null);
      this.executionTimer.stop();
      this.stopServerSynchronization();
      await this.realtime.disconnect();
    }
  }

  private closeSubmissionConfirmation(): void {
    this.submissionConfirmationRef?.close(false);
    this.submissionConfirmationRef = null;
  }

  private readonly handleVisibilityChange = (): void => {
    if (!document.hidden && !this.terminalOutcome()) {
      void this.synchronizeCurrentQuestion();
    }
  };

  private readonly handleTimerDeadlineReached = (): void => {
    if (this.deadlineReached()) return;
    this.deadlineReached.set(true);
    this.closeSubmissionConfirmation();
    void this.handleDeadlineReached();
  };

  private isPartTimeExpired(error: unknown): boolean {
    if (!(error instanceof HttpErrorResponse)) return false;

    const body: unknown = error.error;
    if (!body || typeof body !== 'object' || !('error' in body)) return false;

    const errors: unknown = body.error;
    return (
      Array.isArray(errors) &&
      errors.some(
        (item: unknown) =>
          item !== null &&
          typeof item === 'object' &&
          'message' in item &&
          item.message === 'CANDIDATE_EXAM_PART_TIME_EXPIRED',
      )
    );
  }
}
