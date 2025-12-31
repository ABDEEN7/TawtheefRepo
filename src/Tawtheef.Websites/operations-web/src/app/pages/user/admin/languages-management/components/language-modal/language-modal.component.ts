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
import {LanguageDto} from '../../models/language.dto';

@Component({
  selector: 'app-language-modal',
  standalone: true,
  templateUrl: './language-modal.component.html',
  styleUrls: ['./language-modal.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, I18nNamespaceDirective]
})
export class LanguageModalComponent implements OnInit, OnChanges {
  private fb = inject(FormBuilder);
  private languageService = inject(LanguageService);
  private translate = inject(TranslateService);

  @Input() visible = false;
  @Input() mode: 'create' | 'edit' = 'create';
  @Input() language: LanguageDto | null = null;
  @Input() loading = false;

  @Output() cancel = new EventEmitter<void>();
  @Output() create = new EventEmitter<LanguageDto>();
  @Output() update = new EventEmitter<{ id: string; payload: LanguageDto }>();

  currentLang = signal<Lang>(this.languageService.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  submitted = false;

  form = this.fb.nonNullable.group({
    nameAr: ['', Validators.required],
    nameEn: ['', Validators.required],
    descriptionAr: [''],
    descriptionEn: [''],
    isActive: [true]
  });

  ngOnInit(): void {
    this.languageService.current$.subscribe(lang => this.currentLang.set(lang));
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.visible && (changes['visible'] || changes['language'] || changes['mode'])) {
      this.patchForm();
    }
  }

  private patchForm() {
    this.submitted = false;
    this.form.reset({
      nameAr: '',
      nameEn: '',
      descriptionAr: '',
      descriptionEn: '',
      isActive: true
    });

    if (this.language) {
      this.form.patchValue({
        nameAr: this.language.nameAr,
        nameEn: this.language.nameEn,
        descriptionAr: this.language.descriptionAr || '',
        descriptionEn: this.language.descriptionEn || '',
        isActive: this.language.isActive
      });
    }
  }

  isCreateMode() {
    return this.mode === 'create';
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

    const {nameAr, nameEn, descriptionAr, descriptionEn, isActive} = this.form.getRawValue();

    if (this.isEditMode() && this.language && this.language.id) {
      this.update.emit({
        id: this.language.id,
        payload: {nameAr, nameEn, descriptionAr, descriptionEn, isActive}
      });
    } else {
      this.create.emit({id: null, nameAr, nameEn, descriptionAr, descriptionEn, isActive});
    }
  }

  close() {
    this.cancel.emit();
  }

  requiredError(controlName: 'nameAr' | 'nameEn'): string {
    if (this.form.controls[controlName].hasError('required')) {
      return this.translate.instant('LANGUAGES.FIELD_REQUIRED');
    }
    return '';
  }
}
