import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { SelectModule } from 'primeng/select';
import { VerifyTestSlotCandidateIdentityRequest } from '../../models/test-slot-details.dto';

@Component({
  selector: 'app-verify-identity-dialog',
  standalone: true,
  imports: [FormsModule, TranslatePipe, SelectModule],
  template: `
    <div class="d-grid gap-3">
      <label for="identity-match">{{ 'TEST_SLOT_DETAILS.VERIFY_IDENTITY' | translate }}</label>
      <p-select inputId="identity-match" [options]="choices" optionLabel="label" optionValue="value"
        [(ngModel)]="matched" [placeholder]="'TEST_SLOT_DETAILS.SELECT_IDENTITY_RESULT' | translate" appendTo="body" />
      <label for="identity-notes">{{ 'TEST_SLOT_DETAILS.IDENTITY_NOTES' | translate }}
        @if (matched === false) { <span class="text-danger">*</span> }
      </label>
      <textarea id="identity-notes" class="form-control" rows="3" maxlength="1000" [(ngModel)]="notes"></textarea>
      @if (matched === false && !notes.trim()) {
        <small class="text-danger" role="alert">{{ 'validation.required_filed' | translate }}</small>
      }
      <div class="d-flex justify-content-end gap-2">
        <button type="button" class="btn btn-outline-secondary" (click)="ref.close()">{{ 'TEST_SLOT_DETAILS.CLOSE' | translate }}</button>
        <button type="button" class="btn btn-primary" [disabled]="matched === null || (matched === false && !notes.trim())" (click)="save()">{{ 'TEST_SLOT_DETAILS.VERIFY_IDENTITY' | translate }}</button>
      </div>
    </div>
  `,
})
export class VerifyIdentityDialogComponent {
  readonly ref = inject(DynamicDialogRef);
  private readonly translate = inject(TranslateService);
  readonly choices = [
    { label: this.translate.instant('TEST_SLOT_DETAILS.IDENTITY_MATCHED'), value: true },
    { label: this.translate.instant('TEST_SLOT_DETAILS.IDENTITY_NOT_MATCHED'), value: false },
  ];
  matched: boolean | null = null;
  notes = '';

  save(): void {
    if (this.matched === null || (this.matched === false && !this.notes.trim())) return;
    const request: VerifyTestSlotCandidateIdentityRequest = {
      matched: this.matched,
      notes: this.notes.trim() || null,
    };
    this.ref.close(request);
  }
}
