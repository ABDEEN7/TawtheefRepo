import { Injectable, inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { JobPointRuleTypeEnum } from '../enums/job-point-rule-type';
import { JobPointsResponse } from '../models/job-points-response';
import { JobPointsDetailResponse } from '../models/job-points-detail-response';

@Injectable({ providedIn: 'root' })
export class JobPointsMapperService {
  private fb = inject(FormBuilder);

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
      switch (detail.type) {
        case JobPointRuleTypeEnum.ApplicantCategory:
          this.setFormValue(form, 'applicantCategory', detail.code, detail.points);
          break;

        case JobPointRuleTypeEnum.Education:
          this.setFormValue(form, 'education', detail.code, detail.points);
          break;

        case JobPointRuleTypeEnum.Training:
          this.setFormValue(form, 'training', detail.code, detail.points);
          break;

        case JobPointRuleTypeEnum.Language:
          this.setFormValue(form, 'languages', detail.code, detail.points);
          break;

        case JobPointRuleTypeEnum.Skill:
          this.setFormValue(form, 'skills', detail.code, detail.points);
          break;

        case JobPointRuleTypeEnum.Certificate:
          this.setFormValue(form, 'certificates', detail.code, detail.points);
          break;

        case JobPointRuleTypeEnum.Experience:
          this.setFormValue(form,'experience',detail.code,detail.points)
          break;
      }
    });
  }

  private setFormValue(form: FormGroup, groupName: string, controlName: string, value: number): void {
    const group = form.get(groupName) as FormGroup;
    if (group) {
      if (!group.get(controlName)) {
        group.addControl(controlName, this.fb.control(0));
      }
      group.get(controlName)?.setValue(value);
    }
  }
}