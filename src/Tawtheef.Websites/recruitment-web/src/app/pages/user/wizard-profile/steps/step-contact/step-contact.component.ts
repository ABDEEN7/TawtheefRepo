import {Component, EventEmitter, Output, inject, OnInit} from '@angular/core';
import { DataService } from '../../services/data.service';
import {ProfileLookupsService} from '../../services/profile-lookups.service';
import {CountryISO, SearchCountryField} from 'ngx-intl-tel-input';
import {GeoIpService} from '../../../../../core/services/geo-ip.service';
import { PhoneNumberUtil } from 'google-libphonenumber';
import {CandidateType} from '../../../../../core/enums/lookups.enum';
import {ContactVerificationService} from '../../services/contact-verification.service';
import {PhoneNumber} from '../../models/phone-number.model';
import {TranslateService} from '@ngx-translate/core';

@Component({
  selector: 'app-step-contact',
  templateUrl: './step-contact.component.html',
  styleUrl: './step-contact.component.scss',
  standalone: false,
})
export class StepContactComponent implements OnInit{
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();
  ds = inject(DataService);
  lookups = inject(ProfileLookupsService);
  verification = inject(ContactVerificationService);
  translate = inject(TranslateService);
  geoIp = inject(GeoIpService);
  protected readonly phoneNumberUtil = PhoneNumberUtil.getInstance();
  protected readonly SearchCountryField = SearchCountryField;
  phoneView: any = null;
  phoneValid = false;
  phoneTouched = false;
  phoneOtp: string = '';
  phoneVerificationStatus: 'idle' | 'sending' | 'codeSent' | 'verifying' | 'verified' | 'failed' = 'idle';
  // Phone cooldown
  phoneCooldown = 0;
  private phoneCooldownTimer?: any;

  emailView: any = null;
  emailValid = false;
  emailTouched = false;
  emailOtp: string = '';
  emailVerificationStatus: 'idle' | 'sending' | 'linkSent' | 'verifying' | 'verified' | 'failed' = 'idle';
  emailVerificationUseCode = true;
  // Email cooldown
  emailCooldown = 0;
  private emailCooldownTimer?: any;

  phoneErrorMessage: string | null = null;
  emailErrorMessage: string | null = null;
  selectedCountryIso2: CountryISO = CountryISO.Qatar;
  ngOnInit(): void {
    this.geoIp.getCountryIso2().subscribe(code => {
      this.selectedCountryIso2 = code.toLowerCase() as CountryISO;
    });
    const state = this.ds.state();
    if (state.phoneVerified) this.phoneVerificationStatus = 'verified';
    if (state.emailVerified) this.emailVerificationStatus = 'verified';
    if (state.phone) this.phoneView = state.phone.e164Number;
    if (state.email) this.emailView = state.email;
  }

  get isResidentQatar(){
    return [CandidateType.ResidentQatar,CandidateType.Qatari,
      CandidateType.SonOfQatariMother, CandidateType.WifeOfQatari].includes(
      this.ds.state().candidateType?.backendName as CandidateType
    );
  }
  private startPhoneCooldown(seconds: number) {
    this.phoneCooldown = seconds;
    if (this.phoneCooldownTimer) clearInterval(this.phoneCooldownTimer);

    this.phoneCooldownTimer = setInterval(() => {
      this.phoneCooldown--;
      if (this.phoneCooldown <= 0) {
        this.phoneCooldown = 0;
        clearInterval(this.phoneCooldownTimer);
      }
    }, 1000);
  }

  private startEmailCooldown(seconds: number) {
    this.emailCooldown = seconds;
    if (this.emailCooldownTimer) clearInterval(this.emailCooldownTimer);

    this.emailCooldownTimer = setInterval(() => {
      this.emailCooldown--;
      if (this.emailCooldown <= 0) {
        this.emailCooldown = 0;
        clearInterval(this.emailCooldownTimer);
      }
    }, 1000);
  }
  onPhoneChange(value: PhoneNumber) {
    if (!value || this.ds.isLocked('phone')) return;

    this.phoneTouched = true;
    this.phoneErrorMessage = null;

    const phoneNumber = this.phoneNumberUtil.parseAndKeepRawInput(value.e164Number);
    this.phoneValid = this.phoneNumberUtil.isValidNumber(phoneNumber);

    if (this.phoneValid) {
      this.ds.up('phone', value);
      if (this.phoneVerificationStatus === 'verified') {
        this.phoneVerificationStatus = 'idle';
        this.ds.up('phoneVerified', false);
      }
    } else {
      this.ds.up('phone', null);
    }
  }

  onEmailChange(value: string) {
    if (!value|| this.ds.isLocked('email')) return;

    this.emailTouched = true;
    this.emailErrorMessage = null;

    this.emailValid = !!value && /\S+@\S+\.\S+/.test(value);
    if (this.emailValid) {
      this.ds.up('email', value);
    } else {
      this.ds.up('email', null);
    }

    if (this.emailVerificationStatus === 'verified') {
      this.emailVerificationStatus = 'idle';
      this.ds.up('emailVerified', false);
    }
  }

  sendPhoneCode() {
    if (!this.phoneValid || !this.ds.state().phone || this.phoneCooldown > 0) return;

    this.phoneVerificationStatus = 'sending';
    this.phoneErrorMessage = null;

    this.verification.requestPhoneCode({
      phoneE164: this.ds.state().phone!.e164Number
    }).subscribe({
      next: () => {
        this.phoneVerificationStatus = 'codeSent';
        this.startPhoneCooldown(60);
      },
      error: (err) => {
        this.phoneVerificationStatus = 'failed';

        if (err.status === 429) {
          const retryAfterHeader = err.headers?.get?.('Retry-After');
          const retrySeconds = retryAfterHeader ? +retryAfterHeader : 60;
          this.startPhoneCooldown(retrySeconds);
          this.phoneErrorMessage = this.translate.instant('wizard.contact.codeSent.phone.cooldown', { seconds: retrySeconds });
        } else {
          this.phoneErrorMessage = this.translate.instant('wizard.contact.codeSent.phone.error');
        }
      }
    });
  }

  verifyPhoneCode() {
    if (!this.phoneOtp) return;

    this.phoneVerificationStatus = 'verifying';
    this.phoneErrorMessage = null;

    this.verification.verifyPhoneCode({
      phoneE164: this.ds.state().phone!.e164Number,
      code: this.phoneOtp
    }).subscribe({
      next: () => {
        this.phoneVerificationStatus = 'verified';
        this.ds.up('phoneVerified', true);
      },
      error: (err) => {
        this.phoneVerificationStatus = 'failed';
        this.phoneErrorMessage = this.translate.instant('wizard.contact.codeSent.phone.error');
      }
    });
  }
  sendEmailVerification() {
    if (!this.emailValid || !this.ds.state().email || this.emailCooldown > 0) return;

    this.emailVerificationStatus = 'sending';
    this.emailErrorMessage = null;

    this.verification.requestEmailVerification({
      email: this.ds.state().email!
    }).subscribe({
      next: () => {
        this.emailVerificationStatus = 'linkSent';
        // start cooldown, e.g. 60 seconds
        this.startEmailCooldown(60);
      },
      error: (err) => {
        this.emailVerificationStatus = 'failed';

        if (err.status === 429) {
          const retryAfterHeader = err.headers?.get?.('Retry-After');
          const retrySeconds = retryAfterHeader ? +retryAfterHeader : 60;
          this.startEmailCooldown(retrySeconds);
          this.emailErrorMessage = this.translate.instant('wizard.contact.codeSent.email.cooldown', { seconds: retrySeconds });
        } else {
          this.emailErrorMessage = this.translate.instant('wizard.contact.codeSent.email.error');
        }
      }
    });
  }
  verifyEmailCode() {
    if (!this.emailOtp || !this.emailVerificationUseCode) return;

    this.emailVerificationStatus = 'verifying';
    this.emailErrorMessage = null;

    this.verification.verifyEmailCode({
      email: this.ds.state().email!,
      code: this.emailOtp
    }).subscribe({
      next: () => {
        this.emailVerificationStatus = 'verified';
        this.ds.up('emailVerified', true);
      },
      error: () => {
        this.emailVerificationStatus = 'failed';
        this.emailErrorMessage = this.translate.instant('wizard.contact.codeSent.email.error');
      }
    });
  }
}
