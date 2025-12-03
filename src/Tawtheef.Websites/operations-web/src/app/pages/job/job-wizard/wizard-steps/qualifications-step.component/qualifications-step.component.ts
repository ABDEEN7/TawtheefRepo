import { Component, OnInit, inject } from "@angular/core";
import { FormBuilder, Validators } from "@angular/forms";
import { debounceTime, filter } from "rxjs";
import { GUID } from "../../../../../shared/types/guid.type";
import { Job } from "../../../models/job.model";
import { JobLookupService } from "../../../services/job-lookup.service";
import { JobService } from "../../../services/job.service";
import { WizardStepComponent } from "../base/wizard-step.component";
import { JobDegree } from "../../../models/job-degree.model";

@Component({
  selector: 'app-qualifications-step.component',
  standalone : false,
  templateUrl: './qualifications-step.component.html',
  styleUrl: './qualifications-step.component.scss',
})

export class QualificationsStepComponent implements WizardStepComponent, OnInit {
  private fb = inject(FormBuilder);
  private jobService = inject(JobService);
  lookupsService = inject(JobLookupService)

  readonly form = this.fb.group({
    degrees: this.fb.control<JobDegree[]>([], Validators.required),
    qualificationsDescription: ['', Validators.required] 
  });

  ngOnInit(): void {
    this.setJobData(this.jobService.newJob());
    
    this.form.valueChanges.pipe(
      debounceTime(300),
      filter(() => this.form.valid)
    ).subscribe(values => {
      this.jobService.updateCurrentJobQualifications(
        values.degrees || [], 
        values.qualificationsDescription || ''
      );
    });
  }

  isValid() {
    return true;
  }

  setJobData(currentJob: Job) {
  if (currentJob.degrees) {
    this.form.patchValue({
      degrees: currentJob.degrees,
      qualificationsDescription: currentJob.qualificationsDescription || ''
    });
  }
}

  toggleDegree(degreeId: GUID, event: Event): void {
  const checked = (event.target as HTMLInputElement).checked;
  const current = this.form.controls.degrees.value || [];

  let updated: JobDegree[];

  if (checked) {
    updated = [...current, { degreeId }];
  } else {
    updated = current.filter(d => d.degreeId !== degreeId);
  }

  this.form.controls.degrees.setValue(updated);
  this.form.controls.degrees.markAsDirty();
}
isDegreeChecked(degreeId: GUID): boolean {
  const degrees = this.form.controls.degrees.value ?? [];
  return degrees.some(d => d.degreeId === degreeId);
}
}