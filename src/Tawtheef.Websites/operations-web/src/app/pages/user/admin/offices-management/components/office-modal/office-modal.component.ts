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
import {MultiSelectModule} from 'primeng/multiselect'
import {dropdownOptionsModel} from '../../../../../../shared/models/dropdown-options.model';
import {OfficeDetailsDto} from '../../models/office-details.dto';

@Component({
  selector: 'app-office-modal',
  standalone: true,
  templateUrl: './office-modal.component.html',
  styleUrls: ['./office-modal.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslatePipe, I18nNamespaceDirective, Select, MultiSelectModule]
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
    adminEmail: ['', [Validators.required, Validators.email]]
  });

  ngOnInit(): void {
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
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
      adminEmail: this.resolveAdminEmail()
    });

    if (this.office) {
      this.form.patchValue({
        nameAr: this.office.nameAr,
        nameEn: this.office.nameEn,
        countryId: this.office.countryId,
        supportedCountryIds: this.office.supportedCountries?.map(sc => sc.id) || [],
        adminEmail: this.resolveAdminEmail()
      });
    }

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
    if (user.isAdmin) {
      return;
    }

    this.makeAdmin.emit(user.id);
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

    const {nameAr, nameEn, countryId, supportedCountryIds, adminEmail} = this.form.getRawValue();

    if (this.isEditMode() && this.office) {
      this.update.emit({
        id: this.office.id,
        payload: {
          nameAr,
          nameEn,
          adminEmail,
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
