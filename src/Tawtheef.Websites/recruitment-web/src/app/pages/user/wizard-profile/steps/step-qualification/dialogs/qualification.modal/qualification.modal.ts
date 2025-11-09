import {Component, EventEmitter, inject, Input, OnInit, Output} from '@angular/core';
import {Button} from 'primeng/button';
import {FormBuilder, FormGroup, FormsModule, NgForm, Validators} from '@angular/forms';
import {InputNumber} from 'primeng/inputnumber';
import {FileUpload} from 'primeng/fileupload';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {Select} from 'primeng/select';
import {DynamicDialogConfig, DynamicDialogRef} from 'primeng/dynamicdialog';
import {DatePicker} from 'primeng/datepicker';
import {NgClass, NgIf} from '@angular/common';

@Component({
  selector: 'app-qualification',
  imports: [
    Button,
    FormsModule,
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
export class QualificationModal implements OnInit{
  private fb = inject(FormBuilder);
  private ref = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig);
  private translate = inject(TranslateService);

  degrees = [ { id: 1, name: "ابتدائية" }, { id: 2, name: "إعدادية" }, { id: 3, name: "ثانوية" }, { id: 4, name: "دبلوم" }, { id: 5, name: "بكالوريوس" }, { id: 6, name: "ماجستير" }, { id: 7, name: "دكتوراه" } ];
  majors = [ { id: 1, name: "اللغة الإنجليزية" }, { id: 2, name: "تربية" }, { id: 3, name: "رياضيات" }, { id: 4, name: "علوم" }, { id: 5, name: "فيزياء" }, { id: 6, name: "كيمياء" }, { id: 7, name: "أحياء" }, { id: 8, name: "حاسوب" } ];
  universities = [ { id: 1, name: "جامعة قطر" }, { id: 2, name: "الجامعة العربية المفتوحة" }, { id: 3, name: "جامعة الخليج" }, { id: 4, name: "جامعة حمد بن خليفة" }, { id: 5, name: "جامعة تكساس إيه آند إم" }, { id: 6, name: "جامعة نورثويسترن في قطر" }, { id: 7, name: "جامعة ويسترن ميشيغان" } ];
  gradCountries = [ { id: 1, name: "قطر" }, { id: 2, name: "الأردنية" }, { id: 3, name: "القاهرة" }, { id: 4, name: "الإسكندرية" }, { id: 5, name: "الخرطوم" }, { id: 6, name: "تونس" }, { id: 7, name: "الرباط" }, { id: 8, name: "الملك سعود" }, { id: 9, name: "الإمارات" } ];

  studySystems = [ { id: 1, name: 'انتظام' }, { id: 2, name: 'انتساب' }, { id: 3, name: 'تعليم مدمج' }];
  grades = [ { id: 1, name: 'مقبول' }, { id: 2, name: 'جيد' }, { id: 3, name: 'جيد جدًا' }, { id: 4, name: 'امتياز' }];

  minYear = 1970;
  maxYear = new Date().getFullYear();
  yearError = false;

  maxFileSize = 1_000_000; // 1MB
  fileError: string | null = null;
  allowedTypes = ['application/pdf', 'image/png', 'image/jpeg', 'image/webp'];


  form: FormGroup = this.fb.group({
    degree: [null, Validators.required],
    gradCountry: [null],
    university: [null, Validators.required],
    major: [null, Validators.required],
    gradYear: [null, [Validators.required, Validators.min(1970), Validators.max(2035)]],
    studySystem: [null, Validators.required],
    gpa: [null, [Validators.required, Validators.pattern(/^\d+(\.\d{1,2})?$/)]],
    grade: [null, Validators.required],
    degreeFile: [null]
  });
  ngOnInit() {
    if (this.config.data && this.config.data.initialValue) {
      this.form.patchValue(this.config.data.initialValue);
    }
  }
  onUpload(evt: any): void {
    this.fileError = null;
    const file: File | undefined = evt?.files?.[0];
    if (!file) { return; }

    if (!this.allowedTypes.includes(file.type)) {
      this.fileError = this.translate.instant('validation.fileType') + ': PDF, PNG, JPEG, WEBP';
      this.form.value.degreeFile = null;
      return;
    }
    if (file.size > this.maxFileSize) {
      this.fileError = this.translate.instant('validation.fileSize', { size: this.maxFileSize / 1000000 }) + 'MB';
      this.form.value.degreeFile = null;
      return;
    }

    this.form.value.degreeFile = file;
  }

  clearFile(): void {
    this.form.value.degreeFile = null;
    this.fileError = null;
  }

  onSave(formRef: NgForm) {
    if (formRef.invalid || this.yearError || !this.form.value.degreeFile) {
      Object.values(formRef.controls).forEach(c => c.markAsTouched());
      return;
    }

    // البيانات النهائية جاهزة للإرسال
    const payload = {
      ...this.form.value,
      gradYear: this.form.value.gradYear
        ? (this.form.value.gradYear as Date).getFullYear()
        : null
    };
    this.ref.close(payload);
  }

  onCancel() {
    this.ref.close();
  }

  validateYear(): void {
    const d = this.form.value.gradYear;
    if (!d) { this.yearError = false; return; }
    const y = d instanceof Date ? d.getFullYear() : new Date(d).getFullYear();
    this.yearError = !(y >= this.minYear && y <= this.maxYear);
  }

  // helpers
  get f() { return this.form.controls; }
}
