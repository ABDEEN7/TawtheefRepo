// dialogs/parent-major-info.dialog.ts
import { CommonModule } from '@angular/common';
import {Component, inject} from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import {UpsertSkillDialogData} from './upsert-skill.dialog';

@Component({
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="modal-body">
      <ng-container *ngIf="parentMajor; else noData">
        <div class="fw-semibold mb-2">{{ parentMajor.name }}</div>
        <div class="text-muted small">{{ parentMajor.description || '-' }}</div>
      </ng-container>

      <ng-template #noData>
        <div class="text-muted">-</div>
      </ng-template>

      <div class="d-flex justify-content-end mt-4">
        <button class="btn btn-outline-secondary" (click)="ref.close()">
          <i class="hgi hgi-stroke hgi-cancel-01 text-primary fs-18"></i>
          Close
        </button>
      </div>
    </div>
  `
})
export class ParentMajorInfoDialogComponent {
  public ref = inject(DynamicDialogRef)
  public config = inject(DynamicDialogConfig)
  parentMajor = this.config?.data?.parentMajor ?? null;
}
