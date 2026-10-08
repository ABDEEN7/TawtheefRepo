import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TextareaModule } from 'primeng/textarea';
import { I18nNamespaceDirective } from '../../../../../../shared/directives/i18n-namespace.directive';

interface TestSessionDecisionDialogData {
  action?: 'return' | 'reject';
}

@Component({
  selector: 'app-test-session-decision-dialog',
  standalone: true,
  templateUrl: './test-session-decision-dialog.component.html',
  styleUrl: '../../../exams-management/return-exam-dialog/return-exam-dialog.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, TranslatePipe, TextareaModule, I18nNamespaceDirective],
})
export class TestSessionDecisionDialogComponent {
  private readonly dialogRef = inject(DynamicDialogRef);
  private readonly data = inject(DynamicDialogConfig<TestSessionDecisionDialogData>).data;

  protected readonly action = this.data?.action ?? 'return';
  protected readonly note = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.pattern(/\S/)],
  });

  protected confirm(): void {
    this.note.markAsTouched();
    if (this.note.invalid) return;
    this.dialogRef.close(this.note.value.trim());
  }

  protected close(): void {
    this.dialogRef.close(false);
  }
}
