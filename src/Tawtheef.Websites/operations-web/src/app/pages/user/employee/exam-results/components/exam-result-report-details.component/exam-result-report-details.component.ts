import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';
import { TooltipModule } from 'primeng/tooltip';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import {
  catchError,
  distinctUntilChanged,
  finalize,
  map,
  of,
  startWith,
  Subject,
  switchMap,
  tap,
} from 'rxjs';
import { AuthService } from '../../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../../core/constants/permissions';
import { LanguageService } from '../../../../../../core/services/language.service';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { portalRoutes } from '../../../../../../routes/portal-routes';
import { I18nNamespaceDirective } from '../../../../../../shared/directives/i18n-namespace.directive';
import { FaDirArrowDirective } from '../../../../../../shared/directives/dir-arrow.directive';
import { HasPermissionDirective } from '../../../../../../shared/directives/has-permission.directive';
import { ConfirmationDialogComponent } from '../../../../../../shared/dialogs/confirmation-dialog/confirmation-dialog.component';
import {
  ExamResultCandidate,
  ExamResultLookup,
  ExamResultReportDetails,
} from '../../models/exam-result-report.models';
import { ExamResultReportsService } from '../../services/exam-result-reports.service';
import { ExamResultCandidateAttemptDialogComponent } from '../../dialogs/exam-result-candidate-attempt/exam-result-candidate-attempt-dialog.component';
import { ResolveExamResultCandidateDialogComponent } from '../../dialogs/resolve-exam-result-candidate/resolve-exam-result-candidate-dialog.component';
import { RescheduleExamResultCandidateDialogComponent } from '../../dialogs/reschedule-exam-result-candidate/reschedule-exam-result-candidate-dialog.component';
import { ReturnExamResultReportDialogComponent } from '../../dialogs/return-exam-result-report/return-exam-result-report-dialog.component';

@Component({
  selector: 'app-exam-result-report-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    TranslatePipe,
    TableModule,
    TagModule,
    TooltipModule,
    I18nNamespaceDirective,
    FaDirArrowDirective,
    HasPermissionDirective,
  ],
  templateUrl: './exam-result-report-details.component.html',
  providers: [DialogService],
})
export class ExamResultReportDetailsComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly service = inject(ExamResultReportsService);
  private readonly language = inject(LanguageService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly dialogs = inject(DialogService);
  private readonly translate = inject(TranslateService);
  private readonly auth = inject(AuthService);
  private readonly notifications = inject(NotificationService);
  private readonly reload$ = new Subject<void>();

  protected readonly routes = portalRoutes;
  protected readonly permissions = Permissions;
  readonly report = signal<ExamResultReportDetails | null>(null);
  readonly loading = signal(true);
  readonly loadError = signal(false);
  readonly submitting = signal(false);
  readonly actionDialogOpen = signal(false);

  ngOnInit(): void {
    this.route.paramMap
      .pipe(
        map((params) => params.get('reportId')),
        distinctUntilChanged(),
        switchMap((reportId) =>
          this.reload$.pipe(
            startWith(void 0),
            switchMap(() => {
              this.loading.set(true);
              this.loadError.set(false);
              this.report.set(null);
              return reportId
                ? this.service.details(reportId).pipe(
                    catchError(() => {
                      this.loadError.set(true);
                      return of(null);
                    }),
                  )
                : of(null);
            }),
          ),
        ),
        tap(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((report) => this.report.set(report));
  }

  localized(ar: string | null, en: string | null): string {
    return (this.language.get() === 'ar' ? ar : en) || ar || '—';
  }

  lookupName(lookup: ExamResultLookup | null): string {
    return lookup ? this.localized(lookup.nameAr, lookup.nameEn) : '—';
  }

  resultStatusSeverity(backendName: string): 'success' | 'warn' | 'secondary' {
    switch (backendName) {
      case 'QualifiedForInterview':
        return 'success';

      case 'RequiresAction':
        return 'warn';

      default:
        return 'secondary';
    }
  }

  attemptStatusSeverity(backendName: string | undefined): 'success' | 'warn' | 'secondary' {
    switch (backendName) {
      case 'Completed':
        return 'success';

      case 'TimedOut':
      case 'Interrupted':
        return 'warn';

      default:
        return 'secondary';
    }
  }

  openAttempt(candidateResultId: string): void {
    const reportId = this.report()?.id;
    if (!reportId || !this.auth.hasPermission(Permissions.Exams.ViewAttemptDetails)) return;
    this.dialogs.open(ExamResultCandidateAttemptDialogComponent, {
      header: this.translate.instant('EXAM_RESULTS.ATTEMPT_DETAILS'),
      width: 'min(72rem, 96vw)',
      contentStyle: { maxHeight: '75vh', overflowY: 'auto' },
      modal: true,
      closable: true,
      dismissableMask: false,
      data: { reportId, candidateResultId },
    });
  }

  hasPendingActions(report: ExamResultReportDetails): boolean {
    return report.candidates.some(
      (candidate) => candidate.resultStatus.backendName === 'RequiresAction',
    );
  }

  approve(): void {
    const report = this.report();
    if (
      !report ||
      report.status.backendName !== 'UnderReview' ||
      this.hasPendingActions(report) ||
      this.loading() ||
      this.submitting() ||
      this.actionDialogOpen() ||
      !this.auth.hasPermission(Permissions.Exams.ApproveResults)
    )
      return;

    this.actionDialogOpen.set(true);
    const ref = this.dialogs.open(ConfirmationDialogComponent, {
      header: this.translate.instant('EXAM_RESULTS.APPROVE_REPORT'),
      width: 'min(32rem, 95vw)',
      modal: true,
      data: {
        type: 'submit',
        description: 'EXAM_RESULTS.APPROVE_CONFIRMATION',
        confirmText: 'EXAM_RESULTS.APPROVE_REPORT',
      },
    });
    if (!ref) {
      this.actionDialogOpen.set(false);
      return;
    }
    ref.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((confirmed) => {
      this.actionDialogOpen.set(false);
      const latestReport = this.report();
      if (
        confirmed !== true ||
        !latestReport ||
        latestReport.id !== report.id ||
        latestReport.status.backendName !== 'UnderReview' ||
        this.hasPendingActions(latestReport)
      )
        return;
      this.submitting.set(true);
      this.service
        .approve(report.id, { decisionNotes: null })
        .pipe(
          finalize(() => this.submitting.set(false)),
          takeUntilDestroyed(this.destroyRef),
        )
        .subscribe({
          next: () => {
            this.notifications.success(this.translate.instant('EXAM_RESULTS.APPROVE_SUCCESS'));
            this.reload$.next();
          },
          error: (error: unknown) =>
            this.notifications.error(
              this.translate.instant(
                this.hasPendingActionsError(error)
                  ? 'EXAM_RESULTS.PENDING_ACTIONS'
                  : 'EXAM_RESULTS.APPROVE_FAILED',
              ),
            ),
        });
    });
  }

  returnForProcessing(): void {
    const report = this.report();
    if (
      !report ||
      report.status.backendName !== 'UnderReview' ||
      !this.hasPendingActions(report) ||
      this.loading() ||
      this.submitting() ||
      this.actionDialogOpen() ||
      !this.auth.hasPermission(Permissions.Exams.WorkflowActions)
    )
      return;

    this.actionDialogOpen.set(true);
    const ref = this.dialogs.open(ReturnExamResultReportDialogComponent, {
      header: this.translate.instant('EXAM_RESULTS.RETURN_FOR_PROCESSING'),
      width: 'min(32rem, 95vw)',
      modal: true,
      closable: true,
      dismissableMask: false,
    });
    if (!ref) {
      this.actionDialogOpen.set(false);
      return;
    }
    ref.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((reason: unknown) => {
      this.actionDialogOpen.set(false);
      const latestReport = this.report();
      if (
        typeof reason !== 'string' ||
        !reason.trim() ||
        !latestReport ||
        latestReport.id !== report.id ||
        latestReport.status.backendName !== 'UnderReview' ||
        !this.hasPendingActions(latestReport)
      )
        return;
      this.submitting.set(true);
      this.service
        .returnForProcessing(report.id, { reason: reason.trim() })
        .pipe(
          finalize(() => this.submitting.set(false)),
          takeUntilDestroyed(this.destroyRef),
        )
        .subscribe({
          next: () => {
            this.notifications.success(this.translate.instant('EXAM_RESULTS.RETURN_SUCCESS'));
            this.reload$.next();
          },
          error: () =>
            this.notifications.error(this.translate.instant('EXAM_RESULTS.RETURN_FAILED')),
        });
    });
  }

  canResolveCandidate(report: ExamResultReportDetails, candidate: ExamResultCandidate): boolean {
    return (
      report.status.backendName === 'Returned' &&
      candidate.resultStatus.backendName === 'RequiresAction' &&
      this.auth.hasPermission(Permissions.Exams.WorkflowActions)
    );
  }

  reschedule(candidate: ExamResultCandidate): void {
    const report = this.report();
    if (
      !report ||
      !this.canResolveCandidate(report, candidate) ||
      this.loading() ||
      this.submitting() ||
      this.actionDialogOpen()
    )
      return;

    this.actionDialogOpen.set(true);
    const ref = this.dialogs.open(RescheduleExamResultCandidateDialogComponent, {
      header: this.translate.instant('EXAM_RESULTS.RESCHEDULE'),
      width: 'min(40rem, 96vw)',
      contentStyle: { maxHeight: '75vh', overflowY: 'auto' },
      modal: true,
      closable: true,
      dismissableMask: false,
      data: { reportId: report.id, candidateResultId: candidate.id },
    });
    if (!ref) {
      this.actionDialogOpen.set(false);
      return;
    }
    ref.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((completed: unknown) => {
      this.actionDialogOpen.set(false);
      if (completed !== true || this.report()?.id !== report.id) return;
      this.notifications.success(this.translate.instant('EXAM_RESULTS.RESCHEDULE_SUCCESS'));
      this.reload$.next();
    });
  }

  resolveNotCompleted(candidate: ExamResultCandidate): void {
    const report = this.report();
    if (
      !report ||
      !this.canResolveCandidate(report, candidate) ||
      this.loading() ||
      this.submitting() ||
      this.actionDialogOpen()
    )
      return;

    this.actionDialogOpen.set(true);
    const ref = this.dialogs.open(ResolveExamResultCandidateDialogComponent, {
      header: this.translate.instant('EXAM_RESULTS.RESOLVE_NOT_COMPLETED'),
      width: 'min(32rem, 95vw)',
      modal: true,
      closable: true,
      dismissableMask: false,
    });
    if (!ref) {
      this.actionDialogOpen.set(false);
      return;
    }
    ref.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((notes: unknown) => {
      this.actionDialogOpen.set(false);
      const latestReport = this.report();
      const latestCandidate = latestReport?.candidates.find((item) => item.id === candidate.id);
      if (
        typeof notes !== 'string' ||
        !notes.trim() ||
        !latestReport ||
        latestReport.id !== report.id ||
        !latestCandidate ||
        !this.canResolveCandidate(latestReport, latestCandidate)
      )
        return;

      this.submitting.set(true);
      this.service
        .resolveNotCompleted(report.id, candidate.id, {
          resolutionNotes: notes.trim(),
        })
        .pipe(
          finalize(() => this.submitting.set(false)),
          takeUntilDestroyed(this.destroyRef),
        )
        .subscribe({
          next: () => {
            this.notifications.success(
              this.translate.instant('EXAM_RESULTS.RESOLVE_NOT_COMPLETED_SUCCESS'),
            );
            this.reload$.next();
          },
          error: () =>
            this.notifications.error(
              this.translate.instant('EXAM_RESULTS.RESOLVE_NOT_COMPLETED_FAILED'),
            ),
        });
    });
  }

  canAcceptCalculatedScore(
    report: ExamResultReportDetails,
    candidate: ExamResultCandidate,
  ): boolean {
    return (
      report.status.backendName === 'Returned' &&
      candidate.resultStatus.backendName === 'RequiresAction' &&
      (candidate.attemptStatus?.backendName === 'Completed' ||
        candidate.attemptStatus?.backendName === 'TimedOut') &&
      candidate.hasOpenInterruption &&
      this.auth.hasPermission(Permissions.Exams.WorkflowActions)
    );
  }

  acceptCalculatedScore(candidate: ExamResultCandidate): void {
    const report = this.report();
    if (
      !report ||
      !this.canAcceptCalculatedScore(report, candidate) ||
      this.loading() ||
      this.submitting() ||
      this.actionDialogOpen()
    )
      return;

    this.actionDialogOpen.set(true);
    const ref = this.dialogs.open(ConfirmationDialogComponent, {
      header: this.translate.instant('EXAM_RESULTS.ACCEPT_CALCULATED_SCORE'),
      width: 'min(32rem, 95vw)',
      modal: true,
      data: {
        type: 'submit',
        description: 'EXAM_RESULTS.ACCEPT_CALCULATED_SCORE_CONFIRMATION',
        confirmText: 'EXAM_RESULTS.ACCEPT_CALCULATED_SCORE',
        showInputField: true,
        inputType: 'textarea',
        inputLabel: 'EXAM_RESULTS.RESOLUTION_NOTES',
        inputPlaceholder: 'EXAM_RESULTS.RESOLUTION_NOTES_REQUIRED',
        inputMaxLength: 2000,
      },
    });
    if (!ref) {
      this.actionDialogOpen.set(false);
      return;
    }
    ref.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((notes: unknown) => {
      this.actionDialogOpen.set(false);
      const latestReport = this.report();
      const latestCandidate = latestReport?.candidates.find((item) => item.id === candidate.id);
      if (
        typeof notes !== 'string' ||
        !notes.trim() ||
        !latestReport ||
        latestReport.id !== report.id ||
        !latestCandidate ||
        !this.canAcceptCalculatedScore(latestReport, latestCandidate)
      )
        return;

      this.submitting.set(true);
      this.service
        .acceptCalculatedScore(report.id, candidate.id, {
          resolutionNotes: notes.trim(),
        })
        .pipe(
          finalize(() => this.submitting.set(false)),
          takeUntilDestroyed(this.destroyRef),
        )
        .subscribe({
          next: () => {
            this.notifications.success(
              this.translate.instant('EXAM_RESULTS.ACCEPT_CALCULATED_SCORE_SUCCESS'),
            );
            this.reload$.next();
          },
          error: () =>
            this.notifications.error(
              this.translate.instant('EXAM_RESULTS.ACCEPT_CALCULATED_SCORE_FAILED'),
            ),
        });
    });
  }

  private hasPendingActionsError(error: unknown): boolean {
    if (!(error instanceof HttpErrorResponse)) return false;
    const body: unknown = error.error;
    if (
      typeof body !== 'object' ||
      body === null ||
      !('error' in body) ||
      !Array.isArray(body.error)
    )
      return false;
    return body.error.some(
      (entry: unknown) =>
        typeof entry === 'object' &&
        entry !== null &&
        'message' in entry &&
        entry.message === 'EXAM_RESULT_REPORT_HAS_PENDING_ACTIONS',
    );
  }
}
