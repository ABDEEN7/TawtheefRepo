import { Component, EventEmitter, Output, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DataService } from '../../services/data.service';
import { Attachment } from '../../models/attachment.model';
import {createStepValiditySignal} from '../../state/profile-step-validity.signal';
import {TranslateService} from '@ngx-translate/core';
import {MessageService} from 'primeng/api';
import {ProfileService} from '../../services/profile.service';

@Component({
  selector: 'app-step-attachments',
  templateUrl: './step-attachments.component.html',
  styleUrl: './step-attachments.component.scss',
  standalone: false,
})
export class StepAttachmentsComponent implements OnInit {
  @Output() next = new EventEmitter<void>();
  @Output() back = new EventEmitter<void>();

  ds = inject(DataService);
  private fb = inject(FormBuilder);
  translate = inject(TranslateService);
  messageService = inject(MessageService);
  profile = inject(ProfileService);

  saving = false;

  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['attachments'];
  }

  private filesStore: (File | null)[] = [];

  form: FormGroup = this.fb.group({
    rows: this.fb.array([]),
  });

  ngOnInit(): void {
    const attachments = this.ds.state().attachments || [];
    attachments.forEach((att: any, idx: number) => {
      const row = this.createRow(att.title ?? '', att.fileName ?? '', false);
      this.rows.push(row);
      this.filesStore[idx] = att.file ?? null;
      row.disable({ emitEvent: false });
    });

    if (attachments.length === 0) {
      this.addRow();
    }
  }

  // ======== FormArray helper ========

  get rows(): FormArray {
    return this.form.get('rows') as FormArray;
  }

  private createRow(title = '', fileName = '', existing = false): FormGroup {
    return this.fb.group({
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
  }

  removeRow(i: number): void {
    this.rows.removeAt(i);
    this.filesStore.splice(i, 1);
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
      grp.patchValue({ file, fileName: file.name });
      grp.updateValueAndValidity({ emitEvent: false });
    }
  }

  changeFile(i: number): void {
    const grp = this.rows.at(i) as FormGroup;
    this.filesStore[i] = null;
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
      const { title, fileName } = grp.getRawValue();

      return {
        name: title,
        fileName,
        file: this.filesStore[index] ?? null,
      } as Attachment;
    });

    // حفظ في الـ DataService
    this.ds.up('attachments', attachments as any);

    this.saving = true;
    this.profile.saveAttachmentsSection(attachments).subscribe({
      next: () => {
        this.saving = false;
        this.next.emit();
      },
      error: err => {
        console.error(err);
        this.saving = false;
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('wizard.errorTitle'),
          detail: this.translate.instant('wizard.attachments.saveError'),
          life: 5000,
        });
      },
    });
  }
}
