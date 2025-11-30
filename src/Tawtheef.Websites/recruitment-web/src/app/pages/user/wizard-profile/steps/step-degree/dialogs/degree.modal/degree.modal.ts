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
import { ProfileLookupsService } from '../../../../services/profile-lookups.service';
import {Degree} from '../../../../models/degree.model';

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
  ],
  templateUrl: './degree.modal.html',
  styleUrl: './degree.modal.scss',
})
export class DegreeModal implements OnInit {
  private fb = inject(FormBuilder);
  private config = inject(DynamicDialogConfig);
  private translate = inject(TranslateService);
  protected lookups = inject(ProfileLookupsService);
  protected ref = inject(DynamicDialogRef);

  minYear = 1970;
  maxYear = new Date().getFullYear();
  yearError = false;

  degreeFile: File | null = null;
  maxFileSize = 1_000_000; // 1MB
  fileError: string | null = null;
  allowedTypes = ['application/pdf', 'image/png', 'image/jpeg', 'image/webp'];
  ngOnInit() {
    if (this.config.data && this.config.data.initialValue) {
      this.form.patchValue(this.config.data.initialValue);
    }
  }
  form: FormGroup = this.fb.group({
    degree: [null, Validators.required],
    gradCountry: [null, Validators.required],
    university: [null, Validators.required],
    major: [null, Validators.required],
    subMajor: [null, Validators.required],
    gradYear: [null, [Validators.required]],
    studySystem: [null, Validators.required],
    gpa: [null,[Validators.required, Validators.pattern(/^\d+(\.\d{1,2})?$/)]],
    grade: [null, Validators.required],
    degreeFileName: [null, Validators.required],
  });

  // onUpload كما هي عندك تقريباً (مع تصحيح بسيط)
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

  onSave() {
    if (this.form.invalid || this.yearError || !this.degreeFile) {
      this.form.markAllAsTouched();
      return;
    }
    const raw = this.form.value;

    const gradYear =
      raw.gradYear instanceof Date
        ? raw.gradYear.getFullYear()
        : new Date(raw.gradYear as any).getFullYear();

    const payload = {
      degree: raw.degree,
      gradCountry: raw.gradCountry,
      university: raw.university,
      major: raw.major,
      subMajor: raw.subMajor,
      gradYear,
      studySystem: raw.studySystem,
      gpa: +raw.gpa,
      grade: raw.grade,
      fileName: raw.degreeFileName,
      file: this.degreeFile as File,
    } as Degree;

    this.ref.close(payload);
  }

  validateYear(): void {
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

  get f() {
    return this.form.controls;
  }
}
