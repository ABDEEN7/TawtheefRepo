import { Component, inject, OnInit, OnDestroy, AfterViewInit, HostListener, DestroyRef } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { FormBuilder, Validators } from '@angular/forms';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { JobService } from '../../services/job.service';
import { JobLookupService } from '../../services/job-lookup.service';
import { GUID } from '../../../../../../shared/types/guid.type';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { TranslateService } from '@ngx-translate/core';
import { Subject, takeUntil, filter } from 'rxjs';
import { Router } from '@angular/router';
import { EndpointsService } from '../../../../../../core/http/endpoints.service';
import { DialogHelperService } from '../../../../../../core/services/dialog-helper.service';
import { JobCopyTemplate } from '../../models/job-copy-template.model';
import { JobTabReviewNoteResponse } from '../../models/job-tab-review-note-response';
import { JobTabType } from '../../enums/job-tab-type';
import { JobStatus, Degree } from '../../../../../../core/enums/lookups.enum';
import { JobReviewResponse } from '../../models/job-review-response';
import { JobResponse } from '../../models/job-response-model';
import { routes } from '../../../../../../routes/routes';
import { JobTabStatus } from '../../enums/job-tab-status';
import { GuidUtils } from '../../../../../../core/utils/guid-utils';

@Component({
  selector: 'app-job-basic-modal',
  templateUrl: './job-basic-modal.component.html',
  styleUrls: ['./job-basic-modal.component.scss'],
  standalone: false
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
  isViewMode = false;
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
    departmentId: [null, GuidUtils.nullGuid],
    yearsOfExperience: [0, [Validators.required, Validators.min(0)]],
    jobTitleId: ['', Validators.required],
    jobCategoryId: ['', Validators.required],
    workLocationId: ['', Validators.required],
    genderId: ['', Validators.required],
    majorId: [null, GuidUtils.nullGuid],
    subMajorId: [null, GuidUtils.nullGuid],
    degrees: this.fb.control<any[]>([], Validators.required),
    workTypeId: ['', Validators.required],
    numberOfVacancies: [1, [Validators.required, Validators.min(1)]],
    closingDate: this.fb.control<Date | null>(null, [Validators.required]),
    minimumAge: [18, [Validators.required, Validators.min(18)]],
    maximumAge: [60, [Validators.required, Validators.min(18)]],
  });

  private simplifiedDegrees: (string | undefined)[] = [
    Degree.Secondary,
    Degree.Preparatory,
    Degree.Primary,
  ];

  majorOptions: any[] = [];
  subMajorOptions: any[] = [];

  private destroyRef = inject(DestroyRef);
  private loaded$ = toObservable(this.lookupsService.loaded);

  ngOnInit(): void {
    const today = new Date();
    this.currentDate = new Date(today.getFullYear(), today.getMonth(), today.getDate() + 1);

    // Set modes based on input data
    this.isViewMode = this.config.data?.isViewMode || false;
    this.isCreateMode = this.config.data?.isCreateMode || false;
    this.showInWizard = this.config.data?.showInWizard || false;
    this.copyTemplate = this.config.data?.copyTemplate || null;
    this.copySourceId = this.config.data?.copySourceId || null;

    if (this.config.data?.jobData || this.config.data?.jobId) {
      // Wait for lookups if they are not loaded yet
      if (!this.lookupsService.loaded()) {
        this.lookupsService.loadAll();
        this.loaded$
          .pipe(
            filter(loaded => loaded),
            takeUntilDestroyed(this.destroyRef)
          )
          .subscribe(() => {
            this.initializeModal();
          });
      } else {
        this.initializeModal();
      }
    } else if (!this.isViewMode && !this.isCreateMode) {
      this.isCreateMode = true;
    }

    if (!this.isViewMode) {
      this.setupSequenceListeners();
    }

    this.setupDegreeValidationListener();
    if (this.copyTemplate) {
      this.isCopyMode = true;
      this.applyTemplateToForm(this.copyTemplate);
    }
  }

  private initializeModal(): void {
    if (this.config.data?.jobData) {
      this.jobId = this.config.data.jobId;
      this.populateForm(this.config.data.jobData);
      if (this.isViewMode) {
        this.form.disable();
      }
    } else if (this.config.data?.jobId) {
      this.jobId = this.config.data.jobId;
      if (!this.isViewMode) {
        this.isEditMode = true;
      }
      this.loadJobForEdit();
    }
  }

  private populateForm(jobResponse: JobResponse): void {
    const deadline = jobResponse.closingDate ? new Date(jobResponse.closingDate) : null;

    this.form.patchValue({
      sectorId: jobResponse.sector.id || '',
      managementId: jobResponse.management.id || '',
      departmentId: jobResponse.department?.id || null,
      yearsOfExperience: jobResponse.yearsOfExperience || 0,
      jobTitleId: jobResponse.jobTitleId || '',
      jobCategoryId: jobResponse.jobCategory.id || '',
      workLocationId: jobResponse.workLocation.id || '',
      genderId: jobResponse.gender?.id || '',
      majorId: jobResponse.major?.id || null,
      subMajorId: jobResponse.subMajor?.id || null,
      workTypeId: jobResponse.workType.id || '',
      numberOfVacancies: jobResponse.numberOfVacancies || 1,
      closingDate: deadline,
      minimumAge: jobResponse.minimumAge || 18,
      maximumAge: jobResponse.maximumAge || 60,
      degrees: jobResponse.degrees || []
    }, { emitEvent: true });


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
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private setupSequenceListeners(): void {
    this.form.controls.managementId.disable({ emitEvent: false });
    this.form.controls.departmentId.disable({ emitEvent: false });

    // this code should be resolve issue Cascading when change the parent control,
    // for example, when change sector should be clear management and department
    this.form.controls.sectorId.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(sectorId => {
        if (sectorId) {
          this.lookupsService.loadManagementsBySector(sectorId as GUID);
          this.form.controls.managementId.enable({ emitEvent: false });
        } else {
          this.lookupsService.resetManagements();
          this.form.controls.managementId.disable({ emitEvent: false });
          this.form.controls.departmentId.disable({ emitEvent: false });
        }
        this.form.controls.managementId.setValue('', { emitEvent: false });
        this.form.controls.departmentId.setValue(null, { emitEvent: false });
        this.lookupsService.resetDepartments();
      });

    this.form.controls.managementId.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(managementId => {
        if (managementId) {
          this.lookupsService.loadDepartmentsByManagement(managementId as GUID);
          this.form.controls.departmentId.enable({ emitEvent: false });
        } else {
          this.lookupsService.resetDepartments();
          this.form.controls.departmentId.disable({ emitEvent: false });
        }
        this.form.controls.departmentId.setValue(null, { emitEvent: false });
      });
  }

  private setupDegreeValidationListener(): void {
    this.form.controls.degrees.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(degrees => {
        const needsMajor = !degrees || degrees.length === 0 ||
          degrees.some(d => {
            const degreeId = d.degreeId || d;
            const degreeObj = this.lookupsService.degrees().find(ld => ld.id === degreeId);
            return !this.simplifiedDegrees.includes(degreeObj?.backendName);
          });

        if (needsMajor) {
          this.form.controls.majorId.addValidators(Validators.required);
          this.form.controls.subMajorId.addValidators(Validators.required);
        } else {
          this.form.controls.majorId.removeValidators(Validators.required);
          this.form.controls.subMajorId.removeValidators(Validators.required);
        }

        this.form.controls.majorId.updateValueAndValidity({ emitEvent: false });
        this.form.controls.subMajorId.updateValueAndValidity({ emitEvent: false });
      });
  }

  toggleDegree(degreeId: string, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    const current = this.form.controls.degrees.value || [];

    let updated: any[];
    if (checked) {
      updated = [...current, { degreeId: degreeId as GUID }];
    } else {
      updated = current.filter(d => (d.degreeId || d) !== degreeId as GUID);
    }

    this.form.controls.degrees.setValue(updated);
    this.form.controls.degrees.markAsDirty();
  }

  isDegreeChecked(degreeId: string): boolean {
    const degrees = this.form.controls.degrees.value ?? [];
    return degrees.some(d => (d.degreeId || d) === degreeId);
  }

  touchDegrees(): void {
    const c = this.form.controls.degrees;
    c.markAsTouched();
    c.updateValueAndValidity({ onlySelf: true });
  }

  private loadJobForEdit(): void {
    if (!this.jobId) return;

    this.isLoading = true;
    this.jobService.loadJobForEdit(this.jobId).subscribe({
      next: (jobResponse) => {
        this.populateForm(jobResponse);
        this.isLoading = false;

        if (this.isViewMode) {
          this.form.disable();
        }
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
        if (this.reviewNote?.tabStatus === JobTabStatus.Approved) {
          this.form.disable();
        }
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
      jobTitleId: formValue.jobTitleId as GUID,
      sectorId: formValue.sectorId as GUID,
      managementId: formValue.managementId as GUID,
      departmentId: formValue.departmentId as GUID | null,
      yearsOfExperience: formValue.yearsOfExperience,
      jobCategoryId: formValue.jobCategoryId as GUID,
      workLocationId: formValue.workLocationId as GUID,
      genderId: formValue.genderId as GUID,
      majorId: formValue.majorId as GUID | null,
      subMajorId: formValue.subMajorId as GUID | null,
      workTypeId: formValue.workTypeId as GUID,
      numberOfVacancies: formValue.numberOfVacancies,
      closingDate: normalizedClosingDate,
      minimumAge: formValue.minimumAge,
      maximumAge: formValue.maximumAge,
      overviewAr: null,
      overviewEn: null,
      benefitsAr: null,
      benefitsEn: null,
      qualificationsDescriptionAr: null,
      qualificationsDescriptionEn: null,
      degrees: formValue.degrees || [],
      conditions: [],
      responsibilities: [],
      skills: [],
      requiredAttachments: []
    };

    if (this.isCopyMode && this.copyTemplate && this.copySourceId) {
      const copyPayload = {
        ...this.copyTemplate,
        jobTitleId: jobData.jobTitleId,
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
        degrees: jobData.degrees,
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
            this.router.navigate([routes.portal.jobEdit(jobId)]);
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
          this.router.navigate([routes.portal.JobDetails(jobId)]);
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
      jobTitleId: formValue.jobTitleId as GUID,
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
      degrees: formValue.degrees || []
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
    if (!d)
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
      departmentId: template.departmentId || null,
      yearsOfExperience: template.yearsOfExperience || 0,
      jobTitleId: template.jobTitleId || '',
      jobCategoryId: template.jobCategoryId || '',
      workLocationId: template.workLocationId || '',
      genderId: template.genderId || '',
      majorId: template.majorId || null,
      subMajorId: template.subMajorId || null,
      workTypeId: template.workTypeId || '',
      numberOfVacancies: template.numberOfVacancies ?? 1,
      closingDate: closingDate,
      minimumAge: template.minimumAge || 18,
      maximumAge: template.maximumAge || 60,
      degrees: template.degrees || []
    });
  }

  touchGender(): void {
    const c = this.form.controls.genderId;
    c.markAsTouched();
    c.updateValueAndValidity({ onlySelf: true });
  }
}
