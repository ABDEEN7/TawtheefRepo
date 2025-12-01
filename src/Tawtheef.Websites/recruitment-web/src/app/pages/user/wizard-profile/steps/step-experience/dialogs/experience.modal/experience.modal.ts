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
import { FileUpload } from 'primeng/fileupload';
import { InputText } from 'primeng/inputtext';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DatePicker } from 'primeng/datepicker';
import { NgClass, NgIf } from '@angular/common';
import {dateToDateOnly} from '../../../../../../../shared/types/dateOnly.type';

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
    NgClass
  ],
  templateUrl: './experience.modal.html',
  styleUrl: './experience.modal.scss',
})
export class ExperienceModal implements OnInit {
  private fb = inject(FormBuilder);
  private ref = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig);
  private translate = inject(TranslateService);

  readonly maxFileSize = 1_000_000; // 1MB
  readonly allowedTypes = ['application/pdf', 'image/png', 'image/jpeg', 'image/webp'];
  fileError: string | null = null;

  today = new Date();

  form: FormGroup = this.fb.group(
    {
      org: ['', [Validators.required, Validators.maxLength(150)]],
      name: ['', [Validators.required, Validators.maxLength(150)]],
      from: [null, [Validators.required]],
      to: [null],
      current: [false],
      description: ['', [Validators.maxLength(1000)]],
      fileName: [''],
      file: [null, Validators.required], // ✅ required
    },
    {
      validators: [
        toRequiredIfNotCurrent('current', 'to'),
        dateRangeValidator('from', 'to'),
        noFutureValidator('from', 'to'),
      ],
    }
  );

  ngOnInit(): void {
    if (this.config.data?.initialValue) {
      this.form.patchValue(this.config.data.initialValue);
    }
    this.syncToDisabled();
  }

  onCurrentToggle() {
    this.syncToDisabled();
    this.touchDates();
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

    if (file.size > this.maxFileSize) {
      this.fileError = this.translate.instant('validation.fileSize', { size: '1MB' });
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

    const payload = {
      org: v.org,
      name: v.name,
      from: dateToDateOnly(v.from),
      to: dateToDateOnly(v.current ? null : v.to),
      current: !!v.current,
      description: v.description,
      file: v.file,
      fileName: v.file?.name ?? v.fileName ?? null,
    };

    this.ref.close(payload);
  }

  onCancel() {
    this.ref.close();
  }

  get f() {
    return this.form.controls;
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
