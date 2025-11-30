import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { Button } from 'primeng/button';
import { FileUpload } from 'primeng/fileupload';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DatePicker } from 'primeng/datepicker';
import { InputText } from 'primeng/inputtext';
import {NgClass, NgIf} from '@angular/common';
import {periodRangeValidator} from '../../../../../../../shared/validator/period-range,validator';

@Component({
  selector: 'app-course',
  standalone: true,
  imports: [
    ReactiveFormsModule,       // مهم لاستخدام formControlName
    Button,
    FileUpload,
    TranslatePipe,
    DatePicker,
    InputText,
    NgClass,
    NgIf,
  ],
  templateUrl: './course.modal.html',
  styleUrl: './course.modal.scss',
})
export class CourseModal implements OnInit {
  private fb = inject(FormBuilder);
  private ref = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig);
  private translate = inject(TranslateService);

  readonly maxFileSize = 1_000_000; // 1MB
  readonly allowedTypes = ['application/pdf', 'image/png', 'image/jpeg', 'image/webp'];
  fileError: string | null = null;

  form: FormGroup = this.fb.group({
    org: ['', [Validators.required, Validators.maxLength(150)]],
    title: ['', [Validators.required, Validators.maxLength(150)]],
    period: [null, [Validators.required, periodRangeValidator]],
    tasks: ['', [Validators.maxLength(500)]],
    fileName: [''],
    file: [null, Validators.required],
  });

  ngOnInit(): void {
    if (this.config.data?.initialValue) {
      this.form.patchValue(this.config.data.initialValue);
    }
  }

  onUpload(evt: any) {
    this.fileError = null;
    const input = evt.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;

    if (!this.allowedTypes.includes(file.type)) {
      this.fileError = this.translate.instant('validation.fileType', { types: 'PDF, PNG, JPEG, WEBP' });
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

  touchPeriod() {
    this.f['period'].markAsTouched();
    this.form.updateValueAndValidity({ onlySelf: false, emitEvent: true });
  }
  onSave() {
    if (this.form.invalid || this.fileError) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.value;
    const period = v.period as Date[] | null;

    const from = period && period.length > 0 ? period[0] : null;
    const to   = period && period.length > 1 ? period[1] : null;

    const payload = {
      org: v.org,
      title: v.title,
      from,
      to,
      tasks: v.tasks,
      file: v.file,
      fileName: v.file?.name ?? v.fileName ?? null
    };

    this.ref.close(payload);
  }


  onCancel() {
    this.ref.close();
  }

  get f() { return this.form.controls; }
}

/** from <= to إذا كانا موجودين */
export function dateRangeValidator(fromKey: string, toKey: string) {
  return (group: AbstractControl): ValidationErrors | null => {
    const from = group.get(fromKey)?.value as Date | null;
    const to = group.get(toKey)?.value as Date | null;
    if (!from || !to) return null;
    const ok = new Date(from).getTime() <= new Date(to).getTime();
    return ok ? null : { dateRange: true };
  };
}
