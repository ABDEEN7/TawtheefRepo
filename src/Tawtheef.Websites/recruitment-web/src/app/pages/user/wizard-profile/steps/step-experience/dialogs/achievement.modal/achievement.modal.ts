import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, ValidationErrors, AbstractControl } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { Button } from 'primeng/button';
import { FileUpload } from 'primeng/fileupload';
import { InputText } from 'primeng/inputtext';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DatePicker } from 'primeng/datepicker';
import {NgClass, NgIf} from '@angular/common';

@Component({
  selector: 'app-achievement',
  standalone: true,
  imports: [
    // Reactive forms أساسي هنا
    ReactiveFormsModule,
    FormsModule,        // يمكن إزالته إن لم تعد تستخدم ngModel في مكان آخر
    Button,
    FileUpload,
    InputText,
    TranslatePipe,
    DatePicker,
    NgClass,
    NgIf
  ],
  templateUrl: './achievement.modal.html',
  styleUrl: './achievement.modal.scss',
})
export class AchievementModal {
  private fb = inject(FormBuilder);
  private ref = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig);
  private translate = inject(TranslateService);

  readonly maxFileSize = 1_000_000; // 1MB
  readonly allowedTypes = ['application/pdf', 'image/png', 'image/jpeg', 'image/webp'];
  fileError: string | null = null;

  form: FormGroup = this.fb.group(
    {
      org: ['', [Validators.required, Validators.maxLength(150)]],
      title: ['', [Validators.required, Validators.maxLength(150)]],
      from: [null],
      to: [null],
      tasks: ['', [Validators.maxLength(500)]],
      fileName: [''],
      file: [null]
    },
    { validators: dateRangeValidator('from', 'to') } // تحقق تقاطعي
  );

  ngOnInit(): void {
    if (this.config.data?.initialValue) {
      this.form.patchValue(this.config.data.initialValue);
    }
  }

  onUpload(ev: any) {
    this.fileError = null;
    const f: File | undefined = ev?.files?.[0];
    if (!f) return;

    if (!this.allowedTypes.includes(f.type)) {
      this.fileError = this.translate.instant('validation.fileType', { types: 'PDF, PNG, JPEG, WEBP' });
      this.form.patchValue({ file: null, fileName: '' });
      return;
    }
    if (f.size > this.maxFileSize) {
      this.fileError = this.translate.instant('validation.fileSize', { size: this.maxFileSize / 1_000_000 });
      this.form.patchValue({ file: null, fileName: '' });
      return;
    }

    this.form.patchValue({ file: f, fileName: f.name });
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
    const v = this.form.value;
    const payload = {
      org: v.org,
      title: v.title,
      from: v.from,  // Date | null (عرض/حفظ بصيغة تناسبك)
      to: v.to,
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

/** Validator: from <= to (إن وُجد التاريخان) */
export function dateRangeValidator(fromKey: string, toKey: string) {
  return (group: AbstractControl): ValidationErrors | null => {
    const from = group.get(fromKey)?.value as Date | null;
    const to = group.get(toKey)?.value as Date | null;
    if (!from || !to) return null;
    const ok = new Date(from).getTime() <= new Date(to).getTime();
    return ok ? null : { dateRange: true };
  };
}
