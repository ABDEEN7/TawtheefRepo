import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { Select } from 'primeng/select';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { NotificationService } from '../../../../../../../core/services/notification.service';
import { SlotPreviewModel } from '../../models/plan-preview.model';

export interface ReassignSlotDialogData {
  candidateInvitationId: string;
  candidateName: string;
  slots: SlotPreviewModel[];
  localized: (ar?: string | null, en?: string | null) => string;
}

interface SlotOption {
  value: string; // startAt
  label: string;
}

@Component({
  selector: 'app-reassign-slot-dialog',
  templateUrl: './reassign-slot-dialog.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, TranslatePipe, Select],
})
export class ReassignSlotDialogComponent implements OnInit {
  private dialogRef = inject(DynamicDialogRef);
  private dialogConfig = inject(DynamicDialogConfig<ReassignSlotDialogData>);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);

  candidateName = '';
  options: SlotOption[] = [];
  selectedSlotStartAt: string | null = null;

  ngOnInit(): void {
    const data = this.dialogConfig.data!;
    this.candidateName = data.candidateName;

    // Only the candidate's own current slot and empty ("Held") slots are offered - slots already taken by
    // someone else are not swappable from here.
    this.options = data.slots
      .filter((s: SlotPreviewModel) => !s.invitationId || s.invitationId === data.candidateInvitationId)
      .map((s: SlotPreviewModel) => ({
        value: s.slot.startAt,
        label: this.slotLabel(s, data.localized),
      }));

    this.selectedSlotStartAt =
      data.slots.find((s: SlotPreviewModel) => s.invitationId === data.candidateInvitationId)?.slot.startAt ?? null;
  }

  private slotLabel(slot: SlotPreviewModel, localized: (ar?: string | null, en?: string | null) => string): string {
    // Same shapes the tables use: dd-MM-yyyy and a 12-hour start - end range.
    const start = new Date(slot.slot.startAt);
    const end = new Date(slot.slot.endAt);
    const time = (d: Date) => d.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit' });
    const day = [
      String(start.getDate()).padStart(2, '0'),
      String(start.getMonth() + 1).padStart(2, '0'),
      start.getFullYear(),
    ].join('-');
    const location = slot.slot.roomId
      ? localized(slot.slot.roomNameAr, slot.slot.roomNameEn)
      : this.translate.instant('INTERVIEW_SCHEDULE.INTERVIEW_TYPE.ONLINE');
    return `${day}  ${time(start)} - ${time(end)}  ${location}`;
  }

  save() {
    if (!this.selectedSlotStartAt) {
      this.notify.error(this.translate.instant('INTERVIEW_SCHEDULE.VALIDATION.SLOT_REQUIRED'));
      return;
    }
    this.dialogRef.close(this.selectedSlotStartAt);
  }

  cancel() {
    this.dialogRef.close();
  }
}
