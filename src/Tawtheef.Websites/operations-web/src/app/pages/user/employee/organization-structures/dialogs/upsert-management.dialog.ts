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
import {Select} from 'primeng/select';

type DialogMode = 'create' | 'edit';

export interface UpsertManagementDialogData {
  mode: DialogMode;
  model?: any;
  sectors: dropdownOptionsModel[];
}

@Component({
  selector: 'app-upsert-management-dialog',
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
  <div class="modal-body">
       <form (ngSubmit)="save()" #f="ngForm" >
             <div class="row">
                  <div class="col-md-12 mb-3">
                    <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_SECTOR' | translate }}</label>
                    <p-select
                      class="w-100"
                      [options]="data.sectors"
                      optionLabel="name"
                      optionValue="id"
                      [(ngModel)]="vm.sectorId"
                      name="sectorId"
                      [showClear]="true"
                      [filter]="true"
                      filterBy="additionalData.nameAr,additionalData.nameEn,name"
                      [placeholder]="'ORG_STRUCTURES.FIELD_SECTOR' | translate">
                    </p-select>
                    <small class="text-muted" *ngIf="f.submitted && !vm.sectorId">{{ 'ORG_STRUCTURES.VALIDATION_REQUIRED' | translate }}</small>
                  </div>
             </div>
        

        <div class="row">
          <div class="col-md-6 mb-3">
              <label class="form-label">{{ 'ORG_STRUCTURES.FIELD_NAME_EN' | translate }}</label>
              <input pInputText class="w-100 form-control"
                    name="nameEn"
                    [(ngModel)]="vm.nameEn"
                    required
                    maxlength="200"
                    [placeholder]="'ORG_STRUCTURES.FIELD_NAME_EN' | translate" />
              <small class="text-muted" *ngIf="f.submitted && !vm.nameEn">{{ 'ORG_STRUCTURES.VALIDATION_REQUIRED' | translate }}</small>
          </div>
          <div class="col-md-6 mb-3">
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

        <div class="row mb-3">
          <div class="col-md-12">
               <div class="d-flex gap-2">
                     <p-toggle-switch [(ngModel)]="vm.isActive" name="isActive"></p-toggle-switch>
                    <span>{{ 'ORG_STRUCTURES.FIELD_ACTIVE' | translate }}</span>
               </div>
           
          </div>
        </div>

        
        <div class="modal-footer justify-content-end gap-3 border-0">
              <button  type="button" class="btn btn-primary-outline d-flex text-primary gap-1 align-items-center" (click)="cancel()">
              <i class="hgi hgi-stroke hgi-cancel-01 text-primary"></i>
              {{ 'ORG_STRUCTURES.CANCEL' | translate }}
            </button>
            <button  type="submit" class="btn btn-primary d-flex gap-2 justify-content-center align-items-center" [disabled]="!isValid()">
              <i class="hgi hgi-stroke hgi-floppy-disk"></i>
              {{ 'ORG_STRUCTURES.SAVE' | translate }}
            </button>
        </div>

      </form>
  </div>
     
  `
})
export class UpsertManagementDialogComponent {
  ref = inject(DynamicDialogRef);
  config = inject(DynamicDialogConfig<UpsertManagementDialogData>);

  data = this.config?.data ?? { mode: 'create', sectors: [] };

  vm = {
    sectorId: this.data.model?.sectorId ?? this.data.model?.sector?.id ?? this.data.sectors[0]?.id ?? '',
    nameEn: this.data.model?.nameEn ?? this.data.model?.name ?? '',
    nameAr: this.data.model?.nameAr ?? '',
    descriptionEn: this.data.model?.descriptionEn ?? this.data.model?.description ?? '',
    descriptionAr: this.data.model?.descriptionAr ?? '',
    isActive: (this.data.model?.isActive ?? true) !== false
  };

  isValid(): boolean {
    return !!this.vm.nameEn?.trim() && !!this.vm.nameAr?.trim() && !!this.vm.sectorId;
  }

  save() {
    if (!this.isValid()) return;
    this.ref.close({
      sectorId: this.vm.sectorId,
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
