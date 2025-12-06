import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ComponentRef,
  inject,
  OnInit,
  Type,
  ViewChild,
  ViewContainerRef,
  ViewEncapsulation,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DialogService } from 'primeng/dynamicdialog';
import { JobService } from '../../services/job.service';
import { NotificationService } from '../../../../core/services/notification.service';
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
})
export class JobWizardComponent implements AfterViewInit, OnInit { // REMOVED 'abstract' keyword
  private cdr = inject(ChangeDetectorRef);
  private jobService = inject(JobService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private notificationService = inject(NotificationService);
  private dialogService = inject(DialogService);
  lookupsService = inject(JobLookupService);
  
  step = 1;
  isEditMode = false;
  jobId: GUID = GuidUtils.emptyGuid;
  isLoading = false;
  hasBasicData = false;

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

  @ViewChild('stepsContainer', { read: ViewContainerRef, static: true })
  private container!: ViewContainerRef;

  private stepRefs: ComponentRef<WizardStepComponent>[] = [];

  get total() {
    return this.stepClasses.length;
  }

  get progress() {
    return Math.round((this.step / this.total) * 100);
  }

  get showWizardContent(): boolean {
    return this.isEditMode && this.hasBasicData;
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
      }, 800);
    }
  }

  ngAfterViewInit(): void {
    if (this.isEditMode && this.jobId && !this.hasBasicData) {
      this.loadJobForWizard();
    }
  }

  private openBasicDataPopup(): void {
    const ref = this.dialogService.open(JobBasicModalComponent, {
      width: '950px',
      modal: true,
      styleClass: 'custom-bootstrap-dialog',
      closable: false,
      closeOnEscape: false,
      data: {
        isCreateMode: true,
        showInWizard: true
      }
    });

    ref?.onClose.subscribe((result) => {
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
    this.jobService.loadJobForEdit(this.jobId).subscribe({
      next: () => {
        const job = this.jobService.getCurrentJob();
        if (job?.majorId) {
          this.lookupsService.loadSkillsByMajor(job.majorId);
        }
        
        this.hasBasicData = true;
        this.loadSteps();
        this.showActive();
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.notificationService.error('فشل في تحميل الوظيفة');
        this.isLoading = false;
        this.router.navigate(['/jobs']);
      }
    });
  }

  private loadSteps(): void {
    this.container.clear();
    this.stepRefs = this.stepClasses.map((cls) => {
      const ref = this.container.createComponent(cls);

      if (ref.instance.setJobData) {
        const currentJob = this.jobService.getCurrentJob();
        if (currentJob) {
          ref.instance.setJobData(currentJob);
        }
      }

      (ref.location.nativeElement as HTMLElement).style.display = 'none';
      return ref;
    });
    this.cdr.detectChanges();
  }

  private showActive(): void {
    this.stepRefs.forEach((r, i) => {
      const el = r.location.nativeElement as HTMLElement;
      el.style.display = i === this.step - 1 ? 'block' : 'none';
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
      this.notificationService.warn('يرجى إكمال جميع الحقول الإلزامية في هذه الخطوة');
    }
  }

  prev(): void {
    if (this.step > 1) {
      this.step--;
      this.showActive();
      this.scrollToActive();
    }
  }

  goTo(n: number): void {
    if (n > this.step) {
      let canProceed = true;
      for (let i = this.step; i < n; i++) {
        if (!this.stepRefs[i]?.instance.isValid()) {
          canProceed = false;
          break;
        }
      }
      
      if (!canProceed) {
        this.notificationService.warn('يجب إكمال الخطوات السابقة أولاً');
        return;
      }
    }
    
    this.step = n;
    this.showActive();
    this.scrollToActive();
  }

  isCurrentStepValid(): boolean {
    const current = this.stepRefs[this.step - 1]?.instance;
    return current ? current.isValid() : false;
  }

  private scrollToActive(): void {
    setTimeout(() => {
      const el = document.querySelector('.step.active');
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
      this.notificationService.error('لا يوجد معرف للوظيفة');
      return;
    }

    if (!this.canGoToReview()) {
      this.notificationService.error('يرجى إكمال جميع الخطوات قبل الإرسال');
      return;
    }

    this.isLoading = true;
    this.jobService.submitJobForApproval(this.jobId).subscribe({
      next: () => {
        this.isLoading = false;
        this.notificationService.success('تم إرسال الوظيفة للاعتماد بنجاح');
        this.router.navigate(['/jobs']);
      },
      error: (err) => {
        this.isLoading = false;
        this.notificationService.error(err.message || 'فشل في إرسال الوظيفة للاعتماد');
      }
    });
  }

  cancelWizard(): void {
    if (confirm('هل تريد إلغاء العملية؟ سيتم فقدان جميع التغييرات غير المحفوظة.')) {
      this.router.navigate(['/jobs']);
    }
  }

  getHeaderTitle(): string {
    return this.isEditMode ? 'JOB_WIZARD.EDIT_JOB' : 'JOB_WIZARD.CREATE_JOB';
  }

  getHeaderSubtitle(): string {
    return this.isEditMode ? 'JOB_WIZARD.EDIT_DETAILS' : 'JOB_WIZARD.ENTER_DETAILS';
  }

  getSaveButtonText(): string {
    if (this.step === this.total) {
      return 'JOB_WIZARD.SHARED.BUTTONS.SUBMIT_FOR_APPROVAL';
    } else {
      return 'JOB_WIZARD.SHARED.BUTTONS.NEXT';
    }
  }
}