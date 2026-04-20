import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormArray, Validators, FormGroup } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { Job } from '../../../models/job.model';
import { debounceTime, filter, Subject, takeUntil } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { JobTabReviewNoteResponse } from '../../../models/job-tab-review-note-response';
import { JobTabStatus } from '../../../enums/job-tab-status';
import { NotificationService } from '../../../../../../../core/services/notification.service';
import { JobStatus } from '../../../../../../../core/enums/lookups.enum';

@Component({
  selector: 'app-responsibilities-step',
  standalone: false,
  templateUrl: './responsibilities-step.component.html',
  styleUrls: ['./responsibilities-step.component.scss'],
})
export class ResponsibilitiesStepComponent extends WizardStepComponent implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  protected readonly jobService = inject(JobService);
  private readonly notificationService = inject(NotificationService);
  private readonly transaltionService = inject(TranslateService);
  jobData!: Job;

  readonly form = this.fb.group({
    responsibilities: this.fb.array([])
  }) as FormGroup;

  newResponsibilityAr = '';
  newResponsibilityEn = '';
  private readonly destroy$ = new Subject<void>();
  note: JobTabReviewNoteResponse | null = null;

  ngOnInit(): void {
    this.form.valueChanges.pipe(
      debounceTime(300),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.updateJobData();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get responsibilitiesArray(): FormArray {
    return this.form.get('responsibilities') as FormArray;
  }

  getResponsibilityGroup(index: number): FormGroup {
    return this.responsibilitiesArray.at(index) as FormGroup;
  }

  override setJobData(job: Job, note: JobTabReviewNoteResponse | null = null): void {
    this.jobData = job;
    this.note = note;
    this.responsibilitiesArray.clear();

    if (job.responsibilities?.length) {
      job.responsibilities.forEach(responsibility => {
        this.addResponsibilityToForm(responsibility.textAr, responsibility.textEn);
      });
    }

    if (note?.tabStatus !== JobTabStatus.Returned && job.jobStatus?.backendName === JobStatus.NeedUpdate) {
      this.form.disable();
    }
  }

addResponsibility(): void {
  const textAr = this.newResponsibilityAr.trim();
  const textEn = this.newResponsibilityEn.trim();

  if (!textAr || !textEn) {
    this.notificationService.error(this.transaltionService.instant('JOB_WIZARD.STEPS.NO_ENTERED_DATA_ERROR'));
    return;
  }

  const isDuplicate = this.responsibilitiesArray.controls.some(control => {
    const group = control as FormGroup;
    return group.get('textAr')?.value === textAr && group.get('textEn')?.value === textEn;
  });

  if (isDuplicate) {
    this.notificationService.error(this.transaltionService.instant('JOB_WIZARD.STEPS.DUPLICATE_ENTRY_ERROR'));
    return;
  }
  this.addResponsibilityToForm(textAr, textEn);
  this.newResponsibilityAr = '';
  this.newResponsibilityEn = '';
}

  removeResponsibility(index: number): void {
    if (this.responsibilitiesArray.length > 1) {
      this.responsibilitiesArray.removeAt(index);
      this.updateJobData();
    }
  }

  isValid(): boolean {
    if(this.form.disabled){
      return true;
    }
    return this.form.valid && this.responsibilitiesArray.length > 0;
  }

  private addResponsibilityToForm(textAr: string, textEn: string = ''): void {
    const responsibilityGroup = this.fb.group({
      textAr: [textAr, [Validators.required, Validators.maxLength(500)]],
      textEn: [textEn, [Validators.required,Validators.maxLength(500)]]
    });

    this.responsibilitiesArray.push(responsibilityGroup);
  }

  private updateJobData(): void {
    const responsibilities = this.responsibilitiesArray.controls.map(control => {
      const group = control as FormGroup;
      return {
        textAr: group.get('textAr')?.value || '',
        textEn: group.get('textEn')?.value || ''
      };
    });

    this.jobService.updateCurrentJobResponsibilities(responsibilities);
  }
}
