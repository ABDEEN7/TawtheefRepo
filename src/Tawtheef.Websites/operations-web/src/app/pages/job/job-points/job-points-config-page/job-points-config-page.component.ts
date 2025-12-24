import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { GUID } from '../../../../shared/types/guid.type';
import { GuidUtils } from '../../../../core/utils/guid-utils';
import { JobService } from '../../services/job.service';
import { JobPointsConfigService } from '../../services/job-points-config.service';
import { JobPointsMapperService } from '../../services/job-points-mapper.service';
import { JobResponse } from '../../models/job-response-model';
import { JobPointsResponse } from '../../models/job-points-response';
import { JobPointsCalculationService } from '../../services/job-points-calculation.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { TranslateService } from '@ngx-translate/core';
import { JobPointsDetail } from '../../models/job-points-details.model';
import { JobPointRuleTypeEnum } from '../../enums/job-point-rule-type';

@Component({
  selector: 'app-job-points-config-page',
  standalone: false,
  templateUrl: './job-points-config-page.component.html',
  styleUrls: ['./job-points-config-page.component.scss'],
})
export class JobPointsConfigPageComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private pointsCalculationService = inject(JobPointsCalculationService);
  private cdr = inject(ChangeDetectorRef);
  private jobService = inject(JobService);
  private jobPointsService = inject(JobPointsConfigService);
  private mapper = inject(JobPointsMapperService);
  private notificationService = inject(NotificationService);
  private translationService = inject(TranslateService);

  jobId: GUID = GuidUtils.emptyGuid;
  job: JobResponse | null = null;
  jobPoints: JobPointsResponse | null = null;
  mainKeys: { key: string; totalPercent: number }[] = [];

  form!: FormGroup;
  isEditMode = false;
  activeTab = '0';

  ngOnInit(): void {
    this.initForm();
    this.handleMainTotal();
    this.handleExperienceTotal();

    const paramId = this.route.snapshot.paramMap.get('id');
    if (paramId) {
      this.jobId = paramId as GUID;
      this.loadJob();
      this.loadJobPointsConfig();
    }
  }

  private initForm(): void {
    this.form = this.fb.group({
      main: this.fb.group({
        applicantCategory: [0],
        education: [0],
        experience: [0],
        training: [0],
        skills: [0],
        languages: [0],
        certificates: [0],
        total: [{ value: 0, disabled: true }],
      }),
      details: this.fb.group({
        applicantCategory: this.fb.group({}),
        education: this.fb.group({}),
        experience: this.fb.group({
          pointsPerYear: [0],
          maxYears: [0],
          total: [{ value: 0, disabled: true }],
        }),
        training: this.fb.group({}),
        languages: this.fb.group({}),
        certificates: this.fb.group({}),
        skills: this.fb.group({}),
      }),
    });
  }

  get mainFormGroup(): FormGroup {
    return this.form.get('main') as FormGroup;
  }

  get detailsFormGroup(): FormGroup {
    return this.form.get('details') as FormGroup;
  }

  private loadJob(): void {
    if (!this.jobId || this.jobId === GuidUtils.emptyGuid) return;

    this.jobService.getById(this.jobId).subscribe((job) => {
      this.job = job;

      this.jobPointsService.getJobPoints(this.jobId).subscribe((response) => {
        this.jobPoints = response;
        this.mapper.mapResponseToForm(response, this.mainFormGroup, this.detailsFormGroup);
        this.isEditMode = true;
        this.cdr.detectChanges();
      });
    });
  }

  private loadJobPointsConfig(): void {
    this.jobPointsService.getJobPointsConfiguration(this.jobId).subscribe((config) => {
      this.mainKeys = [
        { key: 'applicantCategory', totalPercent: config.applicantCategoryMaxPoints },
        { key: 'education', totalPercent: config.educationMaxPoints },
        { key: 'experience', totalPercent: config.experienceMaxPoints },
        { key: 'training', totalPercent: config.trainingMaxPoints },
        { key: 'skills', totalPercent: config.skillsMaxPoints },
        { key: 'languages', totalPercent: config.languagesMaxPoints },
        { key: 'certificates', totalPercent: config.certificatesMaxPoints },
      ];
      this.cdr.detectChanges();
    });
  }

  private handleMainTotal(): void {
    this.mainFormGroup.valueChanges.subscribe((main) => {
      const total = this.mainKeys.reduce((sum, k) => sum + (main[k.key] || 0), 0);
      this.mainFormGroup.get('total')?.setValue(total, { emitEvent: false });
    });
  }

  private handleExperienceTotal(): void {
    const exp = this.detailsFormGroup.get('experience') as FormGroup;
    exp.valueChanges.subscribe((v) => {
      const total = this.pointsCalculationService.experienceTotal(exp);
      exp.get('total')?.setValue(total, { emitEvent: false });
    });
  }

  canAccessDetails(): boolean {
    const max = this.mainKeys.reduce((s, k) => s + k.totalPercent, 0);
    return this.mainFormGroup.get('total')?.value === max;
  }

  areAllCategoriesValid(): boolean {
    return this.pointsCalculationService.areAllCategoriesValid(
      this.mainFormGroup,
      this.detailsFormGroup,
      this.pointsCalculationService.sections
    );
  }

  save(): void {
    if (!this.areAllCategoriesValid()) {
      this.notificationService.warn(
        this.translationService.instant('JOB_POINTS.VALIDATION.DETAILS_EXCEED_MAIN')
      );
      return;
    }

    const isUpdate = this.jobPoints && this.jobPoints.id !== GuidUtils.emptyGuid;
    const payload = {
      request: {
        Id: isUpdate ? this.jobPoints!.id : GuidUtils.emptyGuid,
        jobId: this.jobId,
        ...this.getMainPoints(),
        details: this.getDetails(),
      },
    };

    this.jobPointsService.saveJobPoints(payload).subscribe((responseId) => {
      if (!isUpdate) {
        this.jobPoints = {
          id: responseId,
          jobId: this.jobId,
          ...this.getMainPoints(),
          details: [],
        };
      }
      this.notificationService.success(this.translationService.instant('JOB_POINTS.SAVE.SUCCESS'));
    });
  }

  private getMainPoints(): Omit<JobPointsResponse, 'id' | 'jobId' | 'details'> {
    const main = this.mainFormGroup.getRawValue();
    return {
      applicantCategory: main.applicantCategory || 0,
      education: main.education || 0,
      experience: main.experience || 0,
      training: main.training || 0,
      certificates: main.certificates || 0,
      skills: main.skills || 0,
      languages: main.languages || 0,
      total: main.total || 0,
    };
  }

  private getDetails(): any[] {
    const d = this.detailsFormGroup.getRawValue();
    const result: JobPointsDetail[] = [];

    this.pushCategory(result, d.applicantCategory, JobPointRuleTypeEnum.ApplicantCategory);
    this.pushCategory(result, d.training, JobPointRuleTypeEnum.Training);
    this.pushCategory(result, d.skills, JobPointRuleTypeEnum.Skill, true);
    this.pushCategory(result, d.education, JobPointRuleTypeEnum.Education, true);
    this.pushCategory(result, d.certificates, JobPointRuleTypeEnum.Certificate);
    this.pushCategory(result, d.experience, JobPointRuleTypeEnum.Experience);
    this.pushCategory(result, d.languages, JobPointRuleTypeEnum.Language);

    for (const section of this.pointsCalculationService.sections) {
      const group = this.detailsFormGroup.get(section) as FormGroup;

      let sectionSum: number;
      if (section === 'experience') {
        sectionSum = this.pointsCalculationService.experienceTotal(group);
      } else if (section === 'languages') {
        sectionSum = this.pointsCalculationService.sumLanguagesCategory(group);
      } else {
        sectionSum = this.pointsCalculationService.sumCategory(group);
      }
    }

    return result;
  }

  private pushCategory(
    target: JobPointsDetail[],
    source: any,
    type: JobPointRuleTypeEnum,
    withRef = false,
    parentKey?: string
  ): void {
    Object.keys(source || {}).forEach((key) => {
      const value = source[key];
      const code = parentKey ? `${parentKey}.${key}` : key;

      if (typeof value === 'number' && value > 0) {
        const ref =
          withRef && type === JobPointRuleTypeEnum.Education
            ? this.job?.degrees?.find((d) => d.degree.backendName === code)?.degree?.id
            : withRef && type === JobPointRuleTypeEnum.Skill
            ? this.job?.skills?.find((s) => s.skill.backendName === code)?.skill?.id
            : undefined;

        target.push({
          type,
          code,
          points: value,
          referenceId: ref,
        });
      } else if (typeof value === 'object' && value !== null) {
        this.pushCategory(target, value, type, withRef, code);
      }
    });
  }

  goToDetails(): void {
    if (this.canAccessDetails()) {
      this.activeTab = '1';
    }
  }

  goBack(): void {
  this.activeTab = '0';
}
}
