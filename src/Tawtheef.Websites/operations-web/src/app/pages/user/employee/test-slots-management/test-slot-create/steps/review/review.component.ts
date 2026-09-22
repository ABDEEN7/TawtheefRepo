import { DatePipe } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { LanguageService } from '../../../../../../../core/services/language.service';
import { RoomListItemDto } from '../../../../rooms-management/models/room-list-item.dto';
import { testSlotCreateForm } from '../../helper/test-slot-create.form';
import { TeamAssignmentState } from '../../models/team-assignment-state.model';
import { TestSlotStaffConflictDto } from '../../../models/create-test-slot.dto';

@Component({
  selector: 'app-test-slot-review',
  standalone: true,
  templateUrl: './review.component.html',
  styleUrl: './review.component.scss',
  imports: [DatePipe, TranslatePipe, ButtonModule, TableModule],
})
export class ReviewComponent {
  private readonly language = inject(LanguageService);
  @Input({ required: true }) form!: ReturnType<typeof testSlotCreateForm>;
  @Input() rooms: RoomListItemDto[] = [];
  @Input({ required: true }) team!: TeamAssignmentState;
  @Input() staffConflicts: TestSlotStaffConflictDto[] = [];
  @Input() isViewMode = false;
  @Output() editTeamRequested = new EventEmitter<void>();

  get roomName(): string {
    const room = this.rooms.find((item) => item.id === this.form.controls.roomId.value);
    return (this.language.isRtl ? room?.nameAr : room?.nameEn) ?? '—';
  }

  get roomCapacity(): string | number {
    return this.rooms.find((room) => room.id === this.form.controls.roomId.value)?.capacity ?? '—';
  }

  get staff(): { role: string; employee: string; room: string }[] {
    const roomHeadId = this.team.roomHead?.id;
    const staff = this.team.selectedStaff
      .filter((member) => member.id !== roomHeadId)
      .map((member) => ({ role: 'TEST_SLOT_WIZARD.TEAM_MEMBER', employee: member.name, room: this.roomName }));

    return this.team.roomHead
      ? [{ role: 'TEST_SLOT_WIZARD.ROOM_HEAD', employee: this.team.roomHead.name, room: this.roomName }, ...staff]
      : staff;
  }

  formatConflictTime(value: string): string {
    const [hours = '0', minutes = '0'] = value.split(':');
    const hour = Number(hours);
    const period = hour >= 12 ? 'PM' : 'AM';
    return `${String(hour % 12 || 12).padStart(2, '0')}:${minutes.padStart(2, '0')} ${period}`;
  }
}
