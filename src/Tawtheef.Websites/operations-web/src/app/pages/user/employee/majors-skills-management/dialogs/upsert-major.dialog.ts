import { CommonModule } from '@angular/common';
import {Component, inject} from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import {Textarea} from 'primeng/textarea';
import {TranslatePipe} from '@ngx-translate/core';

type DialogMode = 'create' | 'edit';

export interface UpsertMajorDialogData {
  mode: DialogMode;
  parentId?: string | null;        // used for create sub-major
  parent?: string | null;          // parent name
  model?: any;                     // your MajorListItemModel (or details) when edit
}

@Component({
  standalone: true,
  selector: 'app-upsert-major-dialog',
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    ToggleSwitchModule,
    Textarea,
    TranslatePipe
  ],template: `
    <div class="modal-body">
      <form (ngSubmit)="save()" #f="ngForm" class="d-flex flex-column gap-3">

        <div class="row">
          <div class="col-md-6">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_NAME_EN' | translate }} <span class="text-danger">*</span></label>
            <input pInputText class="w-100 form-control"
                   name="nameEn"
                   [(ngModel)]="vm.nameEn"
                   required
                   maxlength="100"
                   [placeholder]="'MAJORS_SKILLS.PLACEHOLDER_MAJOR_NAME_EN' | translate" />
            <small class="text-muted" *ngIf="f.submitted && !vm.nameEn">
              {{ 'MAJORS_SKILLS.VALIDATION_REQUIRED' | translate }}
            </small>
          </div>

          <div class="col-md-6">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_NAME_AR' | translate }} <span class="text-danger">*</span></label>
            <input pInputText class="w-100 form-control"
                   name="nameAr"
                   [(ngModel)]="vm.nameAr"
                   required
                   maxlength="100"
                   [placeholder]="'MAJORS_SKILLS.PLACEHOLDER_MAJOR_NAME_AR' | translate" />
            <small class="text-muted" *ngIf="f.submitted && !vm.nameAr">
              {{ 'MAJORS_SKILLS.VALIDATION_REQUIRED' | translate }}
            </small>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_DESCRIPTION_EN' | translate }}</label>
            <textarea pInputTextarea rows="3" class="w-100 form-control"
                      name="descriptionEn"
                      [(ngModel)]="vm.descriptionEn"
                      maxlength="500"
                      [placeholder]="'MAJORS_SKILLS.PLACEHOLDER_OPTIONAL' | translate"></textarea>
          </div>

          <div class="col-md-6">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_DESCRIPTION_AR' | translate }}</label>
            <textarea pInputTextarea rows="3" class="w-100 form-control"
                      name="descriptionAr"
                      [(ngModel)]="vm.descriptionAr"
                      maxlength="500"
                      [placeholder]="'MAJORS_SKILLS.PLACEHOLDER_OPTIONAL' | translate"></textarea>
          </div>
        </div>

        <div class="col-md-12">
          <div class="d-flex align-items-center gap-2 mb-3">
            <p-toggle-switch [(ngModel)]="vm.isActive" name="isActive"></p-toggle-switch>
            <span>{{ 'MAJORS_SKILLS.FIELD_ACTIVE' | translate }}</span>
          </div>

          <div class="text-muted small" *ngIf="vm.parentId">
            {{ 'MAJORS_SKILLS.SUB_MAJOR_OF_PARENT' | translate }}: {{ vm.parent.name }}
          </div>
        </div>

         <div class="modal-footer justify-content-end gap-3 border-0">
          <button pButton type="button" class="p-button-outlined mw-200 text-center d-flex text-primary fs-16 gap-1" (click)="cancel()">
            <i class="hgi hgi-stroke hgi-cancel-01 text-primary fs-18"></i>
            {{ 'MAJORS_SKILLS.CANCEL' | translate }}
          </button>
          <button pButton type="submit" class="btn btn-primary mw-200 d-flex gap-2 justify-content-center" [disabled]="!isValid()">
            <i class="hgi hgi-stroke hgi-floppy-disk fs-18"></i>
            {{ 'MAJORS_SKILLS.SAVE' | translate }}
          </button>
        </div>

      </form>
    </div>
  `
})
export class UpsertMajorDialogComponent {
  public ref = inject(DynamicDialogRef)
  public config = inject(DynamicDialogConfig<UpsertMajorDialogData>)

  private data = this.config?.data ?? { mode: 'create' as const };

  vm = {
    id: this.data.model?.id ?? null,
    parentId: (this.data.mode === 'create' ? (this.data.parentId ?? null) : (this.data.model?.parentId ?? this.data.model?.parentMajorId ?? null)) || null,
    parent: { name: this.data.parent ?? this.data.model?.parent?.name ?? '' },

    // Handle names/descriptions more robustly (check top-level, additionalData, and localized object)
    nameEn: this.data.model?.nameEn ?? this.data.model?.additionalData?.nameEn ?? (typeof this.data.model?.name === 'object' ? this.data.model?.name?.en : (this.data.model?.name && !this.data.model?.additionalData?.nameAr ? this.data.model?.name : '')),
    nameAr: this.data.model?.nameAr ?? this.data.model?.additionalData?.nameAr ?? (typeof this.data.model?.name === 'object' ? this.data.model?.name?.ar : ''),

    descriptionEn: this.data.model?.descriptionEn ?? this.data.model?.additionalData?.descriptionEn ?? (typeof this.data.model?.description === 'object' ? this.data.model?.description?.en : (this.data.model?.description && !this.data.model?.additionalData?.descriptionAr ? this.data.model?.description : '')),
    descriptionAr: this.data.model?.descriptionAr ?? this.data.model?.additionalData?.descriptionAr ?? (typeof this.data.model?.description === 'object' ? this.data.model?.description?.ar : ''),

    isActive: (this.data.model?.isActive ?? true) !== false
  };

  isValid(): boolean {
    return !!this.vm.nameEn?.trim() && !!this.vm.nameAr?.trim();
  }

  save() {
    if (!this.isValid()) return;

    // Payload is intentionally flexible to match your API:
    const payload: any = {
      id: this.vm.id ?? undefined,
      parentMajorId: this.vm.parentId ?? undefined,
      nameEn: this.vm.nameEn.trim(),
      nameAr: this.vm.nameAr.trim(),
      descriptionEn: this.vm.descriptionEn?.trim() || '',
      descriptionAr: this.vm.descriptionAr?.trim() || '',
      isActive: this.vm.isActive
    };

    this.ref.close(payload);
  }

  cancel() {
    this.ref.close();
  }
}
