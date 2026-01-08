import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ComponentRef,
  EventEmitter,
  inject,
  OnDestroy,
  OnInit,
  Type,
  ViewChild,
  ViewContainerRef,
  ViewEncapsulation,
} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {DialogService} from 'primeng/dynamicdialog';
import {TranslateService} from '@ngx-translate/core';
import {EMPTY, map, of, Subject, switchMap, takeUntil} from 'rxjs';
import {JobService} from '../../services/job.service';
import {WizardStepComponent} from '../wizard-steps/base/wizard-step.component';
import {ConditionsStepComponent} from '../wizard-steps/conditions-step.component/conditions-step.component';
import {SkillsStepComponent} from '../wizard-steps/skills-step.component/skills-step.component';
import {GUID} from '../../../../shared/types/guid.type';
import {GuidUtils} from '../../../../core/utils/guid-utils';
import {JobLookupService} from '../../services/job-lookup.service';
import {QualificationsStepComponent} from '../wizard-steps/qualifications-step.component/qualifications-step.component';
import {
  ResponsibilitiesStepComponent
} from '../wizard-steps/responsibilities-step.component/responsibilities-step.component';
import {OverviewStepComponent} from '../wizard-steps/overview-step.component.ts/overview-step.component';
import {BenefitsStepComponent} from '../wizard-steps/benefits-step.component/benefits-step.component';
import {AttachmentStepComponent} from '../wizard-steps/attachment-step.component/attachment-step.component';
import {ReviewStepComponent} from '../wizard-steps/review-step.component/review-step.component';
import {JobBasicModalComponent} from '../../modals/basics-step-modal/job-basic-modal.component';
import {routes} from '../../../../routes/routes';
import {JobTabType} from '../../enums/job-tab-type';
import {NotificationService} from '../../../../core/services/notification.service';
import {JobReviewResponse} from '../../models/job-review-response';
import {FileUtilsService} from '../../../../core/utils/file-utils';
import {JobStatus} from '../../../../core/enums/lookups.enum';
import {DialogHelperService} from '../../../../core/services/dialog-helper.service';
import {JobCopyTemplate} from '../../models/job-copy-template.model';

@Component({
  selector: 'app-wizard',
  standalone: false,
  templateUrl: './wizard.component.html',
  styleUrls: ['./wizard.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class JobWizardComponent implements AfterViewInit, OnInit, OnDestroy {
  private cdr = inject(ChangeDetectorRef);
  protected jobService = inject(JobService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private notificationService = inject(NotificationService);
  private dialogService = inject(DialogService);
  private translateService = inject(TranslateService);
  lookupsService = inject(JobLookupService);
  protected fileUtils = inject(FileUtilsService);
  private dialogHelperService = inject(DialogHelperService);
  private destroy$ = new Subject<void>();
  private retryCount = 0;
  private maxRetries = 20;

  step = 1;
  isEditMode = false;
  jobId: GUID = GuidUtils.emptyGuid;
  isLoading = false;
  hasBasicData = false;
  stepsLoaded = false;
  showContainer = false;
  copySourceId: GUID | null = null;

  stepClasses: Type<WizardStepComponent>[] = [
    OverviewStepComponent,
    QualificationsStepComponent,
    ResponsibilitiesStepComponent,
    ConditionsStepComponent,
    SkillsStepComponent,
    AttachmentStepComponent,
    BenefitsStepComponent,
    ReviewStepComponent,
  ];

  private tabToStepIndex: Partial<Record<JobTabType, number>> = {
    [JobTabType.Overview]: 0,
    [JobTabType.Qualifications]: 1,
    [JobTabType.Responsibilities]: 2,
    [JobTabType.Conditions]: 3,
    [JobTabType.Skills]: 4,
    [JobTabType.Attachments]: 5,
    [JobTabType.Benefits]: 6,
  };

  @ViewChild('stepsContainer', { read: ViewContainerRef, static: false })
  private container!: ViewContainerRef;

  private stepRefs: ComponentRef<WizardStepComponent>[] = [];

  get total() {
    return this.stepClasses.length;
  }

  get progress() {
    return Math.round((this.step / this.total) * 100);
  }

  get showWizardContent(): boolean {
    return this.hasBasicData;
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    const copyFrom = this.route.snapshot.queryParamMap.get('copyFrom');
    if (id) {
      this.isEditMode = true;
      this.jobId = GuidUtils.asGuid(id);
    }

    this.lookupsService.loadAll();

     if (!this.isEditMode && copyFrom) {
      this.copySourceId = GuidUtils.asGuid(copyFrom);
      this.openCopyBasicDataPopup(this.copySourceId);
      return;
    }

    if (!this.isEditMode) {
      this.jobService.createNewDraft();

      setTimeout(() => {
        this.openBasicDataPopup(true);
      }, 300);
    }
  }

  ngAfterViewInit(): void {
    if (this.isEditMode && this.jobId) {
      this.openBasicDataPopup(false, this.jobId);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.cleanupSteps();
  }

  private openBasicDataPopup(isCreateMode: boolean, jobId?: GUID): void {
    this.dialogService.open(JobBasicModalComponent, {
      width: 'min(920px, 96vw)',
      modal: true,
      header: this.translateService.instant('JOB_BASIC_MODAL.TITLE'),
      styleClass: 'custom-bootstrap-dialog',
      data: {
        isCreateMode: isCreateMode,
        showInWizard: true,
        jobId
      },
    })?.onClose.subscribe((result) => {
      if (result?.success && result?.jobId) {
        this.hasBasicData = true;
        this.jobId = result.jobId;
        this.isEditMode = true;
        this.loadJobForWizard();
        return;
      }
      if (isCreateMode) {
        this.router.navigate([routes.employee.JobList]);
        return;
      }

      if (jobId) {
        this.hasBasicData = true;
        this.loadJobForWizard();
        return;
      }

      this.router.navigate([routes.employee.JobList]);
    });
  }

   private openCopyBasicDataPopup(sourceJobId: GUID): void {
    this.isLoading = true;
    this.jobService.getCopyTemplate(sourceJobId).subscribe({
      next: (template: JobCopyTemplate) => {
        this.isLoading = false;
        this.dialogService.open(JobBasicModalComponent, {
          width: 'min(920px, 96vw)',
          modal: true,
          header: this.translateService.instant('JOB_BASIC_MODAL.TITLE'),
          styleClass: 'custom-bootstrap-dialog',
          data: {
            isCreateMode: true,
            showInWizard: true,
            copyTemplate: template,
            copySourceId: sourceJobId,
          },
        })?.onClose.subscribe((result) => {
          if (result?.success && result?.jobId) {
            this.hasBasicData = true;
            this.jobId = result.jobId;
            this.isEditMode = true;
            this.loadJobForWizard();
            return;
          }
          this.router.navigate([routes.employee.JobList]);
        });
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate([routes.employee.JobList]);
      },
    });
  }

  private loadJobForWizard(): void {
    this.isLoading = true;
    this.showContainer = false;
    this.stepsLoaded = false;

    this.jobService
      .loadJobForEdit(this.jobId)
      .pipe(
        takeUntil(this.destroy$),
        switchMap(() => {
          const job = this.jobService.getCurrentJob();
          if (!job) {
            this.notificationService.error(
              this.translateService.instant('JOB_WIZARD.ERRORS.LOAD_JOB_FAILED')
            );
            this.router.navigate([routes.employee.JobList]);
            return EMPTY;
          }

          if (job.jobStatus?.backendName === JobStatus.NeedUpdate) {
            return this.jobService
              .getLatestReview(this.jobId)
              .pipe(map((review: JobReviewResponse) => ({ job, review })));
          } else {
            return of({ job, review: null });
          }
        })
      )
      .subscribe({
        next: ({ job, review }) => {
          if (review) {
            job.tabReviewNotes = review.tabNoteReviews;
            job.ReviewAttachment = review.reviewAttachment;
          }

          if (job.majorId) {
            this.lookupsService.loadSkillsByMajor(job.majorId);
          }

          this.hasBasicData = true;
          this.showContainer = true;
          this.isLoading = false;
          this.cdr.detectChanges();
          this.initializeSteps();
        },
        error: () => {
          this.isLoading = false;
          this.router.navigate([routes.employee.JobList]);
        },
      });
  }

  private initializeSteps(): void {
    if (!this.container) {
      this.retryCount++;

      if (this.retryCount >= this.maxRetries) {
        this.showErrorMessage('JOB_WIZARD.ERRORS.LOAD_STEPS_FAILED');
        return;
      }
      return;
    }

    try {
      this.loadSteps();
    } catch (error) {
      this.showErrorMessage('JOB_WIZARD.ERRORS.LOAD_STEPS_FAILED');
    }
  }

  private loadSteps(): void {
    this.cleanupSteps();
    try {
      const currentJob = this.jobService.getCurrentJob();

      this.stepRefs = this.stepClasses.map((stepClass, index) => {
        const componentRef = this.container.createComponent(stepClass);

        const stepTab = this.getTabByStepIndex(index);
        const stepNotes = stepTab
          ? currentJob?.tabReviewNotes?.find((note) => note.tab === stepTab) ?? null
          : null;

        if (componentRef.instance.setJobData && currentJob) {
          componentRef.instance.setJobData(currentJob, stepNotes);
        }

         if (this.hasEditStep(componentRef.instance)) {
        componentRef.instance.editStep.subscribe((stepNumber: number) => {
        this.goTo(stepNumber);
        });
        }

        const element = componentRef.location.nativeElement as HTMLElement;
        element.style.display = 'none';

        return componentRef;
      });

      this.stepsLoaded = true;

      this.showActive();
      this.cdr.detectChanges();
    } catch (error) {
      this.showErrorMessage('JOB_WIZARD.ERRORS.LOAD_STEPS_FAILED');
      throw error;
    }
  }

  private cleanupSteps(): void {
    if (this.stepRefs.length > 0) {
      this.stepRefs.forEach((ref) => ref.destroy());
      this.stepRefs = [];
    }

    if (this.container) {
      this.container.clear();
    }
  }

  private showActive(): void {
    if (!this.stepRefs.length) {
      return;
    }

    this.stepRefs.forEach((ref, index) => {
      const element = ref.location.nativeElement as HTMLElement;
      const isActive = index === this.step - 1;
      element.style.display = isActive ? 'block' : 'none';
    });

    this.cdr.detectChanges();
  }

  next(): void {
    const currentStep = this.stepRefs[this.step - 1]?.instance;

    if (currentStep && currentStep.isValid()) {
      this.saveDraft();
      if (this.step < this.total) {
        this.step++;
        this.showActive();
        this.scrollToActive();
      }
    } else {
      this.showWarnMessage('JOB_WIZARD.WARNINGS.COMPLETE_REQUIRED_FIELDS');
    }
  }

  prev(): void {
    if (this.step > 1) {
      this.saveDraft();
      this.step--;
      this.showActive();
      this.scrollToActive();
    }
  }

  goTo(stepNumber: number): void {
    if (stepNumber < 1 || stepNumber > this.total) {
      return;
    }

    if (stepNumber > this.step) {
      let canProceed = true;
      for (let i = this.step - 1; i < stepNumber - 1; i++) {
        if (!this.stepRefs[i]?.instance.isValid()) {
          canProceed = false;
          break;
        }
      }

      if (!canProceed) {
        this.showWarnMessage('JOB_WIZARD.WARNINGS.COMPLETE_PREVIOUS_STEPS');
        return;
      }
      this.saveDraft();
    }

    this.step = stepNumber;
    this.showActive();
    this.scrollToActive();
  }

  isCurrentStepValid(): boolean {
    const current = this.stepRefs[this.step - 1]?.instance;
    return current ? current.isValid() : false;
  }

  private scrollToActive(): void {
    setTimeout(() => {
      const el = document.querySelector('.step-label.active');
      el?.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }, 100);
  }

  canGoToReview(): boolean {
    for (let i = 0; i < this.stepRefs.length - 1; i++) {
      if (!this.stepRefs[i]?.instance.isValid()) {
        return false;
      }
    }
    return true;
  }

  submitForApproval(): void {
    if (!this.jobId) {
      this.showErrorMessage('JOB_WIZARD.ERRORS.NO_JOB_ID');
      return;
    }

    const currentJob = this.jobService.getCurrentJob();
    if (!currentJob) {
      this.showErrorMessage('JOB_WIZARD.ERRORS.NO_JOB_TO_SUBMIT');
      return;
    }

    const validation = this.jobService.validateRequiredFields(currentJob);
    if (!validation.isValid) {
      const errorMessage = validation.errors
        .map((errorKey) => this.translateService.instant(errorKey))
        .join('\n');
      this.notificationService.error(errorMessage);
      return;
    }

    if (!this.canGoToReview()) {
      this.showErrorMessage('JOB_WIZARD.ERRORS.COMPLETE_ALL_STEPS');
      return;
    }

    const pendingStatusId = this.lookupsService.getStatusIdByEnum(JobStatus.PendingApproval);
    if (!pendingStatusId) {
      this.showErrorMessage('JOB_WIZARD.ERRORS.STATUS_NOT_FOUND');
      return;
    }

    this.isLoading = true;
    this.jobService
      .update(this.jobId)
      .pipe(switchMap(() => this.jobService.changeStatus(this.jobId, pendingStatusId)))
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.showSuccessMessage('JOB_WIZARD.SUCCESS.JOB_SUBMITTED');
          this.router.navigate([routes.employee.JobList]);
        },
        error: () => {
          this.isLoading = false;
        },
      });
  }

  cancelWizard(): void {
  const ref = this.dialogHelperService.openConfirmDialog({
    type: 'submit',
    title: 'JOB_WIZARD.CONFIRMATIONS.CANCEL_WIZARD_TITLE',
    description: 'JOB_WIZARD.CONFIRMATIONS.CANCEL_WIZARD',
    cancelText: 'common.cancel',
    confirmText: 'common.confirm',
  });

  ref?.onClose.subscribe((result) => {
    if (!result) return;
    this.router.navigate([routes.employee.JobList]);
  });
}

private saveDraft(): void {
    if (!this.jobId) {
      return;
    }

    this.jobService
      .update(this.jobId)
      .pipe(takeUntil(this.destroy$))
      .subscribe();
  }
  private getTabByStepIndex(stepIndex: number): JobTabType | undefined {
    return (Object.keys(this.tabToStepIndex) as JobTabType[]).find(
      (tab) => this.tabToStepIndex[tab] === stepIndex
    );
  }

  private showSuccessMessage(key: string, detail?: string): void {
    const summary = this.translateService.instant(key);
    const detailText = detail ? this.translateService.instant(detail) : '';

    this.notificationService.success(summary + (detailText ? ': ' + detailText : ''));
  }

  private showErrorMessage(key: string, detail?: string): void {
    const summary = this.translateService.instant(key);
    const detailText = detail ? this.translateService.instant(detail) : '';

    this.notificationService.error(summary + (detailText ? ': ' + detailText : ''));
  }

  private showWarnMessage(key: string, detail?: string): void {
    const summary = this.translateService.instant(key);
    const detailText = detail ? this.translateService.instant(detail) : '';

    this.notificationService.warn(summary + (detailText ? ': ' + detailText : ''));
  }

  getHeaderTitle(): string {
    return this.isEditMode ? 'JOB_WIZARD.TITLES.EDIT_JOB' : 'JOB_WIZARD.TITLES.CREATE_JOB';
  }

  getHeaderSubtitle(): string {
    return this.isEditMode
      ? 'JOB_WIZARD.SUBTITLES.EDIT_DETAILS'
      : 'JOB_WIZARD.SUBTITLES.ENTER_DETAILS';
  }

  getSaveButtonText(): string {
    return this.step === this.total
      ? 'JOB_WIZARD.BUTTONS.SUBMIT_FOR_APPROVAL'
      : 'JOB_WIZARD.BUTTONS.NEXT';
  }

  preview(file: any): void {
    this.fileUtils.previewUrl(file.url).then(() => {});
  }

  download(file: any): void {
    this.fileUtils.downloadUrl(file.url, file.fileName).then(() => {});
  }

  fileIcon(name?: string): string {
    return name ? this.fileUtils.getFileIconClass(name) : '';
  }

  private hasEditStep(instance: any): instance is { editStep: EventEmitter<number> } {
  return instance && 'editStep' in instance && instance.editStep instanceof EventEmitter;
}
}
