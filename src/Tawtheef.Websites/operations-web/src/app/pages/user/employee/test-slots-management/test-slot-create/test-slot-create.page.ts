import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { DialogService } from 'primeng/dynamicdialog';
import { catchError, finalize, of } from 'rxjs';
import { LanguageService } from '../../../../../core/services/language.service';
import { NotificationService } from '../../../../../core/services/notification.service';
import { portalRoutes } from '../../../../../routes/portal-routes';
import { ConfirmationDialogComponent } from '../../../../../shared/dialogs/confirmation-dialog/confirmation-dialog.component';
import { FaDirArrowDirective } from '../../../../../shared/directives/dir-arrow.directive';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { RoomListItemDto } from '../../rooms-management/models/room-list-item.dto';
import { TestSlotsService } from '../services/test-slots.service';
import { SlotDetailsComponent } from './steps/slot-details/slot-details.component';
import { testSlotCreateForm } from './helper/test-slot-create.form';
import { validateSlotDetails } from './helper/test-slot-create.validation';
import { initialTeamAssignmentState, TeamAssignmentState } from './models/team-assignment-state.model';
import { TeamAssignmentComponent } from './steps/team-assignment/team-assignment.component';
import { ReviewComponent } from './steps/review/review.component';
import { CreateTestSlotDto, TestSlotStaffConflictDto, TestSlotStaffRoleIds } from '../models/create-test-slot.dto';

@Component({
  selector: 'app-test-slot-create-page',
  standalone: true,
  templateUrl: './test-slot-create.page.html',
  styleUrl: './test-slot-create.page.scss',
  imports: [
    ReactiveFormsModule,
    TranslatePipe,
    ButtonModule,
    FaDirArrowDirective,
    I18nNamespaceDirective,
    SlotDetailsComponent,
    TeamAssignmentComponent,
    ReviewComponent,
  ],
  providers: [DialogService],
})
export class TestSlotCreatePage implements OnInit {
  private readonly service = inject(TestSlotsService);
  private readonly router = inject(Router);
  private readonly translate = inject(TranslateService);
  private readonly dialogs = inject(DialogService);
  private readonly notifications = inject(NotificationService);
  private readonly destroyRef = inject(DestroyRef);
  readonly language = inject(LanguageService);
  readonly form = testSlotCreateForm();
  readonly rooms = signal<RoomListItemDto[]>([]);
  readonly teamAssignment = signal<TeamAssignmentState>(initialTeamAssignmentState);
  readonly wizardSteps = [
    { id: 1, label: 'TEST_SLOT_WIZARD.SLOT_DETAILS', icon: 'hgi hgi-stroke hgi-calendar-03' },
    { id: 2, label: 'TEST_SLOT_WIZARD.TEAM_ASSIGNMENT', icon: 'hgi hgi-stroke hgi-user-group' },
    { id: 3, label: 'TEST_SLOT_WIZARD.REVIEW', icon: 'hgi hgi-stroke hgi-clipboard-check-01' },
  ];
  step = 1;
  loading = true;
  loadFailed = false;
  errors: string[] = [];
  saving = false;
  readonly staffConflicts = signal<TestSlotStaffConflictDto[]>([]);

  ngOnInit(): void {
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(language => this.loadRooms(language));
    this.form.controls.slotDate.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.clearStaffConflicts());
    this.form.controls.startTime.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.clearStaffConflicts());
    this.form.controls.endTime.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.clearStaffConflicts());
  }

  goTo(target: number): void {
    if (target < 1 || target > this.wizardSteps.length) return;
    if (target > this.step + 1) return;
    this.errors = [];

    if (target > this.step && this.step === 1) {
      this.form.markAllAsTouched();
      this.errors = validateSlotDetails(this.form.getRawValue());
    }

    if (target > this.step && this.step === 2 && !this.teamAssignment().roomHead) {
      this.errors = ['ROOM_HEAD'];
    }

    if (!this.errors.length) this.step = target;
  }

  updateTeamAssignment(state: TeamAssignmentState): void {
    this.teamAssignment.set(state);
    this.clearStaffConflicts();
  }

  create(): void {
    if (this.saving || this.step !== 3 || !this.teamAssignment().roomHead) return;

    this.saving = true;
    this.service
      .create(this.createRequest())
      .pipe(finalize(() => (this.saving = false)), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.form.markAsPristine();
          this.notifications.success(this.translate.instant('TEST_SLOT_WIZARD.CREATED'));
          void this.router.navigateByUrl(portalRoutes.testSlotsManagement);
        },
        error: error => this.handleCreateError(error),
      });
  }

  cancel(): void {
    if (!this.form.dirty) {
      void this.router.navigateByUrl(portalRoutes.testSlotsManagement);
      return;
    }

    this.dialogs
      .open(ConfirmationDialogComponent, {
        header: this.translate.instant('TEST_SLOT_WIZARD.CANCEL'),
        width: 'min(32rem, 95vw)',
        modal: true,
        data: { type: 'warning', description: 'TEST_SLOT_WIZARD.LEAVE_MESSAGE' },
      })
      ?.onClose.pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(confirmed => {
        if (confirmed === true) void this.router.navigateByUrl(portalRoutes.testSlotsManagement);
      });
  }

  private loadRooms(language: string): void {
    this.loading = true;
    this.loadFailed = false;
    this.service
      .getAvailableRoomsForTestSlotWizard(language)
      .pipe(
        catchError(() => {
          this.loadFailed = true;
          return of([]);
        }),
        finalize(() => (this.loading = false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(rooms => this.rooms.set(rooms));
  }

  private createRequest(): CreateTestSlotDto {
    const details = this.form.getRawValue();
    const roomHead = this.teamAssignment().roomHead!;
    const teamMemberIds = new Set(this.teamAssignment().selectedStaff.map(member => member.id));
    teamMemberIds.delete(roomHead.id);

    return {
      titleAr: details.titleAr.trim(),
      titleEn: details.titleEn.trim(),
      roomId: details.roomId,
      slotDate: this.formatDate(details.slotDate!),
      startTime: this.formatTime(details.startTime!),
      endTime: this.formatTime(details.endTime!),
      staff: [
        { staffUserId: roomHead.id, roleId: TestSlotStaffRoleIds.hallSupervisor, isActive: true },
        ...[...teamMemberIds].map(staffUserId => ({
          staffUserId,
          roleId: TestSlotStaffRoleIds.monitor,
          isActive: true,
        })),
      ],
    };
  }

  private formatDate(value: Date): string {
    return `${value.getFullYear()}-${String(value.getMonth() + 1).padStart(2, '0')}-${String(value.getDate()).padStart(2, '0')}`;
  }

  private formatTime(value: Date): string {
    return `${String(value.getHours()).padStart(2, '0')}:${String(value.getMinutes()).padStart(2, '0')}`;
  }

  private handleCreateError(error: unknown): void {
    const body = error instanceof HttpErrorResponse ? error.error : null;
    const apiErrors = Array.isArray(body?.error) ? body.error : [];
    const conflictDetails = apiErrors.flatMap((apiError: { metadata?: Record<string, unknown> }) => {
      const metadata = apiError.metadata ?? {};
      const details = metadata['conflictDetails'] ?? metadata['ConflictDetails'];
      return Array.isArray(details) ? (details as TestSlotStaffConflictDto[]) : [];
    });

    if (conflictDetails.length) {
      this.staffConflicts.set(conflictDetails);
      this.notifications.error(this.translate.instant('TEST_SLOT_WIZARD.STAFF_CONFLICT'));
      return;
    }

    this.clearStaffConflicts();
    const code = apiErrors[0]?.metadata?.code ?? apiErrors[0]?.metadata?.Code ?? apiErrors[0]?.message;
    this.notifications.error(
      code ? this.translate.instant(`server-error.${code}`) : this.translate.instant('server-error.UN_EXPECTED_ERROR'),
    );
  }

  private clearStaffConflicts(): void {
    this.staffConflicts.set([]);
  }
}
