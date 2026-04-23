import { Component, OnInit, inject, DestroyRef, OnDestroy } from "@angular/core";
import { FormBuilder, Validators, FormArray, FormGroup, AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";
import { debounceTime, filter, Subject, takeUntil } from "rxjs";
import { GUID } from "../../../../../../../shared/types/guid.type";
import { Job } from "../../../models/job.model";
import { JobLookupService } from "../../../services/job-lookup.service";
import { JobService } from "../../../services/job.service";
import { WizardStepComponent } from "../base/wizard-step.component";
import { JobDegree } from "../../../models/job-degree.model";
import { JobTabReviewNoteResponse } from "../../../models/job-tab-review-note-response";
import { JobTabStatus } from "../../../enums/job-tab-status";
import { JobStatus, Degree } from "../../../../../../../core/enums/lookups.enum";
import { EndpointsService } from "../../../../../../../core/http/endpoints.service";
import { GuidUtils } from "../../../../../../../core/utils/guid-utils";
import { ConfirmationService } from "primeng/api";
import { NotificationService } from "../../../../../../../core/services/notification.service";
import { TranslateService } from "@ngx-translate/core";
import { JobSpecialization } from "../../../models/job-specialization.model";

@Component({
  selector: 'app-qualifications-step',
  standalone: false,
  templateUrl: './qualifications-step.component.html',
  styleUrl: './qualifications-step.component.scss',
})
export class QualificationsStepComponent extends WizardStepComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  protected jobService = inject(JobService);
  protected lookupsService = inject(JobLookupService);
  protected endpoints = inject(EndpointsService);
  private notificationService = inject(NotificationService);
  private confirmationService = inject(ConfirmationService);
  private translate = inject(TranslateService);

  jobData!: Job;

  readonly form = this.fb.group({
    majorId: [null as GUID | null],
    subMajorId: [null as GUID | null],
    degrees: this.fb.control<any[]>([], Validators.required),
    jobSpecializations: this.fb.array([]),
    qualificationsDescriptionAr: ['', Validators.required],
    qualificationsDescriptionEn: ['', Validators.required]
  }, { validators: [this.duplicateSpecializationValidator] });
  note: JobTabReviewNoteResponse | null = null;
  majorOptions: any[] = [];
  subMajorOptions: any[] = [];
  needsMajor: boolean = true;

  // Track selected object names for review step
  selectedMajor: any = null;
  selectedSubMajor: any = null;
  majorName: string = '';
  subMajorName: string = '';
  private selectionRegistry = new Map<GUID, any>();

  private destroy$ = new Subject<void>();

  private simplifiedDegrees: (string | undefined)[] = [
    Degree.Secondary,
    Degree.Preparatory,
    Degree.Primary,
  ];

  get jobSpecializationsFormArray() {
    return this.form.controls.jobSpecializations as FormArray;
  }

  ngOnInit(): void {
    this.setupDegreeValidationListener();

    this.form.valueChanges.pipe(
      takeUntil(this.destroy$),
      debounceTime(300)
    ).subscribe(_ => {
      this.updateJobData();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  isValid(): boolean {
    if (this.form.disabled) {
      return true;
    }
    return this.form.valid;
  }

  isDegreeChecked(degreeId: string): boolean {
    const degrees = this.form.controls.degrees.value ?? [];
    return degrees.some(d => (d.degreeId || d) === degreeId);
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

  touchDegrees(): void {
    const c = this.form.controls.degrees;
    c.markAsTouched();
    c.updateValueAndValidity({ onlySelf: true });
  }

  isFieldRequired(fieldName: string): boolean {
    const control = this.form.get(fieldName);
    return control ? control.hasValidator(Validators.required) : false;
  }

  canSelectSubMajor(): boolean {
    return !!this.form.controls.majorId.value;
  }

  canSelectAdditionalSubMajor(index: number): boolean {
    const group = this.jobSpecializationsFormArray.at(index) as FormGroup;
    return !!group.controls['majorId'].value;
  }

  addSpecialization(id: GUID | null = null, majorId: GUID | null = null, subMajorId: GUID | null = null, majorOption: any = null, subMajorOption: any = null): void {
    if (majorId) {
      const mainMajorId = this.form.controls.majorId.value;
      const mainSubMajorId = this.form.controls.subMajorId.value;

      const isPrimary = mainMajorId === majorId && (mainSubMajorId || null) === (subMajorId || null);

      const exists = this.jobSpecializationsFormArray.controls.some((c: any) =>
        c.value.majorId === majorId && (c.value.subMajorId || null) === (subMajorId || null)
      );

      if (exists || isPrimary) {
        this.notificationService.warn(this.translate.instant('JOB_WIZARD.VALIDATION.SPECIALIZATION_EXISTS'));
        return;
      }
    }

    const group = this.fb.group({
      id: [id],
      majorId: [majorId, Validators.required],
      subMajorId: [subMajorId],
      majorOptions: [majorOption ? [majorOption] : []],
      subMajorOptions: [subMajorOption ? [subMajorOption] : []]
    });
    this.jobSpecializationsFormArray.push(group);
  }

  removeSpecialization(index: number): void {
    const spec = this.jobSpecializationsFormArray.at(index).value;
    if (spec.id) {
      this.confirmationService.confirm({
        message: this.translate.instant('JOB_WIZARD.CONFIRM.DELETE_SPECIALIZATION'),
        header: this.translate.instant('JOB_BASIC_MODAL.CANCEL_CONFIRM_TITLE'),
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
          this.jobService.deleteSpecialization(spec.id).subscribe(() => {
            this.jobSpecializationsFormArray.removeAt(index);
            this.updateJobData();
            this.notificationService.success(this.translate.instant('JOB_WIZARD.MESSAGES.SPECIALIZATION_DELETED'));
          });
        }
      });
    } else {
      this.jobSpecializationsFormArray.removeAt(index);
      this.updateJobData();
    }
  }

  private setupDegreeValidationListener(): void {
    this.form.controls.degrees.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(degrees => {
        this.updateNeedsMajor(degrees);
      });
  }

  private updateNeedsMajor(degrees: any[] | null): void {
    const needs = !degrees || degrees.length === 0 ||
      degrees.some(d => {
        const degreeId = d.degreeId || d;
        const degreeObj = this.lookupsService.degrees().find(ld => ld.id === degreeId);
        return !this.simplifiedDegrees.includes(degreeObj?.backendName);
      });

    this.needsMajor = needs;

    if (needs) {
      this.form.controls.majorId.addValidators(Validators.required);
    } else {
      this.form.controls.majorId.removeValidators(Validators.required);
      this.form.controls.subMajorId.removeValidators(Validators.required);

      if (this.form.controls.majorId.value || this.form.controls.subMajorId.value || this.jobSpecializationsFormArray.length > 0) {
        this.form.patchValue({ majorId: null, subMajorId: null }, { emitEvent: false });
        while (this.jobSpecializationsFormArray.length !== 0) {
          this.jobSpecializationsFormArray.removeAt(0);
        }
        this.updateJobData();
      }
    }

    this.form.controls.majorId.updateValueAndValidity({ emitEvent: false });
    this.form.controls.subMajorId.updateValueAndValidity({ emitEvent: false });
  }

  private duplicateSpecializationValidator(control: AbstractControl): ValidationErrors | null {
    const majorId = control.get('majorId')?.value;
    const subMajorId = control.get('subMajorId')?.value;
    const specs = (control.get('jobSpecializations') as FormArray)?.value || [];

    const seen = new Set<string>();
    if (majorId) {
      seen.add(`${majorId}_${subMajorId || ''}`);
    }

    for (const spec of specs) {
      if (spec.majorId) {
        const key = `${spec.majorId}_${spec.subMajorId || ''}`;
        if (seen.has(key)) {
          return { duplicateSpecialization: true };
        }
        seen.add(key);
      }
    }
    return null;
  }

  onObjectSelected(obj: any): void {
    if (obj?.id) {
      this.selectionRegistry.set(obj.id, obj);
    }
  }

  override setJobData(job: Job, note: JobTabReviewNoteResponse | null = null): void {
    this.jobData = job;
    this.note = note;

    const degrees = job.degrees?.map(degree => ({
      degreeId: degree.degreeId
    })) || [];

    while (this.jobSpecializationsFormArray.length !== 0) {
      this.jobSpecializationsFormArray.removeAt(0);
    }

    const jobResponse = job as any;

    if (job.majorId) {
      this.majorOptions = jobResponse.major?.id ? [jobResponse.major] : jobResponse.majorOptions ? jobResponse.majorOptions : [];
    }
    if (job.subMajorId) {
      this.subMajorOptions = jobResponse.subMajor?.id ? [jobResponse.subMajor] : jobResponse.subMajorOptions ? jobResponse.subMajorOptions : [];
    }

    this.majorName = job.majorName || '';
    this.subMajorName = job.subMajorName || '';

    if (job.majorId && jobResponse.major) this.onObjectSelected(jobResponse.major);
    if (job.subMajorId && jobResponse.subMajor) this.onObjectSelected(jobResponse.subMajor);

    if (job.jobSpecializations && job.jobSpecializations.length > 0) {
      job.jobSpecializations.forEach((spec: any) => {
        if (spec.major) this.onObjectSelected(spec.major);
        if (spec.subMajor) this.onObjectSelected(spec.subMajor);

        this.addSpecialization(spec.id, spec.majorId, spec.subMajorId);
      });
    }

    this.form.patchValue({
      majorId: job.majorId || null,
      subMajorId: job.subMajorId || null,
      degrees: degrees,
      qualificationsDescriptionAr: job.qualificationsDescriptionAr || '',
      qualificationsDescriptionEn: job.qualificationsDescriptionEn || ''
    }, { emitEvent: false });

    this.selectedMajor = jobResponse.major || null;
    this.selectedSubMajor = jobResponse.subMajor || null;
    this.majorName = job.majorName || '';
    this.subMajorName = job.subMajorName || '';

    this.updateNeedsMajor(degrees);

    if (note?.tabStatus !== JobTabStatus.Returned && job.jobStatus?.backendName === JobStatus.NeedUpdate) {
      this.form.disable();
    }
  }

  protected extractName(option: any): string {
    if (!option) return '';
    const isAr = this.translate.currentLang === 'ar';
    const nameFromAttr = isAr ? option.additionalData?.nameAr : option.additionalData?.nameEn;
    return nameFromAttr || option.name || '';
  }

  protected updateJobData(): void {
    const formValue = this.form.getRawValue();
    const specializations = this.prepareSpecializations(formValue.jobSpecializations);

    this.jobService.updateCurrentJobQualifications(
      formValue.degrees || [],
      formValue.majorId as GUID | null,
      formValue.subMajorId as GUID | null,
      specializations,
      formValue.qualificationsDescriptionAr || '',
      formValue.qualificationsDescriptionEn || '',
      formValue.majorId ? this.extractName(this.selectionRegistry.get(formValue.majorId)) : '',
      formValue.subMajorId ? this.extractName(this.selectionRegistry.get(formValue.subMajorId)) : ''
    );
  }

  private prepareSpecializations(rawSpecs: any[]): JobSpecialization[] {
    const seen = new Set<string>();
    const result: JobSpecialization[] = [];

    (rawSpecs || []).forEach((s) => {
      if (!s.majorId) return;

      const key = `${s.majorId}_${s.subMajorId || ''}`;
      if (!seen.has(key)) {
        seen.add(key);

        result.push({
          id: s.id as GUID,
          majorId: s.majorId as GUID,
          subMajorId: s.subMajorId as GUID
        });
      }
    });

    return result;
  }

}
