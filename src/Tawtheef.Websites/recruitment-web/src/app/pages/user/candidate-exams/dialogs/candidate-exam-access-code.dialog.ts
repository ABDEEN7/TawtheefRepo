import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import { InputText } from 'primeng/inputtext';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { routes } from '../../../../routes/routes';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { CandidateExamRealtimeService } from '../services/candidate-exam-realtime.service';
import { CandidateExamService } from '../services/candidate-exam.service';
import { CandidateExamSessionService } from '../services/candidate-exam-session.service';
import { CandidateExamPendingStartService } from '../services/candidate-exam-pending-start.service';
import { StartCandidateExamResponse } from '../models/candidate-exam.model';

interface CandidateExamAccessCodeDialogData {
  readonly testSessionCandidateId: string;
}

@Component({
  selector: 'app-candidate-exam-access-code-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    InputText,
    I18nNamespaceDirective,
  ],
  templateUrl: './candidate-exam-access-code.dialog.html',
  styleUrl: './candidate-exam-access-code.dialog.scss',
})
export class CandidateExamAccessCodeDialogComponent implements OnDestroy {
  private readonly formBuilder = inject(FormBuilder);
  private readonly config =
    inject(DynamicDialogConfig<CandidateExamAccessCodeDialogData>);
  private readonly ref = inject(DynamicDialogRef);
  private readonly candidateExamService = inject(CandidateExamService);
  private readonly candidateExamSession = inject(CandidateExamSessionService);
  private readonly pendingStart = inject(CandidateExamPendingStartService);
  private readonly realtime = inject(CandidateExamRealtimeService);
  private readonly router = inject(Router);

  readonly recoveringStart = this.pendingStart.get(
    this.config.data.testSessionCandidateId,
  ) !== null;

  readonly form = this.formBuilder.nonNullable.group({
    accessCode: ['', Validators.required],
  });

  readonly validating = signal(false);
  readonly validated = signal(false);
  readonly starting = signal(false);
  readonly retryingRegistration = signal(false);
  readonly validationFailed = signal(false);
  readonly startFailed = signal(false);
  readonly registrationFailed = signal(false);

  private startSucceeded = false;

  ngOnDestroy(): void {
    if (!this.startSucceeded) {
      void this.realtime.disconnect();
    }
  }

  async validateAccessCode(): Promise<void> {
    if (this.validating() || this.starting() || this.validated()) return;

    this.form.controls.accessCode.markAsTouched();
    if (this.form.invalid) return;

    this.validating.set(true);
    this.validationFailed.set(false);

    try {
      await firstValueFrom(
        this.candidateExamService.validateAccessCode(
          this.testSessionCandidateId,
          {
            accessCode: this.form.controls.accessCode.value,
          },
        ),
      );

      this.validated.set(true);
      this.form.controls.accessCode.disable({ emitEvent: false });
    } catch {
      this.validationFailed.set(true);
    } finally {
      this.validating.set(false);
    }
  }

  async startExam(): Promise<void> {
    if (
      (!this.validated() && !this.recoveringStart) ||
      this.starting() ||
      this.registrationFailed()
    ) {
      return;
    }

    this.form.controls.accessCode.markAsTouched();
    if (this.form.invalid) return;

    this.starting.set(true);
    this.startFailed.set(false);
    this.registrationFailed.set(false);

    // The same identifier survives a lost response and a page reload
    // within this browser tab.
    const activeSessionId = this.pendingStart.getOrCreate(
      this.testSessionCandidateId,
    );

    let response: StartCandidateExamResponse;

    try {
      await this.realtime.connect();

      response = await firstValueFrom(
        this.candidateExamService.start(
          this.testSessionCandidateId,
          {
            accessCode: this.form.controls.accessCode.getRawValue(),
          },
          activeSessionId,
        ),
      );
    } catch {
      this.startFailed.set(true);
      await this.realtime.disconnect();
      this.starting.set(false);
      return;
    }

    this.startSucceeded = true;
    this.candidateExamSession.initializeFromStart(response);
    this.pendingStart.clear(this.testSessionCandidateId);

    try {
      await this.realtime.registerAttempt(
        response.testAttemptId,
        response.activeSessionId,
      );

      await this.navigateToExecution();
    } catch {
      this.registrationFailed.set(true);
    } finally {
      this.starting.set(false);
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
      await this.realtime.registerAttempt(
        session.testAttemptId,
        session.activeSessionId,
      );
      await this.navigateToExecution();
    } catch {
      this.registrationFailed.set(true);
    } finally {
      this.retryingRegistration.set(false);
    }
  }

  async cancel(): Promise<void> {
    if (this.starting() || this.registrationFailed()) return;

    if (!this.startSucceeded) {
      await this.realtime.disconnect();
    }

    // Keep a pending identifier: /start may have committed on the server.
    this.ref.close();
  }

  private get testSessionCandidateId(): string {
    return this.config.data.testSessionCandidateId;
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