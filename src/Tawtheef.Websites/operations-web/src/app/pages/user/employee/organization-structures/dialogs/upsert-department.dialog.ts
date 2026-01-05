import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { InputTextModule } from 'primeng/inputtext';
import { Textarea } from 'primeng/textarea';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { ButtonModule } from 'primeng/button';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import { OrganizationStructuresService } from '../services/organization-structures.service';
import {Select} from 'primeng/select';

type DialogMode = 'create' | 'edit';

export interface UpsertDepartmentDialogData {
  mode: DialogMode;
  model?: any;
  sectors: dropdownOptionsModel[];
  managements: dropdownOptionsModel[];
}

@Component({
  selector: 'app-upsert-department-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    InputTextModule,
    Textarea,
    ToggleSwitchModule,
    ButtonModule,
    Select,
  ],
  template: `
    <div class="p-2">
      <form (ngSubmit)="save()" #f="ngForm" class="d-flex flex-column gap-3">
        <div class="d-flex gap-3 flex-wrap">
          <div class="flex-grow-1 min-w-250">
            <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_SECTOR' | translate }}</label>
            <p-select
              class="w-100"
              [options]="data.sectors"
              optionLabel="name"
              optionValue="id"
              [(ngModel)]="vm.sectorId"
              name="sectorId"
              [filter]="true"
              [showClear]="true"
              [placeholder]="'ORG_STRUCTURES.FIELD_SECTOR' | translate"
              (onChange)="onSectorChange($event.value)">
            </p-select>
            <small class="text-muted" *ngIf="f.submitted && !vm.sectorId">{{ 'ORG_STRUCTURES.VALIDATION_REQUIRED' | translate }}</small>
          </div>
          <div class="flex-grow-1 min-w-250">
            <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_MANAGEMENT' | translate }}</label>
            <p-select
              class="w-100"
              [options]="filteredManagements"
              optionLabel="name"
              optionValue="id"
              [(ngModel)]="vm.managementId"
              name="managementId"
              [filter]="true"
              [showClear]="true"
              [placeholder]="'ORG_STRUCTURES.FIELD_MANAGEMENT' | translate">
            </p-select>
            <small class="text-muted" *ngIf="f.submitted && !vm.managementId">{{ 'ORG_STRUCTURES.VALIDATION_REQUIRED' | translate }}</small>
          </div>
        </div>

        <div class="d-flex gap-3 flex-wrap">
          <div class="flex-grow-1 min-w-250">
            <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_NAME_EN' | translate }}</label>
            <input pInputText class="w-100"
                   name="nameEn"
                   [(ngModel)]="vm.nameEn"
                   required
                   maxlength="200"
                   [placeholder]="'ORG_STRUCTURES.FIELD_NAME_EN' | translate" />
            <small class="text-muted" *ngIf="f.submitted && !vm.nameEn">{{ 'ORG_STRUCTURES.VALIDATION_REQUIRED' | translate }}</small>
          </div>
          <div class="flex-grow-1 min-w-250">
            <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_NAME_AR' | translate }}</label>
            <input pInputText class="w-100"
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
            <textarea pInputTextarea rows="3" class="w-100"
                      name="descriptionEn"
                      [(ngModel)]="vm.descriptionEn"
                      maxlength="500"
                      [placeholder]="'ORG_STRUCTURES.FIELD_DESCRIPTION_EN' | translate"></textarea>
          </div>

          <div class="flex-grow-1 min-w-250">
            <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_DESCRIPTION_AR' | translate }}</label>
            <textarea pInputTextarea rows="3" class="w-100"
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

        <div class="d-flex justify-content-end gap-2 mt-2">
          <button pButton type="button" class="p-button-outlined" (click)="cancel()">
            {{ 'ORG_STRUCTURES.CANCEL' | translate }}
          </button>
          <button pButton type="submit" [disabled]="!isValid()">
            {{ 'ORG_STRUCTURES.SAVE' | translate }}
          </button>
        </div>
      </form>
    </div>
  `
})
export class UpsertDepartmentDialogComponent {
  ref = inject(DynamicDialogRef);
  config = inject(DynamicDialogConfig<UpsertDepartmentDialogData>);
  api = inject(OrganizationStructuresService);

  data = this.config?.data ?? { mode: 'create', sectors: [], managements: [] };

  vm = {
    sectorId: this.data.model?.sector?.id ?? this.data.sectors[0]?.id ?? '',
    managementId: this.data.model?.managementId ?? this.data.model?.management?.id ?? this.data.managements[0]?.id ?? '',
    nameEn: this.data.model?.nameEn ?? this.data.model?.name ?? '',
    nameAr: this.data.model?.nameAr ?? '',
    descriptionEn: this.data.model?.descriptionEn ?? this.data.model?.description ?? '',
    descriptionAr: this.data.model?.descriptionAr ?? '',
    isActive: (this.data.model?.isActive ?? true) !== false
  };

  get filteredManagements(): dropdownOptionsModel[] {
    if (!this.vm.sectorId) return [];
    return this.managementOptions;
  }

  managementOptions: dropdownOptionsModel[] = this.data.managements ?? [];

  onSectorChange(sectorId: string) {
    if (!sectorId) {
      this.vm.managementId = '';
      return;
    }
    this.api.getManagementLookups(sectorId).subscribe({
      next: res => {
        this.managementOptions = res;
        const first = this.managementOptions[0]?.id;
        this.vm.managementId = first ?? '';
      },
      error: () => {
        this.managementOptions = [];
        this.vm.managementId = '';
      }
    });
  }

  isValid(): boolean {
    return !!this.vm.nameEn?.trim() && !!this.vm.nameAr?.trim() && !!this.vm.managementId && !!this.vm.sectorId;
  }

  save() {
    if (!this.isValid()) return;
    this.ref.close({
      managementId: this.vm.managementId,
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
