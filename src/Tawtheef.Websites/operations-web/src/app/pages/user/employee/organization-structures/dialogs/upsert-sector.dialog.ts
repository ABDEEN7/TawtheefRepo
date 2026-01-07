import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { InputTextModule } from 'primeng/inputtext';
import { Textarea } from 'primeng/textarea';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { ButtonModule } from 'primeng/button';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

type DialogMode = 'create' | 'edit';

export interface UpsertSectorDialogData {
  mode: DialogMode;
  model?: any;
}

@Component({
  selector: 'app-upsert-sector-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe, InputTextModule, Textarea, ToggleSwitchModule, ButtonModule],
  template: `
     <div class="modal-body">
          <form (ngSubmit)="save()" #f="ngForm" class="d-flex flex-column gap-3">
              <div class="d-flex gap-3 flex-wrap">
                <div class="flex-grow-1 min-w-250">
                  <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_NAME_EN' | translate }}</label>
                  <input pInputText class="w-100 form-control"
                        name="nameEn"
                        [(ngModel)]="vm.nameEn"
                        required
                        maxlength="200"
                        [placeholder]="'ORG_STRUCTURES.FIELD_NAME_EN' | translate" />
                  <small class="text-muted" *ngIf="f.submitted && !vm.nameEn">{{ 'ORG_STRUCTURES.VALIDATION_REQUIRED' | translate }}</small>
                </div>
                <div class="flex-grow-1 min-w-250">
                  <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_NAME_AR' | translate }}</label>
                  <input pInputText class="w-100 form-control"
                        name="nameAr"
                        [(ngModel)]="vm.nameAr"
                        required
                        maxlength="200"
                        [placeholder]="'ORG_STRUCTURES.FIELD_NAME_AR' | translate" />
                  <small class="text-muted" *ngIf="f.submitted && !vm.nameAr">{{ 'ORG_STRUCTURES.VALIDATION_REQUIRED' | translate }}</small>
                </div>
              </div>

              <div class="d-flex gap-3 flex-wrap">
                <div class="flex-grow-1 min-w-250">
                  <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_DESCRIPTION_EN' | translate }}</label>
                  <textarea pInputTextarea rows="3" class="w-100 form-control"
                            name="descriptionEn"
                            [(ngModel)]="vm.descriptionEn"
                            maxlength="500"
                            [placeholder]="'ORG_STRUCTURES.FIELD_DESCRIPTION_EN' | translate"></textarea>
                </div>

                <div class="flex-grow-1 min-w-250">
                  <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_DESCRIPTION_AR' | translate }}</label>
                  <textarea pInputTextarea rows="3" class="w-100 form-control"
                            name="descriptionAr"
                            [(ngModel)]="vm.descriptionAr"
                            maxlength="500"
                            [placeholder]="'ORG_STRUCTURES.FIELD_DESCRIPTION_AR' | translate"></textarea>
                </div>
              </div>

              <div class="d-flex gap-3 flex-wrap align-items-center">
                <div class="d-flex align-items-center gap-2">
                  <p-toggle-switch [(ngModel)]="vm.isActive" name="isActive"></p-toggle-switch>
                  <span>{{ 'ORG_STRUCTURES.FIELD_ACTIVE' | translate }}</span>
                </div>
              </div>

              <div class="modal-footer justify-content-end gap-3 border-0">
                    <button pButton type="button" class="p-button-outlined mw-200 text-center d-flex text-primary fs-16 gap-1" (click)="cancel()">
                    <i class="hgi hgi-stroke hgi-cancel-01 text-primary fs-18"></i>
                    {{ 'ORG_STRUCTURES.CANCEL' | translate }}
                  </button>
                  <button pButton type="submit" class="btn btn-primary mw-200 d-flex gap-2 justify-content-center" [disabled]="!isValid()">
                    <i class="hgi hgi-stroke hgi-floppy-disk fs-18"></i>
                    {{ 'ORG_STRUCTURES.SAVE' | translate }}
                  </button>
              </div>
      </form>
     </div>
      
  `
})
export class UpsertSectorDialogComponent {
  ref = inject(DynamicDialogRef);
  config = inject(DynamicDialogConfig<UpsertSectorDialogData>);

  private data = this.config?.data ?? { mode: 'create' as const };

  vm = {
    nameEn: this.data.model?.nameEn ?? this.data.model?.name ?? '',
    nameAr: this.data.model?.nameAr ?? '',
    descriptionEn: this.data.model?.descriptionEn ?? this.data.model?.description ?? '',
    descriptionAr: this.data.model?.descriptionAr ?? '',
    isActive: (this.data.model?.isActive ?? true) !== false
  };

  isValid(): boolean {
    return !!this.vm.nameEn?.trim() && !!this.vm.nameAr?.trim();
  }

  save() {
    if (!this.isValid()) return;
    this.ref.close({
      nameEn: this.vm.nameEn.trim(),
      nameAr: this.vm.nameAr.trim(),
      descriptionEn: this.vm.descriptionEn?.trim() || null,
      descriptionAr: this.vm.descriptionAr?.trim() || null,
      isActive: this.vm.isActive
    });
  }

  cancel() {
    this.ref.close();
  }
}
