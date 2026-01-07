import {Component, effect, EventEmitter, inject, OnDestroy, OnInit, Output} from '@angular/core';
import {finalize} from 'rxjs/operators';
import {PhoneNumberUtil} from 'google-libphonenumber';
import {CountryISO, SearchCountryField} from 'ngx-intl-tel-input';
import {ProfileDataService} from '../../../wizard-profile/services/profile-data.service';
import {CountryDto, ProfileLookupsService} from '../../../wizard-profile/services/profile-lookups.service';
import {ContactVerificationService} from '../../../wizard-profile/services/contact-verification.service';
import {TranslateService} from '@ngx-translate/core';
import {GeoIpService} from '../../../../../../core/services/geo-ip.service';
import {ProfileService} from '../../../wizard-profile/services/profile.service';
import {MessageService} from 'primeng/api';
import {FileUtilsService} from '../../../../../../core/utils/file-utils';
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
import {createStepValiditySignal} from '../../../wizard-profile/state/profile-step-validity.signal';
import {mapContactSection} from '../../../wizard-profile/services/profile.mapper';
import {PhoneNumber} from '../../../wizard-profile/models/phone-number.model';
import {VERIFIED_PHONE_KEY} from '../../../../../../core/constants/wizard-keys.const';


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
  standalone: false
})
export class StepContactComponent implements OnInit, OnDestroy {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  // services
  ds = inject(ProfileDataService);
  lookups = inject(ProfileLookupsService);
  verificationService = inject(ContactVerificationService);
  translate = inject(TranslateService);
  geoIp = inject(GeoIpService);
  profileService = inject(ProfileService);
  messageService = inject(MessageService);
  fileUtils = inject(FileUtilsService);

  naFileError: string | null = null;
  maxNaFileSize = 2 * 1024 * 1024; // 2MB
  allowedNaTypes = ['application/pdf', 'image/png', 'image/jpeg', 'image/webp'];
  private naLocalFile: FileSlot = createFileSlot();
  protected readonly phoneNumberUtil = PhoneNumberUtil.getInstance();
  protected readonly SearchCountryField = SearchCountryField;
  private readonly QATAR_E164_PREFIX = '+974';
  private readonly GOOGLE_PROVIDER = 'google';
  private readonly QATAR_PASS_PROVIDER = 'qatarpass';
  private readonly QATAR_RESIDENT_PROVIDER = 'qatarresidentotp';
  // verification states
  phoneInput: PhoneNumber | null = null;
  phone: VerificationState = {
    value: null,
    valid: false,
    touched: false,
    otp: '',
    status: 'idle',
    cooldown: 0,
    errorMessage: null
  };

  email: VerificationState = {
    value: null,
    valid: false,
    touched: false,
    otp: '',
    status: 'idle',
    cooldown: 0,
    errorMessage: null,
    useCode: true
  };

  selectedCountryIso2: CountryISO = CountryISO.UnitedStates;
  onlyPhoneCountries: CountryISO[] = [];
  readonly CountryISO = CountryISO;
  savingContact = false;
  private lastSubmittedSignature: string | null = null;
  private pendingGeoCountryIso2: string | null = null;
  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['contact'];
  }

  private readonly geoCountrySync = effect(() => {
    // Re-run when countries lookup refreshes to apply GeoIP once available.
    this.lookups.countries();
    this.tryApplyPendingGeoCountry();
  });

  ngOnInit(): void {
    this.configurePhoneCountries();
    this.geoIp.getCountryIso2().subscribe({
      next:(code)=>{
        const ipCountry = code as CountryISO;
        if (this.onlyPhoneCountries.includes(ipCountry)) {
          this.selectedCountryIso2 = ipCountry;
          this.pendingGeoCountryIso2 = code;
          this.tryApplyPendingGeoCountry();
        }
      }
    });

    const state = this.ds.state();

    // init phone
    this.phoneInput = state.phone ?? null;
    if (state.phone) {
      this.phone.value = state.phone.e164Number;
      this.phone.valid = true;
    }
    // Enforce rule on initial load:
    if (state.phone && !this.isQatarPhone(state.phone)) {
      // Non-Qatar: never verified
      this.resetPhoneVerificationState();
    } else {
      // Qatar: keep your existing behavior
      if (state.phoneVerified) {
        this.phone.status = 'verified';
        this.setLastVerifiedPhoneE164(state.phone!.e164Number);
      }

      // restore verified in-session if same cached phone
      if (!state.phoneVerified && state.phone?.e164Number) {
        const cached = this.getLastVerifiedPhoneE164();
        if (cached && cached === state.phone.e164Number) {
          this.ds.up('phoneVerified', true);
          this.phone.status = 'verified';
        }
      }
    }
    this.syncCountryDependents(this.ds.state().country ?? null);
    // init email
    if (state.email) {
      this.email.value = state.email;
      this.email.valid = true;
    }
    if (state.emailVerified) {
      this.email.status = 'verified';
    }
    updateRemote(this.naLocalFile, state.naFile);
    this.lastSubmittedSignature = null;
  }

  ngOnDestroy(): void {
    // clear timers to avoid leaks
    if (this.phone.cooldownTimer) clearInterval(this.phone.cooldownTimer);
    if (this.email.cooldownTimer) clearInterval(this.email.cooldownTimer);
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
  get canVerifyPhone(): boolean {
    return this.requiresPhoneVerification;
  }

  private isQatarProvider(): boolean {
    const provider = (this.ds.state().provider ?? '').toLowerCase();
    return provider === this.QATAR_PASS_PROVIDER || provider === this.QATAR_RESIDENT_PROVIDER;
  }

  private isGoogleProvider(): boolean {
    return (this.ds.state().provider ?? '').toLowerCase() === this.GOOGLE_PROVIDER;
  }
  /** Qatar provider requires Qatar phone and OTP verification. */
  get requiresPhoneVerification(): boolean {
    const phone = this.ds.state().phone ?? null;
    return this.isQatarProvider() && this.isQatarPhone(phone);
  }

  private resetPhoneVerificationState(): void {
    this.ds.up('phoneVerified', false);
    this.phone.status = 'idle';
    this.phone.otp = '';
    this.phone.errorMessage = null;
    this.setLastVerifiedPhoneE164(null); // clear cached verified phone
  }
  private startCooldown(target: VerificationState, seconds: number): void {
    target.cooldown = seconds;
    if (target.cooldownTimer) clearInterval(target.cooldownTimer);

    target.cooldownTimer = setInterval(() => {
      target.cooldown--;
      if (target.cooldown <= 0) {
        target.cooldown = 0;
        clearInterval(target.cooldownTimer);
        target.cooldownTimer = undefined;
      }
    }, 1000);
  }

  private configurePhoneCountries(): void {
    if (this.isQatarProvider()) {
      this.onlyPhoneCountries = [CountryISO.Qatar];
      this.selectedCountryIso2 = CountryISO.Qatar;
      return;
    }

    if (this.isGoogleProvider()) {
      this.onlyPhoneCountries = Object.values(CountryISO)
        .filter(c => c !== CountryISO.Qatar) as CountryISO[];
      return;
    }

    this.onlyPhoneCountries = [];
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
      this.syncCountryDependents(this.ds.state().country ?? null);
      return;
    }

    if (this.ds.state().country) {
      this.syncCountryDependents(this.ds.state().country ?? null);
      return;
    }

    const match = this.lookups.countries().find(c => c.code?.toLowerCase() === iso2?.toLowerCase());
    if (match) {
      this.onCountryChange(match);
    }
  }

  private tryApplyPendingGeoCountry(): void {
    if (!this.pendingGeoCountryIso2) return;
    this.setCountryFromIso(this.pendingGeoCountryIso2);
  }

  private syncCountryDependents(country: CountryDto | null): void {
    this.syncInterviewPlace(country);
  }

  private syncInterviewPlace(country: CountryDto | null): void {
    this.ds.up('interviewPlace', country ?? null);
  }

  // ========== Phone ==========
  onPhoneChange(value: PhoneNumber | null): void {
    if (!value || this.ds.isLocked('phone')) return;

    this.phoneInput = value;
    this.phone.touched = true;
    this.phone.errorMessage = null;

    // reset OTP workflow when the phone changes
    if (this.phone.status === 'codeSent' || this.phone.status === 'verifying' || this.phone.status === 'failed') {
      this.phone.otp = '';
      this.phone.status = 'idle';
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

    this.phone.valid = isValid;

    if (!isValid) {
      this.ds.up('phone', null);
      this.ds.up('phoneVerified', false);
      this.phone.status = 'idle';
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
        this.phone.status = 'verified';
        return;
      }

      this.ds.up('phoneVerified', false);
      this.phone.status = 'idle';
      return;
    }

    // Google provider: no OTP required (verification UI hidden)
    if (this.isGoogleProvider()) {
      this.ds.up('phoneVerified', true); // treat as confirmed in UI to allow Next
      this.phone.status = 'verified';
      this.setLastVerifiedPhoneE164(null);
      return;
    }

    // Other providers (if any): default no OTP
    this.ds.up('phoneVerified', true);
    this.phone.status = 'verified';
  }

  onCountryChange(country: CountryDto | null): void {
    this.ds.up('country', country ?? null);
    this.syncCountryDependents(country);
  }

  sendPhoneCode(): void {
    const state = this.ds.state();
    if (!this.isQatarProvider() || !this.isQatarPhone(state.phone ?? null)) {
      return;
    }

    if (!this.phone.valid || !state.phone || this.phone.cooldown > 0) return;

    this.phone.status = 'sending';
    this.phone.errorMessage = null;

    this.verificationService
      .requestPhoneCode({ phoneE164: state.phone.e164Number })
      .subscribe({
        next: () => {
          this.phone.status = 'codeSent';
          this.startCooldown(this.phone, 60);
        },
        error: err => {
          this.phone.status = 'failed';

          if (err.status === 429) {
            const retryAfterHeader = err.headers?.get?.('Retry-After');
            const retrySeconds = retryAfterHeader ? +retryAfterHeader : 60;
            this.startCooldown(this.phone, retrySeconds);
            this.phone.errorMessage = this.translate.instant(
              'wizard.contact.codeSent.phone.cooldown',
              { seconds: retrySeconds }
            );
          } else {
            this.phone.errorMessage = this.translate.instant(
              'wizard.contact.codeSent.phone.error'
            );
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

    if (!this.phone.otp) return;
    if (!state.phone) return;

    this.phone.status = 'verifying';
    this.phone.errorMessage = null;

    this.verificationService.verifyPhoneCode({
      phoneE164: state.phone.e164Number,
      code: this.phone.otp
    }).subscribe({
      next: () => {
        this.phone.status = 'verified';
        this.ds.up('phoneVerified', true);

        // Cache last verified phone outside ProfileState
        this.setLastVerifiedPhoneE164(state.phone!.e164Number);
        this.phone.otp = '';
      },
      error: () => {
        this.phone.status = 'failed';
        this.phone.errorMessage = this.translate.instant('wizard.contact.codeSent.phone.error');
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
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('wizard.validationErrorTitle'),
        detail: this.translate.instant('wizard.contact.googleProviderNonQatarOnly'),
        life: 5000
      });
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
            this.phone.status = 'verified';
            resolve(true);
          },
          error: () => {
            this.messageService.add({
              severity: 'error',
              summary: this.translate.instant('wizard.validationErrorTitle'),
              detail: this.translate.instant('wizard.contact.phoneUpdateFailed'),
              life: 5000
            });
            resolve(false);
          }
        });
    });
  }
  // ========== Email ==========

  onEmailChange(value: string): void {
    if (!value || this.ds.isLocked('email')) return;

    this.email.touched = true;
    this.email.errorMessage = null;

    this.email.valid = !!value && /\S+@\S+\.\S+/.test(value);
    if (this.email.valid) {
      this.email.value = value;
      this.ds.up('email', value);
    } else {
      this.email.value = value;
      this.ds.up('email', null);
    }

    if (this.email.status === 'verified') {
      this.email.status = 'idle';
      this.ds.up('emailVerified', false);
    }
  }

  sendEmailVerification(): void {
    const state = this.ds.state();
    if (!this.email.valid || !state.email || this.email.cooldown > 0) return;

    this.email.status = 'sending';
    this.email.errorMessage = null;

    this.verificationService
      .requestEmailVerification({ email: state.email })
      .subscribe({
        next: () => {
          this.email.status = 'linkSent';
          this.startCooldown(this.email, 60);
        },
        error: (err: any) => {
          this.email.status = 'failed';

          if (err.status === 429) {
            const retryAfterHeader = err.headers?.get?.('Retry-After');
            const retrySeconds = retryAfterHeader ? +retryAfterHeader : 60;
            this.startCooldown(this.email, retrySeconds);
            this.email.errorMessage = this.translate.instant(
              'wizard.contact.codeSent.email.cooldown',
              { seconds: retrySeconds }
            );
          } else {
            this.email.errorMessage = this.translate.instant(
              'wizard.contact.codeSent.email.error'
            );
          }
        }
      });
  }

  verifyEmailCode(): void {
    const state = this.ds.state();
    if (!this.email.useCode || !this.email.otp || !state.email) return;

    this.email.status = 'verifying';
    this.email.errorMessage = null;

    this.verificationService
      .verifyEmailCode({
        email: state.email,
        code: this.email.otp
      })
      .subscribe({
        next: () => {
          this.email.status = 'verified';
          this.ds.up('emailVerified', true);
        },
        error: () => {
          this.email.status = 'failed';
          this.email.errorMessage = this.translate.instant(
            'wizard.contact.codeSent.email.error'
          );
        }
      });
  }

  // ========== NA file ==========

  onNaFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;
    if (!this.allowedNaTypes.includes(file.type)) {
      this.naFileError = this.translate.instant('wizard.nationalAddress.fileTypeError');
      input.value = '';
      return;
    }
    if (file.size > this.maxNaFileSize) {
      this.naFileError = this.translate.instant('wizard.nationalAddress.fileSizeError');
      input.value = '';
      return;
    }

    this.naFileError = null;
    setLocalFile(this.naLocalFile, file);
    this.ds.up('naFileName', file.name);
    this.ds.up('naFile', { resourceId: 'local', fileName: file.name, file: file } as any);
    input.value = '';
  }
  async onNext(): Promise<void> {
    if (!this.step.valid) {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('wizard.validationErrorTitle'),
        detail: this.step.errors.map(e => `* ${this.translate.instant(e.i18nKey)}`).join('\n'),
        life: 5000,
      });
      return;
    }

    // If Google provider: update phone before saving contact section
    const ok = await this.updatePhoneIfGoogleProvider();
    if (!ok) return;

    const s = this.ds.state();
    const dto = mapContactSection(s);
    const signature = this.buildSignature(dto, s);

    if (signature && signature === this.lastSubmittedSignature) {
      this.next.emit();
      return;
    }

    this.savingContact = true;
    this.profileService
      .saveContactSection(dto, { nationalAddressFile: fileToUpload(this.naLocalFile) })
      .pipe(finalize(() => (this.savingContact = false)))
      .subscribe({
        next: () => {
          this.lastSubmittedSignature = signature;
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
}
