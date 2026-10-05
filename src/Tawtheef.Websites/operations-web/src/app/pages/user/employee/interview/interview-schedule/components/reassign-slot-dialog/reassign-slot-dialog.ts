import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { NotificationService } from '../../../../../../../core/services/notification.service';
import { GeneratedSlotModel, SlotPreviewModel } from '../../models/plan-preview.model';
import { PickerCurrentSlot, PickerSlot, SlotPickerComponent } from '../slot-picker/slot-picker';

export interface ReassignSlotDialogData {
  candidateInvitationId: string;
  candidateName: string;
  slots: SlotPreviewModel[];
  isRtl: boolean;
  localized: (ar?: string | null, en?: string | null) => string;
}

@Component({
  selector: 'app-reassign-slot-dialog',
  templateUrl: './reassign-slot-dialog.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [TranslatePipe, SlotPickerComponent],
})
export class ReassignSlotDialogComponent implements OnInit {
  private dialogRef = inject(DynamicDialogRef);
  private dialogConfig = inject(DynamicDialogConfig<ReassignSlotDialogData>);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);

  candidateName = '';
  isRtl = false;
  current!: PickerCurrentSlot;
  // Slots are identified by their start instant here - the draft preview has no persisted ids, and the
  // wizard's reassign pins the candidate by startAt.
  openSlots: PickerSlot[] = [];
  selectedSlotStartAt = signal<string | null>(null);

  ngOnInit(): void {
    const data = this.dialogConfig.data!;
    this.candidateName = data.candidateName;
    this.isRtl = data.isRtl;

    const own = data.slots.find((s: SlotPreviewModel) => s.invitationId === data.candidateInvitationId)!;
    this.current = {
      startAt: own.slot.startAt,
      endAt: own.slot.endAt,
      location: this.location(own.slot, data.localized),
    };

    // Only empty ("Held") slots are offered - the candidate's own slot is the "before" card, and slots taken
    // by someone else are not swappable from here.
    this.openSlots = data.slots
      .filter((s: SlotPreviewModel) => !s.invitationId)
      .map((s: SlotPreviewModel) => ({
        id: s.slot.startAt,
        startAt: s.slot.startAt,
        endAt: s.slot.endAt,
        location: this.location(s.slot, data.localized),
      }));
  }

  save() {
    const startAt = this.selectedSlotStartAt();
    if (!startAt) {
      this.notify.error(this.translate.instant('INTERVIEW_SCHEDULE.VALIDATION.SLOT_REQUIRED'));
      return;
    }
    this.dialogRef.close(startAt);
  }

  cancel() {
    this.dialogRef.close();
  }

  private location(slot: GeneratedSlotModel, localized: ReassignSlotDialogData['localized']): string {
    return slot.roomId
      ? localized(slot.roomNameAr, slot.roomNameEn)
      : this.translate.instant('INTERVIEW_SCHEDULE.INTERVIEW_TYPE.ONLINE');
  }
}
