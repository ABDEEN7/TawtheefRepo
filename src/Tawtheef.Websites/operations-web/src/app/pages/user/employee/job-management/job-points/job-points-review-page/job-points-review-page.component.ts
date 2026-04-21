import { ChangeDetectorRef, Component, inject, OnDestroy, OnInit, computed } from '@angular/core';
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
import { Subject, takeUntil } from 'rxjs';
import { DialogHelperService } from '../../../../../../core/services/dialog-helper.service';
import { JobStatus } from '../../../../../../core/enums/lookups.enum';
import { JobLookupService } from '../../services/job-lookup.service';
import { routes } from '../../../../../../routes/routes';
import { AuthService } from '../../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../../core/constants/permissions';
import { LanguageService } from '../../../../../../core/services/language.service';

@Component({
  selector: 'app-job-points-review-page',
  standalone: false,
  templateUrl:  './job-points-review-page.component.html',
  styleUrls: ['./job-points-review-page.component.scss'],
})
export class JobPointsReviewPageComponent implements OnInit, OnDestroy {
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
  private languageService = inject(LanguageService);

  isRtl = computed(() => this.languageService.get() === 'ar');

  private destroy$ = new Subject<void>();

  jobId: GUID = GuidUtils.emptyGuid;
  job: JobResponse | null = null;
  jobPoints: JobPointsResponse | null = null;
  mainKeys: { key: string; initialValue: number }[] = [];

  systemMaxPoints: number = 0;
  isFinalApprovalAvailable: boolean = false;
  isInitialLoading: boolean = false;
  isSubmitting: boolean = false;
  isReadOnlyMode = true; // Always read-only in review mode
  emptyKeys: string[] = [];

  form!: FormGroup;
  activeTab = '0';

  ngOnInit(): void {
    this.initForm();
    const paramId = this.route.snapshot.paramMap.get('id');
    if (paramId) {
      this.jobId = paramId as GUID;
      this.loadJob();
    }
    this.lookupsService.loadJobStatus().subscribe();
    this.form.disable({ emitEvent: false });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
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
          speaking: this.fb.group({ max: [0], excellent: [0], veryGood: [0], good: [0] }),
          reading: this.fb.group({ max: [0], excellent: [0], veryGood: [0], good: [0] }),
          conversation: this.fb.group({ max: [0], excellent: [0], veryGood: [0], good: [0] }),
          native: [0],
        }),
      }),
    });
  }

  get mainFormGroup(): FormGroup { return this.form.get('main') as FormGroup; }
  get detailsFormGroup(): FormGroup { return this.form.get('details') as FormGroup; }

  private loadJob(): void {
    if (!this.jobId || this.jobId === GuidUtils.emptyGuid) return;
    this.isInitialLoading = true;
    this.jobService.getById(this.jobId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (job) => {
        this.job = job;
        this.emptyKeys = [];
        if (!job.degrees?.length) this.emptyKeys.push('education');
        if (!job.skills?.length) this.emptyKeys.push('skills');
        this.loadJobPointsConfig();
      },
    });
  }

  private loadJobPointsConfig(): void {
    this.jobPointsService.getJobPointsConfiguration().pipe(takeUntil(this.destroy$)).subscribe({
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
        this.loadJobPoints();
      },
    });
  }

  private loadJobPoints(): void {
    this.jobPointsService.getJobPoints(this.jobId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (response) => {
        this.jobPoints = response;
        this.mapper.mapResponseToForm(response, this.mainFormGroup, this.detailsFormGroup);
        this.updateFinalApprovalAvailability();
        this.isInitialLoading = false;
        this.form.disable({ emitEvent: false });
        this.cdr.detectChanges();
      },
    });
  }

  private updateFinalApprovalAvailability(): void {
    const mainTotal = this.mainFormGroup.get('total')?.value || 0;
    const isMainValid = mainTotal === this.systemMaxPoints;
    const areDetailsValid = this.pointsCalculationService.areAllCategoriesValid(this.mainFormGroup, this.detailsFormGroup, this.pointsCalculationService.sections);
    this.isFinalApprovalAvailable = isMainValid && areDetailsValid && (!this.jobPoints || !this.jobPoints.isApproved);
  }

  canAccessDetails(): boolean {
    return (this.mainFormGroup.get('total')?.value || 0) === this.systemMaxPoints;
  }

  approvePoints(): void {
    if (!this.canApprovePoints()) return;
    const ref = this.dialogHelperService.openConfirmDialog({
      type: 'submit',
      title: 'JOB_POINTS.APPROVE.CONFIRMATION_TITLE',
      description: 'JOB_POINTS.APPROVE.CONFIRMATION_DESCRIPTION',
      cancelText: 'common.cancel',
      confirmText: 'common.confirm',
    });

    ref?.onClose.pipe(takeUntil(this.destroy$)).subscribe((result) => {
      if (!result) return;
      this.isSubmitting = true;
      this.jobPointsService.approveJobPoints(this.jobId).pipe(takeUntil(this.destroy$)).subscribe({
        next: (resp) => {
          if (!resp) { this.isSubmitting = false; this.cdr.detectChanges(); return; }
          if (this.jobPoints) this.jobPoints.isApproved = true;
          const statusId = this.lookupsService.getStatusIdByEnum(JobStatus.ReadyForAnnouncement);
          if (!statusId) {
            this.notificationService.success(this.translationService.instant('JOB_POINTS.APPROVE.SUCCESS'));
            this.isSubmitting = false;
            this.router.navigate([routes.portal.JobList]);
            return;
          }
          this.jobService.changeStatus(this.jobId, statusId).pipe(takeUntil(this.destroy$)).subscribe({
            next: () => {
              this.notificationService.success(this.translationService.instant('JOB_POINTS.APPROVE.SUCCESS'));
              this.router.navigate([routes.portal.JobList]);
            },
            error: () => { this.isSubmitting = false; this.cdr.detectChanges(); }
          });
        },
        error: () => { this.isSubmitting = false; this.cdr.detectChanges(); }
      });
    });
  }

  rejectPoints(): void {
    if (!this.canApprovePoints()) return;
    const ref = this.dialogHelperService.openConfirmDialog({
      type: 'warning',
      title: 'JOB_POINTS.REJECT.CONFIRMATION_TITLE',
      description: 'JOB_POINTS.REJECT.CONFIRMATION_DESCRIPTION',
      cancelText: 'common.cancel',
      confirmText: 'common.confirm',
      showInputField: true,
      inputType: 'textarea',
      inputLabel: 'JOB_POINTS.REJECT.REASON_LABEL',
      inputPlaceholder: 'JOB_POINTS.REJECT.REASON_PLACEHOLDER'
    });

    ref?.onClose.pipe(takeUntil(this.destroy$)).subscribe((result) => {
      if (!result) return;
      
      const reason = typeof result === 'string' ? result : '';
      if (reason.trim() === '') {
        return;
      }

      this.isSubmitting = true;
      this.jobPointsService.rejectJobPoints(this.jobId, reason).pipe(takeUntil(this.destroy$)).subscribe({
        next: (resp) => {
          if (!resp) { this.isSubmitting = false; this.cdr.detectChanges(); return; }
          this.notificationService.success(this.translationService.instant('JOB_POINTS.REJECT.SUCCESS'));
          this.router.navigate([routes.portal.JobList]);
        },
        error: () => { this.isSubmitting = false; this.cdr.detectChanges(); }
      });
    });
  }

  goToDetails(): void { if (this.canAccessDetails()) this.activeTab = '1'; }
  goBack(): void { this.activeTab = '0'; }
  onClose() { this.router.navigate([routes.portal.JobList]); }
  canApprovePoints(): boolean { return this.authService.hasPermission(Permissions.JobPoints.Approve); }
}
