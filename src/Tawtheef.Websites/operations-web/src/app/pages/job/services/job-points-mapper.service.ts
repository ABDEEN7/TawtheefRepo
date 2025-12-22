import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { JobPointRuleTypeEnum } from '../enums/job-point-rule-type';
import { JobPointsResponse } from '../models/job-points-response';

@Injectable({ providedIn: 'root' })
export class JobPointsMapperService {

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
      certificates: response.certificates,
      skills: response.skills,
      languages: response.languages,
      total: response.total
    }, { emitEvent: false });
  }

  private mapDetails(details: any[], form: FormGroup): void {
    details.forEach(detail => {
      switch (detail.type) {

        case JobPointRuleTypeEnum.ApplicantCategory:
          form.get('applicantCategory')?.get(detail.code)?.setValue(detail.points);
          break;

        case JobPointRuleTypeEnum.Education:
          form.get('education')?.get(detail.code)?.setValue(detail.points);
          break;

        case JobPointRuleTypeEnum.Training:
          form.get('training')?.get(detail.code)?.setValue(detail.points);
          break;

        case JobPointRuleTypeEnum.Language:
          form.get('languages')?.get(detail.code)?.setValue(detail.points);
          break;

        case JobPointRuleTypeEnum.Skill:
          form.get('skills')?.get(detail.code)?.setValue(detail.points);
          break;

        case JobPointRuleTypeEnum.Certificate:
          form.get('certificates')?.get(detail.code)?.setValue(detail.points);
          break;

        case JobPointRuleTypeEnum.Experience:
          form.get('experience')?.get(detail.code)?.setValue(detail.points);
          break;
      }
    });
  }
}
