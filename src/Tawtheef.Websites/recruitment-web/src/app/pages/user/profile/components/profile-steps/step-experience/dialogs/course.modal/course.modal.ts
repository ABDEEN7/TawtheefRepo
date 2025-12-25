import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { Button } from 'primeng/button';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DatePicker } from 'primeng/datepicker';
import { InputText } from 'primeng/inputtext';
import {NgClass, NgIf} from '@angular/common';
import {periodRangeValidator} from '../../../../../../../../shared/validator/period-range,validator';
import {dateToDateOnly} from '../../../../../../../../shared/types/dateOnly.type';
import {Select} from 'primeng/select';
import {FileUtilsService} from '../../../../../../../../core/utils/file-utils';
import {EXPERIENCE_DIALOG_LIMITS} from '../dialog-config';
import {ProfileLookupsService} from '../../../../../wizard-profile/services/profile-lookups.service';
import {TrainingCourse} from '../../../../../wizard-profile/models/experience.model';
import {Textarea} from 'primeng/textarea';

@Component({
  selector: 'app-course',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    Button,
    TranslatePipe,
    DatePicker,
    InputText,
    NgClass,
    NgIf,
    Select,
    Textarea,
  ],
  templateUrl: './course.modal.html',
  styleUrl: './course.modal.scss',
})
export class CourseModal implements OnInit {
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

  form: FormGroup = this.fb.group({
    org: ['', [Validators.required, Validators.maxLength(150)]],
    name: ['', [Validators.required, Validators.maxLength(150)]],
    country: [null, [Validators.required]],
    period: [null, [Validators.required, periodRangeValidator]],
    description: ['', [Validators.maxLength(this.limits.descriptionMaxLength)]],
    fileName: [''],
    file: [null, Validators.required],
  });

  ngOnInit(): void {
    if (this.config.data?.initialValue) {
      this.form.patchValue(this.config.data.initialValue);
      const init = this.config.data.initialValue as any;
      this.initialAttachmentUrl = init?.attachment?.url ?? init?.attachmentUrl ?? null;
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
    if (file.size > this.limits.maxFileSizeBytes) {
      this.fileError = this.translate.instant('validation.fileSize', { size: this.limits.maxFileSizeLabel });
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

    const from = period && period.length > 0 ? dateToDateOnly(period[0]) : null;
    const to   = period && period.length > 1 ? dateToDateOnly(period[1]) : null;

    const payload = {
      title: v.name,
      provider: v.org,
      from,
      to,
      country: v.country,
      description: v.description,
      file: v.file,
      fileName: v.file?.name ?? v.fileName ?? null
    } as TrainingCourse;

    this.ref.close(payload);
  }


  onCancel() {
    this.ref.close();
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

  get f() { return this.form.controls; }
}
