import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormArray, Validators, FormGroup } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { Job } from '../../../models/job.model';
import { debounceTime, filter, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-responsibilities-step',
  standalone: false,
  templateUrl: './responsibilities-step.component.html',
  styleUrls: ['./responsibilities-step.component.scss'],
})
export class ResponsibilitiesStepComponent extends WizardStepComponent implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  protected readonly jobService = inject(JobService);
  
  jobData!: Job;
  
  readonly form = this.fb.group({
    responsibilities: this.fb.array([])
  }) as FormGroup;
  
  newResponsibilityAr = '';
  newResponsibilityEn = '';
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

  get responsibilitiesArray(): FormArray {
    return this.form.get('responsibilities') as FormArray;
  }

  getResponsibilityGroup(index: number): FormGroup {
    return this.responsibilitiesArray.at(index) as FormGroup;
  }

  setJobData(job: Job): void {
    this.jobData = job;
    this.responsibilitiesArray.clear();
    
    if (job.responsibilities?.length) {
      job.responsibilities.forEach(responsibility => {
        this.addResponsibilityToForm(responsibility.textAr, responsibility.textEn);
      });
    }
  }

  addResponsibility(): void {    
    this.addResponsibilityToForm(this.newResponsibilityAr.trim(), this.newResponsibilityEn.trim());
    this.newResponsibilityAr = '';
    this.newResponsibilityEn = '';
  }

  removeResponsibility(index: number): void {
    if (this.responsibilitiesArray.length > 1) {
      this.responsibilitiesArray.removeAt(index);
    }
  }

  isValid(): boolean {
    return this.form.valid && this.responsibilitiesArray.length > 0;
  }

  private addResponsibilityToForm(textAr: string, textEn: string = ''): void {
    const responsibilityGroup = this.fb.group({
      textAr: [textAr, [Validators.required, Validators.maxLength(500)]],
      textEn: [textEn, [Validators.maxLength(500)]]
    });
    
    this.responsibilitiesArray.push(responsibilityGroup);
  }

  private updateJobData(): void {
    if (this.form.valid) {
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
}