import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { GUID } from '../../../../shared/types/guid.type';
import { GuidUtils } from '../../../../core/utils/guid-utils';

import { JobService } from '../../services/job.service';
import { JobPointsConfigService } from '../../services/job-points-config.service';
import { JobPointsMapperService } from '../../services/job-points-mapper.service';

import { JobResponse } from '../../models/job-response-model';
import { JobPointsDetail } from '../../models/job-points-details.model';

import { JobPointRuleTypeEnum } from '../../enums/job-point-rule-type';

import { NotificationService } from '../../../../core/services/notification.service';
import { TranslateService } from '@ngx-translate/core';
import { JobPointsResponse } from '../../models/job-points-response';

@Component({
  selector: 'app-job-points-config-page',
  standalone: false,
  templateUrl: './job-points-config-page.component.html',
  styleUrls: ['./job-points-config-page.component.scss'],
})
export class JobPointsConfigPageComponent implements OnInit {
  /* -------------------- Injected services -------------------- */
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private cdr = inject(ChangeDetectorRef);

  private jobService = inject(JobService);
  private jobPointsService = inject(JobPointsConfigService);
  private mapper = inject(JobPointsMapperService);

  private notificationService = inject(NotificationService);
  private translationService = inject(TranslateService);

  /* -------------------- State -------------------- */
  jobId: GUID = GuidUtils.emptyGuid;
  job: JobResponse | null = null;
  jobPoints!: JobPointsResponse;

  isEditMode = false;
  activeTab = '0';

  form!: FormGroup;

  mainKeys: { key: string; totalPercent: number }[] = [
    { key: 'applicantCategory', totalPercent: 250 },
    { key: 'education', totalPercent: 200 },
    { key: 'experience', totalPercent: 150 },
    { key: 'training', totalPercent: 150 },
    { key: 'skills', totalPercent: 150 },
    { key: 'languages', totalPercent: 100 },
  ];

  /* -------------------- Lifecycle -------------------- */
  ngOnInit(): void {
    this.initForm();
    this.handleMainTotal();
    this.handleExperienceTotal();

    const paramId = this.route.snapshot.paramMap.get('id');
    if (paramId) {
      this.jobId = paramId as GUID;
      this.loadJob();
    }
  }

  /* -------------------- Form Init -------------------- */
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

  /* -------------------- Getters -------------------- */
  get mainFormGroup(): FormGroup {
    return this.form.get('main') as FormGroup;
  }

  get detailsFormGroup(): FormGroup {
    return this.form.get('details') as FormGroup;
  }

  /* -------------------- Load Data -------------------- */
  private loadJob(): void {
    if (!this.jobId || this.jobId === GuidUtils.emptyGuid) return;

    this.jobService.getById(this.jobId).subscribe((job) => {
      this.job = job;

      this.jobPointsService.getJobPoints(this.jobId).subscribe((response: JobPointsResponse) => {
        this.jobPoints = response;

        // Mapping handled ONLY by mapper
        this.mapper.mapResponseToForm(response, this.mainFormGroup, this.detailsFormGroup);

        this.isEditMode = true;
        this.cdr.detectChanges();
      });
    });
  }

  /* -------------------- Totals -------------------- */
  private handleMainTotal(): void {
    this.mainFormGroup.valueChanges.subscribe((main) => {
      const total = this.mainKeys.reduce((sum, k) => sum + (main[k.key] || 0), 0);
      this.mainFormGroup.get('total')?.setValue(total, { emitEvent: false });
    });
  }

  private handleExperienceTotal(): void {
    const exp = this.detailsFormGroup.get('experience') as FormGroup;
    exp.valueChanges.subscribe((v) => {
      const total = (v.pointsPerYear || 0) * (v.maxYears || 0);
      exp.get('total')?.setValue(total, { emitEvent: false });
    });
  }

  /* -------------------- Validation -------------------- */
  canAccessDetails(): boolean {
    const max = this.mainKeys.reduce((s, k) => s + k.totalPercent, 0);
    return this.mainFormGroup.get('total')?.value === max;
  }

  areAllCategoriesValid(): boolean {
    return (
      this.applicantCategorySum() === this.mainFormGroup.get('applicantCategory')?.value &&
      this.educationSum() === this.mainFormGroup.get('education')?.value &&
      this.experienceTotal() === this.mainFormGroup.get('experience')?.value &&
      this.trainingSum() === this.mainFormGroup.get('training')?.value &&
      this.skillsSum() === this.mainFormGroup.get('skills')?.value &&
      this.languagesSum() === this.mainFormGroup.get('languages')?.value
    );
  }

  onDetailsReady(): void {
    if (!this.jobPoints) return;

    this.mapper.mapResponseToForm(this.jobPoints, this.mainFormGroup, this.detailsFormGroup);

    this.isEditMode = true;
    this.cdr.detectChanges();
  }
  /* -------------------- Save -------------------- */
  save(): void {
    if (!this.areAllCategoriesValid()) return;

    const payload = {
      request: {
        Id: this.jobPoints.id,
        jobId: this.jobId,
        ...this.getMainPoints(),
        details: this.getDetails(),
      },
    };

    this.jobPointsService.saveJobPoints(payload).subscribe(() => {
      this.notificationService.success(
        this.translationService.instant('JOB_POINT_POINTS_SAVED_SUCCESSFULLY')
      );
    });
  }

  /* -------------------- Build Request -------------------- */
  private getMainPoints(): Omit<JobPointsResponse, 'id' | 'jobId' | 'details'> {
    const main = this.mainFormGroup.getRawValue();
    return {
      applicantCategory: main.applicantCategory,
      education: main.education,
      experience: main.experience,
      training: main.training,
      certificates: main.certificates ?? 0,
      skills: main.skills,
      languages: main.languages,
      total: main.total,
    };
  }

  private getDetails(): JobPointsDetail[] {
    const d = this.detailsFormGroup.getRawValue();
    const result: JobPointsDetail[] = [];

    this.pushCategory(result, d.applicantCategory, JobPointRuleTypeEnum.ApplicantCategory);
    this.pushCategory(result, d.education, JobPointRuleTypeEnum.Education, true);
    this.pushCategory(result, d.training, JobPointRuleTypeEnum.Training);
    this.pushCategory(result, d.languages, JobPointRuleTypeEnum.Language);
    this.pushCategory(result, d.skills, JobPointRuleTypeEnum.Skill, true);
    this.pushCategory(result, d.certificates, JobPointRuleTypeEnum.Certificate);

    const expTotal = this.experienceTotal();
    if (expTotal > 0) {
      result.push({
        type: JobPointRuleTypeEnum.Experience,
        code: 'experience',
        points: expTotal,
      });
    }

    return result;
  }

  private pushCategory(
    target: JobPointsDetail[],
    source: any,
    type: JobPointRuleTypeEnum,
    withRef = false
  ): void {
    Object.keys(source || {}).forEach((key) => {
      if (source[key] > 0) {
        const ref =
          withRef && type === JobPointRuleTypeEnum.Education
            ? this.job?.degrees?.find((d) => d.degree.backendName === key)?.degree?.id
            : withRef && type === JobPointRuleTypeEnum.Skill
            ? this.job?.skills?.find((s) => s.skill.backendName === key)?.skill?.id
            : undefined;

        target.push({
          type,
          code: key,
          points: source[key],
          referenceId: ref,
        });
      }
    });
  }

  /* -------------------- Helpers -------------------- */
  private experienceTotal(): number {
    const exp = this.detailsFormGroup.get('experience')?.getRawValue();
    return (exp?.pointsPerYear || 0) * (exp?.maxYears || 0);
  }

  private sumCategory(group: FormGroup | null): number {
    if (!group) return 0;
    return Object.keys(group.controls)
      .filter((k) => !['pointsPerYear', 'maxYears', 'total'].includes(k))
      .reduce((s, k) => s + (group.get(k)?.value || 0), 0);
  }

  applicantCategorySum(): number {
    return this.sumCategory(this.detailsFormGroup.get('applicantCategory') as FormGroup);
  }

  educationSum(): number {
    return this.sumCategory(this.detailsFormGroup.get('education') as FormGroup);
  }

  trainingSum(): number {
    return this.sumCategory(this.detailsFormGroup.get('training') as FormGroup);
  }

  skillsSum(): number {
    return this.sumCategory(this.detailsFormGroup.get('skills') as FormGroup);
  }

  languagesSum(): number {
    return this.sumCategory(this.detailsFormGroup.get('languages') as FormGroup);
  }
}
