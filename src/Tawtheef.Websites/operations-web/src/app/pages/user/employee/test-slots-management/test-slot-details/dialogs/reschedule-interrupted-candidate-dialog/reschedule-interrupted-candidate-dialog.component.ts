import { DatePipe } from '@angular/common';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TextareaModule } from 'primeng/textarea';
import { finalize } from 'rxjs';
import { LanguageService } from '../../../../../../../core/services/language.service';
import { NotificationService } from '../../../../../../../core/services/notification.service';
import { CandidateExamRescheduleOptionDto } from '../../../models/test-slot-details.dto';
import { TestSlotsService } from '../../../services/test-slots.service';

interface RescheduleInterruptedCandidateDialogData {
  testAttemptId: string;
}

@Component({
  selector: 'app-reschedule-interrupted-candidate-dialog',
  standalone: true,
  imports: [ReactiveFormsModule, TranslatePipe, TextareaModule, DatePipe],
  templateUrl: './reschedule-interrupted-candidate-dialog.component.html',
})
export class RescheduleInterruptedCandidateDialogComponent implements OnInit {
  private readonly data = inject(DynamicDialogConfig<RescheduleInterruptedCandidateDialogData>)
    .data!;

  private readonly ref = inject(DynamicDialogRef);

  private readonly service = inject(TestSlotsService);

  private readonly language = inject(LanguageService);

  private readonly notifications = inject(NotificationService);

  private readonly translate = inject(TranslateService);

  private readonly destroyRef = inject(DestroyRef);

  readonly options = signal<CandidateExamRescheduleOptionDto[]>([]);

  readonly loading = signal(true);
  readonly loadError = signal(false);
  readonly submitting = signal(false);

  readonly targetSessionId = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required],
  });

  readonly reason = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.pattern(/\S/), Validators.maxLength(2000)],
  });

  ngOnInit(): void {
    this.loadOptions();
  }

  loadOptions(): void {
    this.loading.set(true);
    this.loadError.set(false);
    this.options.set([]);

    this.service
      .rescheduleOptions(this.data.testAttemptId)
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (options) => this.options.set(options),

        error: () => this.loadError.set(true),
      });
  }

  submit(): void {
    this.targetSessionId.markAsTouched();
    this.reason.markAsTouched();

    const targetSessionId = this.targetSessionId.value;

    if (
      this.loading() ||
      this.submitting() ||
      this.loadError() ||
      this.targetSessionId.invalid ||
      this.reason.invalid ||
      !this.options().some((option) => option.testSessionId === targetSessionId)
    ) {
      return;
    }

    this.submitting.set(true);

    this.service
      .rescheduleInterruptedAttempt(this.data.testAttemptId, {
        targetTestSessionId: targetSessionId,

        reason: this.reason.value.trim(),
      })
      .pipe(
        finalize(() => this.submitting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => this.ref.close(true),

        error: () =>
          this.notifications.error(this.translate.instant('TEST_SLOT_DETAILS.RESCHEDULE_FAILED')),
      });
  }

  close(): void {
    if (!this.submitting()) {
      this.ref.close(false);
    }
  }

  roomName(option: CandidateExamRescheduleOptionDto): string {
    return (
      (this.language.get() === 'ar' ? option.roomNameAr : option.roomNameEn) || option.roomNameAr
    );
  }

  slotTitle(option: CandidateExamRescheduleOptionDto): string {
    return (
      (this.language.get() === 'ar' ? option.slotTitleAr : option.slotTitleEn) || option.slotTitleAr
    );
  }

  formatTime(value: string): string {
    return value?.slice(0, 5) ?? '';
  }
}
