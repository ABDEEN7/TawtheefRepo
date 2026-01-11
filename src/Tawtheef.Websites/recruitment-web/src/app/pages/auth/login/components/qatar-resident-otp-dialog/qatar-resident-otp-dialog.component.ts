import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnDestroy, inject } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators
} from '@angular/forms';
import { finalize } from 'rxjs/operators';
import {interval, merge, Subject} from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ButtonDirective } from 'primeng/button';
import { InputText } from 'primeng/inputtext';
import { DatePicker } from 'primeng/datepicker';

import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { CountryISO, NgxIntlTelInputModule } from 'ngx-intl-tel-input';

import { SharedModule } from '../../../../../shared/shared.module';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';

import { QatarResidentOtpService } from '../../../../../core/auth/qatar-resident-otp.service';
import { NotificationService } from '../../../../../core/services/notification.service';
import { AuthService } from '../../../../../core/auth/auth.service';

import { toDateOnly } from '../../../../../shared/types/dateOnly.type';

type QatarPhoneNumber = {
  number: string;
  internationalNumber: string;
  nationalNumber: string;
  e164Number: string;
  countryCode: string;
  dialCode: string;
};

@Component({
  selector: 'app-qatar-resident-otp-dialog',
  standalone: true,
  templateUrl: './qatar-resident-otp-dialog.component.html',
  styleUrls: ['./qatar-resident-otp-dialog.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,

    TranslatePipe,
    ButtonDirective,
    InputText,
    DatePicker,

    SharedModule,
    I18nNamespaceDirective,
    NgxIntlTelInputModule
  ]
})
export class QatarResidentOtpDialogComponent implements OnDestroy {
  private fb = inject(FormBuilder);
  private otpService = inject(QatarResidentOtpService);
  private notifier = inject(NotificationService);
  private translate = inject(TranslateService);
  private auth = inject(AuthService);
  private ref = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig);

  readonly qatarOnly = [CountryISO.Qatar];
  readonly today = new Date();

  step: 'identify' | 'otp' = 'identify';
  loading = false;

  // ---- Resend Timer ----
  resendCooldown = 0; // seconds
  private destroy$ = new Subject<void>();
  private cooldownStop$ = new Subject<void>();

  // Form
  readonly form = this.fb.group({
    qid: this.fb.nonNullable.control('', [Validators.required]),
    phoneNumber: this.fb.control<QatarPhoneNumber | null>(null, [
      Validators.required,
      qatarPhoneValidator()
    ]),
    qidExpiry: this.fb.nonNullable.control<string | Date>('', [Validators.required]),
    otp: this.fb.nonNullable.control({ value: '', disabled: true }, [
      Validators.required,
      Validators.minLength(6)
    ])
  });

  ngOnDestroy(): void {
    this.cooldownStop$.next();
    this.cooldownStop$.complete();
    this.destroy$.next();
    this.destroy$.complete();
  }
  // -------------------------
  // Submit Identify
  // -------------------------
  submitIdentification(): void {
    this.form.controls.qid.markAsTouched();
    this.form.controls.phoneNumber.markAsTouched();
    this.form.controls.qidExpiry.markAsTouched();

    if (
      this.form.controls.qid.invalid ||
      this.form.controls.phoneNumber.invalid ||
      this.form.controls.qidExpiry.invalid
    ) return;

    const qid = this.form.controls.qid.value;
    const phone = this.getPhoneE164();
    const qidExpiry = this.getExpiryAsDateOnly();
    if (!qid || !phone || !qidExpiry) return;

    this.loading = true;

    // disable identify inputs while calling API
    this.setIdentifyDisabled(true);

    this.otpService
      .requestOtp(qid, phone, qidExpiry)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: () => {
          this.step = 'otp';

          // keep identify fields disabled, enable otp input
          this.form.controls.otp.enable({ emitEvent: false });
          this.form.controls.otp.reset('', { emitEvent: false });

          // start cooldown (example 152 sec)
          this.startResendCooldown(152);

          this.notifier.success(
            this.translate.instant('auth.login.qatarResidentDialog.sent', { phone: this.getPhoneDisplay() }),
            this.translate.instant('auth.login.qatarResidentDialog.success')
          );
        },
        error: () => {
          // if API fails, allow editing again
          this.setIdentifyDisabled(false);
        }
      });
  }

  // -------------------------
  // Submit OTP
  // -------------------------
  submitOtp(): void {
    this.form.controls.otp.markAsTouched();
    if (this.form.controls.otp.invalid) return;

    const qid = this.form.controls.qid.value;
    const otp = this.form.controls.otp.value;
    const phone = this.getPhoneE164();
    const qidExpiry = this.getExpiryAsDateOnly();

    if (!qid || !phone || !otp || !qidExpiry) return;

    this.loading = true;

    this.otpService
      .verifyOtp(qid, phone, otp, qidExpiry)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: res => {
          this.auth.externalLogin(res).subscribe({
            next: success => {
              if (success) this.ref.close(true);
              else {
                this.notifier.error(
                  this.translate.instant('auth.login.qatarResidentDialog.errorDescription'),
                  this.translate.instant('auth.login.qatarResidentDialog.errorTitle')
                );
              }
            },
            error: () => {
              this.form.controls.otp.reset('', { emitEvent: false });
              this.form.controls.otp.enable({ emitEvent: false });
            }
          });
        }
      });
  }

  // -------------------------
  // Resend OTP (called from HTML)
  // -------------------------
  resendOtp(): void {
    if (this.resendCooldown > 0) return;

    const qid = this.form.controls.qid.value;
    const phone = this.getPhoneE164();
    const qidExpiry = this.getExpiryAsDateOnly();
    if (!qid || !phone || !qidExpiry) return;

    this.loading = true;

    this.otpService
      .requestOtp(qid, phone, qidExpiry)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: () => {
          this.startResendCooldown(152);

          this.notifier.success(
            this.translate.instant('auth.login.qatarResidentDialog.resent', { phone: this.getPhoneDisplay() }),
            this.translate.instant('auth.login.qatarResidentDialog.success')
          );
        }
      });
  }

  // -------------------------
  // Close (called from HTML)
  // -------------------------
  close(): void {
    this.ref.close(this.config.data?.returnValue ?? null);
  }

  // -------------------------
  // Template Helpers (called from HTML)
  // -------------------------
  maskQid(qid: string | null | undefined): string {
    if (!qid) return '';
    const s = String(qid);
    if (s.length <= 4) return s;
    // 29478801376 -> 2947******76
    return `${s.slice(0, 4)}******${s.slice(-2)}`;
  }

  formatCooldown(totalSeconds: number): string {
    const m = Math.floor(totalSeconds / 60);
    const s = totalSeconds % 60;
    return `${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
  }

  // IMPORTANT: must be public because template calls it
  getPhoneDisplay(): string | null {
    const value = this.form.controls.phoneNumber.value as QatarPhoneNumber | null;
    return value?.internationalNumber ?? value?.e164Number ?? null;
  }

  // -------------------------
  // Internal Helpers
  // -------------------------
  private startResendCooldown(seconds: number): void {
    this.resendCooldown = seconds;

    // stop previous cooldown stream
    this.cooldownStop$.next();

    interval(1000)
      .pipe(takeUntil(merge(this.destroy$, this.cooldownStop$)))
      .subscribe(() => {
        if (this.resendCooldown > 0) this.resendCooldown--;
      });
  }

  private setIdentifyDisabled(disabled: boolean): void {
    const opt = { emitEvent: false };
    if (disabled) {
      this.form.controls.qid.disable(opt);
      this.form.controls.phoneNumber.disable(opt);
      this.form.controls.qidExpiry.disable(opt);
    } else {
      this.form.controls.qid.enable(opt);
      this.form.controls.phoneNumber.enable(opt);
      this.form.controls.qidExpiry.enable(opt);
    }
  }

  private getPhoneE164(): string | null {
    const value = this.form.controls.phoneNumber.value as QatarPhoneNumber | null;
    return value?.e164Number ?? null;
  }

  private getExpiryAsDateOnly(): any | null {
    const raw = this.form.controls.qidExpiry.value;
    if (!raw) return null;
    return toDateOnly(raw as any);
  }
}

/**
 * Standalone validator (no `this` binding issues)
 */
function qatarPhoneValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value as QatarPhoneNumber | null;
    if (!value) return null;

    const countryCode = value.countryCode?.toUpperCase?.();
    const isQatarDialCode = value.dialCode === '+974' || value.e164Number?.startsWith('+974');

    return (countryCode === CountryISO.Qatar.toUpperCase() || isQatarDialCode)
      ? null
      : { nonQatar: true };
  };
}
