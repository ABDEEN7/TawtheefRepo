import {Component, EventEmitter, Output, inject, OnInit} from '@angular/core';
import {FormArray, FormBuilder, FormGroup, Validators} from '@angular/forms';
import { DataService } from '../../services/data.service';

@Component({
  selector: 'app-step-attachments',
  templateUrl: './step-attachments.component.html',
  styleUrl: './step-attachments.component.scss',
  standalone: false,
})
export class StepAttachmentsComponent implements OnInit{
  @Output() next = new EventEmitter<void>();
  @Output() back = new EventEmitter<void>();

  ds = inject(DataService);
  private filesStore: (File | null)[] = [];
  form: FormGroup;
  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      rows: this.fb.array([])
    });
  }

  ngOnInit(): void {
    const attachments = this.ds.state().attachments || [];
    attachments.forEach((att: any) => {
      this.rows.push(this.createRow(att.title, att.fileName));
      const i = this.rows.length - 1;
      (this.rows.at(i) as FormGroup).disable({ emitEvent: false });
    });
    if (attachments.length === 0) {
      this.addRow();
    }
  }

  get rows(): FormArray {
    return this.form.get('rows') as FormArray;
  }

  private createRow(title = '', fileName = ''): FormGroup {
    return this.fb.group({
      title: [title, Validators.required],
      file: [null, Validators.required],
      fileName: [fileName]
    });
  }

  addRow(): void {
    this.rows.push(this.createRow());
  }

  removeRow(i: number): void {
    this.rows.removeAt(i);
    this.ds.delAttachment(i);
  }

  confirmRow(i: number): void {
    const g = this.rows.at(i) as FormGroup;
    if (g.invalid) {
      g.markAllAsTouched();
      return;
    }
    const { title, fileName, file } = g.getRawValue();
    this.ds.addAttachment({ title, fileName, file });
    g.disable({ emitEvent: false });
  }

  onFileChange(i: number, ev: Event): void {
    const input = ev.target as HTMLInputElement;
    const file = input.files && input.files[0];
    const grp = this.rows.at(i) as FormGroup;

    if (file) {
      this.filesStore[i] = file; // persist the actual File
      grp.patchValue({ file, fileName: file.name });
      grp.updateValueAndValidity({ emitEvent:false });
    }
  }

  changeFile(i: number): void {
    // clear current selection and show the picker again
    const grp = this.rows.at(i) as FormGroup;
    this.filesStore[i] = null;
    grp.patchValue({ file: null, fileName: '' });
    // We keep controls enabled state as-is; UI will show the input because hasFile(i) becomes false.
  }

  hasFile(i: number): boolean {
    const grp = this.rows.at(i) as FormGroup;
    const name = grp.get('fileName')?.value;
    const file = grp.get('file')?.value;
    // true if either a File is present now OR we restored it from the store
    return !!(name && (file || this.filesStore[i]));
  }

  getFileName(i: number): string {
    const grp = this.rows.at(i) as FormGroup;
    return grp.get('fileName')?.value || (this.filesStore[i]?.name ?? 'No file chosen');
  }

  editRow(i: number): void {
    (this.rows.at(i) as FormGroup).enable({ emitEvent: false });
  }

  isInvalid(i: number, control: 'title' | 'file'): boolean {
    const c = (this.rows.at(i) as FormGroup).get(control);
    return !!c && c.enabled && c.invalid && (c.dirty || c.touched);
  }
}
