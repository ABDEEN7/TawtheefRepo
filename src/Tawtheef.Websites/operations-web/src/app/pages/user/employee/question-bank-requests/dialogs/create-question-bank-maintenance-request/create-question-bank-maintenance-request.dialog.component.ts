import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TextareaModule } from 'primeng/textarea';
import { finalize } from 'rxjs';
import { Lang, LanguageService } from '../../../../../../core/services/language.service';
import { QuestionBankMaintenanceDetails } from '../../models/question-bank-request.models';
import { QuestionBankRequestsService } from '../../services/question-bank-requests.service';

@Component({
  selector: 'app-create-question-bank-maintenance-request-dialog',
  standalone: true,
  templateUrl: './create-question-bank-maintenance-request.dialog.component.html',
  styleUrls: ['./create-question-bank-maintenance-request.dialog.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, TextareaModule],
})
export class CreateQuestionBankMaintenanceRequestDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(QuestionBankRequestsService);
  private readonly ref = inject(DynamicDialogRef);
  private readonly config = inject(DynamicDialogConfig);
  private readonly destroyRef = inject(DestroyRef);
  private readonly language = inject(LanguageService);
  readonly bank = this.config.data?.bank as QuestionBankMaintenanceDetails;
  readonly saving = signal(false);
  readonly currentLang = signal<Lang>(this.language.get());
  readonly form = this.fb.group({ reason: ['', Validators.maxLength(2000)] });

  constructor() {
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((language) => this.currentLang.set(language));
  }

  name(ar?: string | null, en?: string | null): string {
    return (this.currentLang() === 'ar' ? ar : en) || '-';
  }

  submit(): void {
    if (this.form.invalid || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.service
      .createMaintenance(this.bank.questionBankId, this.form.controls.reason.value?.trim() || null)
      .pipe(
        finalize(() => this.saving.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({ next: (id) => this.ref.close(id) });
  }

  cancel(): void {
    if (!this.saving()) this.ref.close();
  }
}
