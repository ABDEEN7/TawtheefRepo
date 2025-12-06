import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormArray, Validators, FormGroup } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { Job } from '../../../models/job.model';
import { debounceTime, filter, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-conditions-step',
  standalone: false,
  templateUrl: './conditions-step.component.html'
})
export class ConditionsStepComponent extends WizardStepComponent implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  protected readonly jobService = inject(JobService);
  
  jobData!: Job;
  
  readonly form = this.fb.group({
    conditions: this.fb.array([])
  }) as FormGroup;
  
  newConditionAr = '';
  newConditionEn = '';
  private readonly destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.form.valueChanges.pipe(
      debounceTime(300),
      filter(() => this.form.valid),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.updateJobData();
    });
    
    const currentJob = this.jobService.getCurrentJob();
    if (currentJob) {
      this.setJobData(currentJob);
    }
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

  setJobData(job: Job): void {
    this.jobData = job;
    this.conditionsArray.clear();
    
    if (job.conditions?.length) {
      job.conditions.forEach(condition => {
        this.addConditionToForm(condition.textAr, condition.textEn);
      });
    }
  }

  addCondition(): void {
    const trimmedAr = this.newConditionAr.trim();
    
    if (!trimmedAr) {
      return;
    }
    
    this.addConditionToForm(trimmedAr, this.newConditionEn.trim());
    this.newConditionAr = '';
    this.newConditionEn = '';
  }

  removeCondition(index: number): void {
    if (this.conditionsArray.length > 1) {
      this.conditionsArray.removeAt(index);
    }
  }

  isValid(): boolean {
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