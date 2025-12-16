import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormArray, Validators, FormGroup } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { Job } from '../../../models/job.model';
import { debounceTime, filter, Subject, takeUntil } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { JobTabReviewNoteResponse } from '../../../models/job-tab-review-note-response';
import { JobTabStatus } from '../../../enums/job-tab-status';
import { NotificationService } from '../../../../../core/services/notification.service';

@Component({
  selector: 'app-conditions-step',
  standalone: false,
  templateUrl: './conditions-step.component.html',
  styleUrl : './conditions-step.component.scss',
})
export class ConditionsStepComponent extends WizardStepComponent implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  protected readonly jobService = inject(JobService);
  private readonly notificationService = inject(NotificationService);
  private readonly transaltionService = inject(TranslateService);
  jobData!: Job;
  
  readonly form = this.fb.group({
    conditions: this.fb.array([])
  }) as FormGroup;
  
  newConditionAr = '';
  newConditionEn = '';
  private readonly destroy$ = new Subject<void>();
  note: JobTabReviewNoteResponse | null = null;

  ngOnInit(): void {
    this.form.valueChanges.pipe(
      debounceTime(300),
      filter(() => this.form.valid),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.updateJobData();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get conditionsArray(): FormArray {
    return this.form.get('conditions') as FormArray;
  }

  getConditionGroup(index: number): FormGroup {
    return this.conditionsArray.at(index) as FormGroup;
  }

 setJobData(job: Job, note: JobTabReviewNoteResponse | null = null): void {
    this.jobData = job;
    this.note = note;
    this.conditionsArray.clear();
    
    if (job.conditions?.length) {
      job.conditions.forEach(condition => {
        this.addConditionToForm(condition.textAr, condition.textEn);
      });
    }

    if (note?.tabStatus !== JobTabStatus.Returned) {
      this.form.disable();  }

  }

  addCondition(): void {
  const textAr = this.newConditionAr.trim();
  const textEn = this.newConditionEn.trim();

    if (!textAr || !textEn) {
    this.notificationService.error(this.transaltionService.instant('JOB_WIZARD.STEPS.NO_ENTERED_DATA_ERROR'));  
    return; 
  }

  const isDuplicate = this.conditionsArray.controls.some(control => {
    const group = control as FormGroup;
    return group.get('textAr')?.value === textAr && group.get('textEn')?.value === textEn;
  });
  
  if (isDuplicate) {
    this.notificationService.error(this.transaltionService.instant('JOB_WIZARD.STEPS.DUPLICATE_ENTRY_ERROR'));
    return; 
  }
    
    this.addConditionToForm(textAr, textEn);
    this.newConditionAr = '';
    this.newConditionEn = '';
  }

  removeCondition(index: number): void {
    if (this.conditionsArray.length > 1) {
      this.conditionsArray.removeAt(index);
    }
  }

  isValid(): boolean {
    if(this.form.disabled){
      return true;  
    }
    return this.form.valid && this.conditionsArray.length > 0;
  }

  private addConditionToForm(textAr: string, textEn: string = ''): void {
    const conditionGroup = this.fb.group({
      textAr: [textAr, [Validators.required, Validators.maxLength(500)]],
      textEn: [textEn, [Validators.maxLength(500)]]
    });
    
    this.conditionsArray.push(conditionGroup);
  }

  private updateJobData(): void {
    if (this.form.valid) {
      const conditions = this.conditionsArray.controls.map(control => {
        const group = control as FormGroup;
        return {
          textAr: group.get('textAr')?.value || '',
          textEn: group.get('textEn')?.value || ''
        };
      });
      
      this.jobService.updateCurrentJobConditions(conditions);
    }
  }
}