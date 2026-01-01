import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TranslateService } from '@ngx-translate/core';
import { QatarResidentOtpService } from '../../../../../core/auth/qatar-resident-otp.service';
import { NotificationService } from '../../../../../core/services/notification.service';
import { AuthService } from '../../../../../core/auth/auth.service';
import { finalize } from 'rxjs/operators';
import { CountryISO } from 'ngx-intl-tel-input';

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
  templateUrl: './qatar-resident-otp-dialog.component.html',
  styleUrl: './qatar-resident-otp-dialog.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class QatarResidentOtpDialogComponent {
  private fb = inject(FormBuilder);
  private otpService = inject(QatarResidentOtpService);
  private notifier = inject(NotificationService);
  private translate = inject(TranslateService);
  private auth = inject(AuthService);
  private ref = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig);

  readonly qatarOnly = [CountryISO.Qatar];

  step: 'identify' | 'otp' = 'identify';
  loading = false;

  readonly requestForm = this.fb.group({
    qid: ['', [Validators.required]],
    phoneNumber: [
      null as QatarPhoneNumber | null,
      [Validators.required, this.qatarPhoneValidator]
    ]
  });

  readonly otpForm = this.fb.group({
    otp: ['', [Validators.required, Validators.minLength(4)]]
  });

  get qidControl() {
    return this.requestForm.get('qid');
  }

  get phoneControl() {
    return this.requestForm.get('phoneNumber');
  }

  get otpControl() {
    return this.otpForm.get('otp');
  }

  submitIdentification(): void {
    if (this.requestForm.invalid) {
      this.requestForm.markAllAsTouched();
      return;
    }

    const { qid } = this.requestForm.getRawValue();
    const phone = this.getPhoneE164();
    if (!qid || !phone) return;

    this.loading = true;
    this.otpService
      .requestOtp(qid, phone)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: () => {
          this.step = 'otp';
          this.requestForm.disable();
          const sentMsg = this.translate.instant('auth.login.qatarResidentDialog.sent', {
            phone: this.getPhoneDisplay()
          });
          this.notifier.success(this.translate.instant('auth.login.qatarResidentDialog.success'), sentMsg);
        },
        error: err => this.handleError(err)
      });
  }

  submitOtp(): void {
    if (this.otpForm.invalid || this.requestForm.invalid) {
      this.otpForm.markAllAsTouched();
      return;
    }

    const { qid } = this.requestForm.getRawValue();
    const { otp } = this.otpForm.getRawValue();
    const phone = this.getPhoneE164();
    if (!qid || !phone || !otp) return;

    this.loading = true;
    this.otpService
      .verifyOtp(qid, phone, otp)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: res => {
          this.auth.externalLogin(res).subscribe(success => {
            if (success) {
              this.ref.close(true);
            } else {
              this.notifier.error(
                this.translate.instant('auth.login.qatarResidentDialog.errorTitle'),
                this.translate.instant('auth.login.qatarResidentDialog.errorDescription')
              );
            }
          });
        },
        error: err => this.handleError(err)
      });
  }

  close(): void {
    this.ref.close(this.config.data?.returnValue ?? null);
  }

  private handleError(err: any): void {
    const detail = err?.error?.message || err?.error || this.translate.instant('common.errorGeneric');
    this.notifier.error(
      this.translate.instant('auth.login.qatarResidentDialog.errorTitle'),
      detail
    );
  }

  private qatarPhoneValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value as QatarPhoneNumber | null;
    if (!value) return null;

    const countryCode = value.countryCode?.toUpperCase?.();
    const isQatarDialCode = value.dialCode === '+974' || value.e164Number?.startsWith('+974');

    return countryCode === CountryISO.Qatar.toUpperCase() || isQatarDialCode ? null : { nonQatar: true };
  }

  private getPhoneE164(): string | null {
    const value = this.phoneControl?.value as QatarPhoneNumber | null;
    if (!value) return null;

    return value.e164Number ?? null;
  }

  private getPhoneDisplay(): string | null {
    const value = this.phoneControl?.value as QatarPhoneNumber | null;
    return value?.internationalNumber ?? value?.e164Number ?? null;
  }
}
