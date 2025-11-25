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
import { FileUpload } from 'primeng/fileupload';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Select } from 'primeng/select';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DatePicker } from 'primeng/datepicker';
import { NgClass, NgIf } from '@angular/common';
import { ProfileLookupsService } from '../../../../services/profile-lookups.service';

@Component({
  selector: 'app-qualification',
  imports: [
    ReactiveFormsModule,
    Button,
    InputNumber,
    FileUpload,
    TranslatePipe,
    Select,
    DatePicker,
    NgClass,
    NgIf,
  ],
  templateUrl: './qualification.modal.html',
  styleUrl: './qualification.modal.scss',
})
export class QualificationModal implements OnInit {
  private fb = inject(FormBuilder);
  private ref = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig);
  private translate = inject(TranslateService);
  protected lookpus = inject(ProfileLookupsService);

  minYear = 1970;
  maxYear = new Date().getFullYear();
  yearError = false;

  maxFileSize = 1_000_000; // 1MB
  fileError: string | null = null;
  allowedTypes = ['application/pdf', 'image/png', 'image/jpeg', 'image/webp'];

  form: FormGroup = this.fb.group({
    degree: [null, Validators.required],
    gradCountry: [null, Validators.required],
    university: [null, Validators.required],
    major: [null, Validators.required],
    gradYear: [
      null,
      [Validators.required /* + range check يدوي عبر yearError */],
    ],
    studySystem: [null, Validators.required],
    gpa: [
      null,
      [Validators.required, Validators.pattern(/^\d+(\.\d{1,2})?$/)],
    ],
    grade: [null, Validators.required],
    degreeFile: [null],
  });

  ngOnInit() {
    if (this.config.data && this.config.data.initialValue) {
      this.form.patchValue(this.config.data.initialValue);
    }
  }

  onUpload(evt: any): void {
    this.fileError = null;
    const file: File | undefined = evt?.files?.[0];
    if (!file) return;

    if (!this.allowedTypes.includes(file.type)) {
      this.fileError =
        this.translate.instant('validation.fileType') +
        ': PDF, PNG, JPEG, WEBP';
      this.form.patchValue({ degreeFile: null });
      return;
    }

    if (file.size > this.maxFileSize) {
      this.fileError =
        this.translate.instant('validation.fileSize', {
          size: this.maxFileSize / 1_000_000,
        }) + 'MB';
      this.form.patchValue({ degreeFile: null });
      return;
    }

    this.form.patchValue({ degreeFile: file });
  }

  clearFile(): void {
    this.form.patchValue({ degreeFile: null });
    this.fileError = null;
  }

  onSave() {
    if (this.form.invalid || this.yearError || !this.form.value.degreeFile) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.value;

    const payload = {
      ...raw,
      gradYear: raw.gradYear
        ? (raw.gradYear instanceof Date
          ? raw.gradYear.getFullYear()
          : new Date(raw.gradYear as any).getFullYear())
        : null,
    };

    this.ref.close(payload);
  }

  onCancel() {
    this.ref.close();
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
