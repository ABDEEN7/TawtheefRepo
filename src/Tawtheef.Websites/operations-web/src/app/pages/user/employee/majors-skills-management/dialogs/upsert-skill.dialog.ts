import { CommonModule } from '@angular/common';
import {Component, inject} from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import {UpsertMajorDialogData} from './upsert-major.dialog';
import {Textarea} from 'primeng/textarea';
import {TranslatePipe} from '@ngx-translate/core';

export interface DropdownOption {
  id: string;
  name: string;
}

type DialogMode = 'create' | 'edit';

export interface UpsertSkillDialogData {
  mode: DialogMode;
  model?: any;                 // SkillListItemModel or details
  skillTypes: DropdownOption[]; // store.skillTypes()
}

@Component({
  standalone: true,
  selector: 'app-upsert-skill-dialog',
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    InputNumberModule,
    SelectModule,
    ToggleSwitchModule,
    Textarea,
    TranslatePipe
  ],template: `
    <div class="modal-body">
      <form (ngSubmit)="save()" #f="ngForm" class="d-flex flex-column gap-3">

        <div class="row">
          <div class="col-md-6">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_NAME_EN' | translate }}</label>
            <input pInputText class="w-100"
                   name="nameEn"
                   [(ngModel)]="vm.nameEn"
                   required maxlength="100"
                   [placeholder]="'MAJORS_SKILLS.PLACEHOLDER_SKILL_NAME_EN' | translate" />
            <small class="text-muted" *ngIf="f.submitted && !vm.nameEn">
              {{ 'MAJORS_SKILLS.VALIDATION_REQUIRED' | translate }}
            </small>
          </div>

          <div class="col-md-6">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_NAME_AR' | translate }}</label>
            <input pInputText class="w-100"
                   name="nameAr"
                   [(ngModel)]="vm.nameAr"
                   required maxlength="100"
                   [placeholder]="'MAJORS_SKILLS.PLACEHOLDER_SKILL_NAME_AR' | translate" />
            <small class="text-muted" *ngIf="f.submitted && !vm.nameAr">
              {{ 'MAJORS_SKILLS.VALIDATION_REQUIRED' | translate }}
            </small>
          </div>
        </div>

        <div class="row">
             <div class="col-12">
                  <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_SKILL_TYPE' | translate }}</label>
                  <p-select
                    class="w-100"
                    [options]="skillTypes"
                    optionLabel="name"
                    optionValue="id"
                    [showClear]="true"
                    [appendTo]="'body'"
                    name="skillTypeId"
                    [(ngModel)]="vm.skillTypeId"
                    required
                    [placeholder]="'MAJORS_SKILLS.PLACEHOLDER_SELECT_SKILL_TYPE' | translate">
                  </p-select>
                  <small class="text-muted" *ngIf="f.submitted && !vm.skillTypeId">
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
                      maxlength="1000"
                      [placeholder]="'MAJORS_SKILLS.PLACEHOLDER_OPTIONAL' | translate"></textarea>
          </div>

          <div class="col-md-6">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_DESCRIPTION_AR' | translate }}</label>
            <textarea pInputTextarea rows="3" class="w-100 form-control"
                      name="descriptionAr"
                      [(ngModel)]="vm.descriptionAr"
                      maxlength="1000"
                      [placeholder]="'MAJORS_SKILLS.PLACEHOLDER_OPTIONAL' | translate"></textarea>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_DISPLAY_ORDER' | translate }}</label>
            <p-inputNumber
              class="w-100"
              name="displayOrder"
              [(ngModel)]="vm.displayOrder"
              [min]="1"
              [useGrouping]="false"
              [placeholder]="'MAJORS_SKILLS.PLACEHOLDER_OPTIONAL' | translate">
            </p-inputNumber>
          </div>

          <div class="col-md-6">
            <div class="d-flex align-items-center gap-2">
              <p-toggle-switch [(ngModel)]="vm.isActive" name="isActive"></p-toggle-switch>
              <span>{{ 'MAJORS_SKILLS.FIELD_ACTIVE' | translate }}</span>
            </div>
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
export class UpsertSkillDialogComponent {
  public ref = inject(DynamicDialogRef)
  public config = inject(DynamicDialogConfig<UpsertSkillDialogData>)
  get skillTypes(): DropdownOption[] {
    return this.config.data?.skillTypes ?? [];
  }

  private model = this.config?.data?.model;

  vm = {
    id: this.model?.id ?? null,

    nameEn: this.model?.nameEn ?? this.model?.name ?? '',
    nameAr: this.model?.nameAr ?? '',
    skillTypeId: this.model?.skillTypeId ?? this.model?.skillType?.id ?? '',
    descriptionEn: this.model?.descriptionEn ?? '',
    descriptionAr: this.model?.descriptionAr ?? '',
    displayOrder: this.model?.displayOrder ?? null,
    isActive: (this.model?.isActive ?? true) !== false
  };

  isValid(): boolean {
    return !!this.vm.nameEn?.trim()
      && !!this.vm.nameAr?.trim()
      && !!this.vm.skillTypeId;
  }

  save() {
    if (!this.isValid()) return;

    const payload: any = {
      id: this.vm.id ?? undefined,
      nameEn: this.vm.nameEn.trim(),
      nameAr: this.vm.nameAr.trim(),
      skillTypeId: this.vm.skillTypeId,
      descriptionEn: this.vm.descriptionEn?.trim() || null,
      descriptionAr: this.vm.descriptionAr?.trim() || null,
      displayOrder: this.vm.displayOrder ?? undefined,
      isActive: this.vm.isActive
    };

    this.ref.close(payload);
  }

  cancel() {
    this.ref.close();
  }
}
