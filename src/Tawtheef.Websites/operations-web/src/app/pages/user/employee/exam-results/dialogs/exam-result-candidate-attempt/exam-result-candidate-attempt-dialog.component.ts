import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslatePipe } from '@ngx-translate/core';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { TagModule } from 'primeng/tag';
import { LanguageService } from '../../../../../../core/services/language.service';
import { I18nNamespaceDirective } from '../../../../../../shared/directives/i18n-namespace.directive';
import {
  ExamResultCandidateAttemptDetails,
  ExamResultLookup,
} from '../../models/exam-result-report.models';
import { ExamResultReportsService } from '../../services/exam-result-reports.service';
import { DatePipe } from '@angular/common';
import { TabsModule } from 'primeng/tabs';
import { ExamResultCandidateAttemptDialogData } from './models/exam-result-candidate-attempt-dialog.model';
import { StatusPillComponent } from '../../../../../../shared/components/status-pill/status-pill.component';
import { attemptPartStatusTone, attemptStatusTone } from '../../exam-result-status-presentation';

@Component({
  selector: 'app-exam-result-candidate-attempt-dialog',
  standalone: true,
  imports: [TranslatePipe, TagModule, StatusPillComponent, I18nNamespaceDirective, DatePipe, TabsModule],
  templateUrl: './exam-result-candidate-attempt-dialog.component.html',
  styleUrl:'./exam-result-candidate-attempt-dialog.component.scss',
})
export class ExamResultCandidateAttemptDialogComponent implements OnInit {
  readonly attemptStatusTone = attemptStatusTone;
  readonly attemptPartStatusTone = attemptPartStatusTone;
  private readonly data = inject(DynamicDialogConfig<ExamResultCandidateAttemptDialogData>).data;
  private readonly service = inject(ExamResultReportsService);
  private readonly language = inject(LanguageService);
  private readonly destroyRef = inject(DestroyRef);

  readonly attempt = signal<ExamResultCandidateAttemptDetails | null>(null);
  readonly loading = signal(true);
  readonly loadError = signal(false);

  activePartId: string | number = '';

  ngOnInit(): void {
    this.service
      .candidateAttempt(this.data.reportId, this.data.candidateResultId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (attempt) => {
          this.attempt.set(attempt);

          this.activePartId = attempt.parts[0]?.testAttemptPartId ?? '';

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

  optionStateClass(isSelected: boolean, isCorrect: boolean | null): string {
    if (!isSelected) return '';

    if (isCorrect === true) return 'attempt-option--correct';

    if (isCorrect === false) return 'attempt-option--incorrect';

    return 'attempt-option--selected';
  }
}
