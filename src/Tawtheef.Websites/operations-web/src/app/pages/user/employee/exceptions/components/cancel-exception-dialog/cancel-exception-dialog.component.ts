import {
  ChangeDetectionStrategy,
  Component,
  inject
} from '@angular/core';
import {
  FormControl,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import {
  DynamicDialogConfig,
  DynamicDialogRef
} from 'primeng/dynamicdialog';
import { Textarea } from 'primeng/textarea';

import { I18nNamespaceDirective } from '../../../../../../shared/directives/i18n-namespace.directive';

export interface CancelExceptionDialogData {
  candidateName: string;
  jobTitle: string;
  statusLabel: string;
  invitationSent: boolean;
}

@Component({
  selector: 'app-cancel-exception-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    TranslatePipe,
    Textarea,
    I18nNamespaceDirective
  ],
  templateUrl: './cancel-exception-dialog.component.html',
  styleUrl: './cancel-exception-dialog.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CancelExceptionDialogComponent {
  private readonly dialogRef = inject(DynamicDialogRef);

  protected readonly data =
    inject(DynamicDialogConfig<CancelExceptionDialogData>).data!;

  protected readonly reasonMaxLength = 2000;

  protected readonly reason = new FormControl('', {
    nonNullable: true,
    validators: [
      Validators.required,
      Validators.pattern(/\S/),
      Validators.maxLength(this.reasonMaxLength)
    ]
  });

  protected confirm(): void {
    this.reason.markAsTouched();

    if (this.reason.invalid) {
      return;
    }

    const reason = this.reason.value.trim();

    if (!reason) {
      return;
    }

    this.dialogRef.close(reason);
  }

  protected close(): void {
    this.dialogRef.close();
  }
}