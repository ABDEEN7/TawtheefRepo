import { Component, inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
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
})
export class BasicsStepComponent implements WizardStepComponent, OnInit {
  private fb = inject(FormBuilder);
  private jobService = inject(JobService);
  lookupsService = inject(JobLookupService);

  lazyLoading = false;
  loadLazyTimeout = 0;
  
  readonly form = this.fb.nonNullable.group({
    sectorId: ['', Validators.required],          
    managementId: ['', Validators.required],         
    requestingDepartmentId: ['', Validators.required], 
    jobCategoryId: ['', Validators.required],      
    workTypeId: ['', Validators.required],         
    title: ['', Validators.required],               
    vacancies: [0, [Validators.required, Validators.min(1)]],
    deadline: this.fb.control<Date | null>(null, Validators.required), 
    majorId: ['', Validators.required],             
    subMajorId: [''],                              
    genderId: [''],                               
    workLocationId: ['', Validators.required],     
    minimumExperienceYears: [0, [Validators.min(0)]], 
    minimumAge: [0, [Validators.min(0), Validators.max(100)]], 
    maximumAge: [0, [Validators.min(0), Validators.max(100)]],
  }, { validators: [this.ageRangeValidator.bind(this)] });

  private ageRangeValidator(control: AbstractControl): ValidationErrors | null {
    const minimumAge = control.get('minimumAge')?.value;
    const maximumAge = control.get('maximumAge')?.value;
    
    if (minimumAge != null && maximumAge != null) {
      if (minimumAge > maximumAge) {
        control.get('minimumAge')?.setErrors({ 'minGreaterThanMax': true });
        return { 'minGreaterThanMax': true };
      }
      
      if (control.get('minimumAge')?.hasError('minGreaterThanMax')) {
        control.get('minimumAge')?.setErrors(null);
      }
    }
    return null;
  }

  majorOptions: ScrollerOptions = {
    delay: 100,
    showLoader: true,
    lazy: true,
    onLazyLoad: this.loadMajorsLazy.bind(this)
  };

  subMajorOptions: ScrollerOptions = {
    delay: 100,
    showLoader: true,
    lazy: true,
    onLazyLoad: this.loadSubMajorsLazy.bind(this)
  };

  departmentOptions: ScrollerOptions = {
    delay: 100,
    showLoader: true,
    lazy: true,
    onLazyLoad: this.loadDepartmentsLazy.bind(this)
  };

  loadMajorsLazy(event: any) {
    this.lazyLoading = true;
    this.loadLazyTimeout = setTimeout(() => {
      const {first, last} = event;
      const items = [...this.lookupsService.majors()];
      for (let i = first; i < last; i++) {
        items[i] = this.lookupsService.majors()[i];
      }
      this.lookupsService.majors.set(items);
      this.lazyLoading = false;
    }, Math.random() * 1000 + 250);
  }

  loadSubMajorsLazy(event: any) {
    this.lazyLoading = true;
    this.loadLazyTimeout = setTimeout(() => {
      const {first, last} = event;
      const items = [...this.lookupsService.subMajors()];
      for (let i = first; i < last; i++) {
        items[i] = this.lookupsService.subMajors()[i];
      }
      this.lookupsService.subMajors.set(items);
      this.lazyLoading = false;
    }, Math.random() * 1000 + 250);
  }

  loadDepartmentsLazy(event: any) {
    this.lazyLoading = true;
    this.loadLazyTimeout = setTimeout(() => {
      const {first, last} = event;
      const items = [...this.lookupsService.departments()];
      for (let i = first; i < last; i++) {
        items[i] = this.lookupsService.departments()[i];
      }
      this.lookupsService.departments.set(items);
      this.lazyLoading = false;
    }, Math.random() * 1000 + 250);
  }

  ngOnInit() {
    this.setJobData(this.jobService.newJob());
    this.setupFormListeners();
    
    this.form.controls.majorId.valueChanges.subscribe(majorId => {
      if (majorId) {
        this.lookupsService.loadSubMajorsByMajor(majorId);
        this.form.controls.subMajorId.setValue('');
      }
    });
  }

  private setupFormListeners(): void {
    this.form.valueChanges
      .pipe(
        debounceTime(300),
        filter(() => this.form.valid),
      )
      .subscribe((value) => {
        this.updateJobService(value as JobBasics);
      });
  }

  private updateJobService(value: JobBasics): void {
    const updatedJobBasics: JobBasics = {
      title: value.title || '',
      vacancies: value.vacancies || 0,
      deadline: value.deadline || null,
      sectorId: value.sectorId || '' as GUID,
      managementId: value.managementId || '' as GUID,
      requestingDepartmentId: value.requestingDepartmentId || '' as GUID,
      jobCategoryId: value.jobCategoryId || '' as GUID,
      workTypeId: value.workTypeId || '' as GUID,
      majorId: value.majorId || '' as GUID,
      degreeIds: value.degreeIds || [],
      workLocationId: value.workLocationId || '' as GUID,
      ...(value.subMajorId && { subMajorId: value.subMajorId }),
      ...(value.genderId && { genderId: value.genderId }),
      ...(value.minimumExperienceYears !== undefined && { minimumExperienceYears: value.minimumExperienceYears }),
      ...(value.minimumAge !== undefined && { minimumAge: value.minimumAge }),
      ...(value.maximumAge !== undefined && { maximumAge: value.maximumAge }),
    };
    
    this.jobService.updateCurrentJobBasics(updatedJobBasics);
  }

  setJobData(job: Job): void {
    if (job) {
      const deadline = job.deadline ? new Date(job.deadline) : null;

      this.form.patchValue({
        sectorId: job.sectorId || '',
        managementId: job.managementId || '',
        requestingDepartmentId: job.requestingDepartmentId || '',
        jobCategoryId: job.jobCategoryId || '',
        workTypeId: job.workTypeId || '',
        title: job.title || '',
        vacancies: job.vacancies || 0,
        deadline: deadline,
        majorId: job.majorId || '',
        subMajorId: job.subMajorId || '',
        genderId: job.genderId || '',
        workLocationId: job.workLocationId || '',
        minimumExperienceYears: job.minimumExperienceYears || 0,
        minimumAge: job.minimumAge || 0,
        maximumAge: job.maximumAge || 0,
      }, { emitEvent: false });
      
      if (job.majorId) {
        this.lookupsService.loadSubMajorsByMajor(job.majorId);
      }
    }
  }


  onDateSelect(): void {
    this.form.controls.deadline.markAsDirty();
  }

  isValid(): boolean {
    return true
  }

  get showSubMajorField(): boolean {
    return !!this.form.controls.majorId.value && this.lookupsService.subMajors().length > 0;
  }
}