import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { JobService } from '../../services/job.service';
import { JobLookupService } from '../../services/job-lookup.service';
import { GUID } from '../../../../shared/types/guid.type';
import { NotificationService } from '../../../../core/services/notification.service';
import { TranslateService } from '@ngx-translate/core';
import { Subject, takeUntil } from 'rxjs';
import { Router } from '@angular/router';
import { routes } from '../../../../routes/routes';
import { EndpointsService } from '../../../../core/http/endpoints.service';
import { DialogHelperService } from '../../../../core/services/dialog-helper.service';

@Component({
  selector: 'app-job-basic-modal',
  templateUrl: './job-basic-modal.component.html',
  styleUrls: ['./job-basic-modal.component.scss'],
  standalone:false
})
export class JobBasicModalComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private jobService = inject(JobService);
  private notificationService = inject(NotificationService);
  private translationService = inject(TranslateService);
  private router = inject(Router);
  public ref = inject(DynamicDialogRef);
  public config = inject(DynamicDialogConfig);
  private dialogHelperService = inject(DialogHelperService);
  protected endpoints = inject(EndpointsService);


  lookupsService = inject(JobLookupService);

  private destroy$ = new Subject<void>();
  isLoading = false;
  isEditMode = false;
  isCreateMode = false;
  showInWizard = false;
  jobId: GUID | null = null;
  currentDate: Date | undefined;

  form = this.fb.nonNullable.group({
    sectorId: ['', Validators.required],
    managementId: ['', Validators.required],
    departmentId: ['', Validators.required],
    yearsOfExperience: [0, [Validators.required, Validators.min(0)]],
    titleAr: ['', [Validators.required, Validators.maxLength(200)]],
    titleEn: ['', [Validators.required, Validators.maxLength(200)]],
    jobCategoryId: ['', Validators.required],
    workLocationId: ['', Validators.required],
    genderId: [''],
    majorId: ['', Validators.required],
    subMajorId: [''],
    workTypeId: ['', Validators.required],
    numberOfVacancies: [1, [Validators.required, Validators.min(1)]],
    closingDate: this.fb.control<Date | null>(null, [Validators.required]),
    minimumAge: [18, [Validators.required, Validators.min(18)]],
    maximumAge: [60, [Validators.required, Validators.min(18)]],
  });

  majorOptions: any[] = [];
  subMajorOptions: any[] = [];

  ngOnInit(): void {
    const today = new Date();
    this.currentDate = today;
    this.isCreateMode = this.config.data?.isCreateMode || false;
    this.showInWizard = this.config.data?.showInWizard || false;

    if (this.config.data?.jobId) {
      this.isEditMode = true;
      this.jobId = this.config.data.jobId;
      this.loadJobForEdit();
    } else if (!this.isCreateMode) {
      this.isCreateMode = true;
    }

    this.setupSequenceListeners();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private setupSequenceListeners(): void {
    this.form.controls.sectorId.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(sectorId => {
        if (sectorId) {
          this.lookupsService.loadManagementsBySector(sectorId as GUID);
          this.form.controls.managementId.setValue('');
          this.form.controls.departmentId.setValue('');
          this.lookupsService.resetDepartments();
        } else {
          this.lookupsService.resetManagements();
          this.form.controls.managementId.setValue('');
          this.form.controls.departmentId.setValue('');
        }
      });

    this.form.controls.managementId.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(managementId => {
        if (managementId) {
          this.lookupsService.loadDepartmentsByManagement(managementId as GUID);
          this.form.controls.departmentId.setValue('');
        } else {
          this.lookupsService.resetDepartments();
          this.form.controls.departmentId.setValue('');
        }
      });

    this.form.controls.majorId.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(majorId => {
        if (majorId) {
          this.lookupsService.loadSubMajorsByMajor(majorId as GUID);
          this.form.controls.subMajorId.setValue('');
        } else {
          this.lookupsService.resetSubMajors();
          this.form.controls.subMajorId.setValue('');
        }
      });
  }

  private loadJobForEdit(): void {
    if (!this.jobId) return;

    this.isLoading = true;
    this.jobService.getById(this.jobId).subscribe({
      next: (jobResponse) => {
        const deadline = jobResponse.closingDate ? new Date(jobResponse.closingDate) : null;

        this.form.patchValue({
          sectorId: jobResponse.sector.id || '',
          managementId: jobResponse.management.id || '',
          departmentId: jobResponse.department.id || '',
          yearsOfExperience: jobResponse.yearsOfExperience || 0,
          titleAr: jobResponse.titleAr || '',
          titleEn: jobResponse.titleEn || '',
          jobCategoryId: jobResponse.jobCategory.id || '',
          workLocationId: jobResponse.workLocation.id || '',
          genderId: jobResponse.gender?.id || '',
          majorId: jobResponse.major.id || '',
          subMajorId: jobResponse.subMajor?.id || '',
          workTypeId: jobResponse.workType.id || '',
          numberOfVacancies: jobResponse.numberOfVacancies || 1,
          closingDate: deadline,
          minimumAge: jobResponse.minimumAge || 18,
          maximumAge: jobResponse.maximumAge || 60
        });

        this.majorOptions = jobResponse.major?.id ? [jobResponse.major] : [];
        this.subMajorOptions = jobResponse.subMajor?.id ? [jobResponse.subMajor] : [];

        if (jobResponse.sector.id) {
          this.lookupsService.loadManagementsBySector(jobResponse.sector.id as GUID);
        }

        if (jobResponse.management.id) {
          this.lookupsService.loadDepartmentsByManagement(jobResponse.management.id as GUID);
        }



        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.ref.close({ success: false });
      }
    });
  }

  onDateSelect(): void {
    this.form.controls.closingDate.markAsDirty();
  }

  startJobCreation(): void {
    if (!this.form.valid) {
      this.markAllAsTouched();
      return;
    }

    this.isLoading = true;

    const formValue = this.form.getRawValue();
    const jobData = {
      titleAr: formValue.titleAr,
      titleEn: formValue.titleEn,
      sectorId: formValue.sectorId as GUID,
      managementId: formValue.managementId as GUID,
      departmentId: formValue.departmentId as GUID,
      yearsOfExperience: formValue.yearsOfExperience,
      jobCategoryId: formValue.jobCategoryId as GUID,
      workLocationId: formValue.workLocationId as GUID,
      genderId: formValue.genderId as GUID,
      majorId: formValue.majorId as GUID,
      subMajorId: formValue.subMajorId as GUID,
      workTypeId: formValue.workTypeId as GUID,
      numberOfVacancies: formValue.numberOfVacancies,
      closingDate: this.normalizeDate(formValue.closingDate),
      minimumAge: formValue.minimumAge,
      maximumAge: formValue.maximumAge,
      overviewAr: '',
      overviewEn: '',
      benefitsAr: '',
      benefitsEn: '',
      qualificationsDescriptionAr: '',
      qualificationsDescriptionEn: '',
      degrees: [],
      conditions: [],
      responsibilities: [],
      skills: [],
      requiredAttachments: []
    };


    this.jobService.create(jobData).subscribe({
      next: (jobId) => {
        this.isLoading = false;
        this.notificationService.success(this.translationService.instant('JOB_BASIC_MODAL.SUCCESS.CREATED'));

        if (this.showInWizard) {
          this.ref.close({
            success: true,
            jobId: jobId,
            data: jobData
          });
        } else {
          this.ref.close({ success: true, jobId });
          this.router.navigate([routes.employee.JobList, jobId, 'wizard']);
        }
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  updateJobBasics(): void {
    if (!this.form.valid || !this.jobId) {
      this.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const formValue = this.form.getRawValue();

    const updateData = {
      titleAr: formValue.titleAr,
      titleEn: formValue.titleEn,
      sectorId: formValue.sectorId as GUID,
      managementId: formValue.managementId as GUID,
      departmentId: formValue.departmentId as GUID,
      yearsOfExperience: formValue.yearsOfExperience,
      jobCategoryId: formValue.jobCategoryId as GUID,
      workLocationId: formValue.workLocationId as GUID,
      genderId: formValue.genderId as GUID,
      majorId: formValue.majorId as GUID,
      subMajorId: formValue.subMajorId as GUID,
      workTypeId: formValue.workTypeId as GUID,
      numberOfVacancies: formValue.numberOfVacancies,
      closingDate: formValue.closingDate ? new Date(formValue.closingDate) : new Date(),
      minimumAge: formValue.minimumAge,
      maximumAge: formValue.maximumAge
    };

    this.jobService.update(this.jobId).subscribe({
      next: () => {
        this.isLoading = false;
        this.notificationService.success(this.translationService.instant('JOB_BASIC_MODAL.SUCCESS.UPDATED'));
        this.ref.close({ success: true, jobId: this.jobId });
      },
      error: (err) => {
        this.isLoading = false;
      }
    });
  }

  cancel(): void {
  if (this.isCreateMode && this.showInWizard) {
    const ref = this.dialogHelperService.openConfirmDialog({
      type: 'submit',
      title: 'JOB_BASIC_MODAL.CANCEL_CONFIRM_TITLE',
      description: 'JOB_BASIC_MODAL.CANCEL_CONFIRM',
      cancelText: 'common.cancel',
      confirmText: 'common.confirm',
    });

    ref?.onClose.subscribe((result) => {
      if (!result) return;
      this.ref.close({ success: false });
    });
  } else {
    this.ref.close({ success: false });
  }
}

  private markAllAsTouched(): void {
    Object.values(this.form.controls).forEach(control => {
      control.markAsTouched();
    });
  }

  getSaveButtonText(): string {
    if (this.isEditMode) {
      return 'JOB_BASIC_MODAL.UPDATE_BUTTON';
    } else if (this.isCreateMode) {
      return 'JOB_BASIC_MODAL.START_CREATION_BUTTON';
    }
    return 'JOB_BASIC_MODAL.SAVE_BUTTON';
  }

  isFieldRequired(fieldName: string): boolean {
    const control = this.form.get(fieldName);
    return control ? control.hasValidator(Validators.required) : false;
  }

  canSelectDepartment(): boolean {
    return !!this.form.controls.managementId.value;
  }

  canSelectSubMajor(): boolean {
    return !!this.form.controls.majorId.value;
  }

  private normalizeDate(d: Date | null): Date {
    if(!d)
      return new Date();

  const x = new Date(d);
  x.setHours(12, 0, 0, 0); // noon local time
  return x;
}
}
