import { ChangeDetectionStrategy, Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';

import { Textarea } from 'primeng/textarea';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { NotificationService } from '../../../../../../../core/services/notification.service';

import { AppointmentModel } from '../../models/appointment.model';
import { RescheduleSlotModel } from '../../models/reschedule-slot.model';
import { RescheduleAppointmentPayload } from '../../services/interview-schedule.service';
import {
  PickerCurrentSlot,
  PickerSlot,
  SlotPickerComponent,
  SlotPickerStatus,
} from '../slot-picker/slot-picker';

export interface RescheduleDialogData {
  scheduleId: string;
  appointment: AppointmentModel;
  candidateName: string;
  isRtl: boolean;
  loadSlots: (scheduleId: string, appointmentId: string) => Observable<RescheduleSlotModel[]>;
}

@Component({
  selector: 'app-reschedule-dialog',
  templateUrl: './reschedule-dialog.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, TranslatePipe, Textarea, SlotPickerComponent],
})
export class RescheduleDialogComponent implements OnInit {
  private dialogRef = inject(DynamicDialogRef);
  private dialogConfig = inject(DynamicDialogConfig<RescheduleDialogData>);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);
  private destroyRef = inject(DestroyRef);

  candidateName = '';
  isRtl = false;
  current!: PickerCurrentSlot;
  reason = '';

  loadState = signal<SlotPickerStatus>('loading');
  private slots = signal<RescheduleSlotModel[]>([]);
  selectedSlotId = signal<string | null>(null);

  pickerSlots = computed<PickerSlot[]>(() =>
    this.slots().map((slot) => ({
      id: slot.slotId,
      startAt: slot.startAt,
      endAt: slot.endAt,
      location: this.location(slot.roomId, slot.roomNameAr, slot.roomNameEn),
    })),
  );

  ngOnInit(): void {
    const data = this.dialogConfig.data!;
    this.candidateName = data.candidateName;
    this.isRtl = data.isRtl;

    const appointment = data.appointment;
    this.current = {
      startAt: appointment.startAt,
      endAt: appointment.endAt,
      location: this.location(appointment.roomId, appointment.roomNameAr, appointment.roomNameEn),
    };

    this.load();
  }

  load() {
    const { scheduleId, appointment, loadSlots } = this.dialogConfig.data!;
    this.loadState.set('loading');

    loadSlots(scheduleId, appointment.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (slots: RescheduleSlotModel[]) => {
          this.slots.set(slots);
          this.selectedSlotId.set(null);
          this.loadState.set('ready');
        },
        error: () => this.loadState.set('error'),
      });
  }

  save() {
    const targetSlotId = this.selectedSlotId();
    if (!targetSlotId) return this.fail('INTERVIEW_SCHEDULE.VALIDATION.SLOT_REQUIRED');
    if (!this.reason.trim())
      return this.fail('INTERVIEW_SCHEDULE.VALIDATION.RESCHEDULE_REASON_REQUIRED');

    const payload: RescheduleAppointmentPayload = {
      appointmentId: this.dialogConfig.data!.appointment.id,
      targetSlotId,
      reason: this.reason.trim(),
    };

    this.dialogRef.close(payload);
  }

  cancel() {
    this.dialogRef.close();
  }

  private location(roomId?: string | null, nameAr?: string | null, nameEn?: string | null): string {
    if (!roomId) return this.translate.instant('INTERVIEW_SCHEDULE.INTERVIEW_TYPE.ONLINE');
    return this.isRtl ? nameAr || nameEn || '' : nameEn || nameAr || '';
  }

  private fail(key: string) {
    this.notify.error(this.translate.instant(key));
  }
}
