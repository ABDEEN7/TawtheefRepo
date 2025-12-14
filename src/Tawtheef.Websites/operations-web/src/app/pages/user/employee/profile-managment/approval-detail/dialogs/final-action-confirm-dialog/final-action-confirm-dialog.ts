import { Component, Inject } from '@angular/core';
import {DynamicDialogConfig, DynamicDialogRef} from 'primeng/dynamicdialog';
import {FormsModule} from '@angular/forms';

export interface FinalActionConfirmDialogData {
  action: 'approve' | 'reject';
  profileName: string;
}

@Component({
  selector: 'app-final-action-confirm-dialog',
  templateUrl: './final-action-confirm-dialog.html',
  styleUrls: ['./final-action-confirm-dialog.scss'],
  imports: [
    FormsModule
  ]
})
export class FinalActionConfirmDialogComponent {
  model: any = {};
  constructor(
    public dialogRef: DynamicDialogRef<FinalActionConfirmDialogComponent>,
    @Inject(DynamicDialogConfig) public data: FinalActionConfirmDialogData
  ) {}

  onConfirm(): void {
    this.dialogRef.close(true);
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  onFileSelected(event: any): void {
    const file: File = event.target.files[0];
    if (file) {
      this.model.attachment = file;
    }
  }
}
