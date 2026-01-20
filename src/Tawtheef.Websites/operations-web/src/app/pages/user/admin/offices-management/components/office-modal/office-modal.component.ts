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

type PhoneCountryCodeOption = {
  id: string;
  name: string;
  countryId: string;
};

@Component({
  selector: 'app-office-modal',
  standalone: true,
  templateUrl: './office-modal.component.html',
  styleUrls: ['./office-modal.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, I18nNamespaceDirective, Select]
})
export class OfficeModalComponent implements OnInit, OnChanges {
  private fb = inject(FormBuilder);
  private language = inject(LanguageService);
  private translate = inject(TranslateService);

  @Input() visible = false;
  @Input() mode: 'create' | 'edit' | 'view' = 'create';
  @Input() countries: dropdownOptionsModel[] = [];
  @Input() office: OfficeDetailsDto | null = null;
  @Input() officeUsers: OfficeUserDto[] = [];
  @Input() loading = false;

  phoneCountryOptions: PhoneCountryCodeOption[] = [];

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
    phoneCountryCode: ['', Validators.required],
    phoneNumber: ['', Validators.required]
  });

  ngOnInit(): void {
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
    this.form.controls.countryId.valueChanges.subscribe(countryId => {
      this.syncCountryFields(countryId);
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['countries']) {
      this.phoneCountryOptions = this.buildPhoneCountryOptions();
    }
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
      phoneCountryCode: '',
      phoneNumber: ''
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
        phoneCountryCode: this.office.phoneCountryCode || '',
        phoneNumber: this.office.phoneNumber || ''
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
      this.form.controls.phoneCountryCode.setValue('', {emitEvent: false});
      return;
    }

    this.form.controls.supportedCountryIds.setValue([countryId], {emitEvent: false});

    const selectedCountry = this.countries.find(country => country.id === countryId);
    const dialCode = this.formatDialCode(selectedCountry?.description);
    if (dialCode) {
      this.form.controls.phoneCountryCode.setValue(dialCode, {emitEvent: false});
    }
  }

  private buildPhoneCountryOptions() {
    return this.countries
      .map(country => {
        const dialCode = this.formatDialCode(country.description);
        if (!dialCode) {
          return null;
        }
        return {
          id: dialCode,
          name: `${dialCode} - ${country.name}`,
          countryId: country.id
        };
      })
      .filter((option): option is PhoneCountryCodeOption => option !== null);
  }

  private formatDialCode(value?: string) {
    const trimmed = value?.trim() || '';
    if (!trimmed) {
      return '';
    }
    return trimmed.startsWith('+') ? trimmed : `+${trimmed}`;
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

    const {
      nameAr,
      nameEn,
      countryId,
      adminNameAr,
      adminNameEn,
      adminEmail,
      phoneCountryCode,
      phoneNumber
    } = this.form.getRawValue();
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
          phoneNumber,
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
        phoneNumber
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
