import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ComponentRef,
  inject,
  OnDestroy,
  OnInit,
  Type,
  ViewChild,
  ViewContainerRef,
  ViewEncapsulation,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DialogService } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';
import { Subject, takeUntil } from 'rxjs';
import { JobService } from '../../services/job.service';
import { WizardStepComponent } from '../wizard-steps/base/wizard-step.component';
import { ConditionsStepComponent } from '../wizard-steps/conditions-step.component/conditions-step.component';
import { SkillsStepComponent } from '../wizard-steps/skills-step.component/skills-step.component';
import { GUID } from '../../../../shared/types/guid.type';
import { GuidUtils } from '../../../../core/utils/guid-utils';
import { JobLookupService } from '../../services/job-lookup.service';
import { QualificationsStepComponent } from '../wizard-steps/qualifications-step.component/qualifications-step.component';
import { ResponsibilitiesStepComponent } from '../wizard-steps/responsibilities-step.component/responsibilities-step.component';
import { OverviewStepComponent } from '../wizard-steps/overview-step.component.ts/overview-step.component';
import { BenefitsStepComponent } from '../wizard-steps/benefits-step.component/benefits-step.component';
import { AttachmentStepComponent } from '../wizard-steps/attachment-step.component/attachment-step.component';
import { ReviewStepComponent } from '../wizard-steps/review-step.component/review-step.component';
import { JobBasicModalComponent } from '../../modals/basics-step-modal/job-basic-modal.component';

@Component({
  selector: 'app-wizard',
  standalone: false,
  templateUrl: './wizard.component.html',
  styleUrls: ['./wizard.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [MessageService]
})
export class JobWizardComponent implements AfterViewInit, OnInit, OnDestroy {
  private cdr = inject(ChangeDetectorRef);
  private jobService = inject(JobService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private messageService = inject(MessageService);
  private dialogService = inject(DialogService);
  private translateService = inject(TranslateService);
  lookupsService = inject(JobLookupService);
  
  private destroy$ = new Subject<void>();
  private retryCount = 0;
  private maxRetries = 20;
  
  step = 1;
  isEditMode = false;
  jobId: GUID = GuidUtils.emptyGuid;
  isLoading = false;
  hasBasicData = false;
  stepsLoaded = false;
  showContainer = false; // New flag to control container visibility

  stepClasses: Type<WizardStepComponent>[] = [
    OverviewStepComponent,
    QualificationsStepComponent,
    ResponsibilitiesStepComponent,
    ConditionsStepComponent,
    SkillsStepComponent,
    AttachmentStepComponent,
    BenefitsStepComponent,
    ReviewStepComponent 
  ];

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
    if (id) {
      this.isEditMode = true;
      this.jobId = GuidUtils.asGuid(id);
    }
    
    this.lookupsService.loadAll();
    
    if (!this.isEditMode) {
      this.jobService.createNewDraft();
      
      setTimeout(() => {
        this.openBasicDataPopup();
      }, 300);
    }
  }

  ngAfterViewInit(): void {
    if (this.isEditMode && this.jobId) {
      this.loadJobForWizard();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.cleanupSteps();
  }

  private openBasicDataPopup(): void {
    const ref = this.dialogService.open(JobBasicModalComponent, {
      width: '950px',
      modal: true,
      header:this.translateService.instant("JOB_BASIC_MODAL.TITLE"),
      styleClass: 'custom-bootstrap-dialog',
      closable: true,
      closeOnEscape: false,
      data: {
        isCreateMode: true,
        showInWizard: true
      }
    });

    ref?.onClose
      .pipe(takeUntil(this.destroy$))
      .subscribe((result) => {
        if (result?.success && result?.jobId) {
          this.hasBasicData = true;
          this.jobId = result.jobId;
          this.isEditMode = true;
          
          this.loadJobForWizard();
        } else {
          this.router.navigate(['/jobs']);
        }
      });
  }

  private loadJobForWizard(): void {
    this.isLoading = true;
    this.showContainer = false;
    this.stepsLoaded = false;
    this.retryCount = 0;
        
    this.jobService.loadJobForEdit(this.jobId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          const job = this.jobService.getCurrentJob();
          
          if (job?.majorId) {
            this.lookupsService.loadSkillsByMajor(job.majorId);
          }
          
          this.hasBasicData = true;
          this.showContainer = true; 
          this.isLoading = false;
          
          this.cdr.detectChanges();
          this.initializeSteps();
        },
        error: (error) => {
          this.showErrorMessage('JOB_WIZARD.ERRORS.LOAD_JOB_FAILED');
          this.isLoading = false;
          this.router.navigate(['/jobs']);
        }
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
        
        if (componentRef.instance.setJobData && currentJob) {
          componentRef.instance.setJobData(currentJob);
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
      this.stepRefs.forEach(ref => ref.destroy());
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

    if (!this.canGoToReview()) {
      this.showErrorMessage('JOB_WIZARD.ERRORS.COMPLETE_ALL_STEPS');
      return;
    }

    this.isLoading = true;
    this.jobService.update(this.jobId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.showSuccessMessage('JOB_WIZARD.SUCCESS.JOB_SUBMITTED');
          this.router.navigate(['/jobs']);
        },
        error: (err) => {
          this.isLoading = false;
          const errorMessage = err.message || 'JOB_WIZARD.ERRORS.SUBMIT_JOB_FAILED';
          this.showErrorMessage(errorMessage);
        }
      });
  }

  cancelWizard(): void {
    const confirmMessage = this.translateService.instant('JOB_WIZARD.CONFIRMATIONS.CANCEL_WIZARD');
    if (confirm(confirmMessage)) {
      this.router.navigate(['/jobs']);
    }
  }

  private showSuccessMessage(key: string, detail?: string): void {
    const summary = this.translateService.instant(key);
    const detailText = detail ? this.translateService.instant(detail) : '';
    
    this.messageService.add({
      severity: 'success',
      summary: summary,
      detail: detailText,
      life: 5000
    });
  }

  private showErrorMessage(key: string, detail?: string): void {
    const summary = this.translateService.instant(key);
    const detailText = detail ? this.translateService.instant(detail) : '';
    
    this.messageService.add({
      severity: 'error',
      summary: summary,
      detail: detailText,
      life: 7000
    });
  }

  private showWarnMessage(key: string, detail?: string): void {
    const summary = this.translateService.instant(key);
    const detailText = detail ? this.translateService.instant(detail) : '';
    
    this.messageService.add({
      severity: 'warn',
      summary: summary,
      detail: detailText,
      life: 5000
    });
  }

  getHeaderTitle(): string {
    return this.isEditMode ? 'JOB_WIZARD.TITLES.EDIT_JOB' : 'JOB_WIZARD.TITLES.CREATE_JOB';
  }

  getHeaderSubtitle(): string {
    return this.isEditMode ? 'JOB_WIZARD.SUBTITLES.EDIT_DETAILS' : 'JOB_WIZARD.SUBTITLES.ENTER_DETAILS';
  }

  getSaveButtonText(): string {
    return this.step === this.total 
      ? 'JOB_WIZARD.BUTTONS.SUBMIT_FOR_APPROVAL' 
      : 'JOB_WIZARD.BUTTONS.NEXT';
  }
}