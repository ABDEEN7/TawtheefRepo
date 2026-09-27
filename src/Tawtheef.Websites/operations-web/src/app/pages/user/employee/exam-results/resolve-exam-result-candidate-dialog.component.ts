import { Component, inject } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { TextareaModule } from 'primeng/textarea';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';

@Component({
  selector: 'app-resolve-exam-result-candidate-dialog',
  standalone: true,
  imports: [ReactiveFormsModule, TranslatePipe, TextareaModule, I18nNamespaceDirective],
  templateUrl: './resolve-exam-result-candidate-dialog.component.html',
})
export class ResolveExamResultCandidateDialogComponent {
  private readonly ref = inject(DynamicDialogRef);
  protected readonly notes = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.pattern(/\S/), Validators.maxLength(2000)],
  });

  protected confirm(): void {
    this.notes.markAsTouched();
    if (this.notes.invalid) return;
    this.ref.close(this.notes.value.trim());
  }

  protected close(): void {
    this.ref.close(null);
  }
}
