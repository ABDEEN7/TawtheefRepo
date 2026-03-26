import { Component, OnInit, inject } from "@angular/core";
import { FormBuilder, Validators } from "@angular/forms";
import { debounceTime, filter } from "rxjs";
import { GUID } from "../../../../../../../shared/types/guid.type";
import { Job } from "../../../models/job.model";
import { JobLookupService } from "../../../services/job-lookup.service";
import { JobService } from "../../../services/job.service";
import { WizardStepComponent } from "../base/wizard-step.component";
import { JobDegree } from "../../../models/job-degree.model";
import { JobTabReviewNoteResponse } from "../../../models/job-tab-review-note-response";
import { JobTabStatus } from "../../../enums/job-tab-status";
import { JobStatus } from "../../../../../../../core/enums/lookups.enum";

@Component({
  selector: 'app-qualifications-step',
  standalone : false,
  templateUrl: './qualifications-step.component.html',
  styleUrl: './qualifications-step.component.scss',
})
export class QualificationsStepComponent extends WizardStepComponent implements OnInit {
  private fb = inject(FormBuilder);
  protected jobService = inject(JobService);
  protected lookupsService = inject(JobLookupService);

  jobData!: Job;

  readonly form = this.fb.group({
    qualificationsDescriptionAr: ['', Validators.required],
    qualificationsDescriptionEn: ['', Validators.required]
  });
  note: JobTabReviewNoteResponse | null = null;

  ngOnInit(): void {
    this.form.valueChanges.pipe(
      debounceTime(300),
      filter(() => this.form.valid)
    ).subscribe(_ => {
      this.updateJobData();
    });
  }

  isValid(): boolean {
    if (this.form.disabled) {
      return true;
    }
    return this.form.valid;
  }

  override setJobData(job: Job, note: JobTabReviewNoteResponse | null = null): void {
     this.jobData = job;
     this.note = note;

    const degrees = job.degrees?.map(degree => ({
      degreeId: degree.degreeId
    })) || [];

    this.form.patchValue({
      qualificationsDescriptionAr: job.qualificationsDescriptionAr || '',
      qualificationsDescriptionEn: job.qualificationsDescriptionEn || ''
    }, { emitEvent: false });

    if (note?.tabStatus !== JobTabStatus.Returned && job.jobStatus?.backendName === JobStatus.NeedUpdate) {
      this.form.disable();
    }
  }

  private updateJobData(): void {
    if (this.form.valid) {
      const { qualificationsDescriptionAr, qualificationsDescriptionEn } = this.form.value;

      this.jobService.updateCurrentJobQualifications(
        this.jobData?.degrees || [],
        qualificationsDescriptionAr || '',
        qualificationsDescriptionEn || ''
      );
    }
  }

}
