import {
  Component,
  EventEmitter,
  inject,
  Input,
  isDevMode,
  OnInit,
  Output,
  computed,
  input,
  output,
  ChangeDetectionStrategy,
  signal
} from '@angular/core';
import { CommonModule, NgClass } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { FaDirArrowDirective } from '../../../../../../shared/directives/dir-arrow.directive';
import { ProfileDataService } from '../../../wizard-profile/services/profile-data.service';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';
import { UploadedFileRef } from '../../../wizard-profile/models/profile-state.model';
import { Attachment } from '../../../wizard-profile/models/attachment.model';
import { NotificationService } from '../../../../../../core/services/notification.service';

@Component({
  selector: 'app-step-attachments',
  templateUrl: './step-attachments.component.html',
  styleUrl: './step-attachments.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TranslatePipe,
    ButtonModule,
    TooltipModule,
    FaDirArrowDirective
  ]
})
export class StepAttachmentsComponent implements OnInit {
  next = output<void>();
  back = output<void>();
  submitLabelKey = input<string>('wizard.buttons.next');
  showBack = input<boolean>(true);
  requireChanges = input<boolean>(false);
  private readonly maxFileSizeBytes = 5 * 1024 * 1024; // 5MB
  private readonly textPattern = /^[\p{L}\p{N}\s]*$/u;
  private readonly allowedMimeTypes = new Set<string>([
    'application/pdf',
    'image/jpeg',
    'image/png',
  ]);

  ds = inject(ProfileDataService);
  private fb = inject(FormBuilder);
  translate = inject(TranslateService);
  notificationService = inject(NotificationService);
  profile = inject(ProfileService);
  fileUtils = inject(FileUtilsService);

  saving = signal(false);
  private lastSubmittedSignature: string | null = null;

  step = computed(() => this.ds.stepValidationDetailed().attachments);

  private filesStore: (File | null)[] = [];
  private fileRefs: (UploadedFileRef | null)[] = [];

  form: FormGroup = this.fb.group({
    rows: this.fb.array([]),
  });

  ngOnInit(): void {
    const attachments = this.ds.state().attachments || [];
    attachments.forEach((att: any, idx: number) => {
      const row = this.createRow(
        att.title ?? '',
        att.fileName ?? att.fileRef?.resourceName ?? att.name ?? '',
        true,
        att.id,
        att.attachmentId
      );
      this.rows.push(row);
      this.filesStore[idx] = att.file ?? null;
      this.fileRefs[idx] = att.fileRef ?? null;
    });

    this.lastSubmittedSignature = null;
  }

  // ======== FormArray helper ========

  get rows(): FormArray {
    return this.form.get('rows') as FormArray;
  }

  private createRow(title = '', fileName = '', existing = false, id?: string, attachmentId?: string): FormGroup {
    return this.fb.group({
      id: [id ?? null],
      attachmentId: [attachmentId ?? null],
      title: [title, [Validators.required, Validators.pattern(this.textPattern)]],
      file: [
        null,
        existing ? [] : [Validators.required],
      ],
      fileName: [fileName],
    });
  }

  // ======== Row actions ========


  sanitizeTitle(i: number, event: Event): void {
    const input = event.target as HTMLInputElement;
    const sanitized = (input.value ?? '').replace(/[^\p{L}\p{N}\s]/gu, '').slice(0, 150);
    if (sanitized !== input.value) {
      input.value = sanitized;
      (this.rows.at(i) as FormGroup).get('title')?.setValue(sanitized, { emitEvent: false });
    }
  }


  addRow(): void {
    this.rows.push(this.createRow());
    this.filesStore.push(null);
    this.fileRefs.push(null);
  }

  removeRow(i: number): void {
    this.rows.removeAt(i);
    this.filesStore.splice(i, 1);
    this.fileRefs.splice(i, 1);
  }

  confirmRow(i: number): void {
    const g = this.rows.at(i) as FormGroup;
    if (g.invalid) {
      g.markAllAsTouched();
      return;
    }
    g.disable({ emitEvent: false });
  }

  editRow(i: number): void {
    (this.rows.at(i) as FormGroup).enable({ emitEvent: false });
  }

  // ======== File handling ========

  onFileChange(i: number, ev: Event): void {
    const input = ev.target as HTMLInputElement;
    const file = input.files && input.files[0];
    const grp = this.rows.at(i) as FormGroup;

    if (!file) return;

    // 1) النوع
    if (!this.isAllowedFile(file)) {
      this.rejectFile(
        i,
        input,
        this.translate.instant('wizard.attachments.onlyPdfOrImages')
      );
      return;
    }

    // 2) الحجم
    if (file.size > this.maxFileSizeBytes) {
      this.rejectFile(
        i,
        input,
        this.translate.instant('wizard.attachments.maxSize5mb')
      );
      return;
    }

    // قبول الملف
    this.filesStore[i] = file;
    this.fileRefs[i] = null;

    grp.get('file')?.setErrors(null);
    grp.patchValue({ file, fileName: file.name }, { emitEvent: false });
    grp.updateValueAndValidity({ emitEvent: false });
  }

  changeFile(i: number): void {
    const grp = this.rows.at(i) as FormGroup;
    this.filesStore[i] = null;
    this.fileRefs[i] = null;
    grp.patchValue({ file: null, fileName: '' });
  }

  hasFile(i: number): boolean {
    const grp = this.rows.at(i) as FormGroup;
    const fileName = grp.get('fileName')?.value;
    const file = grp.get('file')?.value;
    return !!fileName || !!file || !!this.filesStore[i];
  }

  getFileName(i: number): string {
    const grp = this.rows.at(i) as FormGroup;
    return (
      grp.get('fileName')?.value ||
      this.filesStore[i]?.name ||
      'No file chosen'
    );
  }

  isInvalid(i: number, control: 'title' | 'file'): boolean {
    const c = (this.rows.at(i) as FormGroup).get(control);
    return !!c && c.enabled && c.invalid && (c.dirty || c.touched);
  }

  // ======== Navigation ========

  onBack(): void {
    this.back.emit();
  }

  onNext(): void {
    this.rows.controls.forEach(g => {
      const grp = g as FormGroup;
      if (grp.enabled) {
        grp.markAllAsTouched();
      }
    });

    if (this.form.invalid) {
      this.notificationService.error(this.translate.instant('wizard.attachments.empty'), this.translate.instant('wizard.validationErrorTitle'));
      return;
    }

    const attachments: Attachment[] = this.rows.controls.map((g, index) => {
      const grp = g as FormGroup;
      const { title, fileName, id, attachmentId } = grp.getRawValue();

      return {
        id,
        attachmentId,
        title,
        fileName,
        file: this.filesStore[index] ?? null,
        fileRef: this.fileRefs[index] ?? null,
      } as Attachment;
    });

    // حفظ في الـ ProfileDataService
    this.ds.up('attachments', attachments as any);

    const signature = this.buildSignature(attachments);
    if (signature && signature === this.lastSubmittedSignature) {
      if (this.requireChanges()) {
        this.notificationService.error(this.translate.instant('profileView.notifications.noChanges'));
        return;
      }
      this.notificationService.info(this.translate.instant('profileView.notifications.noChanges'));
      this.next.emit();
      return;
    }

    this.saving.set(true);
    this.profile.saveAttachmentsSection(attachments).subscribe({
      next: () => {
        this.saving.set(false);
        this.lastSubmittedSignature = signature;
        this.ds.markStepSubmitted('attachments');
        if (this.profile.isChangeRequestMode()) {
          this.notificationService.success(this.translate.instant('profileView.notifications.changeRequestSent'));
        }
        this.next.emit();
      },
      error: (err: any) => {
        if (isDevMode())
          console.error(err);
        this.saving.set(false);
      },
    });
  }

  previewFile(i: number, ev?: Event): void {
    ev?.stopPropagation();
    const local = this.filesStore[i];
    if (local) {
      this.fileUtils.previewBlob(local);
      return;
    }

    const ref = this.fileRefs[i];
    if (ref?.url) {
      this.fileUtils.previewUrl(ref.url, ref.resourceName || this.getFileName(i), false);
    }
  }

  private buildSignature(attachments: Attachment[]): string {
    return JSON.stringify(
      (attachments ?? []).map((a, index) => ({
        id: a.id ?? null,
        attachmentId: a.attachmentId ?? null,
        title: a.title ?? '',
        fileName: a.file?.name ?? a.fileName ?? a.fileRef?.resourceName ?? null,
        refId: a.fileRef?.resourceId ?? null,
        localStoreName: this.filesStore[index]?.name ?? null,
      }))
    );
  }

  canPreview(i: number): boolean {
    return !!this.filesStore[i] || !!this.fileRefs[i]?.url;
  }

  private isAllowedFile(file: File): boolean {
    // بعض المتصفحات ممكن ترجع type فاضي => نتحقق من الامتداد كخطة B
    if (file.type && this.allowedMimeTypes.has(file.type)) return true;

    const name = (file.name || '').toLowerCase();
    const ext = name.split('.').pop() || '';
    return ['pdf', 'jpg', 'jpeg', 'png'].includes(ext);
  }

  private rejectFile(i: number, input: HTMLInputElement, message: string): void {
    // امسح الاختيار من الـ input ومن الفورم + المخزن
    input.value = '';
    this.filesStore[i] = null;
    this.fileRefs[i] = null;

    const grp = this.rows.at(i) as FormGroup;
    grp.patchValue({ file: null, fileName: '' }, { emitEvent: false });
    grp.get('file')?.markAsTouched();
    grp.get('file')?.setErrors({ invalidFile: true });

    this.notificationService.error(message, this.translate.instant('wizard.validationErrorTitle'));
  }
}
