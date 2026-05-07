import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { SelectModule } from 'primeng/select';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

import { RemoteSelectComponent } from '../../../../../shared/components/remote-select/remote-select';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { TranslatePipe } from '@ngx-translate/core';
import {MajorSkillDetailsModel} from '../models/major-skill-details.model';
import {majorDetails} from '../models/major.details';
import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';

export interface DropdownOption {
  id: string;
  name: string;
}

type DialogMode = 'create' | 'edit';

export interface UpsertMajorSkillDialogData {
  mode: DialogMode;
  model?: MajorSkillDetailsModel;
  skillTypes: DropdownOption[];

  // preselect from filters when create:
  parentMajorId?: string;
  subMajorId?: string;
}

type Vm = {
  id: string | null;
  parentMajorId: string;
  subMajorId: string;
  skillId: string;
  isActive: boolean;
};

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
    TranslatePipe,
  ],
  template: `
    <div class="modal-body">
      <form (ngSubmit)="save()" #f="ngForm" class="d-flex flex-column gap-3">

        <div class="row">
          <div class="col-12 col-md-6 mb-3">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_PARENT_MAJOR' | translate }} <span class="text-danger">*</span></label>

            <app-remote-select
              [searchUrl]="selectCfg.parentMajor.searchUrl"
              optionLabel="name" [extraQueryParams]="{ IncludeOrphanMajors: true }"
              optionValue="id"
              [showClear]="true"
              [minChars]="1"
              [ngModel]="vm().parentMajorId"
              (ngModelChange)="onParentMajorChange($event)"
              name="parentMajorId"
              [preloadedOptions]="parentMajorOptions()"
              [appendTo]="'body'">
            </app-remote-select>

            <small class="text-muted" *ngIf="f.submitted && !vm().parentMajorId">
              {{ 'MAJORS_SKILLS.VALIDATION_REQUIRED' | translate }}
            </small>
          </div>

          <div class="col-12 col-md-6 mb-3">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_SUB_MAJOR_OPTIONAL' | translate }}</label>

            <app-remote-select
              [searchUrl]="selectCfg.subMajor.searchUrl"
              optionLabel="name"
              optionValue="id"
              [showClear]="true"
              [requireParent]="true"
              [parentId]="vm().parentMajorId"
              parentParamName="parentId"
              [minChars]="1"
              [ngModel]="vm().subMajorId"
              (ngModelChange)="patchVm({ subMajorId: $event })"
              name="subMajorId"
              [preloadedOptions]="subMajorOptions()"
              [appendTo]="'body'">
            </app-remote-select>
          </div>

          <div class="col-12 mb-4">
            <label class="form-label">{{ 'MAJORS_SKILLS.FIELD_SKILL' | translate }} <span class="text-danger">*</span></label>

            <app-remote-select
              [searchUrl]="selectCfg.skill.searchUrl"
              optionLabel="name"
              optionValue="id"
              [showClear]="true"
              [minChars]="1"
              [ngModel]="vm().skillId"
              (ngModelChange)="patchVm({ skillId: $event })"
              name="skillId"
              [preloadedOptions]="skillOptions()"
              [appendTo]="'body'">
            </app-remote-select>

            <small class="text-muted" *ngIf="f.submitted && !vm().skillId">
              {{ 'MAJORS_SKILLS.VALIDATION_REQUIRED' | translate }}
            </small>
          </div>

          <div class="col-12 col-md-6 d-flex align-items-end justify-content-between gap-3">
            <div class="d-flex align-items-center gap-2">
              <p-toggle-switch
                [(ngModel)]="vm().isActive"
                (ngModelChange)="patchVm({ isActive: $event })"
                name="isActive">
              </p-toggle-switch>
              <span>{{ 'MAJORS_SKILLS.FIELD_ACTIVE' | translate }}</span>
            </div>
          </div>
        </div>


        <div class="modal-footer justify-content-end gap-3 border-0">
          <button  type="button" class="btn btn-primary-outline d-flex text-primary gap-1 align-items-center" (click)="cancel()">
            <i class="hgi hgi-stroke hgi-cancel-01 text-primary fs-18"></i>
             {{ 'MAJORS_SKILLS.CANCEL' | translate }}
          </button>
          <button  type="submit" class="btn btn-primary d-flex gap-2 justify-content-center align-items-center" [disabled]="!isValid()">
            <i class="hgi hgi-stroke hgi-floppy-disk fs-18"></i>
             {{ 'MAJORS_SKILLS.SAVE' | translate }}
          </button>
        </div>

      </form>
    </div>
  `,
})
export class UpsertMajorSkillDialogComponent {
  private readonly endpoints = inject(EndpointsService);
  private readonly ref = inject(DynamicDialogRef);
  private readonly config = inject(DynamicDialogConfig<UpsertMajorSkillDialogData>);

  // centralize URLs (template becomes dumb)
  readonly selectCfg = {
    parentMajor: { searchUrl: this.endpoints.majorSkillsManagement.lookups.majors },
    subMajor: { searchUrl: this.endpoints.majorSkillsManagement.lookups.subMajors },
    skill: { searchUrl: this.endpoints.majorSkillsManagement.lookups.skills },
  };

  // Normalize incoming data safely
  private readonly data: UpsertMajorSkillDialogData = this.config.data ?? {
    mode: 'create',
    skillTypes: [],
  };

  private readonly model: MajorSkillDetailsModel | undefined = this.data.model;

  // Determine parent/child relationship once
  private readonly isParentMajor = !this.model?.major?.parentId;

  // View model in a signal: easier to patch, no accidental mutation in multiple places
  readonly vm = signal<Vm>(this.buildInitialVm());

  // Preloaded options as computed values (always consistent with model/vm)
  readonly parentMajorOptions = computed<majorDetails[]>(() => {
    const major = this.model?.major ?? null;
    if (!major) return [];

    return this.isParentMajor
      ? [major]
      : major.parent
        ? [major.parent]
        : [];
  });

  readonly subMajorOptions = computed<majorDetails[]>(() => {
    const major = this.model?.major ?? null;
    if (!major) return [];

    return this.isParentMajor ? [] : [major];
  });

  readonly skillOptions = computed<dropdownOptionsModel[]>(() => {
    const skill = this.model?.skill ?? null;
    return skill ? [skill] : [];
  });

  get skillTypes(): DropdownOption[] {
    return this.data.skillTypes ?? [];
  }

  patchVm(patch: Partial<Vm>) {
    this.vm.update((cur) => ({ ...cur, ...patch }));
  }

  onParentMajorChange(id: string) {
    // if parent changes, always clear sub major
    this.patchVm({ parentMajorId: id, subMajorId: '' });
  }

  isValid(): boolean {
    const v = this.vm();
    return !!v.parentMajorId && !!v.skillId;
  }

  save() {
    const v = this.vm();
    if (!this.isValid()) return;

    // Decide the actual majorId to send:
    // If user selected a sub-major, use it. Otherwise use parent major.
    const majorIdToSave = v.subMajorId || v.parentMajorId;

    this.ref.close({
      id: v.id ?? undefined,
      majorId: majorIdToSave,
      skillId: v.skillId,
      isActive: v.isActive,
    });
  }

  cancel() {
    this.ref.close();
  }

  private buildInitialVm(): Vm {
    // If create mode with preselected filters, prefer those.
    // If edit mode, derive from model.
    const preParent = this.data.parentMajorId ?? '';
    const preSub = this.data.subMajorId ?? '';

    const model = this.model;

    const parentFromModel =
      (this.isParentMajor ? model?.major?.id : model?.major?.parentId) ?? '';

    const subFromModel =
      this.isParentMajor ? '' : (model?.majorId ?? model?.major?.id ?? '');

    return {
      id: (model?.id ?? null) as string | null,
      parentMajorId: preParent || parentFromModel,
      subMajorId: preSub || subFromModel,
      skillId: model?.skillId ?? model?.skill?.id ?? '',
      isActive: (model?.isActive ?? true),
    };
  }
}
