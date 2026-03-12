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
import {AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {Lang, LanguageService} from '../../../../../../core/services/language.service';
import {I18nNamespaceDirective} from '../../../../../../shared/directives/i18n-namespace.directive';
import {CreateOfficeRequest} from '../../models/create-office-request.dto';
import {UpdateOfficeRequest} from '../../models/update-office-request.dto';
import {OfficeUserDto} from '../../models/office-user.dto';
import {Select} from 'primeng/select';
import {dropdownOptionsModel} from '../../../../../../shared/models/dropdown-options.model';
import {OfficeDetailsDto} from '../../models/office-details.dto';
import {CountryISO, NgxIntlTelInputModule, SearchCountryField} from 'ngx-intl-tel-input';

type PhoneNumberValue = {
  number: string;
  internationalNumber: string;
  nationalNumber: string;
  e164Number: string;
  countryCode: string;
  dialCode: string;
};

@Component({
  selector: 'app-office-modal',
  standalone: true,
  templateUrl: './office-modal.component.html',
  styleUrls: ['./office-modal.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    I18nNamespaceDirective,
    Select,
    NgxIntlTelInputModule
  ]
})
export class OfficeModalComponent implements OnInit, OnChanges {
  private fb = inject(FormBuilder);
  private language = inject(LanguageService);
  private translate = inject(TranslateService);

  @Input() visible = false;
  @Input() mode: 'create' | 'edit' | 'view' = 'create';
  @Input() countries: dropdownOptionsModel[] = [];
  @Input() assignedCountryIds: string[] = [];
  @Input() office: OfficeDetailsDto | null = null;
  @Input() officeUsers: OfficeUserDto[] = [];
  @Input() loading = false;

  searchCountryFields = [SearchCountryField.Iso2, SearchCountryField.Name];
  selectedCountryIso = signal<CountryISO>(CountryISO.Qatar);

  availableCountries = computed(() => {
    const assigned = new Set(this.assignedCountryIds);
    const currentCountryId = this.office?.countryId;

    return this.countries.filter(country => !assigned.has(country.id) || country.id === currentCountryId);
  });

  phoneCountries = computed(() => {
    const countries = this.countries
      .map(country => this.getCountryIso(country))
      .filter((country): country is CountryISO => Boolean(country));
    return countries.length ? countries : undefined;
  });

  @Output() cancel = new EventEmitter<void>();
  @Output() create = new EventEmitter<CreateOfficeRequest>();
  @Output() update = new EventEmitter<{ id: string; payload: UpdateOfficeRequest }>();
  @Output() toggleBlock = new EventEmitter<{ userId: string; isBlocked: boolean }>();
  @Output() makeAdmin = new EventEmitter<string>();

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
    adminNameAr: ['', Validators.required],
    adminNameEn: ['', Validators.required],
    adminEmail: ['', [Validators.required, Validators.email]],
    phoneNumber: this.fb.control<PhoneNumberValue | null>(null, Validators.required)
  });

  ngOnInit(): void {
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
    this.form.controls.countryId.valueChanges.subscribe(countryId => {
      this.syncCountryFields(countryId);
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.visible && (changes['visible'] || changes['office'] || changes['officeUsers'] || changes['mode'])) {
      this.patchForm();
    }
  }

  private patchForm() {
    this.submitted = false;
    this.form.enable({emitEvent: false});
    this.form.reset({
      nameAr: '',
      nameEn: '',
      countryId: '',
      supportedCountryIds: [],
      adminNameAr: '',
      adminNameEn: '',
      adminEmail: this.resolveAdminEmail(),
      phoneNumber: null
    });

    if (this.office) {
      this.form.patchValue({
        nameAr: this.office.nameAr,
        nameEn: this.office.nameEn,
        countryId: this.office.countryId,
        supportedCountryIds: this.office.countryId ? [this.office.countryId] : [],
        adminNameAr: this.office.adminNameAr || '',
        adminNameEn: this.office.adminNameEn || '',
        adminEmail: this.resolveAdminEmail(),
        phoneNumber: this.buildPhoneValue()
      });
    }

    this.syncCountryFields(this.form.controls.countryId.value);

    if (this.mode !== 'create') {
      this.form.controls.countryId.disable({emitEvent: false});
    }

    if (this.isViewMode()) {
      this.form.disable({emitEvent: false});
    }
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isEditMode() {
    return this.mode === 'edit';
  }

  isViewMode() {
    return this.mode === 'view';
  }

  isOfficeAdmin(user: OfficeUserDto) {
    return user.isAdmin || user.email === this.office?.adminEmail;
  }

  private resolveAdminEmail() {
    const adminUser = this.officeUsers.find(u => u.isAdmin);
    if (adminUser) {
      return adminUser.email;
    }

    return this.office?.adminEmail || '';
  }

  localizedCountry(nameAr: string, nameEn: string) {
    return this.currentLang() === 'ar' ? nameAr || nameEn : nameEn || nameAr;
  }

  localizedUserName(user: OfficeUserDto) {
    return this.currentLang() === 'ar'
      ? user.fullNameAr || user.fullNameEn
      : user.fullNameEn || user.fullNameAr;
  }

  toggleUserBlock(user: OfficeUserDto) {
    this.toggleBlock.emit({userId: user.id, isBlocked: !user.isBlocked});
  }

  promoteToAdmin(user: OfficeUserDto) {
    if (this.isOfficeAdmin(user)) {
      return;
    }

    this.makeAdmin.emit(user.id);
  }

  private syncCountryFields(countryId: string) {
    if (!countryId) {
      this.form.controls.supportedCountryIds.setValue([], {emitEvent: false});
      return;
    }

    this.form.controls.supportedCountryIds.setValue([countryId], {emitEvent: false});

    const selectedCountry = this.availableCountries().find(country => country.id === countryId);
    this.selectedCountryIso.set(this.getCountryIso(selectedCountry));
  }

  private formatDialCode(value?: string | number) {
    const rawValue = value?.toString().trim() || '';
    if (!rawValue) {
      return '';
    }
    return rawValue.startsWith('+') ? rawValue : `+${rawValue}`;
  }

  private buildPhoneValue(): PhoneNumberValue | null {
    if (!this.office?.phoneNumber || !this.office?.phoneCountryCode) {
      return null;
    }

    const dialCode = this.formatDialCode(this.office.phoneCountryCode);
    const country = this.availableCountries().find(item => item.id === this.office?.countryId);
    const countryCode = this.getCountryIso(country) ?? '';
    const e164Number = `${dialCode}${this.office.phoneNumber}`;

    return {
      number: this.office.phoneNumber,
      internationalNumber: e164Number,
      nationalNumber: this.office.phoneNumber,
      e164Number,
      countryCode,
      dialCode
    };
  }

  private getCountryIso(country?: dropdownOptionsModel | null): CountryISO {
    const iso = (country?.additionalData?.['isoCode'] || country?.additionalData?.['codeAlpha']) as string;
    if (!iso) {
      return CountryISO.Qatar;
    }
    const normalized = iso.trim().toUpperCase();
    if (normalized.length !== 2) {
      return CountryISO.Qatar;
    }
    return normalized as CountryISO;
  }

  submit() {
    if (this.isViewMode()) {
      return;
    }

    this.submitted = true;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const selectedCountryId = this.form.controls.countryId.value;
    const selectedCountry = this.availableCountries().find(country => country.id === selectedCountryId);

    if (!selectedCountry) {
      this.form.controls.countryId.setErrors({required: true});
      return;
    }

    const {
      nameAr,
      nameEn,
      countryId,
      adminNameAr,
      adminNameEn,
      adminEmail,
      phoneNumber
    } = this.form.getRawValue();
    const phoneCountryCode = phoneNumber?.dialCode ? this.formatDialCode(phoneNumber.dialCode) : '';
    const phoneValue = phoneNumber?.number ?? '';
    const supportedCountryIds = countryId ? [countryId] : [];

    if (this.isEditMode() && this.office) {
      this.update.emit({
        id: this.office.id,
        payload: {
          nameAr,
          nameEn,
          adminEmail,
          adminNameAr,
          adminNameEn,
          phoneCountryCode,
          phoneNumber: phoneValue,
          supportedCountryIds
        }
      });
    } else {
      this.create.emit({
        nameAr,
        nameEn,
        countryId,
        supportedCountryIds,
        adminEmail,
        adminNameAr,
        adminNameEn,
        phoneCountryCode,
        phoneNumber: phoneValue
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
