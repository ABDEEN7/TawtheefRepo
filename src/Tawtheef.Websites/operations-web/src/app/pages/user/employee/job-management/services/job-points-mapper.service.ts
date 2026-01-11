import { Injectable, inject } from '@angular/core';
import { JobPointRuleTypeEnum } from '../enums/job-point-rule-type';
import { JobPointsResponse } from '../models/job-points-response';
import { JobPointsDetailResponse } from '../models/job-points-detail-response';
import { FormBuilder, FormGroup } from '@angular/forms';

@Injectable({ providedIn: 'root' })
export class JobPointsMapperService {
  private fb = inject(FormBuilder);

  private categoryMappings = {
    [JobPointRuleTypeEnum.ApplicantCategory]: 'applicantCategory',
    [JobPointRuleTypeEnum.Education]: 'education',
    [JobPointRuleTypeEnum.Training]: 'training',
    [JobPointRuleTypeEnum.Skill]: 'skills',
    [JobPointRuleTypeEnum.Certificate]: 'certificates',
    [JobPointRuleTypeEnum.Experience]: 'experience',
  } as const;

  mapResponseToForm(
    response: JobPointsResponse,
    mainForm: FormGroup,
    detailsForm: FormGroup
  ): void {
    this.mapMain(response, mainForm);
    this.mapDetails(response.details, detailsForm);
  }

  private mapMain(response: JobPointsResponse, form: FormGroup): void {
    form.patchValue({
      applicantCategory: response.applicantCategory,
      education: response.education,
      experience: response.experience,
      training: response.training,
      skills: response.skills,
      languages: response.languages,
      certificates: response.certificates,
      total: response.total,
    }, { emitEvent: false });
  }

  private mapDetails(details: JobPointsDetailResponse[], form: FormGroup): void {
    details.forEach(detail => {
      if (detail.type === JobPointRuleTypeEnum.Language) {
        this.setLanguageFormValue(form, detail.code, detail.points);
      } else {
        const groupName = this.categoryMappings[detail.type as keyof typeof this.categoryMappings];
        if (groupName) {
          this.setFormValue(form, groupName, detail.code, detail.points);
        }
      }
    });
  }

  private setFormValue(form: FormGroup, groupName: string, controlPath: string, value: number): void {
    const group = form.get(groupName) as FormGroup;
    if (group) {
      this.setNestedControlValue(group, controlPath, value);
    }
  }

  private setLanguageFormValue(form: FormGroup, controlPath: string, value: number): void {
    const languagesGroup = form.get('languages') as FormGroup;
    if (!languagesGroup) return;
    
    if (controlPath === 'native') {
      this.setControlValue(languagesGroup, 'native', value);
    } else {
      this.setNestedControlValue(languagesGroup, controlPath, value);
    }
  }

  private setNestedControlValue(parentGroup: FormGroup, controlPath: string, value: number): void {
    const parts = controlPath.split('.');
    const [groupPath, controlName] = parts.length > 1 
      ? [parts.slice(0, -1).join('.'), parts[parts.length - 1]]
      : ['', parts[0]];

    let targetGroup = parentGroup;
    
    if (groupPath) {
      targetGroup = this.getOrCreateNestedGroup(parentGroup, groupPath);
    }
    
    this.setControlValue(targetGroup, controlName, value);
  }

  private getOrCreateNestedGroup(parentGroup: FormGroup, groupPath: string): FormGroup {
    const parts = groupPath.split('.');
    let currentGroup = parentGroup;
    
    for (const part of parts) {
      let nextGroup = currentGroup.get(part) as FormGroup;
      if (!nextGroup) {
        nextGroup = this.fb.group({});
        currentGroup.addControl(part, nextGroup);
      }
      currentGroup = nextGroup;
    }
    
    return currentGroup;
  }

  private setControlValue(group: FormGroup, controlName: string, value: number): void {
    if (!group.get(controlName)) {
      group.addControl(controlName, this.fb.control(0));
    }
    group.get(controlName)?.setValue(value, { emitEvent: false });
  }
}