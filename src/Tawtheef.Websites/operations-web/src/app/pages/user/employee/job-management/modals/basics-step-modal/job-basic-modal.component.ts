import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { JobService } from '../../services/job.service';
import { JobLookupService } from '../../services/job-lookup.service';
import { GUID } from '../../../../../../shared/types/guid.type';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { TranslateService } from '@ngx-translate/core';
import { Subject, takeUntil } from 'rxjs';
import { Router } from '@angular/router';
import { EndpointsService } from '../../../../../../core/http/endpoints.service';
import { DialogHelperService } from '../../../../../../core/services/dialog-helper.service';
import { JobCopyTemplate } from '../../models/job-copy-template.model';
import { JobTabReviewNoteResponse } from '../../models/job-tab-review-note-response';
import { JobTabType } from '../../enums/job-tab-type';
import { JobStatus } from '../../../../../../core/enums/lookups.enum';
import { JobReviewResponse } from '../../models/job-review-response';
import { JobResponse } from '../../models/job-response-model';
import {routes} from '../../../../../../routes/routes';

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
  isCopyMode = false;
  copyTemplate: JobCopyTemplate | null = null;
  copySourceId: GUID | null = null;
  reviewNote: JobTabReviewNoteResponse | null = null;

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
    this.copyTemplate = this.config.data?.copyTemplate || null;
    this.copySourceId = this.config.data?.copySourceId || null;

    if (this.config.data?.jobId) {
      this.isEditMode = true;
      this.jobId = this.config.data.jobId;
      this.loadJobForEdit();
    } else if (!this.isCreateMode) {
      this.isCreateMode = true;
    }

    this.setupSequenceListeners();
    if (this.copyTemplate) {
      this.isCopyMode = true;
      this.applyTemplateToForm(this.copyTemplate);
    }
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
    this.jobService.loadJobForEdit(this.jobId).subscribe({
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
        }, { emitEvent: false });


        this.majorOptions = jobResponse.major?.id ? [jobResponse.major] : [];
        this.subMajorOptions = jobResponse.subMajor?.id ? [jobResponse.subMajor] : [];

        if (jobResponse.sector.id) {
          this.lookupsService.loadManagementsBySector(jobResponse.sector.id as GUID);
        }

        if (jobResponse.management.id) {
          this.lookupsService.loadDepartmentsByManagement(jobResponse.management.id as GUID);
        }

        if (jobResponse.major?.id) {
          this.lookupsService.loadSubMajorsByMajor(jobResponse.major.id as GUID);
        }

        this.loadReviewNote(jobResponse);
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

  private loadReviewNote(jobResponse: JobResponse): void {
    if (jobResponse.jobStatus?.backendName !== JobStatus.NeedUpdate) {
      this.reviewNote = null;
      return;
    }

    this.jobService.getLatestReview(jobResponse.id).pipe(takeUntil(this.destroy$)).subscribe({
      next: (review: JobReviewResponse) => {
        this.reviewNote =
          review.tabNoteReviews.find((note) => note.tab === JobTabType.BasicData) ?? null;
      },
      error: () => {
        this.reviewNote = null;
      },
    });
  }

  startJobCreation(): void {
    if (!this.form.valid) {
      this.markAllAsTouched();
      return;
    }

    this.isLoading = true;

    const formValue = this.form.getRawValue();
    const normalizedClosingDate = this.normalizeDate(formValue.closingDate);
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
      closingDate: normalizedClosingDate,
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

    if (this.isCopyMode && this.copyTemplate && this.copySourceId) {
      const copyPayload = {
        ...this.copyTemplate,
        titleAr: jobData.titleAr,
        titleEn: jobData.titleEn,
        sectorId: jobData.sectorId,
        managementId: jobData.managementId,
        departmentId: jobData.departmentId,
        yearsOfExperience: jobData.yearsOfExperience,
        jobCategoryId: jobData.jobCategoryId,
        workLocationId: jobData.workLocationId,
        genderId: jobData.genderId,
        majorId: jobData.majorId,
        subMajorId: jobData.subMajorId,
        workTypeId: jobData.workTypeId,
        numberOfVacancies: jobData.numberOfVacancies,
        closingDate: normalizedClosingDate,
        minimumAge: jobData.minimumAge,
        maximumAge: jobData.maximumAge,
        degrees: this.copyTemplate.degrees ?? [],
        conditions: this.copyTemplate.conditions ?? [],
        responsibilities: this.copyTemplate.responsibilities ?? [],
        skills: this.copyTemplate.skills ?? [],
        requiredAttachments: this.copyTemplate.requiredAttachments ?? [],
        jobPoints: this.copyTemplate.jobPoints ?? null
      };

      this.jobService.createFromPrevious(this.copySourceId, copyPayload).subscribe({
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
            this.router.navigate([routes.employee.jobEdit(jobId)]);
          }
        },
        error: () => {
          this.isLoading = false;
        }
      });
      return;
    }


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
          this.router.navigate([routes.employee.JobDetails(jobId)]);
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
      closingDate: this.normalizeDate(formValue.closingDate),
      minimumAge: formValue.minimumAge,
      maximumAge: formValue.maximumAge
    };

    this.jobService.updateCurrentJobBasics(updateData);
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

private applyTemplateToForm(template: JobCopyTemplate): void {
    const closingDate = template.closingDate ? new Date(template.closingDate) : null;

    this.form.patchValue({
      sectorId: template.sectorId || '',
      managementId: template.managementId || '',
      departmentId: template.departmentId || '',
      yearsOfExperience: template.yearsOfExperience || 0,
      titleAr: template.titleAr || '',
      titleEn: template.titleEn || '',
      jobCategoryId: template.jobCategoryId || '',
      workLocationId: template.workLocationId || '',
      genderId: template.genderId || '',
      majorId: template.majorId || '',
      subMajorId: template.subMajorId || '',
      workTypeId: template.workTypeId || '',
      numberOfVacancies: template.numberOfVacancies ?? 1,
      closingDate: closingDate,
      minimumAge: template.minimumAge || 18,
      maximumAge: template.maximumAge || 60
    });

    if (template.sectorId) {
      this.lookupsService.loadManagementsBySector(template.sectorId);
    }

    if (template.managementId) {
      this.lookupsService.loadDepartmentsByManagement(template.managementId);
    }

    if (template.majorId) {
      this.lookupsService.loadSubMajorsByMajor(template.majorId);
    }
  }
}
