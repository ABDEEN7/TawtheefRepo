import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormArray, Validators, FormControl } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { MessageService } from 'primeng/api';
import { Job } from '../../../models/job.model';
import { JobResponsibility } from '../../../models/job-responsibility.model';
import { debounceTime, filter, Subject, takeUntil } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-responsibilities-step',
  standalone: false,
  templateUrl: './responsibilities-step.component.html',
  styleUrls: ['./responsibilities-step.component.scss'],
})
export class ResponsibilitiesStepComponent implements WizardStepComponent, OnInit, OnDestroy {
  // Services
  private readonly fb = inject(FormBuilder);
  private readonly jobService = inject(JobService);
  private readonly messageService = inject(MessageService);
  private readonly translationService = inject(TranslateService);
  
  // Form
  readonly form = this.fb.group({
    responsibilities: this.fb.array<FormControl<string>>([], [Validators.required])
  });
  
  // State
  newResponsibilityText = '';
  private readonly destroy$ = new Subject<void>();

  // Getters
  get responsibilitiesArray(): FormArray<FormControl<string>> {
    return this.form.controls.responsibilities;
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
      this.saveResponsibilitiesToService();
    });
  }

  /**
   * Populate form with job responsibilities
   */
  setJobData(job: Job): void {
    this.responsibilitiesArray.clear();
    
    if (job.responsibilities?.length) {
      job.responsibilities.forEach(responsibility => {
        this.addResponsibilityToForm(responsibility.text);
      });
    }
    
    this.form.updateValueAndValidity();
  }

  /**
   * Add new responsibility from input field
   */
  addResponsibility(): void {
    const textValue = this.newResponsibilityText.trim();
    
    if (!textValue) {
      this.showValidationWarning();
      return;
    }
    
    this.addResponsibilityToForm(textValue);
    this.clearInput();
  }

  /**
   * Remove responsibility at index
   */
  removeResponsibility(index: number): void {
    this.responsibilitiesArray.removeAt(index);
  }

  /**
   * Check if form is valid
   */
  isValid(): boolean {
    return this.form.valid && this.responsibilitiesArray.length > 0;
  }

  // Private helper methods
  private addResponsibilityToForm(text: string): void {
    const control = this.fb.nonNullable.control(text, Validators.required);
    this.responsibilitiesArray.push(control);
  }

  private saveResponsibilitiesToService(): void {
    const responsibilities: JobResponsibility[] = this.responsibilitiesArray.controls.map(control => ({
      text: control.value
    }));
    
    this.jobService.updateCurrentJobResponsibilities(responsibilities);
  }

  private showValidationWarning(): void {
    this.messageService.add({
      severity: 'warn',
      summary: this.translationService.instant('JOB_WIZARD.VALIDATION.WARNING'),
      detail: this.translationService.instant('JOB_WIZARD.VALIDATION.ADD_RESPONSIBILITY_DETAIL'),
      life: 2500,
    });
  }

  private clearInput(): void {
    this.newResponsibilityText = '';
  }
}