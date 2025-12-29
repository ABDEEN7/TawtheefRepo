import {
  Component,
  EventEmitter,
  Output,
  inject,
  OnInit,
  OnDestroy
} from '@angular/core';
import { finalize } from 'rxjs/operators';
import { PhoneNumberUtil } from 'google-libphonenumber';
import {CountryISO, SearchCountryField} from 'ngx-intl-tel-input';
import {ProfileDataService} from '../../../wizard-profile/services/profile-data.service';
import {ProfileLookupsService} from '../../../wizard-profile/services/profile-lookups.service';
import {ContactVerificationService} from '../../../wizard-profile/services/contact-verification.service';
import {TranslateService} from '@ngx-translate/core';
import {GeoIpService} from '../../../../../../core/services/geo-ip.service';
import {ProfileService} from '../../../wizard-profile/services/profile.service';
import {MessageService} from 'primeng/api';
import {FileUtilsService} from '../../../../../../core/utils/file-utils';
import {
  canPreviewFile,
  createFileSlot, displayedFileName,
  FileSlot, fileSlotSignature,
  fileToUpload, previewFileFromSlot, previewUrlFromSlot,
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

  selectedCountryIso2: CountryISO = CountryISO.Qatar;
  savingContact = false;
  private lastSubmittedSignature: string | null = null;
  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['contact'];
  }

  ngOnInit(): void {
    this.geoIp.getCountryIso2().subscribe(code => {
      this.selectedCountryIso2 = code.toLowerCase() as CountryISO;
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
    // init email
    if (state.email) {
      this.email.value = state.email;
      this.email.valid = true;
    }
    if (state.emailVerified) {
      this.email.status = 'verified';
    }

    const dto = mapContactSection(state);
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
    return this.isQatarPhone(this.ds.state().phone ?? null);
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

  // ========== Phone ==========
  onPhoneChange(value: PhoneNumber | null): void {
    if (!value || this.ds.isLocked('phone')) return;

    this.phoneInput = value;
    this.phone.touched = true;
    this.phone.errorMessage = null;

    // Reset OTP workflow when the phone changes
    if (this.phone.status === 'codeSent' || this.phone.status === 'verifying' || this.phone.status === 'failed') {
      this.phone.otp = '';
      this.phone.status = 'idle';
    }

    // Validate safely
    let isValid = false;
    try {
      const parsed = this.phoneNumberUtil.parseAndKeepRawInput(value.e164Number);
      isValid = this.phoneNumberUtil.isValidNumber(parsed);
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

    // Save phone object to state
    this.ds.up('phone', value);

    // If NOT Qatar: disable verification always
    if (!this.isQatarPhone(value)) {
      this.resetPhoneVerificationState();
      return;
    }

    // Qatar only: allow restore from cached verified phone
    const cached = this.getLastVerifiedPhoneE164();
    if (cached && cached === value.e164Number) {
      this.ds.up('phoneVerified', true);
      this.phone.status = 'verified';
      return;
    }

    // Otherwise require verification for Qatar
    this.ds.up('phoneVerified', false);
    this.phone.status = 'idle';
  }

  sendPhoneCode(): void {
    const state = this.ds.state();
    // Qatar-only
    if (!this.isQatarPhone(state.phone ?? null)) {
      this.resetPhoneVerificationState();
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
    // Qatar-only
    if (!this.isQatarPhone(state.phone ?? null)) {
      this.resetPhoneVerificationState();
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
  onNext(): void {
    if (!this.step.valid) {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('wizard.validationErrorTitle'),
        detail: this.step.errors.map(e => `* ${this.translate.instant(e.i18nKey)}`).join('\n'),
        life: 5000,
      });
      return;
    }

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
        },
        error: (err: any) => console.error(err)
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
      };

      return JSON.stringify({ dto, nationalAddress, contactInfo });
    } catch {
      return null;
    }
  }
}
