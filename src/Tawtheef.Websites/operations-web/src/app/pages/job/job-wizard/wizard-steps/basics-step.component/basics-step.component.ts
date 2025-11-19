import { Component, inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { Job } from '../../../models/job.model';
import { JobBasics } from '../../../models/job-basics.models';
import { debounceTime, filter } from 'rxjs';
import {GenderEnum} from '../../../enums/gender.enum';
import { JobLookupService } from '../../../services/job-lookup.service';

@Component({
  selector: 'app-basics-step',
  standalone: false,
  templateUrl: './basics-step.component.html',
  styleUrls: ['./basics-step.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class BasicsStepComponent implements WizardStepComponent, OnInit {
  private fb = inject(FormBuilder);
  private jobService = inject(JobService);
  lookupsService = inject(JobLookupService);

  readonly form = this.fb.nonNullable.group({
    requestingDept: ['', Validators.required],
    title: ['', Validators.required],
    jobCategory: ['', Validators.required],
    gender: this.fb.control<string[]>([], Validators.required),
    entity: ['', Validators.required],
    major: ['', Validators.required],
    degree: this.fb.control<string[]>([], Validators.required),
    typeOfWork: ['', Validators.required],
    vacancies: [0, [Validators.required, Validators.min(1)]],
    deadline: this.fb.control<Date | null>(null, Validators.required),
  });

  ngOnInit() {
    this.setJobData(this.jobService.currentJob());
    this.setupFormListeners();
  }


  private setupFormListeners(): void {
    this.form.valueChanges
      .pipe(
        debounceTime(300),
        filter(() => this.form.valid),
      )
      .subscribe((value) => {
        this.updateJobService(value as Partial<JobBasics>);
      });
  }

  private updateJobService(basics: Partial<JobBasics>): void {
    const updatedBasics = {
      ...basics,
      degree: basics.degree || []
    };
    this.jobService.updateCurrentJobBasics(updatedBasics);
  }

  setJobData(job: Job): void {
    if (job.basics) {
      const deadline = job.basics.deadline ? new Date(job.basics.deadline) : null;

      this.form.patchValue({
        requestingDept: job.basics.requestingDept || '',
        title: job.basics.title || '',
        jobCategory: job.basics.jobCategory || '',
        gender: job.basics.gender || '',
        entity: job.basics.entity || '',
        major: job.basics.major || '',
        degree: job.basics.degree || [],
        typeOfWork: job.basics.typeOfWork || '',
        vacancies: job.basics.vacancies || 0,
        deadline: deadline,
      }, { emitEvent: false });
    }
  }

  toggleDegree(degree: string, event: Event): void {
  const checked = (event.target as HTMLInputElement).checked;
  const currentDegrees = this.form.controls.degree.value || [];
  const updatedDegrees = checked
    ? [...currentDegrees, degree]
    : currentDegrees.filter(d => d !== degree);

  this.form.controls.degree.setValue(updatedDegrees);
  this.form.controls.degree.markAsDirty();
}

  onDateSelect(): void {
    this.form.controls.deadline.markAsDirty();
  }

  isValid(): boolean {
    return this.form.valid;
  }

  get selectedDegrees(): string {
    return this.form.controls.degree.value?.join('، ') || '';
  }

  get degreePlaceholder(): string {
    return this.selectedDegrees || 'job_wizard.steps.basics.degree_placeholder';
  }
}
