import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslatePipe } from '@ngx-translate/core';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { TagModule } from 'primeng/tag';
import { LanguageService } from '../../../../core/services/language.service';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { ExamResultCandidateAttemptDetails, ExamResultLookup } from './models/exam-result-report.models';
import { ExamResultReportsService } from './exam-result-reports.service';

interface ExamResultCandidateAttemptDialogData {
  reportId: string;
  candidateResultId: string;
}

@Component({
  selector: 'app-exam-result-candidate-attempt-dialog',
  standalone: true,
  imports: [TranslatePipe, TagModule, I18nNamespaceDirective],
  templateUrl: './exam-result-candidate-attempt-dialog.component.html',
})
export class ExamResultCandidateAttemptDialogComponent implements OnInit {
  private readonly data = inject(DynamicDialogConfig<ExamResultCandidateAttemptDialogData>).data;
  private readonly service = inject(ExamResultReportsService);
  private readonly language = inject(LanguageService);
  private readonly destroyRef = inject(DestroyRef);

  readonly attempt = signal<ExamResultCandidateAttemptDetails | null>(null);
  readonly loading = signal(true);
  readonly loadError = signal(false);

  ngOnInit(): void {
    this.service.candidateAttempt(this.data.reportId, this.data.candidateResultId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: attempt => {
          this.attempt.set(attempt);
          this.loading.set(false);
        },
        error: () => {
          this.loadError.set(true);
          this.loading.set(false);
        },
      });
  }

  localized(ar: string | null, en: string | null): string {
    return (this.language.get() === 'ar' ? ar : en) || ar || '—';
  }

  lookupName(lookup: ExamResultLookup | null): string {
    return lookup ? this.localized(lookup.nameAr, lookup.nameEn) : '—';
  }

  formatDate(value: string | null): string {
    return value
      ? new Intl.DateTimeFormat(this.language.get() === 'ar' ? 'ar-SA' : 'en', {
          dateStyle: 'medium', timeStyle: 'short',
        }).format(new Date(value))
      : '—';
  }

  formatNumber(value: number | null): string {
    return value === null ? '—' : new Intl.NumberFormat(
      this.language.get() === 'ar' ? 'ar-SA' : 'en',
    ).format(value);
  }
}
