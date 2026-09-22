import { Component, inject, OnInit, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ButtonModule } from 'primeng/button';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { LanguageService } from '../../../../../../core/services/language.service';
import { UserDto } from '../../../users-management/models/user.dto';
import {
  TestSlotConfigurationDto,
  TestSlotStaffRoleIds,
  UpdateTestSlotAssignmentsDto,
} from '../../models/create-test-slot.dto';
import { TestSlotsService } from '../../services/test-slots.service';
import {
  TestSlotTeamAssignmentSelection,
  TestSlotTeamAssignmentSelectorComponent,
} from '../../test-slot-create/steps/test-slot-team-assignment-selector/test-slot-team-assignment-selector.component';

interface TestSlotAssignmentDialogData {
  testSlotId: string;
}

@Component({
  selector: 'app-test-slot-assignment-dialog',
  standalone: true,
  templateUrl: './test-slot-assignment-dialog.component.html',
  styleUrl: './test-slot-assignment-dialog.component.scss',
  imports: [ButtonModule, TranslatePipe, TestSlotTeamAssignmentSelectorComponent],
})
export class TestSlotAssignmentDialogComponent implements OnInit {
  private readonly service = inject(TestSlotsService);
  private readonly language = inject(LanguageService);
  private readonly notifications = inject(NotificationService);
  private readonly translate = inject(TranslateService);
  private readonly ref = inject(DynamicDialogRef);
  private readonly data = inject(DynamicDialogConfig<TestSlotAssignmentDialogData>).data!;

  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly loadFailed = signal(false);
  readonly showRoomHeadError = signal(false);
  readonly roomHead = signal<UserDto | null>(null);
  readonly teamMembers = signal<UserDto[]>([]);
  private configuration: TestSlotConfigurationDto | null = null;

  ngOnInit(): void {
    this.service.configuration(this.data.testSlotId, this.language.get()).subscribe({
      next: configuration => {
        this.configuration = configuration;
        const users = configuration.staff.map(staff => ({
          id: staff.staffUserId,
          name: staff.name,
          email: '',
          isBlocked: false,
        }));
        const roomHeadId = configuration.staff.find(
          staff => staff.roleId === TestSlotStaffRoleIds.hallSupervisor,
        )?.staffUserId;
        this.roomHead.set(users.find(user => user.id === roomHeadId) ?? null);
        this.teamMembers.set(users.filter(user => user.id !== roomHeadId));
        this.loading.set(false);
      },
      error: () => {
        this.loadFailed.set(true);
        this.loading.set(false);
      },
    });
  }

  updateAssignment(selection: TestSlotTeamAssignmentSelection): void {
    this.roomHead.set(selection.roomHead);
    this.teamMembers.set(selection.selectedStaff);
    if (selection.roomHead) this.showRoomHeadError.set(false);
  }

  save(): void {
    if (this.saving() || !this.configuration) return;
    if (!this.roomHead()) {
      this.showRoomHeadError.set(true);
      this.notifications.error(this.translate.instant('validation.required_filed'));
      return;
    }

    this.saving.set(true);
    this.service
      .updateAssignments(this.data.testSlotId, this.createRequest())
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.notifications.success(this.translate.instant('TEST_SLOT_DETAILS.ASSIGNMENTS_UPDATED'));
          this.ref.close(true);
        },
        // error: () => this.notifications.error(this.translate.instant('TEST_SLOT_WIZARD.STAFF_CONFLICT')),
      });
  }

  cancel(): void {
    if (!this.saving()) this.ref.close();
  }

  private createRequest(): UpdateTestSlotAssignmentsDto {
    const roomHead = this.roomHead()!;
    const teamMemberIds = new Set(this.teamMembers().map(member => member.id));
    teamMemberIds.delete(roomHead.id);
    return {
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
}
