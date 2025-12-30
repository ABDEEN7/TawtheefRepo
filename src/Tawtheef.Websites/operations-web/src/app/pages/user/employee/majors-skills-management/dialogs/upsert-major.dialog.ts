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
    <div class="p-2">
      <form (ngSubmit)="save()" #f="ngForm" class="d-flex flex-column gap-3">

        <div class="d-flex gap-3 flex-wrap">
          <div class="flex-grow-1 min-w-250">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_NAME_EN' | translate }}</label>
            <input pInputText class="w-100"
                   name="nameEn"
                   [(ngModel)]="vm.nameEn"
                   required
                   maxlength="100"
                   [placeholder]="'MAJORS_SKILLS.PLACEHOLDER_MAJOR_NAME_EN' | translate" />
            <small class="text-muted" *ngIf="f.submitted && !vm.nameEn">
              {{ 'MAJORS_SKILLS.VALIDATION_REQUIRED' | translate }}
            </small>
          </div>

          <div class="flex-grow-1 min-w-250">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_NAME_AR' | translate }}</label>
            <input pInputText class="w-100"
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

        <div>
          <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_BACKEND_NAME' | translate }}</label>
          <input pInputText class="w-100"
                 name="backendName"
                 [(ngModel)]="vm.backendName"
                 required
                 maxlength="100"
                 [placeholder]="'MAJORS_SKILLS.PLACEHOLDER_BACKEND_NAME' | translate" />
          <small class="text-muted" *ngIf="f.submitted && !vm.backendName">
            {{ 'MAJORS_SKILLS.VALIDATION_REQUIRED' | translate }}
          </small>
        </div>

        <div class="d-flex gap-3 flex-wrap">
          <div class="flex-grow-1 min-w-250">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_DESCRIPTION_EN' | translate }}</label>
            <textarea pInputTextarea rows="3" class="w-100"
                      name="descriptionEn"
                      [(ngModel)]="vm.descriptionEn"
                      maxlength="500"
                      [placeholder]="'MAJORS_SKILLS.PLACEHOLDER_OPTIONAL' | translate"></textarea>
          </div>

          <div class="flex-grow-1 min-w-250">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_DESCRIPTION_AR' | translate }}</label>
            <textarea pInputTextarea rows="3" class="w-100"
                      name="descriptionAr"
                      [(ngModel)]="vm.descriptionAr"
                      maxlength="500"
                      [placeholder]="'MAJORS_SKILLS.PLACEHOLDER_OPTIONAL' | translate"></textarea>
          </div>
        </div>

        <div class="d-flex align-items-center justify-content-between">
          <div class="d-flex align-items-center gap-2">
            <p-toggle-switch [(ngModel)]="vm.isActive" name="isActive"></p-toggle-switch>
            <span>{{ 'MAJORS_SKILLS.FIELD_ACTIVE' | translate }}</span>
          </div>

          <div class="text-muted small" *ngIf="vm.parentId">
            {{ 'MAJORS_SKILLS.SUB_MAJOR_OF_PARENT' | translate }}: {{ vm.parentId }}
          </div>
        </div>

        <div class="d-flex justify-content-end gap-2 mt-2">
          <button pButton type="button" class="p-button-outlined" (click)="cancel()">
            {{ 'MAJORS_SKILLS.CANCEL' | translate }}
          </button>
          <button pButton type="submit" [disabled]="!isValid()" class="p-button">
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
    parentId: this.data.mode === 'create' ? (this.data.parentId ?? null) : (this.data.model?.parentId ?? this.data.model?.parentMajorId ?? null),

    // try common shapes:
    nameEn: this.data.model?.nameEn ?? this.data.model?.name?.en ?? this.data.model?.name ?? '',
    nameAr: this.data.model?.nameAr ?? this.data.model?.name?.ar ?? '',
    backendName: this.data.model?.backendName ?? '',
    descriptionEn: this.data.model?.descriptionEn ?? this.data.model?.description ?? '',
    descriptionAr: this.data.model?.descriptionAr ?? '',
    isActive: (this.data.model?.isActive ?? true) !== false
  };

  isValid(): boolean {
    return !!this.vm.nameEn?.trim() && !!this.vm.nameAr?.trim() && !!this.vm.backendName?.trim();
  }

  save() {
    if (!this.isValid()) return;

    // Payload is intentionally flexible to match your API:
    const payload: any = {
      id: this.vm.id ?? undefined,
      parentId: this.vm.parentId ?? undefined,
      nameEn: this.vm.nameEn.trim(),
      nameAr: this.vm.nameAr.trim(),
      backendName: this.vm.backendName.trim(),
      descriptionEn: this.vm.descriptionEn?.trim() || null,
      descriptionAr: this.vm.descriptionAr?.trim() || null,
      isActive: this.vm.isActive
    };

    this.ref.close(payload);
  }

  cancel() {
    this.ref.close();
  }
}
