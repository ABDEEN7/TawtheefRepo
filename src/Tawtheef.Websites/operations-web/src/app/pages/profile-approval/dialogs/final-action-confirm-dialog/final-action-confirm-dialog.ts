import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef, DynamicDialogModule } from 'primeng/dynamicdialog';
import { TranslateModule } from '@ngx-translate/core';

export interface FinalConfirmDialogData {
  message: string;
}

@Component({
  selector: 'app-final-action-confirm-dialog',
  standalone: true,
  imports: [CommonModule, TranslateModule, DynamicDialogModule],
  template: `
    <p>{{ data?.message }}</p>

    <div class="dialog-footer">
      <button class="btn ghost" type="button" (click)="onCancel()">
        {{ 'common.cancel' | translate }}
      </button>
      <button class="btn primary" type="button" (click)="onConfirm()">
        {{ 'common.confirm' | translate }}
      </button>
    </div>
  `,
  styles: [`
    .dialog-footer {
      display: flex;
      justify-content: flex-end;
      gap: 0.5rem;
      margin-top: 1rem;
    }
  `],
})
export class FinalActionConfirmDialogComponent {
  data: FinalConfirmDialogData | null;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {
    this.data = this.config.data as FinalConfirmDialogData;
  }

  onCancel(): void {
    this.ref.close(false);
  }

  onConfirm(): void {
    this.ref.close(true);
  }
}
