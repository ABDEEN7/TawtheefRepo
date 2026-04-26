import {CommonModule} from '@angular/common';
import {
  Component,
  computed,
  EventEmitter,
  inject,
  Input,
  OnChanges,
  OnDestroy,
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
import {EndpointsService} from '../../../../../../core/http/endpoints.service';
import {FileUtilsService} from '../../../../../../core/utils/file-utils';

@Component({
  selector: 'app-university-modal',
  standalone: true,
  templateUrl: './university-modal.component.html',
  styleUrls: ['./university-modal.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, I18nNamespaceDirective]
})
export class UniversityModalComponent implements OnInit, OnChanges, OnDestroy {
  private fb = inject(FormBuilder);
  private languageService = inject(LanguageService);
  private translate = inject(TranslateService);
  private universitiesService = inject(UniversitiesService);
  private endpoints = inject(EndpointsService);
  private fileUtils = inject(FileUtilsService);

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
  logoArPreview = signal<string | null>(null);
  logoEnPreview = signal<string | null>(null);
  logoArError = signal<string | null>(null);
  logoEnError = signal<string | null>(null);

  private logoArObjectUrl: string | null = null;
  private logoEnObjectUrl: string | null = null;

  logoArName = signal<string | null>(null);
  logoEnName = signal<string | null>(null);

  logoArDisplayName = computed(() => this.logoArName() || this.getExistingLogoName('ar'));
  logoEnDisplayName = computed(() => this.logoEnName() || this.getExistingLogoName('en'));

  form = this.fb.nonNullable.group({
    nameAr: ['', Validators.required],
    nameEn: ['', Validators.required],
    descriptionAr: [''],
    descriptionEn: [''],
    countryId: ['', Validators.required],
    cityId: ['', Validators.required],
    webSite: ['', [Validators.pattern(/^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$/)]],
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

  ngOnDestroy(): void {
    this.revokeObjectUrl('ar');
    this.revokeObjectUrl('en');
  }

  private patchForm() {
    this.submitted = false;
    this.logoArFile = null;
    this.logoEnFile = null;
    this.logoArName.set(null);
    this.logoEnName.set(null);
    this.logoArError.set(null);
    this.logoEnError.set(null);


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
      this.setPreview('ar', this.resolveLogoUrl(this.university.logoAr ?? null));
      this.setPreview('en', this.resolveLogoUrl(this.university.logoEn ?? null));
      this.logoArName.set(this.getExistingLogoName('ar'));
      this.logoEnName.set(this.getExistingLogoName('en'));
    } else {
      this.cities.set([]);
      this.setPreview('ar', null);
      this.setPreview('en', null);
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
    this.handleLogoSelection(event, 'ar');
  }

  onLogoEnSelected(event: Event) {
    this.handleLogoSelection(event, 'en');
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isEditMode() {
    return this.mode === 'edit';
  }

  submit() {
    this.submitted = true;
    if (this.form.invalid || this.logoArError() || this.logoEnError()) {
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

  requiredError(controlName: 'nameAr' | 'nameEn' | 'countryId' | 'cityId' | 'email' | 'webSite'): string {
    const control = this.form.controls[controlName];
    if (control.hasError('required')) {
      return this.translate.instant('UNIVERSITIES.FIELD_REQUIRED');
    }
    if (control.hasError('email')) {
      return this.translate.instant('UNIVERSITIES.INVALID_EMAIL');
    }
    if (control.hasError('pattern') && controlName === 'webSite') {
      return this.translate.instant('UNIVERSITIES.INVALID_URL');
    }
    return '';
  }

  private handleLogoSelection(event: Event, type: 'ar' | 'en') {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;

    const errorSignal = type === 'ar' ? this.logoArError : this.logoEnError;
    const nameSignal  = type === 'ar' ? this.logoArName  : this.logoEnName;

    const existingLogo = type === 'ar'
      ? this.university?.logoAr ?? null
      : this.university?.logoEn ?? null;

    const resolvedExisting = this.resolveLogoUrl(existingLogo);

    if (file && !file.type.startsWith('image/')) {
      errorSignal.set(this.translate.instant('UNIVERSITIES.INVALID_LOGO_TYPE'));
      this.setLogoFile(type, null);
      nameSignal.set(this.getExistingLogoName(type));
      this.setPreview(type, resolvedExisting);
      input.value = '';
      return;
    }

    errorSignal.set(null);
    this.setLogoFile(type, file);

    if (file) {
      nameSignal.set(file.name);
      this.setPreview(type, URL.createObjectURL(file), true);
    } else {
      nameSignal.set(this.getExistingLogoName(type));
      this.setPreview(type, resolvedExisting);
    }
  }
  async previewLogo(ev: MouseEvent, type: 'ar' | 'en') {
    ev.stopPropagation();
    ev.preventDefault();

    const file = type === 'ar' ? this.logoArFile : this.logoEnFile;
    const preview = type === 'ar' ? this.logoArPreview() : this.logoEnPreview();

    // 1) If user selected a new file => preview it
    if (file) {
      this.fileUtils.previewBlob(file);
      return;
    }

    // 2) Otherwise preview existing URL (protected endpoint => forceAuthFetch)
    if (preview) {
      await this.fileUtils.previewUrl(preview, '', false);
    }
  }
  clearLogo(ev: MouseEvent, type: 'ar' | 'en') {
    ev.stopPropagation();
    ev.preventDefault();

    const existingLogo = type === 'ar'
      ? this.university?.logoAr ?? null
      : this.university?.logoEn ?? null;

    this.setLogoFile(type, null);

    if (type === 'ar') {
      this.logoArError.set(null);
      this.logoArName.set(this.getExistingLogoName('ar'));
    } else {
      this.logoEnError.set(null);
      this.logoEnName.set(this.getExistingLogoName('en'));
    }

    this.setPreview(type, this.resolveLogoUrl(existingLogo));
  }
  private getExistingLogoName(type: 'ar' | 'en'): string | null {
    const raw = type === 'ar' ? (this.university?.logoAr ?? null) : (this.university?.logoEn ?? null);
    if (!raw) return null;

    // If backend sends a URL: try extract last segment
    if (raw.startsWith('http')) {
      try {
        const u = new URL(raw);
        const last = u.pathname.split('/').filter(Boolean).pop();
        return last ?? null;
      } catch {
        return raw.split('/').pop() ?? null;
      }
    }

    // If it’s an ID, show short friendly label
    return `${this.translate.instant(type === 'ar' ? 'UNIVERSITIES.LOGO_AR' : 'UNIVERSITIES.LOGO_EN')} (${raw.slice(0, 8)}...)`;
  }
  private setLogoFile(type: 'ar' | 'en', file: File | null) {
    if (type === 'ar') {
      this.logoArFile = file;
      return;
    }

    this.logoEnFile = file;
  }

  private setPreview(type: 'ar' | 'en', url: string | null, isObjectUrl = false) {
    this.revokeObjectUrl(type);
    if (type === 'ar') {
      this.logoArPreview.set(url);
      this.logoArObjectUrl = isObjectUrl ? url : null;
      return;
    }

    this.logoEnPreview.set(url);
    this.logoEnObjectUrl = isObjectUrl ? url : null;
  }

  private revokeObjectUrl(type: 'ar' | 'en') {
    if (type === 'ar' && this.logoArObjectUrl) {
      URL.revokeObjectURL(this.logoArObjectUrl);
      this.logoArObjectUrl = null;
      return;
    }

    if (type === 'en' && this.logoEnObjectUrl) {
      URL.revokeObjectURL(this.logoEnObjectUrl);
      this.logoEnObjectUrl = null;
    }
  }

  private resolveLogoUrl(fileId: string | null): string | null {
    if (!fileId) {
      return null;
    }

    if (fileId.startsWith('http')) {
      return fileId;
    }

    return this.endpoints.files.download(fileId);
  }
}
