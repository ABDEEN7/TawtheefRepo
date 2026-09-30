import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { TableModule } from 'primeng/table';
import { DialogService } from 'primeng/dynamicdialog';
import { finalize } from 'rxjs';
import { AuthService } from '../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../core/constants/permissions';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { TestSessionCandidateSummaryDto } from '../models/test-session-candidate.dto';
import { TestSessionExamDetailsDto } from '../models/test-session-exam-details.dto';
import { LocalTestSession } from '../models/ready-test-slot.dto';
import {
  TestSlotConfigurationStaffDto,
  TestSlotStaffRoleIds,
} from '../../test-slots-management/models/create-test-slot.dto';
import { TestSlotsService } from '../../test-slots-management/services/test-slots.service';
import {
  TestSessionEditDto,
  TestSessionLookupsDto,
  TEST_SESSION_STATUS_IDS,
} from '../models/test-session-list-item.dto';
import {
  TestSessionGenderFilter,
  TestSessionNationalityFilter,
} from '../models/test-session-setup.dto';
import { LanguageService } from '../../../../../core/services/language.service';
import { NotificationService } from '../../../../../core/services/notification.service';
import { portalRoutes } from '../../../../../routes/portal-routes';
import { TestSessionsService } from '../services/test-sessions.service';
import { TestSessionCandidatesStepComponent } from './test-session-candidates-step/test-session-candidates-step.component';
import { TestSessionExamSelectionStepComponent } from './test-session-exam-selection-step/test-session-exam-selection-step.component';
import { TestSessionPeriodsStepComponent } from './test-session-periods-step/test-session-periods-step.component';
import { TestSessionWorkflowActionComponent } from './test-session-workflow-action/test-session-workflow-action.component';
import { ConfirmationDialogComponent } from '../../../../../shared/dialogs/confirmation-dialog/confirmation-dialog.component';
import { TestSessionDecisionDialogComponent } from './test-session-decision-dialog/test-session-decision-dialog.component';

@Component({
  selector: 'app-test-session-workflow',
  standalone: true,
  templateUrl: './test-session-workflow.component.html',
  styleUrl: './test-session-workflow.component.scss',
  imports: [
    CommonModule,
    TranslatePipe,
    TestSessionExamSelectionStepComponent,
    TestSessionCandidatesStepComponent,
    TestSessionPeriodsStepComponent,
    TestSessionWorkflowActionComponent,
    TableModule,
    I18nNamespaceDirective,
  ],
  providers: [DialogService],
})
export class TestSessionWorkflowComponent {
  private readonly service = inject(TestSessionsService);
  private readonly auth = inject(AuthService);
  private readonly dialogs = inject(DialogService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly language = inject(LanguageService);
  private readonly testSlotsService = inject(TestSlotsService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly translate = inject(TranslateService);
  private readonly notifications = inject(NotificationService);

  readonly lookups = signal<Pick<TestSessionLookupsDto, 'exams' | 'rooms'>>({
    exams: [],
    rooms: [],
  });
  readonly details = signal<TestSessionExamDetailsDto | undefined>(undefined);
  readonly candidateSummary = signal<TestSessionCandidateSummaryDto>({
    total: 0,
    eligible: 0,
    notReady: 0,
    excluded: 0,
  });
  readonly candidateGenderFilter = signal<TestSessionGenderFilter>(null);
  readonly candidateNationalityFilter = signal<TestSessionNationalityFilter>(null);
  readonly selectedCandidateIds = signal<string[]>([]);
  readonly localSession = signal<LocalTestSession | null>(null);
  readonly testSessionId = signal<string | null>(null);
  readonly sessionNo = signal<string | null>(null);
  readonly saving = signal(false);
  readonly editLoading = signal(false);
  readonly editMode = signal(false);
  readonly viewMode = signal(false);
  readonly statusId = signal<string | null>(null);
  readonly decisionNote = signal<string | null>(null);
  readonly testSlotStaff = signal<TestSlotConfigurationStaffDto[]>([]);
  readonly periodTeamLoading = signal(false);
  readonly periodTeamLoadFailed = signal(false);
  private loadedTeamTestSlotId: string | null = null;
  private periodTeamLoadingSlotId: string | null = null;
  private periodTeamLoadRequest = 0;
  readonly candidateSelectionInitialized = signal(false);
  readonly candidateSearch = signal('');
  step = 1;
  selectedExamId?: string;
  loading = true;
  readonly wizardSteps = [
    {
      id: 1,
      label: 'TEST_SESSION_WIZARD.STEP_EXAM',
      icon: 'hgi hgi-stroke hgi-clipboard-check-01 me-1',
    },
    {
      id: 2,
      label: 'TEST_SESSION_WIZARD.STEP_CANDIDATES',
      icon: 'hgi hgi-stroke hgi-user-group me-1',
    },
    {
      id: 3,
      label: 'TEST_SESSION_WIZARD.STEP_PERIODS',
      icon: 'hgi hgi-stroke hgi-calendar-03 me-1',
    },
    {
      id: 4,
      label: 'TEST_SESSION_WIZARD.STEP_ROOM_TEAM',
      icon: 'hgi hgi-stroke hgi-user-group me-1',
    },
    {
      id: 5,
      label: 'TEST_SESSION_WIZARD.STEP_REVIEW',
      icon: 'hgi hgi-stroke hgi-clipboard-check-01 me-1',
    },
  ];

  constructor() {
    this.loadLookups(this.language.get());
    const testSessionId = this.route.snapshot.paramMap.get('testSessionId');
    if (testSessionId) {
      this.viewMode.set(this.route.snapshot.data['testSessionMode'] === 'view');
      this.editMode.set(!this.viewMode());
      this.loadForEdit(testSessionId);
    }
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((language) => this.loadLookups(language));
  }

  private loadForEdit(testSessionId: string): void {
    this.editLoading.set(true);
    const loadSession = this.viewMode()
      ? this.service.view(testSessionId, this.language.get())
      : this.service.edit(testSessionId, this.language.get());
    loadSession
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (session) => {
          this.restoreEditState(session);
          this.editLoading.set(false);
          this.service
            .examDetails(session.examId, this.language.get())
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe({
              next: (details) => this.details.set(details),
              error: () =>
                this.notifications.error(this.translate.instant('TEST_SESSION_WIZARD.SAVE_FAILED')),
            });
        },
        error: () => {
          this.editLoading.set(false);
          this.notifications.error(this.translate.instant('TEST_SESSION_WIZARD.SAVE_FAILED'));
          void this.router.navigateByUrl(portalRoutes.testSessionsManagement);
        },
      });
  }

  private restoreEditState(session: TestSessionEditDto): void {
    this.testSessionId.set(session.testSessionId);
    this.sessionNo.set(session.sessionNo);
    this.statusId.set(session.statusId.toLowerCase());
    this.decisionNote.set(session.decisionNote);
    this.selectedExamId = session.examId;
    this.candidateGenderFilter.set(session.genderFilter);
    this.candidateNationalityFilter.set(session.nationalityFilter);
    this.selectedCandidateIds.set(session.invitationIds);
    this.candidateSelectionInitialized.set(this.viewMode() || session.invitationIds.length > 0);

    if (
      session.testSlotId && session.slotDate && session.slotStartTime && session.slotEndTime &&
      session.roomId && session.roomName && session.slotName && session.roomCapacity != null &&
      session.startTime && session.endTime
    ) {
      this.localSession.set({
        slot: {
          testSlotId: session.testSlotId,
          slotName: session.slotName,
          slotDate: session.slotDate,
          startTime: session.slotStartTime,
          endTime: session.slotEndTime,
          roomId: session.roomId,
          roomName: session.roomName,
          roomCapacity: session.roomCapacity,
          currentReservations: Math.max(0, session.roomCapacity - session.availableCapacity),
          existingSessionCount: 0,
          existingExamSessionCount: 0,
          remainingCapacity: session.availableCapacity,
          status: '',
        },
        startTime: session.startTime,
        endTime: session.endTime,
        availableCapacity: session.availableCapacity,
      });
    }
  }

  onExamSelected(examId?: string): void {
    if (this.editMode()) return;
    this.selectedExamId = examId;
    this.testSessionId.set(null);
    this.sessionNo.set(null);
    this.details.set(undefined);
    this.candidateGenderFilter.set(null);
    this.candidateNationalityFilter.set(null);
    this.selectedCandidateIds.set([]);
    this.candidateSelectionInitialized.set(false);
    this.candidateSearch.set('');
    if (examId) {
      this.service
        .examDetails(examId, this.language.get())
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe((details) => this.details.set(details));
    }
  }

  onCandidateGenderFilterChanged(filter: TestSessionGenderFilter): void {
    this.candidateGenderFilter.set(filter);
  }

  onCandidateNationalityFilterChanged(filter: TestSessionNationalityFilter): void {
    this.candidateNationalityFilter.set(filter);
  }

  onCandidateSelectionInitialized(candidateIds: string[]): void {
    if (!this.candidateSelectionInitialized()) {
      this.selectedCandidateIds.set(candidateIds);
      this.candidateSelectionInitialized.set(true);
    }
  }

  goTo(target: number): void {
    if (target < 1 || target > 5 || (target > 1 && !this.selectedExamId)) return;
    if (this.viewMode()) {
      if (target === 4) this.loadPeriodTeam();
      this.step = target;
      return;
    }
    if (target === 4 && (this.step !== 3 || !this.canGoNext())) return;
    if (target === 5 && (this.step !== 4 || !this.isReviewValid())) return;
    if (target === 4) this.loadPeriodTeam();
    this.step = target;
  }

  canGoNext(): boolean {
    if (this.step === 1) return !!this.selectedExamId;
    if (this.step === 4) return this.isReviewValid();
    if (this.step !== 3) return true;

    const session = this.localSession();
    const candidateCount = this.selectedCandidateIds().length;
    const examDuration = this.details()?.durationMinutes ?? 0;
    if (!session || candidateCount === 0 || session.availableCapacity < candidateCount)
      return false;

    const startTime = this.timeValue(session.startTime);
    const endTime = this.timeValue(session.endTime);
    return (
      startTime >= this.timeValue(session.slot.startTime) &&
      endTime <= this.timeValue(session.slot.endTime) &&
      endTime > startTime &&
      endTime - startTime >= examDuration
    );
  }

  isReviewValid(): boolean {
    const exam = this.details();
    const session = this.localSession();
    const selectedCandidateCount = this.selectedCandidateIds().length;

    if (
      !this.selectedExamId ||
      !exam ||
      exam.examId !== this.selectedExamId ||
      selectedCandidateCount === 0 ||
      !session ||
      !session.slot.testSlotId ||
      session.availableCapacity < selectedCandidateCount
    )
      return false;

    const startTime = this.timeValue(session.startTime);
    const endTime = this.timeValue(session.endTime);
    const sessionTimeIsValid =
      startTime >= this.timeValue(session.slot.startTime) &&
      endTime <= this.timeValue(session.slot.endTime) &&
      endTime > startTime &&
      endTime - startTime >= exam.durationMinutes;
    if (!sessionTimeIsValid) return false;

    const roomHead = this.roomHeadStaff();
    if (
      this.periodTeamLoading() ||
      this.periodTeamLoadFailed() ||
      this.loadedTeamTestSlotId !== session.slot.testSlotId ||
      !roomHead?.staffUserId
    )
      return false;

    const memberIds = this.teamMemberStaff().map(member => member.staffUserId);
    const assignedIds = [roomHead.staffUserId, ...memberIds];
    return (
      memberIds.every(id => id !== roomHead.staffUserId) &&
      new Set(assignedIds).size === assignedIds.length
    );
  }

  remainingReviewCapacity(): number {
    const session = this.localSession();
    return session
      ? Math.max(session.availableCapacity - this.selectedCandidateIds().length, 0)
      : 0;
  }

  saveDraft(): void {
    this.persistSession(false);
  }

  sendToApprove(): void {
    this.persistSession(true);
  }

  onApproveClicked(): void {
    const testSessionId = this.testSessionId();
    if (!testSessionId || this.saving() || this.statusId() !== TEST_SESSION_STATUS_IDS.pendingApproval ||
      !this.auth.hasPermission(Permissions.TestSessions.WorkflowActions)) return;

    this.saving.set(true);
    const dialogRef = this.dialogs.open(ConfirmationDialogComponent, {
      header: this.translate.instant('TEST_SESSION_WIZARD.APPROVE_TITLE'),
      width: 'min(32rem, 95vw)',
      modal: true,
      data: {
        type: 'submit',
        description: 'TEST_SESSION_WIZARD.APPROVE_CONFIRMATION',
        confirmText: 'TEST_SESSION_WIZARD.CONFIRM_APPROVE',
      },
    });
    if (!dialogRef) {
      this.saving.set(false);
      return;
    }
    dialogRef.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((confirmed) => {
      if (confirmed !== true || this.statusId() !== TEST_SESSION_STATUS_IDS.pendingApproval) {
        this.saving.set(false);
        return;
      }
      this.service.approve(testSessionId).pipe(finalize(() => this.saving.set(false)),
        takeUntilDestroyed(this.destroyRef)).subscribe({
        next: () => {
          this.notifications.success(this.translate.instant('TEST_SESSION_WIZARD.APPROVE_SUCCESS'));
          void this.router.navigateByUrl(portalRoutes.testSessionsManagement);
        },
        error: () => {},
      });
    });
  }

  onReturnClicked(): void {
    this.openDecisionDialog('return');
  }

  onRejectClicked(): void {
    this.openDecisionDialog('reject');
  }

  private openDecisionDialog(action: 'return' | 'reject'): void {
    const testSessionId = this.testSessionId();
    if (!testSessionId || this.saving() || this.statusId() !== TEST_SESSION_STATUS_IDS.pendingApproval ||
      !this.auth.hasPermission(Permissions.TestSessions.WorkflowActions)) return;

    const dialogRef = this.dialogs.open(TestSessionDecisionDialogComponent, {
      header: this.translate.instant(`TEST_SESSION_WIZARD.${action.toUpperCase()}_TITLE`),
      width: 'min(32rem, 95vw)',
      closable: true,
      modal: true,
      data: { action },
    });
    if (!dialogRef) return;

    dialogRef.onClose.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((note) => {
      if (typeof note !== 'string' || !note.trim()) return;

      this.saving.set(true);
      const decision = action === 'return'
        ? this.service.returnForEdit(testSessionId, note.trim())
        : this.service.reject(testSessionId, note.trim());
      decision
        .pipe(finalize(() => this.saving.set(false)), takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: () => {
            if (action === 'return') {
              this.notifications.success(this.translate.instant('TEST_SESSION_WIZARD.RETURN_SUCCESS'));
              void this.router.navigateByUrl(portalRoutes.testSessionsManagement);
              return;
            }

            this.notifications.success(this.translate.instant('TEST_SESSION_WIZARD.REJECT_SUCCESS'));
            void this.router.navigateByUrl(portalRoutes.testSessionsManagement);
          },
          error: () => this.notifications.error(this.translate.instant(
            `TEST_SESSION_WIZARD.${action.toUpperCase()}_FAILED`,
          )),
        });
    });
  }

  private persistSession(sendToApprove: boolean): void {
    const session = this.localSession();
    if (
      !this.selectedExamId ||
      this.saving() ||
      (sendToApprove && (!this.isReviewValid() || !session))
    )
      return;

    this.saving.set(true);
    this.service
      .saveSessionSetup({
        testSessionId: this.testSessionId() ?? undefined,
        examId: this.selectedExamId,
        testSlotId: session?.slot.testSlotId,
        startTime: session?.startTime,
        endTime: session?.endTime,
        genderFilter: this.candidateGenderFilter(),
        nationalityFilter: this.candidateNationalityFilter(),
        invitationIds: this.selectedCandidateIds(),
        sendToApprove,
      })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: result => {
          this.testSessionId.set(result.testSessionId);
          this.sessionNo.set(result.sessionNo);
          this.saving.set(false);
          if (sendToApprove) {
            this.notifications.success(this.translate.instant('TEST_SESSION_WIZARD.SUBMIT_SUCCESS'));
            void this.router.navigateByUrl(portalRoutes.testSessionsManagement);
          } else {
            this.notifications.success(this.translate.instant('TEST_SESSION_WIZARD.DRAFT_SAVED'));
          }
        },
        error: error => {
          this.saving.set(false);
          const capacityError = String(error?.error?.message ?? '').match(
            /^TEST_SESSION_CAPACITY_INSUFFICIENT:(\d+)$/,
          );
          const message = capacityError
            ? this.translate.instant('TEST_SESSION_WIZARD.INSUFFICIENT_CAPACITY', {
                count: Number(capacityError[1]),
              })
            : null;
          this.notifications.error(
            message ?? this.translate.instant('TEST_SESSION_WIZARD.SAVE_FAILED'),
          );
        },
      });
  }

  roomHeadStaff(): TestSlotConfigurationStaffDto | null {
    return (
      this.testSlotStaff().find(staff => staff.roleId === TestSlotStaffRoleIds.hallSupervisor) ?? null
    );
  }

  teamMemberStaff(): TestSlotConfigurationStaffDto[] {
    const roomHeadId = this.roomHeadStaff()?.staffUserId;
    return this.testSlotStaff().filter(staff => staff.staffUserId !== roomHeadId);
  }

  private loadPeriodTeam(): void {
    const session = this.localSession();
    if (!session) return;

    const testSlotId = session.slot.testSlotId;
    if (this.loadedTeamTestSlotId === testSlotId || this.periodTeamLoadingSlotId === testSlotId) return;

    this.testSlotStaff.set([]);
    this.loadedTeamTestSlotId = null;
    this.periodTeamLoadingSlotId = testSlotId;
    this.periodTeamLoading.set(true);
    this.periodTeamLoadFailed.set(false);
    const requestId = ++this.periodTeamLoadRequest;

    this.testSlotsService
      .configuration(testSlotId, this.language.get())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: configuration => {
          if (!this.isCurrentPeriodTeamRequest(requestId, testSlotId)) return;

          this.testSlotStaff.set(configuration.staff);
          this.loadedTeamTestSlotId = testSlotId;
          this.periodTeamLoadingSlotId = null;
          this.periodTeamLoading.set(false);
        },
        error: () => {
          if (!this.isCurrentPeriodTeamRequest(requestId, testSlotId)) return;
          this.periodTeamLoadingSlotId = null;
          this.periodTeamLoading.set(false);
          this.periodTeamLoadFailed.set(true);
        },
      });
  }

  private isCurrentPeriodTeamRequest(requestId: number, testSlotId: string): boolean {
    return (
      requestId === this.periodTeamLoadRequest &&
      this.localSession()?.slot.testSlotId === testSlotId
    );
  }

  private timeValue(time: string): number {
    const [hour = '0', minute = '0'] = time.split(':');
    return Number(hour) * 60 + Number(minute.slice(0, 2));
  }

  private loadLookups(language: string): void {
    this.loading = true;
    this.service
      .lookups(language)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (lookups) => {
          this.lookups.set(lookups);
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
  }
}
