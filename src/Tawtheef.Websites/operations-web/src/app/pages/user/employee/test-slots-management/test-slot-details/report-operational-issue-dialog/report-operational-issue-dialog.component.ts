import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-report-operational-issue-dialog',
  standalone: true,
  imports: [FormsModule, TranslatePipe],
  template: `
    <div class="d-grid gap-3">
      <label for="operational-issue-reason">
        {{ 'TEST_SLOT_DETAILS.OPERATIONAL_ISSUE_REASON' | translate }}
      </label>
      <textarea id="operational-issue-reason" class="form-control" rows="5" maxlength="2000"
        [(ngModel)]="reason"></textarea>
      @if (!reason.trim()) {
        <small class="text-danger" role="alert">
          {{ 'TEST_SLOT_DETAILS.OPERATIONAL_ISSUE_REASON_REQUIRED' | translate }}
        </small>
      }
      <div class="d-flex justify-content-end gap-2">
        <button type="button" class="btn btn-outline-secondary" (click)="ref.close()">
          {{ 'TEST_SLOT_DETAILS.CLOSE' | translate }}
        </button>
        <button type="button" class="btn btn-warning" [disabled]="!reason.trim()" (click)="save()">
          {{ 'TEST_SLOT_DETAILS.REPORT_OPERATIONAL_ISSUE' | translate }}
        </button>
      </div>
    </div>
  `,
})
export class ReportOperationalIssueDialogComponent {
  readonly ref = inject(DynamicDialogRef);
  reason = '';

  save(): void {
    const value = this.reason.trim();
    if (value) this.ref.close(value);
  }
}
