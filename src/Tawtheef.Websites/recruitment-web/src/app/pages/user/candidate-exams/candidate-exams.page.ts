import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';
import { LanguageService } from '../../../core/services/language.service';
import { routes } from '../../../routes/routes';
import { I18nNamespaceDirective } from '../../../shared/directives/i18n-namespace.directive';
import { CandidateExamAccessCodeDialogComponent } from './dialogs/candidate-exam-access-code.dialog';
import { CandidateExamResumeDialogComponent } from './dialogs/candidate-exam-resume.dialog';
import { CandidateExamAssignment, CandidateExamEntryState } from './models/candidate-exam.model';
import { CandidateExamService } from './services/candidate-exam.service';
import { CandidateExamSessionService } from './services/candidate-exam-session.service';
import { CandidateExamPendingStartService } from './services/candidate-exam-pending-start.service';

type CandidateExamStatusTone = 'success' | 'warning' | 'danger' | 'info' | 'neutral';

interface CandidateExamStatusPresentation {
  readonly labelKey: string;
  readonly tone: CandidateExamStatusTone;
}

const ENTRY_STATE_PRESENTATION: Record<CandidateExamEntryState, CandidateExamStatusPresentation> = {
  Assigned: { labelKey: 'candidate_exams.states.assigned', tone: 'neutral' },
  NotEligible: { labelKey: 'candidate_exams.states.notEligible', tone: 'danger' },
  WaitingForSession: {
    labelKey: 'candidate_exams.states.waitingForSession',
    tone: 'info',
  },
  WaitingForAttendance: {
    labelKey: 'candidate_exams.states.waitingForAttendance',
    tone: 'warning',
  },
  WaitingForIdentityVerification: {
    labelKey: 'candidate_exams.states.waitingForIdentityVerification',
    tone: 'warning',
  },
  WaitingForAuthorization: {
    labelKey: 'candidate_exams.states.waitingForAuthorization',
    tone: 'info',
  },
  ReadyToEnter: { labelKey: 'candidate_exams.states.readyToEnter', tone: 'success' },
  Started: { labelKey: 'candidate_exams.states.inProgress', tone: 'info' },
  NoShow: { labelKey: 'candidate_exams.states.noShow', tone: 'danger' },
  Completed: { labelKey: 'candidate_exams.states.completed', tone: 'success' },
  Rescheduled: { labelKey: 'candidate_exams.states.rescheduled', tone: 'warning' },
  Cancelled: { labelKey: 'candidate_exams.states.cancelled', tone: 'danger' },
  Closed: { labelKey: 'candidate_exams.states.closed', tone: 'neutral' },
};

@Component({
  selector: 'app-candidate-exams',
  standalone: true,
  imports: [CommonModule, TranslatePipe, I18nNamespaceDirective],
  templateUrl: './candidate-exams.page.html',
  styleUrl: './candidate-exams.page.scss',
})
export class CandidateExamsPage implements OnInit {
  private readonly candidateExamService = inject(CandidateExamService);
  private readonly candidateExamSession = inject(CandidateExamSessionService);
  private readonly language = inject(LanguageService);
  private readonly dialogService = inject(DialogService);
  private readonly translate = inject(TranslateService);
  private readonly router = inject(Router);
  private readonly pendingStart = inject(CandidateExamPendingStartService);

  readonly assignmentsResource = rxResource<CandidateExamAssignment[], void>({
    stream: () => this.candidateExamService.getAssignments(),
    defaultValue: [],
  });

  ngOnInit(): void {
    if (!this.candidateExamSession.hasActiveSession()) {
      this.candidateExamSession.restore();
    }
  }

  examTitle(assignment: CandidateExamAssignment): string {
    return this.localizedValue(assignment.examTitleAr, assignment.examTitleEn);
  }

  roomName(assignment: CandidateExamAssignment): string {
    return this.localizedValue(assignment.roomNameAr, assignment.roomNameEn);
  }

  locationName(assignment: CandidateExamAssignment): string {
    return this.localizedValue(assignment.locationNameAr, assignment.locationNameEn);
  }

  displayTime(value: string): string {
    return value.slice(0, 5);
  }

  statusPresentation(assignment: CandidateExamAssignment): CandidateExamStatusPresentation {
    if (assignment.entryState !== 'Started') {
      return ENTRY_STATE_PRESENTATION[assignment.entryState];
    }

    if (assignment.attemptStatus?.backendName === 'Interrupted') {
      return { labelKey: 'candidate_exams.states.interrupted', tone: 'warning' };
    }

    return ENTRY_STATE_PRESENTATION.Started;
  }

  canEnterExam(assignment: CandidateExamAssignment): boolean {
    return assignment.entryState === 'ReadyToEnter';
  }

  canContinueExam(assignment: CandidateExamAssignment): boolean {
    const session = this.candidateExamSession.session();
    return (
      assignment.entryState === 'Started' &&
      assignment.attemptStatus?.backendName === 'InProgress' &&
      assignment.testAttemptId !== null &&
      session?.testAttemptId === assignment.testAttemptId
    );
  }

  canRecoverStart(assignment: CandidateExamAssignment): boolean {
    return (
      assignment.entryState === 'Started' &&
      assignment.attemptStatus?.backendName === 'InProgress' &&
      !this.canContinueExam(assignment) &&
      this.pendingStart.get(assignment.testSessionCandidateId) !== null
    );
  }

  canResumeExam(assignment: CandidateExamAssignment): boolean {
    return (
      assignment.entryState === 'Started' &&
      assignment.attemptStatus?.backendName === 'Interrupted' &&
      assignment.testAttemptId !== null
    );
  }

  openAccessCodeDialog(assignment: CandidateExamAssignment): void {
    if (!this.canEnterExam(assignment) && !this.canRecoverStart(assignment)) {
      return;
    }

    this.dialogService.open(CandidateExamAccessCodeDialogComponent, {
      header: this.translate.instant('candidate_exams.entry.dialogTitle'),
      width: '32rem',
      breakpoints: { '640px': 'calc(100vw - 2rem)' },
      closable: false,
      closeOnEscape: false,
      dismissableMask: false,
      draggable: false,
      data: { testSessionCandidateId: assignment.testSessionCandidateId },
    });
  }

  continueExam(assignment: CandidateExamAssignment): void {
    if (!this.canContinueExam(assignment)) return;
    void this.router.navigateByUrl(routes.user.candidateExamExecution);
  }

  openResumeDialog(assignment: CandidateExamAssignment): void {
    if (!this.canResumeExam(assignment) || !assignment.testAttemptId) return;

    this.dialogService.open(CandidateExamResumeDialogComponent, {
      header: this.translate.instant('candidate_exams.resume.dialogTitle'),
      width: '32rem',
      breakpoints: { '640px': 'calc(100vw - 2rem)' },
      closable: false,
      closeOnEscape: false,
      dismissableMask: false,
      draggable: false,
      data: { testAttemptId: assignment.testAttemptId },
    });
  }

  retry(): void {
    this.assignmentsResource.reload();
  }

  public localizedValue(arabic: string, english: string | null): string {
    const preferred = this.language.get() === 'ar' ? arabic : english;
    const fallback = this.language.get() === 'ar' ? english : arabic;
    return preferred?.trim() || fallback?.trim() || '';
  }
}
