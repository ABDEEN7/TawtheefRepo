import { Component, OnInit, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
  AbstractControl,
  ValidationErrors
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
import {EXPERIENCE_DIALOG_LIMITS} from '../dialog-config';
import {dropdownOptionsModel} from '../../../../../../../../shared/models/dropdown-options.model';
import {ProfileLookupsService} from '../../../../../wizard-profile/services/profile-lookups.service';
import {Experience} from '../../../../../wizard-profile/models/experience.model';
import {Degree} from '../../../../../wizard-profile/models/degree.model';

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
    Select
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

  readonly limits = EXPERIENCE_DIALOG_LIMITS;
  readonly allowedTypes = ['application/pdf', 'image/png', 'image/jpeg', 'image/webp'];
  fileError: string | null = null;
  initialAttachmentUrl: string | null = null;

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
      file: [null, Validators.required], // ✅ required
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
    if (this.config.data?.initialValue) {
      this.form.patchValue(this.config.data.initialValue);
      const init = this.config.data.initialValue as any;
      this.initialAttachmentUrl = init?.attachment?.url ?? init?.attachmentUrl ?? null;
      if (init?.qualificationId) {
        this.form.patchValue({ hasQualification: true });
      }
    }
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

  onUpload(evt: any) {
    this.fileError = null;
    const input = evt.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;

    if (!this.allowedTypes.includes(file.type)) {
      this.fileError = this.translate.instant('validation.fileType', {
        types: 'PDF, PNG, JPEG, WEBP'
      });
      this.form.patchValue({ file: null, fileName: '' });
      return;
    }

    if (file.size > this.limits.maxFileSizeBytes) {
      this.fileError = this.translate.instant('validation.fileSize', { size: this.limits.maxFileSizeLabel });
      this.form.patchValue({ file: null, fileName: '' });
      return;
    }

    this.form.patchValue({ file: file, fileName: file.name });
  }

  touchDates() {
    this.f['from'].markAsTouched();
    this.f['to'].markAsTouched();
    this.form.updateValueAndValidity({ onlySelf: false, emitEvent: true });
  }

  onSave() {
    if (this.form.invalid || this.fileError) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();
    const qualificationOption = this.degreeOptions.find(d => d.id === v.qualificationId) ?? null;
    if(qualificationOption){
      if(v.from.getFullYear() < qualificationOption.additionalData.year){
        this.form.setErrors({ invalidQualificationDate: true });
        this.form.markAllAsTouched();
        return;
      }
    }

    const payload = {
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
      this.fileUtils.previewUrl(this.initialAttachmentUrl, this.form.get('fileName')?.value ?? '', false);
    }
  }

  get degreeOptions(): dropdownOptionsModel[] {
    const degrees = (this.config.data?.degrees as Degree[] | undefined) ?? [];
    return degrees
      .filter(d => !!d.id)
      .map(d => ({
        id: d.id!,
        backendName: d.degree?.backendName ?? '',
        name: `${d.degree?.name ?? ''} - ${d.major?.name ?? ''} (${d.gradYear ?? ''})`,
        description: d.university?.name ?? '',
        additionalData: {
          year: d.gradYear
        }
      }));
  }

  private syncQualification() {
    if (!this.f['hasQualification'].value) {
      this.f['qualificationId'].setValue(null, { emitEvent: false });
    }
    this.form.updateValueAndValidity({ emitEvent: false });
  }
}

// ===== Validators =====

export function dateRangeValidator(fromKey: string, toKey: string) {
  return (group: AbstractControl): ValidationErrors | null => {
    const from = group.get(fromKey)?.value as Date | null;
    const to = group.get(toKey)?.value as Date | null;
    if (!from || !to) return null;
    return new Date(from).getTime() <= new Date(to).getTime()
      ? null
      : { dateRange: true };
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
