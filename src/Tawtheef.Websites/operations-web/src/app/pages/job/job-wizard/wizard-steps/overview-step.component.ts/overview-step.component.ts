import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { WizardStepComponent } from '../base/wizard-step.component';
import { Job } from '../../../models/job.model';
import { JobLookupService } from '../../../services/job-lookup.service';
import { JobService } from '../../../services/job.service';
import { JobTabReviewNoteResponse } from '../../../models/job-tab-review-note-response';
import { JobTabStatus } from '../../../enums/job-tab-status';
import { JobStatus } from '../../../../../core/enums/lookups.enum';

@Component({
  selector: 'app-overview-step',
  templateUrl: './overview-step.component.html',
  styleUrls: ['./overview-step.component.scss'],
  standalone:false
})
export class OverviewStepComponent extends WizardStepComponent implements OnInit {
  private fb = inject(FormBuilder);
  protected jobService = inject(JobService);
  protected lookupsService = inject(JobLookupService);

  readonly form = this.fb.group({
    overviewAr: ['', Validators.required],
    overviewEn: ['', Validators.required],
  });
  jobData!: Job;
  note: JobTabReviewNoteResponse | null = null;

  ngOnInit(): void {
    this.form.valueChanges.subscribe(() => {
      this.updateJobData();
    });
  }

 override setJobData(job: Job, note: JobTabReviewNoteResponse | null = null): void {
    this.jobData = job;
    this.note = note;
    this.loadData();
    if (note?.tabStatus !== JobTabStatus.Returned && job.jobStatus?.backendName === JobStatus.NeedUpdate) {
    this.form.disable();
  }
  }

  private loadData(): void {
    if (this.jobData) {
      this.form.patchValue({
        overviewAr: this.jobData.overviewAr || '',
        overviewEn: this.jobData.overviewEn || '',
      });
    }
  }

  private updateJobData(): void {
    if (this.form.valid) {
      const { overviewAr, overviewEn } = this.form.value;
      if(overviewAr && overviewEn)
      this.jobService.updateCurrentJobOverview(overviewAr, overviewEn);
    }
  }

  isValid(): boolean {
  return this.form.disabled ? true : this.form.valid;
  }
}
