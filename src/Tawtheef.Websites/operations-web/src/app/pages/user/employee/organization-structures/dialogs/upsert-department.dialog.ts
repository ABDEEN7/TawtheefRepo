import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { InputTextModule } from 'primeng/inputtext';
import { Textarea } from 'primeng/textarea';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { ButtonModule } from 'primeng/button';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import { OrganizationStructuresService } from '../services/organization-structures.service';
import { Select } from 'primeng/select';
import { RemoteSelectComponent } from '../../../../../shared/components/remote-select/remote-select';
import { EndpointsService } from '../../../../../core/http/endpoints.service';

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
    RemoteSelectComponent,
  ],
  template: `
    <div class="modal-body">
      <form (ngSubmit)="save()" #f="ngForm" class="d-flex flex-column gap-3">
        <div class="row">
          <div class="col-md-6 mb-3">
            <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_SECTOR' | translate }}</label>
            <app-remote-select
              [searchUrl]="endpoints.organizationStructures.lookups.sectors"
              optionLabel="name"
              optionValue="id"
              [(ngModel)]="vm.sectorId"
              name="sectorId"
              [showClear]="true"
              [placeholder]="'ORG_STRUCTURES.FIELD_SECTOR' | translate"
              (valueChange)="onSectorChange($event)"
              [preloadedOptions]="data.sectors">
            </app-remote-select>
            <small class="text-muted" *ngIf="f.submitted && !vm.sectorId">{{ 'ORG_STRUCTURES.VALIDATION_REQUIRED' | translate }}</small>
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_MANAGEMENT' | translate }}</label>
            <app-remote-select
              [searchUrl]="endpoints.organizationStructures.lookups.managements(vm.sectorId)"
              optionLabel="name"
              optionValue="id"
              [(ngModel)]="vm.managementId"
              name="managementId"
              [showClear]="true"
              [requireParent]="true"
              [parentId]="vm.sectorId"
              parentParamName="sectorId"
              [placeholder]="'ORG_STRUCTURES.FIELD_MANAGEMENT' | translate"
              [preloadedOptions]="managementOptions">
            </app-remote-select>
            <small class="text-muted" *ngIf="f.submitted && !vm.managementId">{{ 'ORG_STRUCTURES.VALIDATION_REQUIRED' | translate }}</small>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6 mb-3">
            <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_NAME_EN' | translate }}</label>
            <input pInputText class="w-100  form-control"
                   name="nameEn"
                   [(ngModel)]="vm.nameEn"
                   required
                   maxlength="200"
                   [placeholder]="'ORG_STRUCTURES.FIELD_NAME_EN' | translate" />
            <small class="text-muted" *ngIf="f.submitted && !vm.nameEn">{{ 'ORG_STRUCTURES.VALIDATION_REQUIRED' | translate }}</small>
          </div>
          <div class="col-md-6 mb-3">
            <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_NAME_AR' | translate }}</label>
            <input pInputText class="w-100  form-control"
                   name="nameAr"
                   [(ngModel)]="vm.nameAr"
                   required
                   maxlength="200"
                   [placeholder]="'ORG_STRUCTURES.FIELD_NAME_AR' | translate" />
            <small class="text-muted" *ngIf="f.submitted && !vm.nameAr">{{ 'ORG_STRUCTURES.VALIDATION_REQUIRED' | translate }}</small>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6 mb-3">
            <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_DESCRIPTION_EN' | translate }}</label>
            <textarea pInputTextarea rows="3" class="w-100 form-control"
                      name="descriptionEn"
                      [(ngModel)]="vm.descriptionEn"
                      maxlength="500"
                      [placeholder]="'ORG_STRUCTURES.FIELD_DESCRIPTION_EN' | translate"></textarea>
          </div>

          <div class="col-md-6 mb-3">
            <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_DESCRIPTION_AR' | translate }}</label>
            <textarea pInputTextarea rows="3" class="w-100 form-control"
                      name="descriptionAr"
                      [(ngModel)]="vm.descriptionAr"
                      maxlength="500"
                      [placeholder]="'ORG_STRUCTURES.FIELD_DESCRIPTION_AR' | translate"></textarea>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6 d-flex align-items-center gap-2 mb-3">
            <p-toggle-switch [(ngModel)]="vm.isActive" name="isActive"></p-toggle-switch>
            <span>{{ 'ORG_STRUCTURES.FIELD_ACTIVE' | translate }}</span>
          </div>
        </div>

        <div class="modal-footer justify-content-end gap-3 border-0">
          <button  type="button" class="btn btn-primary-outline d-flex text-primary gap-1 align-items-center" (click)="cancel()">
                  <i class="hgi hgi-stroke hgi-cancel-01 text-primary"></i>
            {{ 'ORG_STRUCTURES.CANCEL' | translate }}
          </button>
          <button  type="submit" class="btn btn-primary d-flex gap-2 justify-content-center align-items-center" [disabled]="!isValid()">
            <i class="hgi hgi-stroke hgi-floppy-disk"></i> {{ 'ORG_STRUCTURES.SAVE' | translate }}
          </button>
        </div>
      </form>
    </div>
  `
})
export class UpsertDepartmentDialogComponent implements OnInit {
  ref = inject(DynamicDialogRef);
  config = inject(DynamicDialogConfig<UpsertDepartmentDialogData>);
  api = inject(OrganizationStructuresService);
  endpoints = inject(EndpointsService);

  data = this.config?.data ?? { mode: 'create', sectors: [], managements: [] };

  vm = {
    sectorId: this.data.model?.sectorId ?? this.data.model?.sector?.id ?? this.data.sectors[0]?.id ?? '',
    managementId: this.data.model?.managementId ?? this.data.model?.management?.id ?? '',
    nameEn: this.data.model?.nameEn ?? this.data.model?.additionalData?.nameEn ?? this.data.model?.name ?? '',
    nameAr: this.data.model?.nameAr ?? this.data.model?.additionalData?.nameAr ?? '',
    descriptionEn: this.data.model?.descriptionEn ?? this.data.model?.additionalData?.descriptionEn ?? this.data.model?.description ?? '',
    descriptionAr: this.data.model?.descriptionAr ?? this.data.model?.additionalData?.descriptionAr ?? '',
    isActive: (this.data.model?.isActive ?? true) !== false
  };

  get filteredManagements(): dropdownOptionsModel[] {
    if (!this.vm.sectorId) return [];
    return this.managementOptions;
  }

  managementOptions: dropdownOptionsModel[] = this.data.managements ?? [];

  ngOnInit() {
    // If we're editing, or have a sector, reload managements to be sure they match the sector
    if (this.vm.sectorId) {
      this.api.getManagementLookups(this.vm.sectorId).subscribe(res => {
        this.managementOptions = res;
        // If managementId was not set (create mode with first sector), or NOT found in current options
        if (!this.vm.managementId || !res.find(m => m.id === this.vm.managementId)) {
          if (this.data.mode === 'create') {
            this.vm.managementId = res[0]?.id ?? '';
          }
        }
      });
    }
  }

  onSectorChange(sectorId: string) {
    if (!sectorId) {
      this.vm.managementId = '';
      this.managementOptions = [];
      return;
    }
    this.api.getManagementLookups(sectorId).subscribe({
      next: res => {
        this.managementOptions = res;
        this.vm.managementId = res[0]?.id ?? '';
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
