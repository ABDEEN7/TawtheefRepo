import { DatePipe } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { LanguageService } from '../../../../../../../core/services/language.service';
import { RoomListItemDto } from '../../../../rooms-management/models/room-list-item.dto';
import { testSlotCreateForm } from '../../helper/test-slot-create.form';
import { TeamAssignmentState } from '../../models/team-assignment-state.model';
import {
  TestSlotTeamAssignmentSelectorComponent,
  TestSlotTeamAssignmentSelection,
} from '../test-slot-team-assignment-selector/test-slot-team-assignment-selector.component';

@Component({
  selector: 'app-team-assignment',
  standalone: true,
  templateUrl: './team-assignment.component.html',
  styleUrl: './team-assignment.component.scss',
  imports: [DatePipe, TranslatePipe, TestSlotTeamAssignmentSelectorComponent],
})
export class TeamAssignmentComponent {
  readonly language = inject(LanguageService);

  @Input({ required: true }) form!: ReturnType<typeof testSlotCreateForm>;
  @Input() rooms: RoomListItemDto[] = [];
  @Input({ required: true }) state!: TeamAssignmentState;
  @Input() isViewMode = false;
  @Input() showRoomHeadError = false;
  @Output() stateChange = new EventEmitter<TeamAssignmentState>();

  get room(): RoomListItemDto | undefined { return this.rooms.find(room => room.id === this.form.controls.roomId.value); }
  get roomName(): string { return (this.language.isRtl ? this.room?.nameAr : this.room?.nameEn) ?? '—'; }
  get roomCapacity(): number | string { return this.room?.capacity ?? '—'; }

  updateAssignment(selection: TestSlotTeamAssignmentSelection): void {
    this.stateChange.emit(selection);
  }
}
