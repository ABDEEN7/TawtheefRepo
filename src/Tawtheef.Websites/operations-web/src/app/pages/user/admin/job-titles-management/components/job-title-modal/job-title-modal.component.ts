import {CommonModule} from '@angular/common';
import {
  Component,
  computed,
  EventEmitter,
  inject,
  Input,
  OnChanges,
  OnInit,
  Output,
  signal,
  SimpleChanges
} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {Lang, LanguageService} from '../../../../../../core/services/language.service';
import {I18nNamespaceDirective} from '../../../../../../shared/directives/i18n-namespace.directive';
import {JobTitleDto} from '../../models/job-title.dto';

@Component({
  selector: 'app-job-title-modal',
  standalone: true,
  templateUrl: './job-title-modal.component.html',
  styleUrls: ['./job-title-modal.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, I18nNamespaceDirective]
})
export class JobTitleModalComponent implements OnInit, OnChanges {
  private fb = inject(FormBuilder);
  private languageService = inject(LanguageService);
  private translate = inject(TranslateService);

  @Input() visible = false;
  @Input() mode: 'create' | 'edit' = 'create';
  @Input() jobTitle: JobTitleDto | null = null;

  @Output() cancel = new EventEmitter<void>();
  @Output() create = new EventEmitter<JobTitleDto>();
  @Output() update = new EventEmitter<{id: string; payload: JobTitleDto}>();

  currentLang = signal<Lang>(this.languageService.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  submitted = false;

  form = this.fb.nonNullable.group({
    jobNumber: ['', Validators.required],
    jobNameAr: ['', Validators.required],
    jobNameEn: ['', Validators.required]
  });

  ngOnInit(): void {
    this.languageService.current$.subscribe(lang => this.currentLang.set(lang));
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.visible && (changes['visible'] || changes['jobTitle'] || changes['mode'])) {
      this.patchForm();
    }
  }

  private patchForm() {
    this.submitted = false;
    this.form.reset({
      jobNumber: '',
      jobNameAr: '',
      jobNameEn: ''
    });

    if (this.jobTitle) {
      this.form.patchValue({
        jobNumber: this.jobTitle.jobNumber,
        jobNameAr: this.jobTitle.jobNameAr,
        jobNameEn: this.jobTitle.jobNameEn
      });
    }
  }

  isEditMode() {
    return this.mode === 'edit';
  }

  submit() {
    this.submitted = true;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const {jobNumber, jobNameAr, jobNameEn} = this.form.getRawValue();

    if (this.isEditMode() && this.jobTitle?.id) {
      this.update.emit({
        id: this.jobTitle.id,
        payload: {id: this.jobTitle.id, jobNumber, jobNameAr, jobNameEn}
      });
    } else {
      this.create.emit({id: null, jobNumber, jobNameAr, jobNameEn});
    }
  }

  close() {
    this.cancel.emit();
  }

  requiredError(controlName: 'jobNumber' | 'jobNameAr' | 'jobNameEn'): string {
    if (this.form.controls[controlName].hasError('required')) {
      return this.translate.instant('JOB_TITLES.FIELD_REQUIRED');
    }
    return '';
  }
}
