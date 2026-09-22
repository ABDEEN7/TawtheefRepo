import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';

import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { NotificationService } from '../../../../../../../core/services/notification.service';

import { LazySelectComponent } from '../lazy-select/lazy-select';
import { LazySelectOption } from '../../models/lazy-select-option.model';
import { InterviewType } from '../../models/enums';
import { RoomOptionModel } from '../../models/schedule.model';
import { PeriodDraft } from '../../models/wizard-draft.model';

export interface PeriodDialogData {
  period: PeriodDraft | null;
  interviewType: InterviewType;
  searchRooms: (term: string) => Observable<LazySelectOption<RoomOptionModel>[]>;
  localized: (ar?: string | null, en?: string | null) => string;
}

// A new period starts at the beginning of the working day.
const DEFAULT_START_HOUR = 8;

const minutesOfDay = (d: Date) => d.getHours() * 60 + d.getMinutes();

@Component({
  selector: 'app-period-dialog',
  templateUrl: './period-dialog.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, TranslatePipe, DatePickerModule, InputTextModule, LazySelectComponent],
})
export class PeriodDialogComponent implements OnInit {
  private dialogRef = inject(DynamicDialogRef);
  private dialogConfig = inject(DynamicDialogConfig<PeriodDialogData>);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);

  readonly InterviewType = InterviewType;

  // Past days are not selectable - same [minDate] pattern the job closing-date picker uses (job-basic-modal).
  readonly minDate = this.startOfToday();

  interviewType!: InterviewType;
  searchRooms!: (term: string) => Observable<LazySelectOption<RoomOptionModel>[]>;
  isEdit = false;

  date: Date | null = null;
  startTime: Date | null = this.timeToday(DEFAULT_START_HOUR, 0);
  endTime: Date | null = null;
  room: LazySelectOption<RoomOptionModel> | null = null;
  remoteMeetingUrl = '';
  remoteMeetingInstructions = '';

  // Shown inline (and blocks Save) the moment the chosen end is not after the start.
  get endBeforeStart(): boolean {
    return !!this.startTime && !!this.endTime && minutesOfDay(this.endTime) <= minutesOfDay(this.startTime);
  }

  ngOnInit(): void {
    const data = this.dialogConfig.data!;
    this.interviewType = data.interviewType;
    this.searchRooms = data.searchRooms;

    const period = data.period;
    if (!period) return;

    this.isEdit = true;
    this.date = this.parseDate(period.date);
    this.startTime = this.parseTime(period.startTime);
    this.endTime = this.parseTime(period.endTime);
    this.remoteMeetingUrl = period.remoteMeetingUrl ?? '';
    this.remoteMeetingInstructions = period.remoteMeetingInstructions ?? '';
    if (period.room) {
      this.room = {
        value: period.room.id,
        label: data.localized(period.room.nameAr, period.room.nameEn),
        data: period.room,
      };
    }
  }

  onRoomSelected(option: LazySelectOption<RoomOptionModel> | null) {
    this.room = option;
  }

  save() {
    if (!this.date || !this.startTime || !this.endTime) {
      return this.fail('INTERVIEW_SCHEDULE.VALIDATION.PERIOD_DATE_TIME_REQUIRED');
    }
    if (this.date < this.minDate) {
      return this.fail('INTERVIEW_SCHEDULE.VALIDATION.PERIOD_DATE_PAST');
    }
    if (this.endBeforeStart) {
      return this.fail('INTERVIEW_SCHEDULE.VALIDATION.PERIOD_END_AFTER_START');
    }
    if (this.interviewType === InterviewType.InPerson && !this.room) {
      return this.fail('INTERVIEW_SCHEDULE.VALIDATION.PERIOD_ROOM_REQUIRED');
    }
    if (this.interviewType === InterviewType.Remote && !this.remoteMeetingUrl.trim()) {
      return this.fail('INTERVIEW_SCHEDULE.VALIDATION.PERIOD_URL_REQUIRED');
    }

    const draft: PeriodDraft = {
      localId: this.dialogConfig.data!.period?.localId ?? crypto.randomUUID(),
      date: this.formatDate(this.date),
      startTime: this.formatTime(this.startTime),
      endTime: this.formatTime(this.endTime),
      room: this.interviewType === InterviewType.InPerson ? (this.room?.data ?? null) : null,
      remoteMeetingUrl: this.interviewType === InterviewType.Remote ? this.remoteMeetingUrl.trim() : null,
      remoteMeetingInstructions: this.interviewType === InterviewType.Remote
        ? this.remoteMeetingInstructions.trim() || null
        : null,
    };

    this.dialogRef.close(draft);
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

  private timeToday(hours: number, minutes: number): Date {
    const d = new Date();
    d.setHours(hours, minutes, 0, 0);
    return d;
  }

  private formatDate(d: Date): string {
    const y = d.getFullYear();
    const m = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${y}-${m}-${day}`;
  }

  private formatTime(d: Date): string {
    return `${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`;
  }

  private parseDate(value: string): Date | null {
    if (!value) return null;
    const [y, m, d] = value.split('-').map(Number);
    return new Date(y, m - 1, d);
  }

  private parseTime(value: string): Date | null {
    if (!value) return null;
    const [h, m] = value.split(':').map(Number);
    return this.timeToday(h, m);
  }
}
