import { Component, OnInit, inject } from "@angular/core";
import { FormBuilder, Validators } from "@angular/forms";
import { debounceTime, filter } from "rxjs";
import { GUID } from "../../../../../shared/types/guid.type";
import { Job } from "../../../models/job.model";
import { JobLookupService } from "../../../services/job-lookup.service";
import { JobService } from "../../../services/job.service";
import { WizardStepComponent } from "../base/wizard-step.component";
import { JobDegree } from "../../../models/job-degree.model";
import { JobTabReviewNoteResponse } from "../../../models/job-tab-review-note-response";
import { JobTabStatus } from "../../../enums/job-tab-status";

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
    degrees: this.fb.control<JobDegree[]>([], Validators.required),
    qualificationsDescriptionAr: ['', Validators.required],
    qualificationsDescriptionEn: ['',Validators.required]
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
    return this.form.valid && this.form.controls.degrees.value!.length > 0;
  }

  setJobData(job: Job, note: JobTabReviewNoteResponse | null = null): void {
     this.jobData = job;
     this.note = note;
    
    const degrees = job.degrees?.map(degree => ({
      degreeId: degree.degreeId
    })) || [];
    
    this.form.patchValue({
      degrees: degrees,
      qualificationsDescriptionAr: job.qualificationsDescriptionAr || '',
      qualificationsDescriptionEn: job.qualificationsDescriptionEn || ''
    }, { emitEvent: false });

    if (note?.tabStatus !== JobTabStatus.Returned) {
      this.form.disable();
    }
  }

  toggleDegree(degreeId: string, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    const current = this.form.controls.degrees.value || [];

    let updated: JobDegree[];

    if (checked) {
      updated = [...current, { degreeId: degreeId as GUID }];
    } else {
      updated = current.filter(d => d.degreeId !== degreeId as GUID);
    }

    this.form.controls.degrees.setValue(updated);
    this.form.controls.degrees.markAsDirty();
    
    this.updateJobData();
  }

  isDegreeChecked(degreeId: string): boolean {
    const degrees = this.form.controls.degrees.value ?? [];
    return degrees.some(d => d.degreeId === degreeId);
  }

  private updateJobData(): void {
    if (this.form.valid) {
      const { degrees, qualificationsDescriptionAr, qualificationsDescriptionEn } = this.form.value;
      
      this.jobService.updateCurrentJobQualifications(
        degrees || [],
        qualificationsDescriptionAr || '',
        qualificationsDescriptionEn || ''
      );
    }
  }
}