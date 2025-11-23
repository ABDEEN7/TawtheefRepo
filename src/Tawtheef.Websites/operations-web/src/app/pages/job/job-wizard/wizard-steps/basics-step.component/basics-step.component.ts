import { Component, inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { Job } from '../../../models/job.model';
import { JobBasics } from '../../../models/job-basics.models';
import { debounceTime, filter } from 'rxjs';
import { JobLookupService } from '../../../services/job-lookup.service';
import { ScrollerOptions } from 'primeng/api';
import { GUID } from '../../../../../shared/types/guid.type';

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

  lazyLoading = false
  loadLazyTimeout = 0
  readonly form = this.fb.nonNullable.group({
    requestingDeptId: ['', Validators.required],
    title: ['', Validators.required],
    jobCategoryId: ['', Validators.required],
    genderId: ['', Validators.required],
    entityId: ['', Validators.required],
    majorId: ['', Validators.required],
    degreeIds: this.fb.control<GUID[]>([], Validators.required),
    workTypeId: ['', Validators.required],
    vacancies: [0, [Validators.required, Validators.min(1)]],
    deadline: this.fb.control<Date | null>(null, Validators.required),
  });

   majorOptions: ScrollerOptions = {
    delay: 100,
    showLoader: true,
    lazy: true,
    onLazyLoad: this.loadMajorsLazy.bind(this)
  };

  departmentOptions: ScrollerOptions = {
    delay: 100,
    showLoader: true,
    lazy: true,
    onLazyLoad: this.loadDerpartmentsLazy.bind(this)
  };


  loadMajorsLazy(event: any) {
    this.lazyLoading = true;
    this.loadLazyTimeout = setTimeout(() => {
      const {first, last} = event;
      const items = [...this.lookupsService.majors()];
      for (let i = first; i < last; i++) {
        items[i] = this.lookupsService.majors()[i];
      }
      this.lookupsService.nationalities.set(items);
      this.lazyLoading = false;
    }, Math.random() * 1000 + 250);
  }

  loadDerpartmentsLazy(event: any) {
    this.lazyLoading = true;
    this.loadLazyTimeout = setTimeout(() => {
      const {first, last} = event;
      const items = [...this.lookupsService.majors()];
      for (let i = first; i < last; i++) {
        items[i] = this.lookupsService.majors()[i];
      }
      this.lookupsService.nationalities.set(items);
      this.lazyLoading = false;
    }, Math.random() * 1000 + 250);
  }

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
        this.updateJobService(value as Partial<JobBasics>,value.degreeIds as GUID[]);
      });
  }

  private updateJobService(basics: Partial<JobBasics>,degreeIds:GUID[]): void {
    const updatedBasics = {
      ...basics,
    };
    this.jobService.updateCurrentJobBasics(updatedBasics,degreeIds);
  }

  setJobData(job: Job): void {
    if (job.basics) {
      const deadline = job.basics.deadline ? new Date(job.basics.deadline) : null;

      this.form.patchValue({
        requestingDeptId: job.basics.requestingDeptId || '',
        title: job.basics.title || '',
        jobCategoryId: job.basics.jobCategoryId || '',
        genderId: job.basics.genderId || '',
        entityId: job.basics.entityId || '',
        majorId: job.basics.majorId || '',
        degreeIds: job.degreeIds || [],
        workTypeId: job.basics.workTypeId || '',
        vacancies: job.basics.vacancies || 0,
        deadline: deadline,
      }, { emitEvent: false });
    }
  }

  toggleDegree(degree: GUID, event: Event): void {
  const checked = (event.target as HTMLInputElement).checked;
  const currentDegrees = this.form.controls.degreeIds.value || [];
  const updatedDegrees = checked
    ? [...currentDegrees, degree]
    : currentDegrees.filter(d => d !== degree);

  this.form.controls.degreeIds.setValue(updatedDegrees);
  this.form.controls.degreeIds.markAsDirty();
}

  onDateSelect(): void {
    this.form.controls.deadline.markAsDirty();
  }

  isValid(): boolean {
    return this.form.valid;
  }

  get selectedDegrees(): string {
    return this.form.controls.degreeIds.value?.join('، ') || '';
  }

  get degreePlaceholder(): string {
    return this.selectedDegrees || 'job_wizard.steps.basics.degree_placeholder';
  }
}
