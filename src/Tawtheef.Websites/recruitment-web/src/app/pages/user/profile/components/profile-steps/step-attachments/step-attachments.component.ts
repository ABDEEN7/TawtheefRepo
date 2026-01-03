import { Component, EventEmitter, Output, inject, OnInit } from '@angular/core';
import {FormArray, FormBuilder, FormGroup, Validators} from '@angular/forms';
import {TranslateService} from '@ngx-translate/core';
import {MessageService} from 'primeng/api';
import {createStepValiditySignal} from '../../../wizard-profile/state/profile-step-validity.signal';
import {ProfileDataService} from '../../../wizard-profile/services/profile-data.service';
import {ProfileService} from '../../../wizard-profile/services/profile.service';
import {FileUtilsService} from '../../../../../../core/utils/file-utils';
import {UploadedFileRef} from '../../../wizard-profile/models/profile-state.model';
import {Attachment} from '../../../wizard-profile/models/attachment.model';

@Component({
  selector: 'app-step-attachments',
  templateUrl: './step-attachments.component.html',
  styleUrl: './step-attachments.component.scss',
  standalone: false
})
export class StepAttachmentsComponent implements OnInit {
  @Output() next = new EventEmitter<void>();
  @Output() back = new EventEmitter<void>();

  ds = inject(ProfileDataService);
  private fb = inject(FormBuilder);
  translate = inject(TranslateService);
  messageService = inject(MessageService);
  profile = inject(ProfileService);
  fileUtils = inject(FileUtilsService);

  saving = false;
  private lastSubmittedSignature: string | null = null;

  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['attachments'];
  }

  private filesStore: (File | null)[] = [];
  private fileRefs: (UploadedFileRef | null)[] = [];

  form: FormGroup = this.fb.group({
    rows: this.fb.array([]),
  });

  ngOnInit(): void {
    const attachments = this.ds.state().attachments || [];
    attachments.forEach((att: any, idx: number) => {
      const row = this.createRow(att.name ?? '', att.fileName ?? att.fileRef?.resourceName ?? att.name ?? '', true, att.id, att.attachmentId);
      this.rows.push(row);
      this.filesStore[idx] = att.file ?? null;
      this.fileRefs[idx] = att.fileRef ?? null;
      row.disable({ emitEvent: false });
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
      title: [title, Validators.required],
      file: [
        null,
        existing ? [] : [Validators.required],
      ],
      fileName: [fileName],
    });
  }

  // ======== Row actions ========

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

    if (file) {
      this.filesStore[i] = file;
      this.fileRefs[i] = null;
      grp.patchValue({ file, fileName: file.name });
      grp.updateValueAndValidity({ emitEvent: false });
    }
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
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('wizard.validationErrorTitle'),
        detail: this.translate.instant('wizard.attachments.empty'),
        life: 5000,
      });
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
      this.next.emit();
      return;
    }

    this.saving = true;
    this.profile.saveAttachmentsSection(attachments).subscribe({
      next: () => {
        this.saving = false;
        this.lastSubmittedSignature = signature;
        this.next.emit();
      },
      error: (err: any) => {
        console.error(err);
        this.saving = false;
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
}
