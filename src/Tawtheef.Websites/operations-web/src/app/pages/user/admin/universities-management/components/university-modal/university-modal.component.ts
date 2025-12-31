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
import {UniversityDto} from '../../models/university.dto';
import {dropdownOptionsModel} from '../../../../../../shared/models/dropdown-options.model';
import {UniversitiesService} from '../../services/universities.service';
import {UniversityFormPayload} from '../../models/university-form.payload';
import {finalize} from 'rxjs/operators';

@Component({
  selector: 'app-university-modal',
  standalone: true,
  templateUrl: './university-modal.component.html',
  styleUrls: ['./university-modal.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, I18nNamespaceDirective]
})
export class UniversityModalComponent implements OnInit, OnChanges {
  private fb = inject(FormBuilder);
  private languageService = inject(LanguageService);
  private translate = inject(TranslateService);
  private universitiesService = inject(UniversitiesService);

  @Input() visible = false;
  @Input() mode: 'create' | 'edit' = 'create';
  @Input() university: UniversityDto | null = null;
  @Input() loading = false;
  @Input() countries: dropdownOptionsModel[] = [];

  @Output() cancel = new EventEmitter<void>();
  @Output() create = new EventEmitter<UniversityFormPayload>();
  @Output() update = new EventEmitter<{ id: string; payload: UniversityFormPayload }>();

  currentLang = signal<Lang>(this.languageService.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  submitted = false;
  cities = signal<dropdownOptionsModel[]>([]);
  citiesLoading = signal(false);

  logoArFile: File | null = null;
  logoEnFile: File | null = null;

  form = this.fb.nonNullable.group({
    nameAr: ['', Validators.required],
    nameEn: ['', Validators.required],
    descriptionAr: [''],
    descriptionEn: [''],
    countryId: ['', Validators.required],
    cityId: ['', Validators.required],
    webSite: [''],
    phone: [''],
    email: ['', Validators.email],
    code: [''],
    originalName: [''],
    isActive: [true]
  });

  ngOnInit(): void {
    this.languageService.current$.subscribe(lang => this.currentLang.set(lang));
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.visible && (changes['visible'] || changes['university'] || changes['mode'])) {
      this.patchForm();
    }
  }

  private patchForm() {
    this.submitted = false;
    this.logoArFile = null;
    this.logoEnFile = null;

    this.form.reset({
      nameAr: '',
      nameEn: '',
      descriptionAr: '',
      descriptionEn: '',
      countryId: '',
      cityId: '',
      webSite: '',
      phone: '',
      email: '',
      code: '',
      originalName: '',
      isActive: true
    });

    if (this.university) {
      this.form.patchValue({
        nameAr: this.university.nameAr,
        nameEn: this.university.nameEn,
        descriptionAr: this.university.descriptionAr || '',
        descriptionEn: this.university.descriptionEn || '',
        countryId: this.university.countryId,
        cityId: this.university.cityId,
        webSite: this.university.webSite || '',
        phone: this.university.phone || '',
        email: this.university.email || '',
        code: this.university.code || '',
        originalName: this.university.originalName || '',
        isActive: this.university.isActive
      });
      if (this.university.countryId) {
        this.loadCities(this.university.countryId, true);
      }
    } else {
      this.cities.set([]);
    }
  }

  onCountryChange(event: Event) {
    const value = (event.target as HTMLSelectElement).value;
    this.form.patchValue({cityId: ''});
    this.cities.set([]);
    if (value) {
      this.loadCities(value);
    }
  }

  private loadCities(countryId: string, keepSelection = false) {
    this.citiesLoading.set(true);
    this.universitiesService.getCities(countryId)
      .pipe(finalize(() => this.citiesLoading.set(false)))
      .subscribe({
        next: res => {
          this.cities.set(res);
          if (!keepSelection) {
            this.form.patchValue({cityId: ''});
          }
        },
        error: () => {
          this.cities.set([]);
        }
      });
  }

  onLogoArSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    this.logoArFile = input.files?.[0] ?? null;
  }

  onLogoEnSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    this.logoEnFile = input.files?.[0] ?? null;
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

    const payload: UniversityFormPayload = {
      id: this.university?.id,
      nameAr: this.form.controls.nameAr.value,
      nameEn: this.form.controls.nameEn.value,
      descriptionAr: this.form.controls.descriptionAr.value || undefined,
      descriptionEn: this.form.controls.descriptionEn.value || undefined,
      countryId: this.form.controls.countryId.value,
      cityId: this.form.controls.cityId.value,
      webSite: this.form.controls.webSite.value || undefined,
      phone: this.form.controls.phone.value || undefined,
      email: this.form.controls.email.value || undefined,
      code: this.form.controls.code.value || undefined,
      originalName: this.form.controls.originalName.value || undefined,
      isActive: this.form.controls.isActive.value,
      logoArFile: this.logoArFile,
      logoEnFile: this.logoEnFile
    };

    if (this.isEditMode() && this.university?.id) {
      this.update.emit({id: this.university.id, payload});
    } else {
      this.create.emit(payload);
    }
  }

  close() {
    this.cancel.emit();
  }

  requiredError(controlName: 'nameAr' | 'nameEn' | 'countryId' | 'cityId' | 'email'): string {
    const control = this.form.controls[controlName];
    if (control.hasError('required')) {
      return this.translate.instant('UNIVERSITIES.FIELD_REQUIRED');
    }
    if (control.hasError('email')) {
      return this.translate.instant('UNIVERSITIES.INVALID_EMAIL');
    }
    return '';
  }
}
