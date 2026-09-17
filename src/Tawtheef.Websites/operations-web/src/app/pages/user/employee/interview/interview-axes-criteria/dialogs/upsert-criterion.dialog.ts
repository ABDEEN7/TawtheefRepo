import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { InputTextModule } from 'primeng/inputtext';
import { Textarea } from 'primeng/textarea';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { ButtonModule } from 'primeng/button';
import { Select } from 'primeng/select';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { CriterionModel } from '../models/criterion.model';
import { AxisOption } from '../interview-axes-criteria.store';

type DialogMode = 'create' | 'edit';

export interface UpsertCriterionDialogData {
  mode: DialogMode;
  model?: CriterionModel;
  axisOptions: AxisOption[];
  defaultAxisId?: string;
}

@Component({
  selector: 'app-upsert-criterion-dialog',
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
      <form (ngSubmit)="save()" #f="ngForm" class="d-flex flex-column gap-3">
        <div class="row">
          <div class="col-md-12 mb-3 mb-md-0">
            <label class="form-label">{{ 'INTERVIEW_AXES_CRITERIA.FIELD_AXIS' | translate }}</label>
            <p-select
              class="w-100"
              [options]="data.axisOptions"
              optionLabel="label"
              optionValue="value"
              name="axisId"
              [(ngModel)]="vm.axisId"
              required
              [placeholder]="'INTERVIEW_AXES_CRITERIA.FIELD_AXIS' | translate"
            >
            </p-select>
            <small class="text-muted" *ngIf="f.submitted && !vm.axisId">{{
              'INTERVIEW_AXES_CRITERIA.VALIDATION_REQUIRED' | translate
            }}</small>
          </div>
        </div>

        <div class="row">
          <div class="col-md-6 mb-3 mb-md-0">
            <label class="form-label">{{
              'INTERVIEW_AXES_CRITERIA.FIELD_NAME_AR' | translate
            }}</label>
            <input
              pInputText
              class="w-100 form-control"
              name="nameAr"
              [(ngModel)]="vm.nameAr"
              required
              maxlength="200"
              [placeholder]="'INTERVIEW_AXES_CRITERIA.FIELD_NAME_AR' | translate"
            />
            <small class="text-muted" *ngIf="f.submitted && !vm.nameAr">{{
              'INTERVIEW_AXES_CRITERIA.VALIDATION_REQUIRED' | translate
            }}</small>
          </div>
          <div class="col-md-6 mb-3 mb-md-0">
            <label class="form-label">{{
              'INTERVIEW_AXES_CRITERIA.FIELD_NAME_EN' | translate
            }}</label>
            <input
              pInputText
              class="w-100 form-control"
              name="nameEn"
              [(ngModel)]="vm.nameEn"
              maxlength="200"
              [placeholder]="'INTERVIEW_AXES_CRITERIA.FIELD_NAME_EN' | translate"
            />
          </div>
        </div>

        <div class="row">
          <div class="col-md-6 mb-3 mb-md-0">
            <label class="form-label">{{
              'INTERVIEW_AXES_CRITERIA.FIELD_DESCRIPTION_AR' | translate
            }}</label>
            <textarea
              pInputTextarea
              rows="3"
              class="w-100 form-control"
              name="descriptionAr"
              [(ngModel)]="vm.descriptionAr"
              maxlength="500"
              [placeholder]="'INTERVIEW_AXES_CRITERIA.FIELD_DESCRIPTION_AR' | translate"
            ></textarea>
          </div>
          <div class="col-md-6 mb-3 mb-md-0">
            <label class="form-label">{{
              'INTERVIEW_AXES_CRITERIA.FIELD_DESCRIPTION_EN' | translate
            }}</label>
            <textarea
              pInputTextarea
              rows="3"
              class="w-100 form-control"
              name="descriptionEn"
              [(ngModel)]="vm.descriptionEn"
              maxlength="500"
              [placeholder]="'INTERVIEW_AXES_CRITERIA.FIELD_DESCRIPTION_EN' | translate"
            ></textarea>
          </div>
          <div class="col-md-12 mt-3">
            <div class="d-flex align-items-center gap-2">
              <p-toggle-switch [(ngModel)]="vm.isActive" name="isActive"></p-toggle-switch>
              <span>{{ 'INTERVIEW_AXES_CRITERIA.FIELD_ACTIVE' | translate }}</span>
            </div>
          </div>
        </div>

        <div class="modal-footer justify-content-end gap-3 border-0">
          <button
            type="button"
            class="btn btn-primary-outline btn-sm d-flex text-primary gap-1 align-items-center"
            (click)="cancel()"
          >
            <i class="hgi hgi-stroke hgi-cancel-01 text-primary fs-18"></i>
            {{ 'INTERVIEW_AXES_CRITERIA.CANCEL' | translate }}
          </button>
          <button
            type="submit"
            class="btn btn-primary btn-sm d-flex gap-2 justify-content-center align-items-center"
            [disabled]="!isValid()"
          >
            <i class="hgi hgi-stroke hgi-floppy-disk fs-18"></i>
            {{ 'INTERVIEW_AXES_CRITERIA.SAVE' | translate }}
          </button>
        </div>
      </form>
    </div>
  `,
})
export class UpsertCriterionDialogComponent {
  ref = inject(DynamicDialogRef);
  config = inject(DynamicDialogConfig<UpsertCriterionDialogData>);

  data = this.config?.data ?? { mode: 'create' as const, axisOptions: [] };

  vm = {
    axisId:
      this.data.model?.interviewEvaluationAxisId ??
      this.data.defaultAxisId ??
      this.data.axisOptions[0]?.value ??
      '',
    nameEn: this.data.model?.nameEn ?? '',
    nameAr: this.data.model?.nameAr ?? '',
    descriptionEn: this.data.model?.descriptionEn ?? '',
    descriptionAr: this.data.model?.descriptionAr ?? '',
    isActive: (this.data.model?.isActive ?? true) !== false,
  };

  isValid(): boolean {
    return !!this.vm.nameAr?.trim() && !!this.vm.axisId;
  }

  save() {
    if (!this.isValid()) return;
    this.ref.close({
      interviewEvaluationAxisId: this.vm.axisId,
      nameAr: this.vm.nameAr.trim(),
      nameEn: this.vm.nameEn?.trim() || null,
      descriptionAr: this.vm.descriptionAr?.trim() || null,
      descriptionEn: this.vm.descriptionEn?.trim() || null,
      isActive: this.vm.isActive,
    });
  }

  cancel() {
    this.ref.close();
  }
}
