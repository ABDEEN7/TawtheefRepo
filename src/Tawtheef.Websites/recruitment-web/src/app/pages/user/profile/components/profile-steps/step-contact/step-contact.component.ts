import {
  Component,
  EventEmitter,
  inject,
  Input,
  OnDestroy,
  OnInit,
  Output,
  computed,
  input,
  output,
  ChangeDetectionStrategy, ChangeDetectorRef, signal
} from '@angular/core';
import { CommonModule, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize } from 'rxjs/operators';
import { PhoneNumberUtil } from 'google-libphonenumber';
import { CountryISO, SearchCountryField, NgxIntlTelInputModule } from 'ngx-intl-tel-input';
import { SelectModule } from 'primeng/select';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { FaDirArrowDirective } from '../../../../../../shared/directives/dir-arrow.directive';
import { ProfileDataService } from '../../../wizard-profile/services/profile-data.service';
import { CountryVM, ProfileLookupsService } from '../../../wizard-profile/services/profile-lookups.service';
import { ContactVerificationService } from '../../../wizard-profile/services/contact-verification.service';
import { GeoIpService } from '../../../../../../core/services/geo-ip.service';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';
import {
  canPreviewFile,
  createFileSlot,
  displayedFileName,
  FileSlot,
  fileSlotSignature,
  fileToUpload,
  previewFileFromSlot,
  previewUrlFromSlot,
  setLocalFile,
  updateRemote
} from '../../../wizard-profile/utils/file-slot';
import { mapContactSection } from '../../../wizard-profile/services/profile.mapper';
import { PhoneNumber } from '../../../wizard-profile/models/phone-number.model';
import { VERIFIED_PHONE_KEY } from '../../../../../../core/constants/wizard-keys.const';
import { NotificationService } from '../../../../../../core/services/notification.service';

type NaField = 'naZone' | 'naStreet' | 'naBuilding' | 'naUnit';
type VerificationStatus =
  | 'idle'
  | 'sending'
  | 'codeSent'
  | 'linkSent'
  | 'verifying'
  | 'verified'
  | 'failed';

interface VerificationState {
  value: any;
  valid: boolean;
  touched: boolean;
  otp: string;
  status: VerificationStatus;
  cooldown: number;
  errorMessage: string | null;
  useCode?: boolean;
  cooldownTimer?: any;
}

@Component({
  selector: 'app-step-contact',
  templateUrl: './step-contact.component.html',
  styleUrl: './step-contact.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    NgxIntlTelInputModule,
    SelectModule,
    ButtonModule,
    InputTextModule,
    FaDirArrowDirective
  ]
})
export class StepContactComponent implements OnInit, OnDestroy {
  back = output<void>();
  next = output<void>();
  submitLabelKey = input<string>('wizard.buttons.next');
  showBack = input<boolean>(true);
  showInterviewPlace = input<boolean>(true);
  requireChanges = input<boolean>(false);

  // services
  ds = inject(ProfileDataService);
  lookups = inject(ProfileLookupsService);
  verificationService = inject(ContactVerificationService);
  translate = inject(TranslateService);
  geoIp = inject(GeoIpService);
  profileService = inject(ProfileService);
  notificationService = inject(NotificationService);
  fileUtils = inject(FileUtilsService);

  naFileError = signal<string | null>(null);
  maxNaFileSize = 2 * 1024 * 1024; // 2MB
  allowedNaTypes = ['application/pdf', 'image/png', 'image/jpeg'];
  private naLocalFile: FileSlot = createFileSlot();
  protected readonly phoneNumberUtil = PhoneNumberUtil.getInstance();
  protected readonly SearchCountryField = SearchCountryField;
  private readonly QATAR_E164_PREFIX = '+974';
  private readonly GOOGLE_PROVIDER = 'google';
  private readonly QATAR_PASS_PROVIDER = 'qatarpass';
  private readonly QATAR_RESIDENT_PROVIDER = 'qatarresidentotp';
  // state
  phoneValue = signal<string | null>(null);
  phoneOtp = signal('');
  phone = signal<VerificationState>({
    value: null,
    valid: false,
    touched: false,
    otp: '',
    status: 'idle',
    cooldown: 0,
    errorMessage: null
  });

  emailValue = signal<string | null>(null);
  emailOtp = signal('');
  email = signal<VerificationState>({
    value: null,
    valid: false,
    touched: false,
    otp: '',
    status: 'idle',
    cooldown: 0,
    errorMessage: null,
    useCode: true
  });

  selectedCountryIso2 = signal<CountryISO>(CountryISO.UnitedStates);
  onlyPhoneCountries = signal<CountryISO[]>([]);
  savingContact = signal(false);
  private lastSubmittedSignature: string | null = null;
  private pendingGeoCountryIso2: string | null = null;
  step = computed(() => this.ds.stepValidationDetailed().contact);

  requiresPhoneVerification = computed(() => {
    const phone = this.ds.state().phone ?? null;
    return this.isQatarProvider() && this.isQatarPhone(phone);
  });

  canVerifyPhone = computed(() => this.requiresPhoneVerification());

  ngOnInit(): void {
    this.configurePhoneCountries();
    this.geoIp.getCountryIso2().subscribe({
      next: (code) => {
        const ipCountry = this.toCountryIso(code);
        if (this.onlyPhoneCountries().includes(ipCountry)) {
          this.selectedCountryIso2.set(ipCountry);
          this.pendingGeoCountryIso2 = code;
          this.tryApplyPendingGeoCountry();
        }
      }
    });
    const state = this.ds.state();
    // init phone
    if (state.phone) {
      if (state.phone?.e164Number) {
        try {
          const parsed = this.phoneNumberUtil.parse(state.phone.e164Number); // "+97433632375"
          const national = String(parsed.getNationalNumber());              // "33632375"
          const iso2 = this.phoneNumberUtil.getRegionCodeForNumber(parsed); // "QA"

          this.setSelectedCountryIso2IfAllowed(iso2);

          // خزن بالحقل قيمة بدون +974 (إذا separateDialCode = true)
          this.phoneValue.set(national);

          // وخزن بالـ state نفس كائن PhoneNumber عندك (e164Number + countryCode)
          this.phone.update(s => ({ ...s, value: state.phone!.e164Number, valid: true }));
        } catch {
          // fallback
          this.phoneValue.set(state.phone.e164Number);
        }
      }
      this.phone.update(s => ({ ...s, value: state.phone!.e164Number, valid: true }));
      this.setSelectedCountryIso2IfAllowed(state.phone.countryCode);
    }
    // Enforce rule on initial load:
    if (state.phone && !this.isQatarPhone(state.phone)) {
      // Non-Qatar: never verified
      this.resetPhoneVerificationState();
    } else {
      // Qatar: keep your existing behavior
      if (state.phoneVerified) {
        this.phone.update(s => ({ ...s, status: 'verified' }));
        this.setLastVerifiedPhoneE164(state.phone!.e164Number);
      }

      // restore verified in-session if same cached phone
      if (!state.phoneVerified && state.phone?.e164Number) {
        const cached = this.getLastVerifiedPhoneE164();
        if (cached && cached === state.phone.e164Number) {
          this.ds.up('phoneVerified', true, { markDirty: false });
          this.phone.update(s => ({ ...s, status: 'verified' }));
        }
      }
    }
    this.syncCountryDependents(this.ds.state().country ?? null, false);
    // init email
    if (state.email) {
      this.emailValue.set(state.email);
      this.email.update(s => ({ ...s, value: state.email!, valid: true }));
    }
    if (state.emailVerified) {
      this.email.update(s => ({ ...s, status: 'verified' }));
    }
    updateRemote(this.naLocalFile, state.naFile);
    const dto = mapContactSection(state);
    this.lastSubmittedSignature = this.buildSignature(dto, state);
  }

  ngOnDestroy(): void {
    // clear timers to avoid leaks
    if (this.phone().cooldownTimer) clearInterval(this.phone().cooldownTimer);
    if (this.email().cooldownTimer) clearInterval(this.email().cooldownTimer);
    this.setLastVerifiedPhoneE164(null);
  }

  // ========== Cooldown helper ==========

  private getLastVerifiedPhoneE164(): string | null {
    try { return sessionStorage.getItem(VERIFIED_PHONE_KEY); } catch { return null; }
  }

  private setLastVerifiedPhoneE164(e164: string | null): void {
    try {
      if (!e164) sessionStorage.removeItem(VERIFIED_PHONE_KEY);
      else sessionStorage.setItem(VERIFIED_PHONE_KEY, e164);
    } catch {
      // ignore storage failures (private mode etc.)
    }
  }

  private isQatarPhone(value: PhoneNumber | null): boolean {
    return (value as any)?.countryCode?.toUpperCase?.() === 'QA' || (value?.e164Number ?? '').startsWith('+974');
  }
  private isQatarProvider(): boolean {
    const provider = (this.ds.state().provider ?? '').toLowerCase();
    return provider === this.QATAR_PASS_PROVIDER || provider === this.QATAR_RESIDENT_PROVIDER;
  }

  private isGoogleProvider(): boolean {
    return (this.ds.state().provider ?? '').toLowerCase() === this.GOOGLE_PROVIDER;
  }

  private resetPhoneVerificationState(): void {
    this.ds.up('phoneVerified', false);
    this.phone.update(s => ({ ...s, status: 'idle', otp: '', errorMessage: null }));
    this.phoneOtp.set('');
    this.setLastVerifiedPhoneE164(null); // clear cached verified phone
  }
  private startCooldown(target: any, seconds: number): void {
    target.update((s: any) => ({ ...s, cooldown: seconds }));
    if (target().cooldownTimer) clearInterval(target().cooldownTimer);

    const timer = setInterval(() => {
      target.update((s: any) => {
        const nextCooldown = s.cooldown - 1;
        if (nextCooldown <= 0) {
          clearInterval(timer);
          return { ...s, cooldown: 0, cooldownTimer: undefined };
        }
        return { ...s, cooldown: nextCooldown };
      });
    }, 1000);

    target.update((s: any) => ({ ...s, cooldownTimer: timer }));
  }

  private configurePhoneCountries(): void {
    if (this.isQatarProvider()) {
      this.onlyPhoneCountries.set([CountryISO.Qatar]);
      this.selectedCountryIso2.set(CountryISO.Qatar);
      return;
    }

    if (this.isGoogleProvider()) {
      this.onlyPhoneCountries.set(Object.values(CountryISO)
        .filter(c => c !== CountryISO.Qatar && c !== CountryISO.Israel) as CountryISO[]);
      return;
    }

    this.onlyPhoneCountries.set(Object.values(CountryISO)
      .filter(c => c !== CountryISO.Israel) as CountryISO[]);
  }

  private toCountryIso(iso2: string | null | undefined): CountryISO {
    return (iso2 ?? '').toLowerCase() as CountryISO;
  }

  private setSelectedCountryIso2IfAllowed(iso2: string | null | undefined): void {
    const countryIso = this.toCountryIso(iso2);
    if (!countryIso) return;

    const isAllowed =
      this.onlyPhoneCountries().length === 0 ||
      this.onlyPhoneCountries().includes(countryIso);

    if (isAllowed) {
      this.selectedCountryIso2.set(countryIso);
    }
  }

  private setCountryFromIso(iso2: string): void {
    if (!iso2) return;

    const countries = this.lookups.countries();
    if (!countries || countries.length === 0) {
      this.pendingGeoCountryIso2 = iso2;
      return;
    }

    this.pendingGeoCountryIso2 = null;

    if (this.ds.isLocked('country') && this.ds.state().country) {
      this.syncCountryDependents(this.ds.state().country ?? null, false);
      return;
    }

    if (this.ds.state().country) {
      this.syncCountryDependents(this.ds.state().country ?? null, false);
      return;
    }

    const match = this.lookups.countries().find(c => c.code?.toLowerCase() === iso2?.toLowerCase());
    if (match) {
      this.onCountryChange(match, false);
    }
  }

  private tryApplyPendingGeoCountry(): void {
    if (!this.pendingGeoCountryIso2) return;
    this.setCountryFromIso(this.pendingGeoCountryIso2);
  }

  private syncCountryDependents(country: CountryVM | null, markDirty = true): void {
    this.syncInterviewPlace(country, markDirty);
  }

  private syncInterviewPlace(country: CountryVM | null, markDirty = true): void {
    this.ds.up('interviewPlace', country ?? null, { markDirty });
  }
  // ========== Phone ==========
  onPhoneChange(value: PhoneNumber | null): void {
    if (!value || this.ds.isLocked('phone')) return;

    this.phoneValue.set(value.e164Number.replace(value.dialCode, ''));
    this.setSelectedCountryIso2IfAllowed(value.countryCode);
    this.phone.update(s => ({ ...s, touched: true, errorMessage: null }));

    // reset OTP workflow when the phone changes
    if (this.phone().status === 'codeSent' || this.phone().status === 'verifying' || this.phone().status === 'failed') {
      this.phone.update(s => ({ ...s, status: 'idle' }));
      this.phoneOtp.set('');
    }

    // Validate safely
    let isValid = false;
    try {
      const parsed = this.phoneNumberUtil.parseAndKeepRawInput(value.e164Number);
      isValid = this.phoneNumberUtil.isValidNumber(parsed);

      // Qatar provider must be Qatar number only (UI already restricts, but keep server-safe guard)
      if (this.isQatarProvider()) {
        isValid = isValid && this.isQatarPhone(value);
      }

      // Google provider must be NON-Qatar (UI already restricts, but keep server-safe guard)
      if (this.isGoogleProvider()) {
        isValid = isValid && !this.isQatarPhone(value);
      }
    } catch {
      isValid = false;
    }

    this.phone.update(s => ({ ...s, valid: isValid }));

    if (!isValid) {
      this.ds.up('phone', null);
      this.ds.up('phoneVerified', false);
      this.phone.update(s => ({ ...s, status: 'idle' }));
      return;
    }

    // Save phone in state
    this.ds.up('phone', value);

    // Behavior by provider:

    // Qatar provider: require OTP verification
    if (this.isQatarProvider()) {
      // restore verified in-session if same cached phone
      const cached = this.getLastVerifiedPhoneE164();
      if (cached && cached === value.e164Number) {
        this.ds.up('phoneVerified', true);
        this.phone.update(s => ({ ...s, status: 'verified' }));
        return;
      }

      this.ds.up('phoneVerified', false);
      this.phone.update(s => ({ ...s, status: 'idle' }));
      return;
    }

    // Google provider: no OTP required (verification UI hidden)
    if (this.isGoogleProvider()) {
      this.ds.up('phoneVerified', true); // treat as confirmed in UI to allow Next
      this.phone.update(s => ({ ...s, status: 'verified' }));
      this.setLastVerifiedPhoneE164(null);
      return;
    }

    // Other providers (if any): default no OTP
    this.ds.up('phoneVerified', true);
    this.phone.update(s => ({ ...s, status: 'verified' }));
  }

  onPhoneOtpChange(otp: string): void {
    this.phoneOtp.set(otp);
  }

  onCountryChange(country: CountryVM | null, markDirty = true): void {
    this.ds.up('country', country ?? null, { markDirty });
    this.syncCountryDependents(country, markDirty);
  }

  sendPhoneCode(): void {
    const state = this.ds.state();
    if (!this.isQatarProvider() || !this.isQatarPhone(state.phone ?? null)) {
      return;
    }

    if (!this.phone().valid || !state.phone || this.phone().cooldown > 0) return;

    this.phone.update(s => ({ ...s, status: 'sending', errorMessage: null }));

    this.verificationService
      .requestPhoneCode({ phoneE164: state.phone.e164Number })
      .subscribe({
        next: () => {
          this.phone.update(s => ({ ...s, status: 'codeSent' }));
          this.startCooldown(this.phone, 60);
        },
        error: err => {
          this.phone.update(s => ({ ...s, status: 'failed' }));

          if (err.status === 429) {
            const retryAfterHeader = err.headers?.get?.('Retry-After');
            const retrySeconds = retryAfterHeader ? +retryAfterHeader : 60;
            this.startCooldown(this.phone, retrySeconds);
            this.phone.update(s => ({
              ...s,
              errorMessage: this.translate.instant(
                'wizard.contact.codeSent.phone.cooldown',
                { seconds: retrySeconds }
              )
            }));
          } else {
            this.phone.update(s => ({
              ...s,
              errorMessage: this.translate.instant('wizard.contact.codeSent.phone.error')
            }));
          }
        }
      });
  }
  verifyPhoneCode(): void {
    const state = this.ds.state();

    // Only Qatar provider + Qatar phone can verify OTP
    if (!this.isQatarProvider() || !this.isQatarPhone(state.phone ?? null)) {
      return;
    }

    if (!this.phoneOtp()) return;
    if (!state.phone) return;

    this.phone.update(s => ({ ...s, status: 'verifying', errorMessage: null }));

    this.verificationService.verifyPhoneCode({
      phoneE164: state.phone.e164Number,
      code: this.phoneOtp()
    }).subscribe({
      next: () => {
        this.phone.update(s => ({ ...s, status: 'verified' }));
        this.phoneOtp.set('');
        this.ds.up('phoneVerified', true);

        // Cache last verified phone outside ProfileState
        this.setLastVerifiedPhoneE164(state.phone!.e164Number);
      },
      error: () => {
        this.phone.update(s => ({
          ...s,
          status: 'failed',
          errorMessage: this.translate.instant('wizard.contact.codeSent.phone.error')
        }));
      }
    });
  }
  private updatePhoneIfGoogleProvider(): Promise<boolean> {
    const s = this.ds.state();
    if (!this.isGoogleProvider()) return Promise.resolve(true);

    // If no phone, let step validation handle it
    if (!s.phone?.e164Number) return Promise.resolve(true);

    // Safety: Google must not submit +974 due to backend rule
    if (s.phone.e164Number.startsWith(this.QATAR_E164_PREFIX)) {
      this.notificationService.error(
        this.translate.instant('wizard.contact.googleProviderNonQatarOnly'),
        this.translate.instant('wizard.validationErrorTitle')
      );
      return Promise.resolve(false);
    }

    // Call update endpoint (no OTP)
    return new Promise<boolean>((resolve) => {
      this.verificationService
        .updatePhone({ phoneE164: s.phone!.e164Number })
        .subscribe({
          next: () => {
            // reflect confirmed phone in UI state
            this.ds.up('phoneVerified', true);
            this.phone.update(s => ({ ...s, status: 'verified' }));
            resolve(true);
          },
          error: () => {
            resolve(false);
          }
        });
    });
  }
  // ========== Email ==========

  onEmailChange(value: string): void {
    if (this.ds.isLocked('email')) return;

    this.emailValue.set(value);
    this.email.update(s => ({ ...s, touched: true, errorMessage: null }));

    const isValid = !!value && /\S+@\S+\.\S+/.test(value);
    this.email.update(s => ({ ...s, valid: isValid, value: value }));

    if (isValid) {
      this.ds.up('email', value);
    } else {
      this.ds.up('email', null);
    }

    if (this.email().status === 'verified') {
      this.email.update(s => ({ ...s, status: 'idle' }));
      this.ds.up('emailVerified', false);
    }
  }

  onEmailOtpChange(otp: string): void {
    this.emailOtp.set(otp);
  }

  sendEmailVerification(): void {
    const state = this.ds.state();
    if (!this.email().valid || !state.email || this.email().cooldown > 0) return;

    this.email.update(s => ({ ...s, status: 'sending', errorMessage: null }));

    this.verificationService
      .requestEmailVerification({ email: state.email })
      .subscribe({
        next: () => {
          this.email.update(s => ({ ...s, status: 'linkSent' }));
          this.startCooldown(this.email, 60);
        },
        error: (err: any) => {
          this.email.update(s => ({ ...s, status: 'failed' }));

          if (err.status === 429) {
            const retryAfterHeader = err.headers?.get?.('Retry-After');
            const retrySeconds = retryAfterHeader ? +retryAfterHeader : 60;
            this.startCooldown(this.email, retrySeconds);
            this.email.update(s => ({
              ...s,
              errorMessage: this.translate.instant(
                'wizard.contact.codeSent.email.cooldown',
                { seconds: retrySeconds }
              )
            }));
          } else {
            this.email.update(s => ({
              ...s,
              errorMessage: this.translate.instant('wizard.contact.codeSent.email.error')
            }));
          }
        }
      });
  }

  verifyEmailCode(): void {
    const state = this.ds.state();
    if (!this.email().useCode || !this.emailOtp() || !state.email) return;

    this.email.update(s => ({ ...s, status: 'verifying', errorMessage: null }));

    this.verificationService
      .verifyEmailCode({
        email: state.email,
        code: this.emailOtp()
      })
      .subscribe({
        next: () => {
          this.email.update(s => ({ ...s, status: 'verified' }));
          this.emailOtp.set('');
          this.ds.up('emailVerified', true);
        },
        error: () => {
          this.email.update(s => ({
            ...s,
            status: 'failed',
            errorMessage: this.translate.instant('wizard.contact.codeSent.email.error')
          }));
        }
      });
  }

  // ========== NA file ==========

  onNaFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;
    if (!this.allowedNaTypes.includes(file.type)) {
      this.naFileError.set(this.translate.instant('wizard.nationalAddress.fileTypeError'));
      input.value = '';
      return;
    }
    if (file.size > this.maxNaFileSize) {
      this.naFileError.set(this.translate.instant('wizard.nationalAddress.fileSizeError'));
      input.value = '';
      return;
    }

    this.naFileError.set(null);
    setLocalFile(this.naLocalFile, file);
    this.ds.up('naFileName', file.name);
    this.ds.up('naFile', { resourceId: 'local', fileName: file.name, file: file } as any);
    input.value = '';
  }
  async onNext(): Promise<void> {
    if (this.savingContact()) return;

    if (!this.step().valid) {
      this.notificationService.error(this.step().errors.map(e => `* ${this.translate.instant(e.i18nKey)}`).join('\n'), this.translate.instant('wizard.validationErrorTitle'));
      return;
    }

    this.savingContact.set(true);

    // If Google provider: update phone before saving contact section
    const ok = await this.updatePhoneIfGoogleProvider();
    if (!ok) {
      this.savingContact.set(false);
      return;
    }

    const s = this.ds.state();
    const dto = mapContactSection(s);
    const signature = this.buildSignature(dto, s);

    if (signature && signature === this.lastSubmittedSignature && this.ds.isStepSubmitted('contact')) {
      if (this.requireChanges() || this.ds.hasUnsolvedCorrections(3)) {
        const msg = this.ds.hasUnsolvedCorrections(3)
          ? 'يجب عمل التعديلات المذكورة في ملاحظات المراجع'
          : this.translate.instant('profileView.notifications.noChanges');
        this.notificationService.error(msg);
        this.savingContact.set(false);
        return;
      }
      this.notificationService.info(this.translate.instant('profileView.notifications.noChanges'));
      this.savingContact.set(false);
      this.next.emit();
      return;
    }

    this.profileService
      .saveContactSection(dto, { nationalAddressFile: fileToUpload(this.naLocalFile) })
      .pipe(finalize(() => (this.savingContact.set(false))))
      .subscribe({
        next: () => {
          this.lastSubmittedSignature = signature;
          this.ds.markStepSubmitted('contact');
          if (this.profileService.isChangeRequestMode()) {
            this.notificationService.success(this.translate.instant('profileView.notifications.changeRequestSent'));
          }
          this.next.emit();
        }
      });
  }

  previewNaFile(ev?: Event): void {
    ev?.stopPropagation();
    if (!canPreviewFile(this.naLocalFile)) return;

    const local = previewFileFromSlot(this.naLocalFile);
    if (local) {
      this.fileUtils.previewBlob(local);
      return;
    }

    const url = previewUrlFromSlot(this.naLocalFile);
    if (url) {
      this.fileUtils.previewUrl(url, displayedFileName(this.naLocalFile), false);
    }
  }

  private buildSignature(dto: ReturnType<typeof mapContactSection>, state: ReturnType<typeof this.ds.state>): string | null {
    try {
      const nationalAddress = fileSlotSignature(this.naLocalFile);

      const contactInfo = {
        phone: state.phone?.e164Number ?? null,
        phoneVerified: state.phoneVerified ?? false,
        email: state.email ?? null,
        emailVerified: state.emailVerified ?? false,
        availableForRecruitment: state.available,
      };

      return JSON.stringify({ dto, nationalAddress, contactInfo });
    } catch {
      return null;
    }
  }

  onNaNumberChange(field: NaField, value: any): void {
    if (this.ds.isLocked(field)) return;

    // allow empty for unit, but zone/street/building should be required by your step validator
    const raw = String(value ?? '');

    // keep digits only
    const digits = raw.replace(/\D+/g, '');

    // update state with digits (string of numbers)
    this.ds.up(field, digits);

    // optional: if user typed letters, show a small warning once (optional)
    // if (raw !== digits) this.notificationService.info(this.translate.instant('validation.digitsOnly'));
  }

  digitsOnlyKeypress(event: KeyboardEvent): void {
    // allow control keys
    if (event.ctrlKey || event.metaKey || event.altKey) return;

    const allowed = ['Backspace', 'Delete', 'ArrowLeft', 'ArrowRight', 'Tab', 'Enter'];
    if (allowed.includes(event.key)) return;

    // allow digits only
    if (!/^\d$/.test(event.key)) {
      event.preventDefault();
    }
  }

  digitsOnlyPaste(event: ClipboardEvent): void {
    const text = event.clipboardData?.getData('text') ?? '';
    if (!/^\d+$/.test(text)) {
      event.preventDefault();
    }
  }
}
