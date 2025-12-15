import {CommonModule} from '@angular/common';
import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, computed, inject, signal} from '@angular/core';
import {AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {Lang, LanguageService} from '../../../../../../core/services/language.service';
import {I18nNamespaceDirective} from '../../../../../../shared/directives/i18n-namespace.directive';
import {CountryLookupDto} from '../../models/country-lookup.dto';
import {CreateOfficeRequest} from '../../models/create-office-request.dto';
import {UpdateOfficeRequest} from '../../models/update-office-request.dto';
import {OfficeDto} from '../../models/office.dto';

@Component({
  selector: 'app-office-modal',
  standalone: true,
  templateUrl: './office-modal.component.html',
  styleUrls: ['./office-modal.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, I18nNamespaceDirective]
})
export class OfficeModalComponent implements OnInit, OnChanges {
  private fb = inject(FormBuilder);
  private language = inject(LanguageService);
  private translate = inject(TranslateService);

  @Input() visible = false;
  @Input() isEditing = false;
  @Input() countries: CountryLookupDto[] = [];
  @Input() office: OfficeDto | null = null;

  @Output() cancel = new EventEmitter<void>();
  @Output() create = new EventEmitter<CreateOfficeRequest>();
  @Output() update = new EventEmitter<{ id: string; payload: UpdateOfficeRequest }>();

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  submitted = false;

  private readonly supportedRequiredValidator = (control: AbstractControl<string[]>): ValidationErrors | null => {
    const value = control.value || [];
    return value.length > 0 ? null : { required: true };
  };

  form = this.fb.nonNullable.group({
    nameAr: ['', Validators.required],
    nameEn: ['', Validators.required],
    countryId: ['', Validators.required],
    supportedCountryIds: this.fb.nonNullable.control<string[]>([], this.supportedRequiredValidator),
    adminEmail: ['', [Validators.required, Validators.email]]
  });

  ngOnInit(): void {
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible'] && this.visible) {
      this.patchForm();
    }
  }

  private patchForm() {
    this.submitted = false;
    this.form.reset({
      nameAr: '',
      nameEn: '',
      countryId: '',
      supportedCountryIds: [],
      adminEmail: ''
    });

    if (this.office) {
      this.form.patchValue({
        nameAr: this.office.nameAr,
        nameEn: this.office.nameEn,
        countryId: this.office.countryId,
        supportedCountryIds: this.office.supportedCountries?.map(sc => sc.countryId) || [],
        adminEmail: this.office.adminEmail
      });
      this.form.controls.countryId.disable();
      this.form.controls.adminEmail.disable();
    } else {
      this.form.controls.countryId.enable();
      this.form.controls.adminEmail.enable();
    }
  }

  isSupportedSelected(countryId: string) {
    return this.form.controls.supportedCountryIds.value.includes(countryId);
  }

  toggleSupportedCountry(countryId: string) {
    const current = this.form.controls.supportedCountryIds.value;
    const exists = current.includes(countryId);
    const updated = exists ? current.filter(id => id !== countryId) : [...current, countryId];
    this.form.controls.supportedCountryIds.setValue(updated);
    this.form.controls.supportedCountryIds.markAsDirty();
  }

  localizedCountry(nameAr: string, nameEn: string) {
    return this.currentLang() === 'ar' ? nameAr || nameEn : nameEn || nameAr;
  }

  submit() {
    this.submitted = true;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const {nameAr, nameEn, countryId, supportedCountryIds, adminEmail} = this.form.getRawValue();

    if (this.isEditing && this.office) {
      this.update.emit({
        id: this.office.id,
        payload: {
          nameAr,
          nameEn,
          supportedCountryIds
        }
      });
    } else {
      this.create.emit({
        nameAr,
        nameEn,
        countryId,
        supportedCountryIds,
        adminEmail
      });
    }
  }

  close() {
    this.cancel.emit();
  }

  emailError(): string {
    if (this.form.controls.adminEmail.hasError('required')) {
      return this.translate.instant('OFFICES.FIELD_REQUIRED');
    }
    if (this.form.controls.adminEmail.hasError('email')) {
      return this.translate.instant('OFFICES.INVALID_EMAIL');
    }
    return '';
  }
}
