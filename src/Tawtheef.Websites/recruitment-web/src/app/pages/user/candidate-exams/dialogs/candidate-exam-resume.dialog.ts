import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { InputText } from 'primeng/inputtext';
import { routes } from '../../../../routes/routes';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { CandidateExamResumeResult } from '../models/candidate-exam-realtime.model';
import { CandidateExamRealtimeService } from '../services/candidate-exam-realtime.service';
import { CandidateExamSessionService } from '../services/candidate-exam-session.service';

interface CandidateExamResumeDialogData {
  readonly testAttemptId: string;
}

@Component({
  selector: 'app-candidate-exam-resume-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, InputText, I18nNamespaceDirective],
  templateUrl: './candidate-exam-resume.dialog.html',
  styleUrl: './candidate-exam-resume.dialog.scss',
})
export class CandidateExamResumeDialogComponent implements OnDestroy {
  private readonly formBuilder = inject(FormBuilder);
  private readonly config = inject(DynamicDialogConfig<CandidateExamResumeDialogData>);
  private readonly ref = inject(DynamicDialogRef);
  private readonly candidateExamSession = inject(CandidateExamSessionService);
  private readonly realtime = inject(CandidateExamRealtimeService);
  private readonly router = inject(Router);

  readonly form = this.formBuilder.nonNullable.group({
    accessCode: ['', Validators.required],
  });

  readonly resuming = signal(false);
  readonly resumeFailed = signal(false);
  readonly registrationFailed = signal(false);
  readonly retryingRegistration = signal(false);
  private resumeSucceeded = false;

  ngOnDestroy(): void {
    if (!this.resumeSucceeded) {
      void this.realtime.disconnect();
    }
  }

  async resumeExam(): Promise<void> {
    if (this.resuming() || this.resumeSucceeded) return;

    this.form.controls.accessCode.markAsTouched();
    if (this.form.invalid) return;

    this.resuming.set(true);
    this.resumeFailed.set(false);
    this.registrationFailed.set(false);

    let response: CandidateExamResumeResult;
    try {
      await this.realtime.connect();
      response = await this.realtime.resumeAttempt(
        this.testAttemptId,
        this.form.controls.accessCode.value,
      );
    } catch {
      this.resumeFailed.set(true);
      await this.realtime.disconnect();
      this.resuming.set(false);
      return;
    }

    this.resumeSucceeded = true;
    this.candidateExamSession.initializeFromResume(response);
    this.form.controls.accessCode.disable({ emitEvent: false });

    try {
      await this.realtime.registerAttempt(response.testAttemptId, response.activeSessionId);
    } catch (error) {
      console.error('[CandidateExam] RegisterAttempt failed', error);
      this.registrationFailed.set(true);
      return;
    }

    try {
      await this.navigateToExecution();
    } catch (error) {
      console.error('[CandidateExam] Navigation to execution failed', error);
      this.registrationFailed.set(true);
    } finally {
      this.resuming.set(false);
    }
  }

  async retryRegistration(): Promise<void> {
    if (this.retryingRegistration()) return;

    const session = this.candidateExamSession.session();
    if (!session) {
      this.registrationFailed.set(true);
      return;
    }

    this.retryingRegistration.set(true);
    try {
      await this.realtime.connect();
      await this.realtime.registerAttempt(session.testAttemptId, session.activeSessionId);
      await this.navigateToExecution();
    } catch {
      this.registrationFailed.set(true);
    } finally {
      this.retryingRegistration.set(false);
    }
  }

  async cancel(): Promise<void> {
    if (this.resuming() || this.resumeSucceeded) return;
    await this.realtime.disconnect();
    this.ref.close();
  }

  private get testAttemptId(): string {
    return this.config.data.testAttemptId;
  }

  private async navigateToExecution(): Promise<void> {
  const navigated = await this.router.navigateByUrl(
    routes.user.candidateExamExecution,
  );

  if (!navigated) {
    throw new Error(
      'Candidate Exam execution navigation was not completed.',
    );
  }

  this.ref.close(true);
}
}
