import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { Job } from '../../../models/job.model';
import { debounceTime, filter } from 'rxjs';
import { JobTabReviewNoteResponse } from '../../../models/job-tab-review-note-response';
import { JobTabStatus } from '../../../enums/job-tab-status';
import { JobStatus } from '../../../../../../../core/enums/lookups.enum';

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
  note: JobTabReviewNoteResponse | null = null;

  readonly form: FormGroup = this.fb.group({
    benefitsAr: ['', [Validators.maxLength(2000)]],
    benefitsEn: ['', [Validators.maxLength(2000)]]
  });

  ngOnInit(): void {
    const currentJob = this.jobService.getCurrentJob();
    if (currentJob) {
      this.setJobData(currentJob);
    }

    this.form.valueChanges.pipe(
      debounceTime(300)
    ).subscribe(() => {
      this.updateJobData();
    });
  }

  isValid(): boolean {
    return this.form.disabled ? true : this.form.valid;
  }

  override setJobData(job: Job, note: JobTabReviewNoteResponse | null = null): void {
    this.jobData = job;
    this.note = note;
    this.form.patchValue({
      benefitsAr: job.benefitsAr || '',
      benefitsEn: job.benefitsEn || ''
    });
    if (note?.tabStatus !== JobTabStatus.Returned && job.jobStatus?.backendName === JobStatus.NeedUpdate) {
      this.form.disable();
    }
  }

  private updateJobData(): void {
    const { benefitsAr, benefitsEn } = this.form.value;
    this.jobService.updateCurrentJobBenefits(
      benefitsAr || '',
      benefitsEn || ''
    );
  }
}
