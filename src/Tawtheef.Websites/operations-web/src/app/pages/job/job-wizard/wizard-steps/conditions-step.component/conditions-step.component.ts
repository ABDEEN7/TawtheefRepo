import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormArray, Validators, FormControl } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { MessageService } from 'primeng/api';
import { Job } from '../../../models/job.model';
import { JobCondition } from '../../../models/job-condition.model';
import { debounceTime, filter, Subject, takeUntil } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-conditions-step',
  standalone: false,
  templateUrl: './conditions-step.component.html'
})
export class ConditionsStepComponent implements WizardStepComponent, OnInit, OnDestroy {
  // Services
  private readonly fb = inject(FormBuilder);
  private readonly jobService = inject(JobService);
  private readonly messageService = inject(MessageService);
  private readonly translationService = inject(TranslateService);
  
  // Form
  readonly form = this.fb.group({
    conditions: this.fb.array<FormControl<string>>([], [Validators.required])
  });
  
  // State
  newCondition = '';
  private readonly destroy$ = new Subject<void>();

  // Getters
  get conditionsArray(): FormArray<FormControl<string>> {
    return this.form.controls.conditions;
  }

  ngOnInit(): void {
    this.initializeForm();
    this.setupFormSubscription();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Initialize form with existing job data
   */
  private initializeForm(): void {
    const job = this.jobService.newJob();
    this.setJobData(job);
  }

  /**
   * Setup form value changes subscription with debounce
   */
  private setupFormSubscription(): void {
    this.form.valueChanges.pipe(
      debounceTime(300),
      filter(() => this.form.valid),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.saveConditionsToService();
    });
  }

  /**
   * Populate form with job conditions
   */
  setJobData(job: Job): void {
    this.conditionsArray.clear();
    
    if (job.conditions?.length) {
      job.conditions.forEach(condition => {
        this.addConditionToForm(condition.text);
      });
    }
    
    this.form.updateValueAndValidity();
  }

  /**
   * Add new condition from input
   */
  addCondition(): void {
    const trimmedValue = this.newCondition.trim();
    
    if (!trimmedValue) {
      this.showValidationWarning();
      return;
    }
    
    this.addConditionToForm(trimmedValue);
    this.clearInput();
  }

  /**
   * Remove condition at index
   */
  removeCondition(index: number): void {
    this.conditionsArray.removeAt(index);
  }

  /**
   * Check if form is valid
   */
  isValid(): boolean {
    return this.form.valid;
  }

  // Private helper methods
  private addConditionToForm(text: string): void {
    const control = this.fb.nonNullable.control(text, Validators.required);
    this.conditionsArray.push(control);
  }

  private saveConditionsToService(): void {
    const conditions: JobCondition[] = this.conditionsArray.controls.map(control => ({
      text: control.value
    }));
    
    this.jobService.updateCurrentJobConditions(conditions);
  }

  private showValidationWarning(): void {
    this.messageService.add({
      severity: 'warn',
      summary: this.translationService.instant('JOB_WIZARD.VALIDATION.WARNING'),
      detail: this.translationService.instant('JOB_WIZARD.VALIDATION.ADD_CONDITION_DETAIL'),
      life: 2500,
    });
  }

  private clearInput(): void {
    this.newCondition = '';
  }
}