import { CommonModule } from '@angular/common';
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
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Lang, LanguageService } from '../../../../../../core/services/language.service';
import { I18nNamespaceDirective } from '../../../../../../shared/directives/i18n-namespace.directive';
import { CreateOfficeRequest } from '../../models/create-office-request.dto';
import { UpdateOfficeRequest } from '../../models/update-office-request.dto';
import { OfficeUserDto } from '../../models/office-user.dto';
import { Select } from 'primeng/select';
import { dropdownOptionsModel } from '../../../../../../shared/models/dropdown-options.model';
import { OfficeDetailsDto } from '../../models/office-details.dto';
import { CountryISO, NgxIntlTelInputModule, SearchCountryField } from 'ngx-intl-tel-input';

// ─── Types ───────────────────────────────────────────────────────────────────

type ModalMode = 'create' | 'edit' | 'view';

interface PhoneNumberValue {
  number: string;
  internationalNumber: string;
  nationalNumber: string;
  e164Number: string;
  dialCode: string;
}

// ─── Component ───────────────────────────────────────────────────────────────

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

  // ─── DI ────────────────────────────────────────────────────────────────────

  private fb = inject(FormBuilder);
  private language = inject(LanguageService);
  private translate = inject(TranslateService);

  // ─── Inputs ────────────────────────────────────────────────────────────────

  @Input() visible = false;
  @Input() mode: ModalMode = 'create';
  @Input() countries: dropdownOptionsModel[] = [];
  @Input() assignedCountryIds: string[] = [];
  @Input() office: OfficeDetailsDto | null = null;
  @Input() officeUsers: OfficeUserDto[] = [];
  @Input() loading = false;

  // ─── Outputs ───────────────────────────────────────────────────────────────

  @Output() cancel = new EventEmitter<void>();
  @Output() create = new EventEmitter<CreateOfficeRequest>();
  @Output() update = new EventEmitter<{ id: string; payload: UpdateOfficeRequest }>();
  @Output() toggleBlock = new EventEmitter<{ userId: string; isBlocked: boolean }>();
  @Output() makeAdmin = new EventEmitter<string>();

  // ─── Phone input config ────────────────────────────────────────────────────

  readonly searchCountryFields = [SearchCountryField.Iso2, SearchCountryField.Name];
  readonly allowedCountries = Object.values(CountryISO).filter(c => c !== CountryISO.Israel);

  selectedCountryIso = signal<CountryISO>(CountryISO.Qatar);
  phoneInputVisible = signal(true);

  // ─── Computed ──────────────────────────────────────────────────────────────

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  availableCountries = computed(() => {
    const assignedSet = new Set(this.assignedCountryIds);
    const currentCountryId = this.office?.countryId;
    return this.countries.filter(c => !assignedSet.has(c.id) || c.id === currentCountryId);
  });

  phoneCountries = computed(() => {
    const isos = this.countries
      .map(c => this.resolveCountryIso(c))
      .filter((iso): iso is CountryISO => Boolean(iso));
    return isos.length ? isos : undefined;
  });

  // ─── Form ──────────────────────────────────────────────────────────────────

  submitted = false;

  form = this.fb.nonNullable.group({
    nameAr: ['', Validators.required],
    nameEn: ['', Validators.required],
    countryId: ['', Validators.required],
    supportedCountryIds: this.fb.nonNullable.control<string[]>([], this.nonEmptyArrayValidator),
    adminNameAr: ['', Validators.required],
    adminNameEn: ['', Validators.required],
    adminEmail: ['', [Validators.required, Validators.email]],
    phoneNumber: this.fb.control<PhoneNumberValue | null>(null, Validators.required)
  });

  // ─── Lifecycle ─────────────────────────────────────────────────────────────

  ngOnInit(): void {
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
    this.form.controls.countryId.valueChanges.subscribe(id => this.onCountryChange(id));
  }

  ngOnChanges(changes: SimpleChanges): void {
    const relevantChange = changes['visible'] || changes['office'] || changes['officeUsers'] || changes['mode'] || changes['countries'];
    if (this.visible && relevantChange) {
      this.initForm();
    }
  }

  // ─── Mode helpers ──────────────────────────────────────────────────────────

  isCreateMode = () => this.mode === 'create';
  isEditMode = () => this.mode === 'edit';
  isViewMode = () => this.mode === 'view';

  // ─── Form initialization ───────────────────────────────────────────────────

  private initForm(): void {
    this.submitted = false;
    this.phoneInputVisible.set(false);

    this.form.enable({ emitEvent: false });
    this.form.reset({
      nameAr: this.office?.nameAr ?? '',
      nameEn: this.office?.nameEn ?? '',
      countryId: this.office?.countryId ?? '',
      supportedCountryIds: this.office?.countryId ? [this.office.countryId] : [],
      adminNameAr: this.office?.adminNameAr ?? '',
      adminNameEn: this.office?.adminNameEn ?? '',
      adminEmail: this.resolveAdminEmail(),
      phoneNumber: this.buildPhoneValue()
    });

    this.onCountryChange(this.form.controls.countryId.value);

    if (!this.isCreateMode()) {
      this.form.controls.countryId.disable({ emitEvent: false });
    }

    if (this.isViewMode()) {
      this.form.disable({ emitEvent: false });
    }

    setTimeout(() => this.phoneInputVisible.set(true), 0);
  }

  // ─── Event handlers ────────────────────────────────────────────────────────

  submit(): void {
    if (this.isViewMode()) return;

    this.submitted = true;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const selectedCountryId = this.form.controls.countryId.value;
    const selectedCountry = this.availableCountries().find(c => c.id === selectedCountryId);

    if (!selectedCountry) {
      this.form.controls.countryId.setErrors({ required: true });
      return;
    }

    const { nameAr, nameEn, countryId, adminNameAr, adminNameEn, adminEmail, phoneNumber } = this.form.getRawValue();

    const phoneCountryCode = this.formatDialCode(phoneNumber?.dialCode);
    const phoneNumberValue = phoneNumber?.e164Number ?? '';
    const supportedCountryIds = countryId ? [countryId] : [];

    if (this.isEditMode() && this.office) {
      this.update.emit({
        id: this.office.id,
        payload: { nameAr, nameEn, adminEmail, adminNameAr, adminNameEn, phoneCountryCode, phoneNumber: phoneNumberValue, supportedCountryIds }
      });
    } else {
      this.create.emit({
        nameAr, nameEn, countryId, supportedCountryIds,
        adminEmail, adminNameAr, adminNameEn,
        phoneCountryCode, phoneNumber: phoneNumberValue
      });
    }
  }

  close(): void {
    this.cancel.emit();
  }

  toggleUserBlock(user: OfficeUserDto): void {
    this.toggleBlock.emit({ userId: user.id, isBlocked: !user.isBlocked });
  }

  promoteToAdmin(user: OfficeUserDto): void {
    if (!this.isOfficeAdmin(user)) {
      this.makeAdmin.emit(user.id);
    }
  }

  // ─── Display helpers ───────────────────────────────────────────────────────

  isOfficeAdmin(user: OfficeUserDto): boolean {
    return user.isAdmin || user.email === this.office?.adminEmail;
  }

  localizedCountry(nameAr: string, nameEn: string): string {
    return this.currentLang() === 'ar' ? nameAr || nameEn : nameEn || nameAr;
  }

  localizedUserName(user: OfficeUserDto): string {
    return this.currentLang() === 'ar'
      ? user.fullNameAr || user.fullNameEn
      : user.fullNameEn || user.fullNameAr;
  }

  emailError(): string {
    const ctrl = this.form.controls.adminEmail;
    if (ctrl.hasError('required')) return this.translate.instant('OFFICES.FIELD_REQUIRED');
    if (ctrl.hasError('email')) return this.translate.instant('OFFICES.INVALID_EMAIL');
    return '';
  }

  // ─── Private helpers ───────────────────────────────────────────────────────

  private onCountryChange(countryId: string): void {
    const ids = countryId ? [countryId] : [];
    this.form.controls.supportedCountryIds.setValue(ids, { emitEvent: false });

    if (!this.form.controls.phoneNumber.value && !this.selectedCountryIso()) {
      const country = this.availableCountries().find(c => c.id === countryId);
      this.selectedCountryIso.set(this.resolveCountryIso(country));
    }
  }

  private resolveAdminEmail(): string {
    return this.officeUsers.find(u => u.isAdmin)?.email ?? this.office?.adminEmail ?? '';
  }

  private buildPhoneValue(): PhoneNumberValue | null {
    const { phoneNumber, phoneCountryCode } = this.office ?? {};
    if (!phoneNumber || !phoneCountryCode) return null;

    const dialCode = this.formatDialCode(phoneCountryCode);
    const e164Number = `${dialCode}${phoneNumber}`;

    return {
      number: phoneNumber,
      internationalNumber: e164Number,
      nationalNumber: phoneNumber,
      e164Number,
      dialCode
    };
  }

  private formatDialCode(value?: string | number): string {
    const raw = value?.toString().trim() ?? '';
    if (!raw) return '';
    return raw.startsWith('+') ? raw : `+${raw}`;
  }

  private resolveCountryIso(country?: dropdownOptionsModel | null): CountryISO {
    const iso = (country?.additionalData?.['isoCode'] ?? country?.additionalData?.['codeAlpha']) as string;
    if (!iso) return CountryISO.Qatar;

    const normalized = iso.trim().toUpperCase();
    return normalized.length === 2 ? (normalized as CountryISO) : CountryISO.Qatar;
  }

  private nonEmptyArrayValidator(control: AbstractControl<string[]>): ValidationErrors | null {
    return (control.value ?? []).length > 0 ? null : { required: true };
  }
}