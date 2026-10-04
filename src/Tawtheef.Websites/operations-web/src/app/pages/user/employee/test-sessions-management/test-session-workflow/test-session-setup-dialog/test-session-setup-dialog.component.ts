import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { DatePickerModule } from 'primeng/datepicker';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TableModule } from 'primeng/table';
import { TestSessionsService } from '../../services/test-sessions.service';
import { LocalTestSession, ReadyTestSlotListItemDto } from '../../models/ready-test-slot.dto';
import { TestSessionExamDetailsDto } from '../../models/test-session-exam-details.dto';

@Component({
  selector: 'app-test-session-setup-dialog',
  standalone: true,
  templateUrl: './test-session-setup-dialog.component.html',
  styleUrl: './test-session-setup-dialog.component.scss',
  imports: [DatePickerModule, FormsModule, TranslatePipe, TableModule],
})
export class TestSessionSetupDialogComponent implements OnInit {
  private readonly ref = inject(DynamicDialogRef);
  private readonly config = inject(
    DynamicDialogConfig<{
      slot: ReadyTestSlotListItemDto;
      exam: TestSessionExamDetailsDto;
      selectedCandidateCount: number;
      initialSession?: LocalTestSession;
    }>,
  );
  private readonly service = inject(TestSessionsService);
  readonly slot = this.config.data.slot;
  readonly exam: TestSessionExamDetailsDto = this.config.data.exam;
  readonly selectedCandidateCount = this.config.data.selectedCandidateCount;
  readonly initialSession = this.config.data.initialSession;
  from = this.normalizeTime(this.initialSession?.startTime ?? this.slot.startTime);
  to = this.initialSession?.endTime ?? this.addMinutes(this.from, this.exam.durationMinutes);
  fromPickerValue = this.dateFromTime(this.from);
  toPickerValue = this.dateFromTime(this.to);
  added = signal(!!this.initialSession);
  readonly sessions = computed(() => (this.added() ? [true] : []));
  availableSeats = signal(this.initialSession?.availableCapacity ?? 0);
  error = signal<string | null>(null);
  validationPending = signal(false);
  private validationAttempt = 0;

  firstExamDuration(): number | undefined {
    return this.exam.parts.find((part) => part.partNo === 1)?.durationMinutes;
  }

  secondExamDuration(): number | undefined {
    return this.exam.parts.find((part) => part.partNo === 2)?.durationMinutes;
  }

  ngOnInit(): void {
    if (this.initialSession) this.validate();
  }

  addSession(): void {
    this.added.set(true);
    this.validate();
  }
  onFromChanged(): void {
    this.to = this.addMinutes(this.from, this.exam.durationMinutes);
    this.toPickerValue = this.dateFromTime(this.to);
    this.validate();
  }
  onFromPickerChanged(value: Date | null): void {
    if (!value) return;
    this.from = this.timeFromDate(value);
    this.fromPickerValue = value;
    this.onFromChanged();
  }
  onToPickerChanged(value: Date | null): void {
    if (!value) return;
    this.to = this.timeFromDate(value);
    this.toPickerValue = value;
    this.validate();
  }
  validate(): void {
    if (!this.added()) return;
    const attempt = ++this.validationAttempt;
    if (
      this.timeValue(this.from) < this.timeValue(this.slot.startTime) ||
      this.timeValue(this.to) > this.timeValue(this.slot.endTime) ||
      this.timeValue(this.to) <= this.timeValue(this.from) ||
      this.minutes(this.from, this.to) < this.exam.durationMinutes
    ) {
      this.validationPending.set(false);
      this.error.set('TEST_SESSION_WIZARD.SESSION_TIME_INVALID');
      return;
    }
    this.validationPending.set(true);
    this.error.set(null);
    this.service
      .capacity({
        testSlotId: this.slot.testSlotId,
        startTime: this.from,
        endTime: this.to,
        selectedCandidateCount: this.selectedCandidateCount,
      })
      .subscribe({
        next: (result) => {
          if (attempt !== this.validationAttempt) return;
          this.validationPending.set(false);
          this.availableSeats.set(result.availableSeats);
          this.error.set(result.isSufficient ? null : 'TEST_SESSION_WIZARD.INSUFFICIENT_CAPACITY');
        },
        error: () => {
          if (attempt !== this.validationAttempt) return;
          this.validationPending.set(false);
          this.error.set('TEST_SESSION_WIZARD.INSUFFICIENT_CAPACITY');
        },
      });
  }
  apply(): void {
    if (this.added() && !this.validationPending() && !this.error())
      this.ref.close({
        testSlotId: this.slot.testSlotId,
        startTime: this.from,
        endTime: this.to,
        availableSeats: this.availableSeats(),
      });
  }
  cancel(): void {
    this.ref.close();
  }
  delete(): void {
    this.validationAttempt++;
    this.added.set(false);
    this.validationPending.set(false);
    this.error.set(null);
  }
  private addMinutes(time: string, minutes: number): string {
    const total = this.timeValue(time) + minutes;
    return `${String(Math.floor(total / 60)).padStart(2, '0')}:${String(total % 60).padStart(2, '0')}`;
  }
  private minutes(from: string, to: string): number {
    return this.timeValue(to) - this.timeValue(from);
  }
  protected normalizeTime(time: string): string {
    const total = this.timeValue(time);
    return `${String(Math.floor(total / 60)).padStart(2, '0')}:${String(total % 60).padStart(2, '0')}`;
  }
  private timeValue(time: string): number {
    const [hour = '0', minute = '0'] = time.split(':');
    return Number(hour) * 60 + Number(minute.slice(0, 2));
  }
  private dateFromTime(time: string): Date {
    const [hour = '0', minute = '0'] = time.split(':');
    const date = new Date();
    date.setHours(Number(hour), Number(minute.slice(0, 2)), 0, 0);
    return date;
  }
  private timeFromDate(date: Date): string {
    return `${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}`;
  }
}
