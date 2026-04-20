import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { GUID } from '../../../../../../shared/types/guid.type';
import { dropdownOptionsModel } from '../../../../../../shared/models/dropdown-options.model';
import { GuidUtils } from '../../../../../../core/utils/guid-utils';

@Component({
  selector: 'app-job-candidates-specialization-filter-modal',
  templateUrl: './job-candidates-specialization-filter-modal.component.html',
  styleUrl: './job-candidates-specialization-filter-modal.component.scss',
  standalone: false,
})
export class JobCandidatesSpecializationFilterModalComponent implements OnInit {
  private dialogRef = inject(DynamicDialogRef);
  private dialogConfig = inject(DynamicDialogConfig);
  private formBuilder = inject(FormBuilder);

  mainMajor?: dropdownOptionsModel;
  mainSubMajor?: dropdownOptionsModel;
  specializations: { id: GUID; major: dropdownOptionsModel; subMajor?: dropdownOptionsModel }[] = [];
  selectedSpecializations: { majorId: GUID; subMajorId: GUID }[] = [];

  form = this.formBuilder.group({
    specializationSelections: this.formBuilder.array([]),
  });

  get specializationSelections(): FormArray {
    return this.form.get('specializationSelections') as FormArray;
  }

  ngOnInit(): void {
    const data = this.dialogConfig.data ?? {};
    this.mainMajor = data.mainMajor;
    this.mainSubMajor = data.mainSubMajor;
    this.specializations = data.specializations ?? [];
    this.selectedSpecializations = data.selectedSpecializations ?? [];

    // Push Main Specialization
    const isMainSelected = this.selectedSpecializations.length === 0 || 
      this.selectedSpecializations.some(s => s.majorId === this.mainMajor?.id && s.subMajorId === (this.mainSubMajor?.id ?? GuidUtils.emptyGuid));

    this.specializationSelections.push(this.formBuilder.group({
      label: [this.getMainLabel()],
      majorId: [this.mainMajor?.id],
      subMajorId: [this.mainSubMajor?.id ?? GuidUtils.emptyGuid],
      isSelected: [{ value: true, disabled: true }],
      isMain: [true]
    }));

    // Push Additional Specializations
    this.specializations.forEach(spec => {
      const isSelected = this.selectedSpecializations.some(s => s.majorId === spec.major.id && s.subMajorId === (spec.subMajor?.id ?? GuidUtils.emptyGuid));
      this.specializationSelections.push(this.formBuilder.group({
        label: [this.getSpecLabel(spec)],
        majorId: [spec.major.id],
        subMajorId: [spec.subMajor?.id ?? GuidUtils.emptyGuid],
        isSelected: [isSelected],
        isMain: [false]
      }));
    });
  }

  getMainLabel(): string {
    if (!this.mainMajor) return '';
    return `${this.mainMajor.name}${this.mainSubMajor ? ' / ' + this.mainSubMajor.name : ''}`;
  }

  getSpecLabel(spec: any): string {
    return `${spec.major.name}${spec.subMajor ? ' / ' + spec.subMajor.name : ''}`;
  }

  save(): void {
    const selected = this.specializationSelections.getRawValue()
      .filter((s: any) => s.isSelected)
      .map((s: any) => ({
        majorId: s.majorId,
        subMajorId: s.subMajorId
      }));

    this.dialogRef.close(selected);
  }

  close(): void {
    this.dialogRef.close();
  }
}
