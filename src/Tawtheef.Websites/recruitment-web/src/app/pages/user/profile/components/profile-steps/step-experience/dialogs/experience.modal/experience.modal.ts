import { Component, OnInit, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { Button } from 'primeng/button';
import { InputText } from 'primeng/inputtext';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DatePicker } from 'primeng/datepicker';
import { NgClass, NgIf } from '@angular/common';
import {dateToDateOnly} from '../../../../../../../../shared/types/dateOnly.type';
import {Select} from 'primeng/select';
import {FileUtilsService} from '../../../../../../../../core/utils/file-utils';
import {EXPERIENCE_DIALOG_CONFIG} from '../dialog-config';
import {dropdownOptionsModel, DropdownOptionVM} from '../../../../../../../../shared/models/dropdown-options.model';
import {ProfileLookupsService} from '../../../../../wizard-profile/services/profile-lookups.service';
import {Experience} from '../../../../../wizard-profile/models/experience.model';
import {Degree} from '../../../../../wizard-profile/models/degree.model';
import {Textarea} from 'primeng/textarea';
import {I18nNamespaceDirective} from '../../../../../../../../shared/directives/i18n-namespace.directive';
import { NotificationService } from '../../../../../../../../core/services/notification.service';

type ExperienceModalInit = Partial<Experience> & {
  // your parent passes an "initialValue" shaped like Experience-ish,
  // so we accept it as Experience.
};

@Component({
  selector: 'app-experience',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    TranslatePipe,
    DatePicker,
    NgIf,
    Button,
    InputText,
    NgClass,
    Select,
    Textarea,
    I18nNamespaceDirective,
  ],
  templateUrl: './experience.modal.html',
  styleUrl: './experience.modal.scss',
})
export class ExperienceModal implements OnInit {
  private fb = inject(FormBuilder);
  private ref = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig);
  private translate = inject(TranslateService);
  protected lookups = inject(ProfileLookupsService);
  private fileUtils = inject(FileUtilsService);
  private notify = inject(NotificationService);

  readonly limits = EXPERIENCE_DIALOG_CONFIG;
  readonly allowedTypes = ['application/pdf', 'image/png', 'image/jpeg', 'image/webp'];

  fileError: string | null = null;

  initialAttachmentUrl: string | null = null;
  private initialId: string | null = null;
  private initialAttachmentId: string | null = null;

  // NEW: supports edit mode where upload is blocked
  private disableFileUpload = false;

  today = new Date();

  form: FormGroup = this.fb.group(
    {
      org: ['', [Validators.required, Validators.maxLength(150)]],
      name: ['', [Validators.required, Validators.maxLength(150)]],
      country: [null, [Validators.required]],
      from: [null, [Validators.required]],
      to: [null],
      current: [false],
      description: ['', [Validators.maxLength(this.limits.descriptionMaxLength)]],
      fileName: [''],
      file: [null],
      hasQualification: [false],
      qualificationId: [null],
    },
    {
      validators: [
        toRequiredIfNotCurrent('current', 'to'),
        dateRangeValidator('from', 'to'),
        noFutureValidator('from', 'to'),
        qualificationRequiredValidator('hasQualification', 'qualificationId'),
      ],
    }
  );

  ngOnInit(): void {
    this.disableFileUpload = !!this.config.data?.disableFileUpload;

    // Parent passes initialValue for edit mode
    const init = (this.config.data?.initialValue as ExperienceModalInit | undefined) ?? undefined;

    if (init) {
      this.form.patchValue({
        org: init.employerName ?? '',
        name: init.jobTitle ?? '',
        country: this.lookups.countries().find(c => c.id === init.country?.id) ?? null,
        from: init.from ? new Date(init.from) : null,
        to: init.to ? new Date(init.to) : null,
        current: !!init.current,
        description: init.description ?? '',
        fileName: init.fileName ?? init.attachment?.resourceName ?? '',
        hasQualification: !!init.qualificationId,
        qualificationId: init.qualificationId ?? null,
      });

      this.initialAttachmentUrl = init.attachment?.url ?? null;
      this.initialId = this.config.data?.initialId ?? init.id ?? null;
      this.initialAttachmentId =
        this.config.data?.attachmentId ?? init.attachment?.resourceId ?? null;
    } else {
      // create mode: no initial values
      this.initialId = this.config.data?.initialId ?? null;
      this.initialAttachmentId = this.config.data?.attachmentId ?? null;
    }

    // Enforce disable upload (edit mode)
    if (this.disableFileUpload) {
      this.form.get('file')?.disable({ emitEvent: false });
    } else {
      this.form.get('file')?.enable({ emitEvent: false });
    }

    // Set file validators based on mode + existing attachment
    this.applyFileValidators();

    this.syncToDisabled();
    this.syncQualification();
  }

  onCurrentToggle() {
    this.syncToDisabled();
    this.touchDates();
  }

  onQualificationToggle() {
    this.syncQualification();
  }

  private syncToDisabled() {
    if (this.f['current'].value) {
      this.f['to'].disable({ emitEvent: false });
      this.f['to'].setValue(null, { emitEvent: false });
    } else {
      this.f['to'].enable({ emitEvent: false });
    }
    this.form.updateValueAndValidity({ onlySelf: false, emitEvent: true });
  }

  private applyFileValidators() {
    const fileCtrl = this.form.get('file');
    if (!fileCtrl) return;

    const hasExistingAttachment = !!this.initialAttachmentId || !!this.initialAttachmentUrl;

    if (this.disableFileUpload) {
      fileCtrl.clearValidators();
      fileCtrl.updateValueAndValidity({ emitEvent: false });
      return;
    }

    if (hasExistingAttachment) {
      fileCtrl.clearValidators();
    } else {
      fileCtrl.setValidators([Validators.required]);
    }

    fileCtrl.updateValueAndValidity({ emitEvent: false });
  }

  onUpload(evt: any) {
    if (this.disableFileUpload) return;

    this.fileError = null;
    const input = evt.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;

    if (!this.allowedTypes.includes(file.type)) {
      this.fileError = this.translate.instant('validation.fileType', {
        types: 'PDF, PNG, JPEG, WEBP',
      });
      this.form.patchValue({ file: null, fileName: '' });
      return;
    }

    if (file.size > this.limits.maxFileSizeBytes) {
      this.fileError = this.translate.instant('validation.fileSize', {
        size: this.limits.maxFileSizeLabel,
      });
      this.form.patchValue({ file: null, fileName: '' });
      return;
    }

    this.form.patchValue({ file, fileName: file.name });

    this.form.get('file')?.markAsDirty();
    this.form.updateValueAndValidity({ emitEvent: true });
  }

  touchDates() {
    this.f['from'].markAsTouched();
    this.f['to'].markAsTouched();
    this.form.updateValueAndValidity({ onlySelf: false, emitEvent: true });
  }

  onSave() {
    if (this.form.invalid || this.fileError) {
      this.form.markAllAsTouched();

      const v = this.form.getRawValue();
      this.validateData({
        id: this.initialId ?? undefined,
        employerName: v.org,
        jobTitle: v.name,
        from: v.from ? dateToDateOnly(v.from) : null,
        to: v.current ? null : v.to ? dateToDateOnly(v.to) : null,
        current: !!v.current,
        description: v.description,
        file: v.file,
        fileName: v.file?.name ?? v.fileName ?? null,
        qualificationId: v.hasQualification ? v.qualificationId : null,
        attachmentId: this.initialAttachmentId ?? undefined,
      } as unknown as Experience);

      return;
    }

    const v = this.form.getRawValue();
    const qualificationOption = this.degreeOptions.find((d) => d.id === v.qualificationId) ?? null;

    if (qualificationOption?.additionalData) {
      const additionalData = qualificationOption.additionalData as { year?: number };
      if (additionalData.year && v.from && v.from.getFullYear() < additionalData.year) {
        this.form.setErrors({ invalidQualificationDate: true });
        this.form.markAllAsTouched();
        return;
      }
    }

    const payload = {
      id: this.initialId ?? undefined,
      employerName: v.org,
      jobTitle: v.name,
      from: dateToDateOnly(v.from),
      to: dateToDateOnly(v.current ? null : v.to),
      country: v.country,
      current: !!v.current,
      description: v.description,
      file: v.file,
      fileName: v.file?.name ?? v.fileName ?? null,
      qualificationId: v.hasQualification ? v.qualificationId : null,
      qualificationName: qualificationOption?.name ?? null,
      attachmentId: this.initialAttachmentId ?? undefined,
    } as Experience;

    this.ref.close(payload);
  }

  onCancel() {
    this.ref.close();
  }

  get f() {
    return this.form.controls;
  }

  previewFile(ev?: Event): void {
    ev?.stopPropagation();

    const file = this.form.get('file')?.value as File | null;
    if (file) {
      this.fileUtils.previewBlob(file);
      return;
    }

    if (this.initialAttachmentUrl) {
      this.fileUtils.previewUrl(
        this.initialAttachmentUrl,
        this.form.get('fileName')?.value ?? '',
        false
      ).then(r => {});
    }
  }

  get degreeOptions(): DropdownOptionVM[] {
    const degrees = (this.config.data?.degrees as Degree[] | undefined) ?? [];
    return degrees
      .filter((d) => !!d.id)
      .map((d) => new DropdownOptionVM({
        id: d.id!,
        backendName: d.degree?.backendName ?? '',
        name: `${d.degree?.name ?? ''} - ${d.major?.name ?? ''} (${d.gradYear ?? ''})`,
        description: d.university?.name ?? '',
        additionalData: {
          year: d.gradYear,
        },
      }));
  }

  private syncQualification() {
    if (!this.f['hasQualification'].value) {
      this.f['qualificationId'].setValue(null, { emitEvent: false });
    }
    this.form.updateValueAndValidity({ emitEvent: false });
  }

  private validateData(experience: Experience) {
    const errors: { i18nKey: string }[] = [];
    const today = startOfToday();

    if ((!experience.file || !experience.fileName) && !experience.attachmentId) {
      errors.push({ i18nKey: 'wizard.profile.experience.attachment.required' });
    }

    const startDate = parseDate(experience?.from as any);
    const endDate = parseDate(experience?.to as any);

    if (startDate && startDate.getTime() > today.getTime()) {
      errors.push({ i18nKey: 'wizard.profile.experience.futureDate' });
    }

    if (endDate && endDate.getTime() > today.getTime()) {
      errors.push({ i18nKey: 'wizard.profile.experience.futureDate' });
    }

    if (startDate && endDate && startDate.getTime() > endDate.getTime()) {
      errors.push({ i18nKey: 'wizard.profile.experience.invalidRange' });
    }

    if (!errors.length) {
      errors.push({ i18nKey: 'wizard.validationErrorTitle' });
    }

    this.notify.error(
      `${this.translate.instant('wizard.validationErrorTitle')}: ${errors
        .map((e) => `* ${this.translate.instant(e.i18nKey)}`)
        .join('\n')}`
    );
  }
}
function parseDate(value?: string | null): Date | null {
  if (!value) return null;

  const d = new Date(value);
  return isNaN(d.getTime()) ? null : d;
}

function startOfToday(): Date {
  const now = new Date();
  now.setHours(0, 0, 0, 0);
  return now;
}
// ===== Validators =====

export function dateRangeValidator(fromKey: string, toKey: string) {
  return (group: AbstractControl): ValidationErrors | null => {
    const from = group.get(fromKey)?.value as Date | null;
    const to = group.get(toKey)?.value as Date | null;
    if (!from || !to) return null;

    return new Date(from).getTime() <= new Date(to).getTime() ? null : { dateRange: true };
  };
}

export function toRequiredIfNotCurrent(currentKey: string, toKey: string) {
  return (group: AbstractControl): ValidationErrors | null => {
    const current = !!group.get(currentKey)?.value;
    const to = group.get(toKey)?.value;
    return current || !!to ? null : { toRequiredIfNotCurrent: true };
  };
}

export function noFutureValidator(...keys: string[]) {
  return (group: AbstractControl): ValidationErrors | null => {
    const now = new Date();
    now.setHours(0, 0, 0, 0);

    const future = keys.some((k) => {
      const v = group.get(k)?.value as Date | null;
      return v ? new Date(v).getTime() > now.getTime() : false;
    });

    return future ? { futureDate: true } : null;
  };
}

export function qualificationRequiredValidator(flagKey: string, qualificationKey: string) {
  return (group: AbstractControl): ValidationErrors | null => {
    const flagged = !!group.get(flagKey)?.value;
    const qualification = group.get(qualificationKey)?.value;
    return flagged && !qualification ? { qualificationRequired: true } : null;
  };
}
