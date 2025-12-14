import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { Job } from '../../../models/job.model';
import { debounceTime, filter } from 'rxjs';

@Component({
  selector: 'app-benefits-step',
  standalone: false,
  templateUrl: './benefits-step.component.html',
  styleUrls: ['./benefits-step.component.scss']
})
export class BenefitsStepComponent extends WizardStepComponent implements OnInit {
  private fb = inject(FormBuilder);
  protected jobService = inject(JobService);
  
  jobData!: Job;
  
  readonly form: FormGroup = this.fb.group({
    benefitsAr: ['', [Validators.required, Validators.maxLength(2000)]],
    benefitsEn: ['', [Validators.maxLength(2000)]]
  });
  disabled: boolean =false;

  ngOnInit(): void {
    const currentJob = this.jobService.getCurrentJob();
    if (currentJob) {
      this.setJobData(currentJob);
    }
    
    this.form.valueChanges.pipe(
      debounceTime(300),
      filter(() => this.form.valid)
    ).subscribe(() => {
      this.updateJobData();
    });
  }

  isValid(): boolean {
    return this.form.valid;
  }

  setJobData(job: Job,disable: boolean = false): void {
    this.jobData = job;
    
    this.form.patchValue({
      benefitsAr: job.benefitsAr || '',
      benefitsEn: job.benefitsEn || ''
    });

    this.disabled =disable

  }

  private updateJobData(): void {
    if (this.form.valid) {
      const { benefitsAr, benefitsEn } = this.form.value;
      this.jobService.updateCurrentJobBenefits(
        benefitsAr || '',
        benefitsEn || ''
      );
    }
  }
}