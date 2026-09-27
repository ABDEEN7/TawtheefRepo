import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TextareaModule } from 'primeng/textarea';
import { finalize } from 'rxjs';
import { LanguageService } from '../../../../../../core/services/language.service';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { I18nNamespaceDirective } from '../../../../../../shared/directives/i18n-namespace.directive';
import { ExamResultRescheduleOption } from '../../models/exam-result-report.models';
import { ExamResultReportsService } from '../../services/exam-result-reports.service';
import { DatePipe } from '@angular/common';

interface RescheduleExamResultCandidateDialogData {
  reportId: string;
  candidateResultId: string;
}

@Component({
  selector: 'app-reschedule-exam-result-candidate-dialog',
  standalone: true,
  imports: [ReactiveFormsModule, TranslatePipe, TextareaModule, I18nNamespaceDirective, DatePipe],
  templateUrl: './reschedule-exam-result-candidate-dialog.component.html',
})
export class RescheduleExamResultCandidateDialogComponent implements OnInit {
  private readonly data = inject(DynamicDialogConfig<RescheduleExamResultCandidateDialogData>).data;
  private readonly ref = inject(DynamicDialogRef);
  private readonly service = inject(ExamResultReportsService);
  private readonly language = inject(LanguageService);
  private readonly notifications = inject(NotificationService);
  private readonly translate = inject(TranslateService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly options = signal<ExamResultRescheduleOption[]>([]);
  protected readonly loading = signal(true);
  protected readonly loadError = signal(false);
  protected readonly submitting = signal(false);
  protected readonly targetSessionId = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required],
  });
  protected readonly reason = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.pattern(/\S/), Validators.maxLength(2000)],
  });

  ngOnInit(): void {
    this.loadOptions();
  }

  protected loadOptions(): void {
    this.loading.set(true);
    this.loadError.set(false);
    this.options.set([]);
    this.service
      .rescheduleOptions(this.data.reportId, this.data.candidateResultId)
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (options) => this.options.set(options),
        error: () => this.loadError.set(true),
      });
  }

  protected submit(): void {
    this.targetSessionId.markAsTouched();
    this.reason.markAsTouched();
    if (
      this.loading() ||
      this.submitting() ||
      this.loadError() ||
      this.targetSessionId.invalid ||
      this.reason.invalid ||
      !this.options().some((option) => option.testSessionId === this.targetSessionId.value)
    )
      return;

    this.submitting.set(true);
    this.service
      .reschedule(this.data.reportId, this.data.candidateResultId, {
        targetTestSessionId: this.targetSessionId.value,
        reason: this.reason.value.trim(),
      })
      .pipe(
        finalize(() => this.submitting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => this.ref.close(true),
        error: () =>
          this.notifications.error(this.translate.instant('EXAM_RESULTS.RESCHEDULE_FAILED')),
      });
  }

  protected close(): void {
    if (!this.submitting()) this.ref.close(false);
  }

  protected roomName(option: ExamResultRescheduleOption): string {
    return (
      (this.language.get() === 'ar' ? option.roomNameAr : option.roomNameEn) || option.roomNameAr
    );
  }
}
