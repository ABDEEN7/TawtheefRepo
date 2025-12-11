import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef, DynamicDialogModule } from 'primeng/dynamicdialog';
import { TranslateModule } from '@ngx-translate/core';
import {ProfileApprovalSection} from '../../models/profile-approval.models';

export type SectionDialogAction = 'section-approve' | 'section-changes';

export interface SectionDialogData {
  section: ProfileApprovalSection;
  action: SectionDialogAction;
  sectionLabelKey: string;        // e.g. 'profileApproval.sections.basicInfo'
  initialNote?: string | null;    // existing note
}

export interface SectionDialogResult {
  action: SectionDialogAction;
  note?: string;
}

@Component({
  selector: 'app-section-review-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule, DynamicDialogModule],
  template: `
    <div class="dialog-body">
      <div class="muted small">
        {{ data?.sectionLabelKey | translate }}
      </div>

      <textarea
        rows="5"
        class="form-control"
        [placeholder]="'profileApproval.section.notePlaceholder' | translate"
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
export class SectionReviewDialogComponent {
  data: SectionDialogData | null;
  note = '';
  noteRequiredError = false;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {
    this.data = this.config.data as SectionDialogData;
    this.note = this.data?.initialNote ?? '';
  }

  onCancel(): void {
    this.ref.close();
  }

  onSave(): void {
    if (!this.data) {
      this.ref.close();
      return;
    }

    // For section-changes, note is mandatory
    if (this.data.action === 'section-changes' && !this.note.trim()) {
      this.noteRequiredError = true;
      return;
    }

    const result: SectionDialogResult = {
      action: this.data.action,
      note: this.note.trim() || undefined,
    };

    this.ref.close(result);
  }
}
