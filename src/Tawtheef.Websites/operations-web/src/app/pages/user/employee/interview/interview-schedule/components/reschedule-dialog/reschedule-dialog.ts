import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';

import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { Textarea } from 'primeng/textarea';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { NotificationService } from '../../../../../../../core/services/notification.service';

import { LazySelectComponent } from '../lazy-select/lazy-select';
import { LazySelectOption } from '../../models/lazy-select-option.model';
import { InterviewType } from '../../models/enums';
import { RoomOptionModel } from '../../models/schedule.model';
import { AppointmentModel } from '../../models/appointment.model';
import { RescheduleAppointmentPayload } from '../../services/interview-schedule.service';

export interface RescheduleDialogData {
  appointment: AppointmentModel;
  candidateName: string;
  searchRooms: (term: string) => Observable<LazySelectOption<RoomOptionModel>[]>;
}

@Component({
  selector: 'app-reschedule-dialog',
  templateUrl: './reschedule-dialog.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, TranslatePipe, DatePickerModule, InputTextModule, Textarea, LazySelectComponent],
})
export class RescheduleDialogComponent implements OnInit {
  private dialogRef = inject(DynamicDialogRef);
  private dialogConfig = inject(DynamicDialogConfig<RescheduleDialogData>);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);

  readonly InterviewType = InterviewType;

  // Past dates are not selectable - same [minDate] pattern the job closing-date picker uses (job-basic-modal).
  readonly minDate = this.startOfToday();

  candidateName = '';
  interviewType!: InterviewType;
  searchRooms!: (term: string) => Observable<LazySelectOption<RoomOptionModel>[]>;

  startAt: Date | null = null;
  endAt: Date | null = null;
  room: LazySelectOption<RoomOptionModel> | null = null;
  remoteMeetingUrl = '';
  remoteMeetingInstructions = '';
  reason = '';

  ngOnInit(): void {
    const data = this.dialogConfig.data!;
    this.candidateName = data.candidateName;
    this.interviewType = data.appointment.interviewType;
    this.searchRooms = data.searchRooms;
    this.remoteMeetingUrl = data.appointment.remoteMeetingUrl ?? '';
    this.remoteMeetingInstructions = data.appointment.remoteMeetingInstructions ?? '';
    if (data.appointment.roomId) {
      this.room = {
        value: data.appointment.roomId,
        label: data.appointment.roomNameAr ?? data.appointment.roomNameEn ?? '',
      };
    }
  }

  onRoomSelected(option: LazySelectOption<RoomOptionModel> | null) {
    this.room = option;
  }

  save() {
    if (!this.startAt || !this.endAt) return this.fail('INTERVIEW_SCHEDULE.VALIDATION.PERIOD_DATE_TIME_REQUIRED');
    if (this.startAt < new Date()) return this.fail('INTERVIEW_SCHEDULE.VALIDATION.PERIOD_DATE_PAST');
    if (this.endAt <= this.startAt) return this.fail('INTERVIEW_SCHEDULE.VALIDATION.PERIOD_END_AFTER_START');
    if (this.interviewType === InterviewType.InPerson && !this.room) {
      return this.fail('INTERVIEW_SCHEDULE.VALIDATION.PERIOD_ROOM_REQUIRED');
    }
    if (this.interviewType === InterviewType.Remote && !this.remoteMeetingUrl.trim()) {
      return this.fail('INTERVIEW_SCHEDULE.VALIDATION.PERIOD_URL_REQUIRED');
    }
    if (!this.reason.trim()) return this.fail('INTERVIEW_SCHEDULE.VALIDATION.RESCHEDULE_REASON_REQUIRED');

    const payload: RescheduleAppointmentPayload = {
      appointmentId: this.dialogConfig.data!.appointment.id,
      newStartAt: this.startAt.toISOString(),
      newEndAt: this.endAt.toISOString(),
      roomId: this.interviewType === InterviewType.InPerson ? (this.room?.value ?? null) : null,
      remoteMeetingUrl: this.interviewType === InterviewType.Remote ? this.remoteMeetingUrl.trim() : null,
      remoteMeetingInstructions:
        this.interviewType === InterviewType.Remote ? this.remoteMeetingInstructions.trim() || null : null,
      reason: this.reason.trim(),
    };

    this.dialogRef.close(payload);
  }

  cancel() {
    this.dialogRef.close();
  }

  private fail(key: string) {
    this.notify.error(this.translate.instant(key));
  }

  private startOfToday(): Date {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth(), now.getDate());
  }
}
