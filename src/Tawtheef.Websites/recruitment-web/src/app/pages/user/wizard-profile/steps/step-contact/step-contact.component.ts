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
import { CountryISO, SearchCountryField } from 'ngx-intl-tel-input';

import { DataService } from '../../services/data.service';
import { ProfileLookupsService } from '../../services/profile-lookups.service';
import { GeoIpService } from '../../../../../core/services/geo-ip.service';
import { CandidateType } from '../../../../../core/enums/lookups.enum';
import { ContactVerificationService } from '../../services/contact-verification.service';
import { PhoneNumber } from '../../models/phone-number.model';
import { TranslateService } from '@ngx-translate/core';
import { ProfileService } from '../../services/profile.service';
import { mapContactSection } from '../../services/profile.mapper';
import {createStepValiditySignal} from '../../state/profile-step-validity.signal';
import {MessageService} from 'primeng/api';
import {FileUtilsService} from '../../../../../core/utils/file-utils';
import {FileSlot, canPreviewFile, createFileSlot, displayedFileName, fileSlotSignature, fileToUpload, previewFileFromSlot, previewUrlFromSlot, setLocalFile, updateRemote} from '../../utils/file-slot';

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
  ds = inject(DataService);
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

  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['contact'];
  }

  protected readonly phoneNumberUtil = PhoneNumberUtil.getInstance();
  protected readonly SearchCountryField = SearchCountryField;

  // verification states
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

  ngOnInit(): void {
    this.geoIp.getCountryIso2().subscribe(code => {
      this.selectedCountryIso2 = code.toLowerCase() as CountryISO;
    });

    const state = this.ds.state();

    // init phone
    if (state.phone) {
      this.phone.value = state.phone.e164Number;
      this.phone.valid = true;
    }
    if (state.phoneVerified) {
      this.phone.status = 'verified';
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
  }

  // ========== Cooldown helper ==========

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

  onPhoneChange(value: PhoneNumber): void {
    if (!value || this.ds.isLocked('phone')) return;

    this.phone.touched = true;
    this.phone.errorMessage = null;

    const phoneNumber = this.phoneNumberUtil.parseAndKeepRawInput(value.e164Number);
    this.phone.valid = this.phoneNumberUtil.isValidNumber(phoneNumber);

    if (this.phone.valid) {
      this.phone.value = value.e164Number;
      this.ds.up('phone', value);

      if (this.phone.status === 'verified' && (value.e164Number !== this.ds.state().phone?.e164Number || !this.ds.state().phoneVerified)) {
        this.phone.status = 'idle';
        this.ds.up('phoneVerified', false);
      }
    }
    else {
      this.phone.value = null;
      this.ds.up('phone', null);
    }
  }

  sendPhoneCode(): void {
    const state = this.ds.state();
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
    if (!this.phone.otp) return;
    const state = this.ds.state();
    if (!state.phone) return;

    this.phone.status = 'verifying';
    this.phone.errorMessage = null;

    this.verificationService
      .verifyPhoneCode({
        phoneE164: state.phone.e164Number,
        code: this.phone.otp
      })
      .subscribe({
        next: () => {
          this.phone.status = 'verified';
          this.ds.up('phoneVerified', true);
        },
        error: () => {
          this.phone.status = 'failed';
          this.phone.errorMessage = this.translate.instant(
            'wizard.contact.codeSent.phone.error'
          );
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
        error: err => console.error(err)
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
