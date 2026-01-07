import { Component } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-job-apply-confirmation-dialog',
  standalone: true,
  imports: [FormsModule, TranslatePipe],
  templateUrl: './job-apply-confirmation.dialog.component.html',
  styleUrl: './job-apply-confirmation.dialog.component.scss'
})
export class JobApplyConfirmationDialogComponent {
  agreed = false;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {}

  confirm(): void {
    if (!this.agreed) return;
    this.ref.close(true);
  }

  cancel(): void {
    this.ref.close(false);
  }
}
