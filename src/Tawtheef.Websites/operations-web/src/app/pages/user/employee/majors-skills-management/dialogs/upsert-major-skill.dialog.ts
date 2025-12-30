import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { SelectModule } from 'primeng/select';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { RemoteSelectComponent } from '../../../../../shared/components/remote-select/remote-select';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import {UpsertSkillDialogData} from './upsert-skill.dialog';
import {TranslatePipe} from '@ngx-translate/core';

export interface DropdownOption {
  id: string;
  name: string;
}

type DialogMode = 'create' | 'edit';

export interface UpsertMajorSkillDialogData {
  mode: DialogMode;
  model?: any; // MajorSkillDetailsModel for edit
  skillTypes: DropdownOption[];

  // preselect from filters when create:
  parentMajorId?: string;
  subMajorId?: string;
}

@Component({
  standalone: true,
  selector: 'app-upsert-major-skill-dialog',
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    ToggleSwitchModule,
    SelectModule,
    RemoteSelectComponent,
    TranslatePipe
  ],template: `
    <div class="p-2">
      <form (ngSubmit)="save()" #f="ngForm" class="d-flex flex-column gap-3">

        <div class="row g-3">
          <div class="col-12 col-md-6">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_PARENT_MAJOR' | translate }}</label>
            <app-remote-select
              [searchUrl]="endpoints.majorSkillsManagement.lookups.majors"
              optionLabel="name"
              optionValue="id"
              [showClear]="true"
              [minChars]="1"
              [ngModel]="vm.parentMajorId"
              (ngModelChange)="onParentMajorChange($event)"
              name="parentMajorId"
              [appendTo]="'body'">
            </app-remote-select>
            <small class="text-muted" *ngIf="f.submitted && !vm.parentMajorId">
              {{ 'MAJORS_SKILLS.VALIDATION_REQUIRED' | translate }}
            </small>
          </div>

          <div class="col-12 col-md-6">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_SUB_MAJOR_OPTIONAL' | translate }}</label>
            <app-remote-select
              [searchUrl]="endpoints.majorSkillsManagement.lookups.subMajors"
              optionLabel="name"
              optionValue="id"
              [showClear]="true"
              [requireParent]="true"
              [parentId]="vm.parentMajorId"
              parentParamName="parentId"
              [minChars]="1"
              [ngModel]="vm.subMajorId"
              (ngModelChange)="vm.subMajorId = $event"
              name="subMajorId"
              [appendTo]="'body'">
            </app-remote-select>
          </div>

          <div class="col-12">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_SKILL' | translate }}</label>
            <app-remote-select
              [searchUrl]="endpoints.majorSkillsManagement.lookups.skills"
              optionLabel="name"
              optionValue="id"
              [showClear]="true"
              [minChars]="1"
              [ngModel]="vm.skillId"
              (ngModelChange)="vm.skillId = $event"
              name="skillId"
              [appendTo]="'body'">
            </app-remote-select>
            <small class="text-muted" *ngIf="f.submitted && !vm.skillId">
              {{ 'MAJORS_SKILLS.VALIDATION_REQUIRED' | translate }}
            </small>
          </div>

          <div class="col-12 col-md-6 d-flex align-items-end justify-content-between gap-3">
            <div class="d-flex align-items-center gap-2">
              <p-toggle-switch [(ngModel)]="vm.isSkillRequired" name="isSkillRequired"></p-toggle-switch>
              <span>{{ 'MAJORS_SKILLS.FIELD_REQUIRED' | translate }}</span>
            </div>

            <div class="d-flex align-items-center gap-2">
              <p-toggle-switch [(ngModel)]="vm.isActive" name="isActive"></p-toggle-switch>
              <span>{{ 'MAJORS_SKILLS.FIELD_ACTIVE' | translate }}</span>
            </div>
          </div>
        </div>

        <div class="d-flex justify-content-end gap-2 mt-2">
          <button pButton type="button" class="p-button-outlined" (click)="cancel()">
            {{ 'MAJORS_SKILLS.CANCEL' | translate }}
          </button>
          <button pButton type="submit" class="p-button" [disabled]="!isValid()">
            {{ 'MAJORS_SKILLS.SAVE' | translate }}
          </button>
        </div>

      </form>
    </div>
  `

})
export class UpsertMajorSkillDialogComponent {
  endpoints = inject(EndpointsService);
  public ref = inject(DynamicDialogRef)
  public config = inject(DynamicDialogConfig<UpsertMajorSkillDialogData>)

  get skillTypes(): DropdownOption[] {
    return this.config.data?.skillTypes ?? [];
  }

  private data = this.config?.data ?? { mode: 'create' as const };
  private model = this.data.model;

  vm = {
    id: this.model?.id ?? null,
    parentMajorId: this.data.parentMajorId ?? this.model?.majorId ?? this.model?.major?.id ?? '',
    subMajorId: this.data.subMajorId ?? '',
    skillId: this.model?.skillId ?? this.model?.skill?.id ?? '',
    isSkillRequired: this.model?.isSkillRequired ?? false,
    isActive: (this.model?.isActive ?? true) !== false
  };

  onParentMajorChange(id: string) {
    this.vm.parentMajorId = id;
    this.vm.subMajorId = ''; // clear sub when parent changes
  }

  isValid(): boolean {
    return !!this.vm.parentMajorId && !!this.vm.skillId;
  }

  save() {
    if (!this.isValid()) return;

    // Decide the actual majorId to send:
    // If user selected a sub-major, use it. Otherwise use parent major.
    const majorIdToSave = this.vm.subMajorId || this.vm.parentMajorId;

    const payload: any = {
      id: this.vm.id ?? undefined,
      majorId: majorIdToSave,
      skillId: this.vm.skillId,
      isSkillRequired: this.vm.isSkillRequired,
      isActive: this.vm.isActive
    };

    this.ref.close(payload);
  }

  cancel() {
    this.ref.close();
  }
}
