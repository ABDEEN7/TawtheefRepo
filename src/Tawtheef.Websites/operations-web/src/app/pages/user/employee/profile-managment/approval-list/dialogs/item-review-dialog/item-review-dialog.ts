import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef, DynamicDialogModule } from 'primeng/dynamicdialog';
import { TranslateModule } from '@ngx-translate/core';
import {ProfileApprovalItem} from '../../models/profile-approval.models';

export type ItemDialogAction = 'approve' | 'reject' | 'changes';

export interface ItemDialogData {
  item: ProfileApprovalItem;
  action: ItemDialogAction;
}

export interface ItemDialogResult {
  action: ItemDialogAction;
  note?: string;
}

@Component({
  selector: 'app-item-review-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule, DynamicDialogModule],
  template: `
    <div class="dialog-body">
      <div class="muted small" *ngIf="data?.item">{{ data?.item?.title }}</div>

      <textarea
        *ngIf="data?.action !== 'approve'"
        rows="4"
        class="form-control"
        [placeholder]="'profileApproval.dialog.notePlaceholder' | translate"
        [(ngModel)]="note"
      ></textarea>

      <div class="alert alert-warning" *ngIf="noteRequiredError">
        {{ 'profileApproval.validation.noteRequired' | translate }}
      </div>
    </div>

    <div class="dialog-footer">
      <button class="btn ghost" type="button" (click)="onCancel()">
        {{ 'common.cancel' | translate }}
      </button>
      <button class="btn primary" type="button" (click)="onSave()">
        {{ 'common.save' | translate }}
      </button>
    </div>
  `,
  styles: [`
    .dialog-body { display: flex; flex-direction: column; gap: 0.75rem; }
    .dialog-footer {
      display: flex;
      justify-content: flex-end;
      gap: 0.5rem;
      margin-top: 1rem;
    }
  `],
})
export class ItemReviewDialogComponent {
  data: ItemDialogData | null;
  note = '';
  noteRequiredError = false;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {
    this.data = this.config.data as ItemDialogData;
  }

  onCancel(): void {
    this.ref.close();
  }

  onSave(): void {
    if (!this.data) {
      this.ref.close();
      return;
    }

    // Enforce note for reject / changes
    if (this.data.action !== 'approve' && !this.note.trim()) {
      this.noteRequiredError = true;
      return;
    }

    const result: ItemDialogResult = {
      action: this.data.action,
      note: this.note.trim() || undefined,
    };

    this.ref.close(result);
  }
}
