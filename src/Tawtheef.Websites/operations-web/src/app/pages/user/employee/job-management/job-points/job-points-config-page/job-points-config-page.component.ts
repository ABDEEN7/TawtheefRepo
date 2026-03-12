import { ChangeDetectorRef, Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { GUID } from '../../../../../../shared/types/guid.type';
import { GuidUtils } from '../../../../../../core/utils/guid-utils';
import { JobService } from '../../services/job.service';
import { JobPointsConfigService } from '../../services/job-points-config.service';
import { JobPointsMapperService } from '../../services/job-points-mapper.service';
import { JobResponse } from '../../models/job-response-model';
import { JobPointsResponse } from '../../models/job-points-response';
import { JobPointsCalculationService } from '../../services/job-points-calculation.service';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { TranslateService } from '@ngx-translate/core';
import { JobPointsDetail } from '../../models/job-points-details.model';
import { JobPointRuleTypeEnum } from '../../enums/job-point-rule-type';
import { Subject, takeUntil } from 'rxjs';
import { DialogHelperService } from '../../../../../../core/services/dialog-helper.service';
import { JobStatus } from '../../../../../../core/enums/lookups.enum';
import { JobLookupService } from '../../services/job-lookup.service';
import { routes } from '../../../../../../routes/routes';
import { AuthService } from '../../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../../core/constants/permissions';

@Component({
  selector: 'app-job-points-config-page',
  standalone: false,
  templateUrl: './job-points-config-page.component.html',
  styleUrls: ['./job-points-config-page.component.scss'],
})
export class JobPointsConfigPageComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private pointsCalculationService = inject(JobPointsCalculationService);
  private cdr = inject(ChangeDetectorRef);
  private jobService = inject(JobService);
  private jobPointsService = inject(JobPointsConfigService);
  private mapper = inject(JobPointsMapperService);
  private notificationService = inject(NotificationService);
  private translationService = inject(TranslateService);
  private dialogHelperService = inject(DialogHelperService);
  private lookupsService = inject(JobLookupService);
  private router = inject(Router);
  private authService = inject(AuthService);

  private destroy$ = new Subject<void>();

  jobId: GUID = GuidUtils.emptyGuid;
  job: JobResponse | null = null;
  jobPoints: JobPointsResponse | null = null;
  mainKeys: { key: string; initialValue: number }[] = [];

  systemMaxPoints: number = 0;
  isFinalApprovalAvailable: boolean = false;
  isLoading: boolean = false;
  isReadOnlyMode = false;

  form!: FormGroup;
  isEditMode = false;
  activeTab = '0';

  ngOnInit(): void {
    this.initForm();
    this.isReadOnlyMode = this.route.snapshot.queryParamMap.get('mode') === 'view';
    this.handleMainTotal();
    this.handleExperienceTotal();

    const paramId = this.route.snapshot.paramMap.get('id');
    if (paramId) {
      this.jobId = paramId as GUID;
      this.loadJob();
    }
    this.lookupsService.loadJobStatus().subscribe();
    this.applyReadOnlyMode();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initForm(): void {
    this.form = this.fb.group({
      main: this.fb.group({
        applicantCategory: [0, [Validators.min(0)]],
        education: [0, [Validators.min(0)]],
        experience: [0, [Validators.min(0)]],
        training: [0, [Validators.min(0)]],
        skills: [0, [Validators.min(0)]],
        languages: [0, [Validators.min(0)]],
        certificates: [0, [Validators.min(0)]],
        total: [{ value: 0, disabled: true }],
      }),

      details: this.fb.group({
        applicantCategory: this.fb.group({}),
        education: this.fb.group({}),
        skills: this.fb.group({}),

        experience: this.fb.group({
          pointsPerYear: [0],
          maxYears: [0],
          total: [{ value: 0, disabled: true }],
        }),

        training: this.fb.group({
          highLinked: [0],
          mediumLinked: [0],
          lowLinked: [0],
        }),

        certificates: this.fb.group({
          certificatesLinked: [0],
          certificatesNotLinked: [0],
          prize: [0],
        }),

        languages: this.fb.group({
          speaking: this.fb.group({
            max: [0],
            excellent: [0],
            veryGood: [0],
            good: [0],
          }),
          reading: this.fb.group({
            max: [0],
            excellent: [0],
            veryGood: [0],
            good: [0],
          }),
          conversation: this.fb.group({
            max: [0],
            excellent: [0],
            veryGood: [0],
            good: [0],
          }),

          native: [0],
        }),
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

    this.isLoading = true;
    this.jobService
      .getById(this.jobId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (job) => {
          this.job = job;
          this.loadJobPointsConfig();
        },
      });
  }

  private loadJobPointsConfig(): void {
    this.jobPointsService
      .getJobPointsConfiguration()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (config) => {
          this.systemMaxPoints = config.maxPoints;
          this.mainKeys = [
            { key: 'applicantCategory', initialValue: config.applicantCategoryMaxPoints },
            { key: 'education', initialValue: config.educationMaxPoints },
            { key: 'experience', initialValue: config.experienceMaxPoints },
            { key: 'training', initialValue: config.trainingMaxPoints },
            { key: 'skills', initialValue: config.skillsMaxPoints },
            { key: 'languages', initialValue: config.languagesMaxPoints },
            { key: 'certificates', initialValue: config.certificatesMaxPoints },
          ];

          if (this.job?.jobPoints?.id) {
            this.loadJobPoints();
          } else {
            this.mainFormGroup.patchValue(
              {
                applicantCategory: config.applicantCategoryMaxPoints,
                education: config.educationMaxPoints,
                experience: config.experienceMaxPoints,
                training: config.trainingMaxPoints,
                skills: config.skillsMaxPoints,
                languages: config.languagesMaxPoints,
                certificates: config.certificatesMaxPoints,
              },
              { emitEvent: true }
            );
            this.isLoading = false;
            this.applyReadOnlyMode();
            this.cdr.detectChanges();
          }
        },
      });
  }

  private loadJobPoints(): void {
    this.jobPointsService
      .getJobPoints(this.jobId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.jobPoints = response;
          this.mapper.mapResponseToForm(response, this.mainFormGroup, this.detailsFormGroup);
          this.isEditMode = true;
          this.isFinalApprovalAvailable = !response.isApproved && this.areAllCategoriesValid();
          this.isLoading = false;
          this.applyReadOnlyMode();
          this.cdr.detectChanges();
        },
      });
  }

  private handleMainTotal(): void {
    this.mainFormGroup.valueChanges.pipe(takeUntil(this.destroy$)).subscribe((main) => {
      const total = this.mainKeys.reduce((sum, k) => sum + (main[k.key] || 0), 0);
      this.mainFormGroup.get('total')?.setValue(total, { emitEvent: false });

      this.updateFinalApprovalAvailability();
    });
  }

  private handleExperienceTotal(): void {
    const exp = this.detailsFormGroup.get('experience') as FormGroup;
    exp.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(() => {
      const total = this.pointsCalculationService.experienceTotal(exp);
      exp.get('total')?.setValue(total, { emitEvent: false });
      this.updateFinalApprovalAvailability();
    });
  }

  private updateFinalApprovalAvailability(): void {
    const mainTotal = this.mainFormGroup.get('total')?.value || 0;
    const isMainValid = mainTotal === this.systemMaxPoints;
    const areDetailsValid = this.areAllCategoriesValid();

    this.isFinalApprovalAvailable =
      isMainValid && areDetailsValid && (!this.jobPoints || !this.jobPoints.isApproved);
  }

  canAccessDetails(): boolean {
    const mainTotal = this.mainFormGroup.get('total')?.value || 0;
    return mainTotal === this.systemMaxPoints;
  }

  areAllCategoriesValid(): boolean {
    return this.pointsCalculationService.areAllCategoriesValid(
      this.mainFormGroup,
      this.detailsFormGroup,
      this.pointsCalculationService.sections
    );
  }

  save(): void {
    if (!this.canManagePoints()) return;
    if (!this.areAllCategoriesValid()) {
      this.notificationService.warn(
        this.translationService.instant('JOB_POINTS.VALIDATION.DETAILS_EXCEED_MAIN')
      );
      return;
    }

    const mainTotal = this.mainFormGroup.get('total')?.value || 0;
    if (mainTotal !== this.systemMaxPoints) {
      this.notificationService.error(
        this.translationService.instant('JOB_POINTS.VALIDATION.TOTAL_MUST_EQUAL_MAX', {
          required: this.systemMaxPoints,
        })
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
        isApproved: false,
      },
    };

    this.jobPointsService
      .saveJobPoints(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (responseId) => {
          if (!isUpdate) {
            this.jobPoints = {
              id: responseId,
              jobId: this.jobId,
              ...this.getMainPoints(),
              details: [],
              isApproved: false,
            };
          } else {
            this.jobPoints!.isApproved = false;
          }

          this.notificationService.success(
            this.translationService.instant('JOB_POINTS.SAVE.SUCCESS')
          );
          this.updateFinalApprovalAvailability();
          this.cdr.detectChanges();
        },
      });
  }

  approvePoints(): void {
    if (!this.canApprovePoints()) return;
    if (!this.isFinalApprovalAvailable) {
      this.notificationService.warn(
        this.translationService.instant('JOB_POINTS.VALIDATION.CANNOT_APPROVE')
      );
      return;
    }

    const ref = this.dialogHelperService.openConfirmDialog({
      type: 'submit',
      title: 'JOB_POINTS.APPROVE.CONFIRMATION_TITLE',
      description: 'JOB_POINTS.APPROVE.CONFIRMATION_DESCRIPTION',
      cancelText: 'common.cancel',
      confirmText: 'common.confirm',
    });

    ref?.onClose.pipe(takeUntil(this.destroy$)).subscribe((result) => {
      if (!result) return;

      this.isLoading = true;

      this.jobPointsService
        .approveJobPoints(this.jobId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (resp) => {
            if (!resp) {
              this.isLoading = false;
              this.cdr.detectChanges();
              return;
            }

            if (this.jobPoints) {
              this.jobPoints.isApproved = true;
            }

            const statusId = this.lookupsService.getStatusIdByEnum(JobStatus.ReadyForAnnouncement);

            if (!statusId) {
              this.isLoading = false;
              this.cdr.detectChanges();
              return;
            }

            this.jobService
              .changeStatus(this.jobId, statusId)
              .pipe(takeUntil(this.destroy$))
              .subscribe({
                next: () => {
                  this.notificationService.success(
                    this.translationService.instant('JOB_POINTS.APPROVE.SUCCESS')
                  );

                  this.isFinalApprovalAvailable = false;
                  this.isLoading = false;

                  this.router.navigate([routes.portal.JobList]);
                  this.cdr.detectChanges();
                },
                error: () => {
                  this.isLoading = false;
                  this.cdr.detectChanges();
                },
              });
          },
          error: () => {
            this.isLoading = false;
            this.applyReadOnlyMode();
            this.cdr.detectChanges();
          },
        });
    });
  }

  private getMainPoints(): Omit<JobPointsResponse, 'id' | 'jobId' | 'details' | 'isApproved'> {
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

    this.pushCategory(result, d.applicantCategory, JobPointRuleTypeEnum.ApplicantCategory, false, undefined, true);
    this.pushCategory(result, d.training, JobPointRuleTypeEnum.Training, false, undefined, true);
    this.pushCategory(result, d.skills, JobPointRuleTypeEnum.Skill, true, undefined, true);
    this.pushCategory(result, d.education, JobPointRuleTypeEnum.Education, true, undefined, true);
    this.pushCategory(result, d.certificates, JobPointRuleTypeEnum.Certificate, false, undefined, true);
    this.pushCategory(result, d.experience, JobPointRuleTypeEnum.Experience, false, undefined, true);
    this.pushCategory(result, d.languages, JobPointRuleTypeEnum.Language, false, undefined, true);

    return result;
  }

  private pushCategory(
    target: JobPointsDetail[],
    source: any,
    type: JobPointRuleTypeEnum,
    withRef = false,
    parentKey?: string,
    includeZero: boolean = true
  ): void {
    Object.keys(source || {}).forEach((key) => {
      const value = source[key];
      const code = parentKey ? `${parentKey}.${key}` : key;

      if (typeof value === 'number') {
        if (value === null || value === undefined) return;
        if (!includeZero && value <= 0) return;
        if (includeZero && value < 0) return;

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
        return;
      }
      if (typeof value === 'object' && value !== null) {
        this.pushCategory(target, value, type, withRef, code, includeZero);
      }
    });
  }


  private applyReadOnlyMode(): void {
    if (this.isReadOnlyMode) {
      this.form.disable({ emitEvent: false });
      return;
    }

    this.form.enable({ emitEvent: false });
    this.mainFormGroup.get('total')?.disable({ emitEvent: false });
    this.detailsFormGroup.get('experience.total')?.disable({ emitEvent: false });
  }

  goToDetails(): void {
    if (this.canAccessDetails()) {
      this.activeTab = '1';
    }
  }

  goBack(): void {
    this.activeTab = '0';
  }

  canApprovePoints(): boolean {
    if (this.isReadOnlyMode) return false;
    return this.authService.hasPermission(Permissions.JobPoints.Approve);
  }

  canManagePoints(): boolean {
    if (this.isReadOnlyMode) return false;
    return this.authService.hasPermission(Permissions.JobPoints.Manage);
  }
}
