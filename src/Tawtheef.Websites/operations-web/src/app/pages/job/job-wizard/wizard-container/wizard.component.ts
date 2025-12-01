import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ComponentRef,
  inject,
  OnInit,
  Type,
  ViewChild,
  ViewContainerRef, ViewEncapsulation,
} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {JobService} from '../../services/job.service';
import {NotificationService} from '../../../../core/services/notification.service';
import {WizardStepComponent} from '../wizard-steps/base/wizard-step.component';
import {BasicsStepComponent} from '../wizard-steps/basics-step.component/basics-step.component';
import {QuotasStepComponent} from '../wizard-steps/quotas-step.component/quotas-step.component';
import {ConditionsStepComponent} from '../wizard-steps/conditions-step.component/conditions-step.component';
import {SkillsStepComponent} from '../wizard-steps/skills-step.component/skills-step.component';
import {DescriptionStepComponent} from '../wizard-steps/description-step.component/description-step.component';
import {ReviewStepComponent} from '../wizard-steps/review-step.component/review-step.component';
import {Job} from '../../models/job.model';
import { GUID } from '../../../../shared/types/guid.type';
import { GuidUtils } from '../../../../core/utils/guid-utils';
import { JobLookupService } from '../../services/job-lookup.service';

@Component({
  selector: 'app-wizard',
  standalone: false,
  templateUrl: './wizard.component.html',
  styleUrls: ['./wizard.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class JobWizardComponent implements AfterViewInit, OnInit {
  private cdr = inject(ChangeDetectorRef);
  private jobService = inject(JobService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private notificationService = inject(NotificationService);
  lookupsService = inject(JobLookupService);
  step = 1;
  isEditMode = false;
  jobId: GUID = GuidUtils.emptyGuid;

  stepClasses: Type<WizardStepComponent>[] = [
    BasicsStepComponent,
    QuotasStepComponent,
    ConditionsStepComponent,
    SkillsStepComponent,
    DescriptionStepComponent,
    ReviewStepComponent,
  ];

  @ViewChild('stepsContainer', {read: ViewContainerRef, static: true})
  private container!: ViewContainerRef;

  private stepRefs: ComponentRef<WizardStepComponent>[] = [];

  get total() {
    return this.stepClasses.length;
  }

  get progress() {
    return Math.round((this.step / this.total) * 100);
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.jobId = GuidUtils.asGuid(id);
    }
    this.lookupsService.loadAll();
  }

  ngAfterViewInit(): void {
    if (this.isEditMode && this.jobId) {
      this.loadJobForEdit();
    } else {
      this.loadSteps();
      this.showActive();
      this.cdr.detectChanges();
    }
  }

  private loadJobForEdit(): void {
    this.jobService.loadJob(this.jobId!).subscribe({
      next: () => {
        this.loadSteps();
        this.showActive();
        this.cdr.detectChanges();
      },
      error: () => {
        this.loadSteps();
        this.showActive();
        this.cdr.detectChanges();
      },
    });
  }

  private loadSteps(): void {
    this.container.clear();
    this.stepRefs = this.stepClasses.map((cls) => {
      const ref = this.container.createComponent(cls);

      if (ref.instance.setJobData) {
        ref.instance.setJobData(this.jobService.newJob());
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
    const current = this.stepRefs[this.step - 1]?.instance;
    if (current && current.isValid() && this.step < this.total) {
      this.step++;
      this.showActive();
      this.scrollToActive();
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
    const current = this.stepRefs[this.step - 1]?.instance;
    if (n > this.step && current && !current.isValid()) return;
    this.step = n;
    this.showActive();
    this.scrollToActive();
  }

  isCurrentStepValid(): boolean {
    const current = this.stepRefs[this.step - 1]?.instance;
    return current?.isValid();
  }

  private scrollToActive(): void {
    setTimeout(() => {
      const el = document.querySelector('.step.active');
      el?.scrollIntoView({behavior: 'smooth', block: 'center'});
    }, 100);
  }

  saveJob(): void {
    const currentJob = this.jobService.newJob();
    const jobToSave: Job = {
      ...currentJob,
      status: this.lookupsService.jobStatus().find(jobStatus => jobStatus.backendName == "open")?.id || '',
      ...(this.isEditMode && this.jobId && {id: this.jobId}),
    };

    this.jobService.saveJob(jobToSave).subscribe({
      next: () => {
        this.notificationService.success("savedSuccessfully")
        this.router.navigate(['/jobs']).then();
      },
      error: (err) => {
        this.notificationService.error(err)
      },
    });
  }

  getHeaderTitle(): string {
    return this.isEditMode ? 'job_wizard.edit_job' : 'job_wizard.create_job';
  }

  getHeaderSubtitle(): string {
    return this.isEditMode ? 'job_wizard.edit_details' : 'job_wizard.enter_details';
  }

  getSaveButtonText(): string {
    return this.isEditMode
      ? 'job_wizard.shared.buttons.edit_job'
      : 'job_wizard.shared.buttons.publish_job';
  }
}
