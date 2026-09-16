import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TextareaModule } from 'primeng/textarea';
import { finalize } from 'rxjs';
import { NotificationService } from '../../../../../core/services/notification.service';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { ExamsService } from '../services/exams.service';

interface ReturnExamDialogData {
  examId: string;
  action?: 'return' | 'reject';
}

@Component({
  selector: 'app-return-exam-dialog',
  standalone: true,
  templateUrl: './return-exam-dialog.component.html',
  styleUrl: './return-exam-dialog.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, TranslatePipe, TextareaModule, I18nNamespaceDirective],
})
export class ReturnExamDialogComponent {
  private readonly dialogRef = inject(DynamicDialogRef);
  private readonly data = inject(DynamicDialogConfig<ReturnExamDialogData>).data;
  private readonly exams = inject(ExamsService);
  private readonly notifications = inject(NotificationService);
  private readonly translate = inject(TranslateService);

  protected readonly submitting = signal(false);
  protected readonly action = this.data?.action ?? 'return';
  protected readonly note = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.pattern(/\S/)],
  });

  protected confirm(): void {
    this.note.markAsTouched();
    if (this.note.invalid || this.submitting()) return;

    const examId = this.data?.examId;
    if (!examId) return;

    this.submitting.set(true);
    const request = this.action === 'reject'
      ? this.exams.reject(examId, this.note.value.trim())
      : this.exams.returnForEdit(examId, this.note.value.trim());
    request
      .pipe(finalize(() => this.submitting.set(false)))
      .subscribe({
        next: () => this.dialogRef.close(true),
        error: () => this.notifications.error(this.translate.instant(
          this.action === 'reject' ? 'EXAMS.REJECT_FAILED' : 'EXAMS.RETURN_FAILED',
        )),
      });
  }

  protected close(): void {
    if (!this.submitting()) this.dialogRef.close(false);
  }
}
