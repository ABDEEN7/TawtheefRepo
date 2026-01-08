import {
  Component,
  OnInit,
  inject,
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Button } from 'primeng/button';
import { InputNumber } from 'primeng/inputnumber';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Select } from 'primeng/select';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DatePicker } from 'primeng/datepicker';
import { NgClass, NgIf } from '@angular/common';
import {ProfileLookupsService} from '../../../../../wizard-profile/services/profile-lookups.service';
import {RemoteSelectComponent} from '../../../../../../../../shared/components/remote-select/remote-select';
import {EndpointsService} from '../../../../../../../../core/http/endpoints.service';
import {FileUtilsService} from '../../../../../../../../core/utils/file-utils';
import {UploadedFileRef} from '../../../../../wizard-profile/models/profile-state.model';
import {Degree} from '../../../../../wizard-profile/models/degree.model';
import * as Lookups from '../../../../../../../../core/enums/lookups.enum';
import {I18nNamespaceDirective} from '../../../../../../../../shared/directives/i18n-namespace.directive';

@Component({
  selector: 'app-qualification',
  imports: [
    ReactiveFormsModule,
    Button,
    InputNumber,
    TranslatePipe,
    Select,
    DatePicker,
    NgClass,
    NgIf,
    RemoteSelectComponent,
    I18nNamespaceDirective,
  ],
  templateUrl: './degree.modal.html',
  styleUrl: './degree.modal.scss',
})
export class DegreeModal implements OnInit {
  private fb = inject(FormBuilder);
  private config = inject(DynamicDialogConfig<Degree>);
  private translate = inject(TranslateService);
  protected lookups = inject(ProfileLookupsService);
  protected ref = inject(DynamicDialogRef);
  protected endpoints = inject(EndpointsService);
  private fileUtils = inject(FileUtilsService);

  minYear = 1970;
  maxYear = new Date().getFullYear();
  today = new Date();
  yearError = false;

  degreeFile: File | null = null;
  maxFileSize = 1_000_000; // 1MB
  fileError: string | null = null;
  allowedTypes = ['application/pdf', 'image/png', 'image/jpeg', 'image/webp'];
  initialCertificate: UploadedFileRef | null = null;
  private initialId: string | null = null;
  private initialAttachmentId: string | null = null;

  form: FormGroup = this.fb.group({
    degree: [null, Validators.required],
    gradCountry: [null, Validators.required],
    university: [null],
    major: [null],
    subMajor: [null],
    gradYear: [null],
    studySystem: [null],
    gpa: [null],
    grade: [null],
    degreeFileName: [null, Validators.required],
  });

  get isQualification(): boolean {
    const degree = this.form.get('degree')?.value;
    const backendName = degree?.backendName;

    return ![
      Lookups.Degree.Preparatory,
      Lookups.Degree.Primary,
      Lookups.Degree.Secondary,
    ].includes(backendName);
  }
  ngOnInit() {
    if (this.config.data && this.config.data.initialValue) {
      this.form.patchValue(this.config.data.initialValue);
      const initialValue = this.config.data.initialValue as Degree;
      this.initialCertificate = initialValue?.certificate ?? null;
      this.initialId = initialValue?.id ?? null;
      this.initialAttachmentId = initialValue?.attachmentId ?? this.initialCertificate?.resourceId ?? null;
    }

    this.updateQualificationValidators();
    this.form.get('degree')?.valueChanges.subscribe(() => {
      this.updateQualificationValidators();
    });
  }
  private updateQualificationValidators(): void {
    const need = this.isQualification;

    const uni = this.form.get('university');
    const major = this.form.get('major');
    const subMajor = this.form.get('subMajor');
    const gradYear = this.form.get('gradYear');
    const studySystem = this.form.get('studySystem');
    const gpa = this.form.get('gpa');
    const grade = this.form.get('grade');

    if (need) {
      uni?.setValidators([Validators.required]);
      major?.setValidators([Validators.required]);
      subMajor?.setValidators([Validators.required]);
      gradYear?.setValidators([Validators.required]);
      studySystem?.setValidators([Validators.required]);
      gpa?.setValidators([
        Validators.required,
        Validators.pattern(/^\d+(\.\d{1,2})?$/),
      ]);
      grade?.setValidators([Validators.required]);
    } else {
      [uni, major, subMajor, gradYear, studySystem, gpa, grade].forEach(c => {
        c?.clearValidators();
        c?.setValue(null);
        c?.updateValueAndValidity({ emitEvent: false });
      });

      this.yearError = false;
    }

    [uni, major, subMajor, gradYear, studySystem, gpa, grade].forEach(c => {
      c?.updateValueAndValidity({ emitEvent: false });
    });
  }

  validateYear(): void {
    if (!this.isQualification) {
      this.yearError = false;
      return;
    }

    const d = this.form.value.gradYear;
    if (!d) {
      this.yearError = false;
      return;
    }

    const y =
      d instanceof Date
        ? d.getFullYear()
        : new Date(d as any).getFullYear();

    this.yearError = !(y >= this.minYear && y <= this.maxYear);
  }

  onSave() {
    if (this.form.invalid || this.yearError || (!this.degreeFile)) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.value;

    const gradYear =
      raw.gradYear instanceof Date
        ? raw.gradYear.getFullYear()
        : raw.gradYear
          ? new Date(raw.gradYear as any).getFullYear()
          : null;

    const payload = {
      id: this.initialId ?? undefined,
      degree: raw.degree,
      gradCountry: raw.gradCountry,
      university: raw.university,
      major: raw.major,
      subMajor: raw.subMajor,
      gradYear,
      studySystem: raw.studySystem,
      gpa: raw.gpa != null ? +raw.gpa : null,
      grade: raw.grade,
      fileName: raw.degreeFileName ?? this.initialCertificate?.resourceName ?? null,
      file: this.degreeFile as File | null,
      attachmentId: this.initialAttachmentId,
    } as Degree;

    this.ref.close(payload);
  }

  onUpload(evt: Event): void {
    this.fileError = null;
    const input = evt.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;

    if (!this.allowedTypes.includes(file.type)) {
      this.fileError =
        this.translate.instant('validation.fileType') +
        ': PDF, PNG, JPEG, WEBP';
      this.degreeFile = null;
      this.form.patchValue({ degreeFileName: null });
      input.value = '';
      return;
    }

    if (file.size > this.maxFileSize) {
      this.fileError = this.translate.instant('validation.fileSize', {
        size: this.maxFileSize / 1_000_000,
      }) + 'MB';
      this.degreeFile = null;
      this.form.patchValue({ degreeFileName: null });
      input.value = '';
      return;
    }

    this.fileError = null;
    this.degreeFile = file;
    this.form.patchValue({ degreeFileName: file.name });
    input.value = '';
  }

  previewFile(ev?: Event): void {
    ev?.stopPropagation();
    if (this.degreeFile) {
      this.fileUtils.previewBlob(this.degreeFile);
      return;
    }

    if (this.initialCertificate?.url) {
      this.fileUtils.previewUrl(this.initialCertificate.url, this.initialCertificate.resourceName || '', false);
    }
  }

  get f() {
    return this.form.controls;
  }
}
