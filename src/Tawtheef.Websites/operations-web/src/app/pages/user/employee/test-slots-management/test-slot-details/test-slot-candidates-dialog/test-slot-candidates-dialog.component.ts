import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { finalize } from 'rxjs';
import { AuthService } from '../../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../../core/constants/permissions';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { PaginationComponent } from '../../../../../../shared/components/pagination/pagination.component';
import { ConfirmationDialogComponent } from '../../../../../../shared/dialogs/confirmation-dialog/confirmation-dialog.component';
import { LanguageService } from '../../../../../../core/services/language.service';
import {
  TestSlotCandidateFilters,
  TestSlotCandidateListItemDto,
  TestSlotSessionDto,
  VerifyTestSlotCandidateIdentityRequest,
} from '../../models/test-slot-details.dto';
import { TestSlotsService } from '../../services/test-slots.service';
import { VerifyIdentityDialogComponent } from './verify-identity-dialog.component';
import { ReportOperationalIssueDialogComponent } from '../report-operational-issue-dialog/report-operational-issue-dialog.component';
import { StatusSeverity } from '../../../../../../shared/types/status-severity.type';

interface TestSlotCandidatesDialogData {
  testSlotId: string;
  isCurrentUserRoomHead: boolean;
}

@Component({
  selector: 'app-test-slot-candidates-dialog',
  standalone: true,
  templateUrl: './test-slot-candidates-dialog.component.html',
  styleUrl: './test-slot-candidates-dialog.component.scss',
  imports: [
    FormsModule,
    TranslatePipe,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    SelectModule,
    TableModule,
    TagModule,
    PaginationComponent,
  ],
})
export class TestSlotCandidatesDialogComponent implements OnInit {
  private readonly service = inject(TestSlotsService);
  private readonly auth = inject(AuthService);
  private readonly language = inject(LanguageService);
  private readonly notifications = inject(NotificationService);
  private readonly translate = inject(TranslateService);
  private readonly dialogs = inject(DialogService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly ref = inject(DynamicDialogRef);
  private readonly data = inject(DynamicDialogConfig<TestSlotCandidatesDialogData>).data!;

  readonly candidates = signal<TestSlotCandidateListItemDto[]>([]);
  readonly sessions = signal<TestSlotSessionDto[]>([]);
  readonly sessionOptions = computed(() =>
    this.sessions().map((session) => ({
      id: session.id,
      label: `${session.examNo} - ${session.sessionNo}`,
    })),
  );
  readonly totalItems = signal(0);
  readonly loading = signal(false);
  readonly authorizingAttemptId = signal<string | null>(null);
  readonly actingCandidateId = signal<string | null>(null);
  readonly filters = signal<TestSlotCandidateFilters>({
    pageNumber: 1,
    pageSize: 10,
    language: this.language.get(),
    search: null,
    sessionId: null,
  });
  search = '';
  selectedSessionId: string | null = null;

  ngOnInit(): void {
    this.service
      .sessions(this.data.testSlotId, this.language.get())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((sessions) => this.sessions.set(sessions));
    this.loadCandidates();
  }

  applyFilters(): void {
    this.filters.update((filters) => ({
      ...filters,
      pageNumber: 1,
      search: this.search.trim() || null,
      sessionId: this.selectedSessionId,
    }));
    this.loadCandidates();
  }

  clearFilters(): void {
    this.search = '';
    this.selectedSessionId = null;
    this.applyFilters();
  }

  onPageChange(pageNumber: number): void {
    this.filters.update((filters) => ({ ...filters, pageNumber }));
    this.loadCandidates();
  }

  onPageSizeChange(pageSize: number): void {
    this.filters.update((filters) => ({ ...filters, pageSize, pageNumber: 1 }));
    this.loadCandidates();
  }

  close(): void {
    this.ref.close();
  }

  private canAct(candidate: TestSlotCandidateListItemDto): boolean {
  return (
    this.data.isCurrentUserRoomHead &&
    this.auth.hasPermission(Permissions.TestSlots.ManagePeriod) &&
    ['Approved', 'Ready', 'InProgress'].includes(
      candidate.sessionStatusBackendName,
    ) &&
    candidate.candidateStatusBackendName === 'Assigned' &&
    this.actingCandidateId() === null
  );
}

  canMarkPresent(candidate: TestSlotCandidateListItemDto): boolean {
    return this.canAct(candidate) && candidate.attendanceStatusBackendName === 'Pending';
  }

  canVerifyIdentity(candidate: TestSlotCandidateListItemDto): boolean {
    return (
      this.canAct(candidate) &&
      candidate.attendanceStatusBackendName === 'Present' &&
      candidate.identityVerificationStatusBackendName === 'Pending'
    );
  }

  canAuthorizeEntry(candidate: TestSlotCandidateListItemDto): boolean {
    return (
      this.canAct(candidate) &&
      candidate.attendanceStatusBackendName === 'Present' &&
      candidate.identityVerificationStatusBackendName === 'Matched'
    );
  }

  markPresent(candidate: TestSlotCandidateListItemDto): void {
    if (!this.canMarkPresent(candidate)) return;
    this.runCandidateAction(
      candidate.candidateId,
      this.service.markPresent(this.data.testSlotId, candidate.candidateId),
    );
  }

  verifyIdentity(candidate: TestSlotCandidateListItemDto): void {
    if (!this.canVerifyIdentity(candidate)) return;
    const dialog = this.dialogs.open(VerifyIdentityDialogComponent, {
      header: this.translate.instant('TEST_SLOT_DETAILS.VERIFY_IDENTITY'),
      width: 'min(30rem, 95vw)',
      modal: true,
    });
    dialog?.onClose
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((request: VerifyTestSlotCandidateIdentityRequest | undefined) => {
        if (!request || !this.canVerifyIdentity(candidate)) return;
        this.runCandidateAction(
          candidate.candidateId,
          this.service.verifyIdentity(this.data.testSlotId, candidate.candidateId, request),
        );
      });
  }

  authorizeEntry(candidate: TestSlotCandidateListItemDto): void {
    if (!this.canAuthorizeEntry(candidate)) return;
    const dialog = this.dialogs.open(ConfirmationDialogComponent, {
      header: this.translate.instant('TEST_SLOT_DETAILS.AUTHORIZE_ENTRY'),
      width: 'min(30rem, 95vw)',
      modal: true,
      data: {
        type: 'warning',
        primaryActionStyle: true,
        description: 'TEST_SLOT_DETAILS.AUTHORIZE_ENTRY_CONFIRMATION',
        confirmText: 'TEST_SLOT_DETAILS.AUTHORIZE_ENTRY',
      },
    });
    dialog?.onClose
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((confirmed: boolean | undefined) => {
        if (confirmed !== true || !this.canAuthorizeEntry(candidate)) return;
        this.runCandidateAction(
          candidate.candidateId,
          this.service.authorizeEntry(this.data.testSlotId, candidate.candidateId),
        );
      });
  }

  private runCandidateAction(
    candidateId: string,
    action: import('rxjs').Observable<void>,
    successKey = 'TEST_SLOT_DETAILS.HALL_ACTION_SUCCESS',
  ): void {
    this.actingCandidateId.set(candidateId);
    action
      .pipe(
        finalize(() => this.actingCandidateId.set(null)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.notifications.success(this.translate.instant(successKey));
          this.loadCandidates();
        },
        error: () =>
          this.notifications.error(this.translate.instant('TEST_SLOT_DETAILS.HALL_ACTION_FAILED')),
      });
  }

  canReportOperationalIssue(candidate: TestSlotCandidateListItemDto): boolean {
    return (
      this.data.isCurrentUserRoomHead &&
      this.auth.hasPermission(Permissions.TestSlots.ManagePeriod) &&
      candidate.sessionStatusBackendName === 'InProgress' &&
      candidate.candidateStatusBackendName === 'Completed' &&
      candidate.testAttemptId !== null &&
      (candidate.attemptStatusBackendName === 'Completed' ||
        candidate.attemptStatusBackendName === 'TimedOut') &&
      !candidate.hasOpenInterruption &&
      this.actingCandidateId() === null &&
      this.authorizingAttemptId() === null
    );
  }

  isOperationalIssueRecorded(candidate: TestSlotCandidateListItemDto): boolean {
    return (
      candidate.hasOpenInterruption &&
      (candidate.attemptStatusBackendName === 'Completed' ||
        candidate.attemptStatusBackendName === 'TimedOut')
    );
  }

  reportOperationalIssue(candidate: TestSlotCandidateListItemDto): void {
    if (!this.canReportOperationalIssue(candidate) || !candidate.testAttemptId) return;
    const dialog = this.dialogs.open(ReportOperationalIssueDialogComponent, {
      header: this.translate.instant('TEST_SLOT_DETAILS.REPORT_OPERATIONAL_ISSUE'),
      width: 'min(32rem, 95vw)',
      modal: true,
    });
    dialog?.onClose
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((reason: string | undefined) => {
        if (
          typeof reason !== 'string' ||
          !reason.trim() ||
          !this.canReportOperationalIssue(candidate) ||
          !candidate.testAttemptId
        )
          return;
        this.runCandidateAction(
          candidate.candidateId,
          this.service.reportOperationalIssue(candidate.testAttemptId, reason.trim()),
          'TEST_SLOT_DETAILS.OPERATIONAL_ISSUE_SUCCESS',
        );
      });
  }

  canAuthorizeResume(candidate: TestSlotCandidateListItemDto): boolean {
    return (
      candidate.testAttemptId !== null &&
      candidate.attemptStatusBackendName === 'Interrupted' &&
      !candidate.resumeAuthorized &&
      this.data.isCurrentUserRoomHead &&
      this.auth.hasPermission(Permissions.TestSlots.ManagePeriod)
    );
  }

  isResumeAuthorized(candidate: TestSlotCandidateListItemDto): boolean {
    return candidate.attemptStatusBackendName === 'Interrupted' && candidate.resumeAuthorized;
  }

  authorizeResume(candidate: TestSlotCandidateListItemDto): void {
    const testAttemptId = candidate.testAttemptId;
    if (
      !testAttemptId ||
      !this.canAuthorizeResume(candidate) ||
      this.authorizingAttemptId() !== null
    )
      return;

    this.authorizingAttemptId.set(testAttemptId);
    const dialogRef = this.dialogs.open(ConfirmationDialogComponent, {
      header: this.translate.instant('TEST_SLOT_DETAILS.AUTHORIZE_RESUME_TITLE'),
      width: 'min(32rem, 95vw)',
      modal: true,
      data: {
        type: 'warning',
        primaryActionStyle: true,
        description: 'TEST_SLOT_DETAILS.AUTHORIZE_RESUME_CONFIRMATION',
        confirmText: 'TEST_SLOT_DETAILS.AUTHORIZE_RESUME',
      },
    });
    if (!dialogRef) {
      this.authorizingAttemptId.set(null);
      return;
    }

    dialogRef.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((confirmed) => {
      if (confirmed !== true) {
        this.authorizingAttemptId.set(null);
        return;
      }

      this.service
        .authorizeResume(testAttemptId)
        .pipe(
          finalize(() => this.authorizingAttemptId.set(null)),
          takeUntilDestroyed(this.destroyRef),
        )
        .subscribe({
          next: () => {
            this.notifications.success(
              this.translate.instant('TEST_SLOT_DETAILS.AUTHORIZE_RESUME_SUCCESS'),
            );
            this.loadCandidates();
          },
          error: () =>
            this.notifications.error(
              this.translate.instant('TEST_SLOT_DETAILS.AUTHORIZE_RESUME_FAILED'),
            ),
        });
    });
  }

  private loadCandidates(): void {
    this.loading.set(true);
    this.service
      .candidates(this.data.testSlotId, this.filters())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.candidates.set(response.items);
          this.totalItems.set(response.metadata.totalCount);
          this.loading.set(false);
        },
        error: () => {
          this.candidates.set([]);
          this.totalItems.set(0);
          this.loading.set(false);
        },
      });
  }

  attendanceSeverity(backendName: string): StatusSeverity {
    switch (backendName) {
      case 'Present':
        return 'success';

      case 'NoShow':
        return 'danger';

      default:
        return 'secondary';
    }
  }

  identitySeverity(backendName: string): StatusSeverity {
    switch (backendName) {
      case 'Matched':
        return 'success';

      case 'NotMatched':
        return 'danger';

      case 'Pending':
        return 'info';

      default:
        return 'secondary';
    }
  }

  candidateStatusSeverity(backendName: string): StatusSeverity {
    switch (backendName) {
      case 'Authorized':
      case 'Completed':
        return 'success';

      case 'Started':
      case 'Rescheduled':
        return 'info';

      case 'Cancelled':
        return 'danger';

      default:
        return 'secondary';
    }
  }

  attemptStatusSeverity(backendName?: string | null): StatusSeverity {
    switch (backendName) {
      case 'Completed':
        return 'success';

      case 'TimedOut':
      case 'Interrupted':
        return 'warn';

      case 'InProgress':
        return 'info';

      case 'Cancelled':
        return 'danger';

      default:
        return 'secondary';
    }
  }
}
