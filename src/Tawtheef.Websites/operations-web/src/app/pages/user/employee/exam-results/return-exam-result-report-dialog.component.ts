import { Component, inject } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { TextareaModule } from 'primeng/textarea';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';

@Component({
  selector: 'app-return-exam-result-report-dialog',
  standalone: true,
  imports: [ReactiveFormsModule, TranslatePipe, TextareaModule, I18nNamespaceDirective],
  templateUrl: './return-exam-result-report-dialog.component.html',
})
export class ReturnExamResultReportDialogComponent {
  private readonly ref = inject(DynamicDialogRef);
  protected readonly reason = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.pattern(/\S/), Validators.maxLength(2000)],
  });

  protected confirm(): void {
    this.reason.markAsTouched();
    if (this.reason.invalid) return;
    this.ref.close(this.reason.value.trim());
  }

  protected close(): void {
    this.ref.close(null);
  }
}
