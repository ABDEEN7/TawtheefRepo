import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { QatarResidentOtpService } from '../../../../../core/auth/qatar-resident-otp.service';
import { NotificationService } from '../../../../../core/services/notification.service';
import { AuthService } from '../../../../../core/auth/auth.service';
import { finalize } from 'rxjs/operators';
import { CountryISO, NgxIntlTelInputModule } from 'ngx-intl-tel-input';
import { ButtonDirective } from 'primeng/button';
import { InputText } from 'primeng/inputtext';
import { SharedModule } from '../../../../../shared/shared.module';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';

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
  imports: [
    TranslatePipe,
    ButtonDirective,
    InputText,
    SharedModule,
    I18nNamespaceDirective,
    NgxIntlTelInputModule
  ]
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

  // Form واحد لكل الحقول
  readonly form = this.fb.group({
    qid: this.fb.nonNullable.control('', [Validators.required]),
    phoneNumber: this.fb.control<QatarPhoneNumber | null>(
      null,
      [Validators.required, this.qatarPhoneValidator]
    ),
    otp: this.fb.nonNullable.control('', [Validators.required, Validators.minLength(4)])
  });

  submitIdentification(): void {
    // نتحقق فقط من حقول identify
    this.form.controls.qid.markAsTouched();
    this.form.controls.phoneNumber.markAsTouched();

    if (this.form.controls.qid.invalid || this.form.controls.phoneNumber.invalid) return;

    const qid = this.form.controls.qid.value;
    const phone = this.getPhoneE164();
    if (!qid || !phone) return;

    this.loading = true;
    this.otpService.requestOtp(qid, phone)
      .pipe(finalize(() => {
        this.loading = false;
        this.form.controls.qid.enable();
        this.form.controls.phoneNumber.enable();
      }))
      .subscribe({
        next: () => {
          this.step = 'otp';
          this.form.controls.qid.disable();
          this.form.controls.phoneNumber.disable();
          this.notifier.success(
            this.translate.instant('auth.login.qatarResidentDialog.sent', {phone: this.getPhoneDisplay()}),
            this.translate.instant('auth.login.qatarResidentDialog.success'));
        }
      });
  }

  submitOtp(): void {
    this.form.controls.otp.markAsTouched();

    // في خطوة OTP نحتاج otp + (qid/phone موجودين لكنهم disabled؛ نقرأهم بـ getRawValue)
    if (this.form.controls.otp.invalid) return;

    const raw = this.form.getRawValue();
    const qid = raw.qid;
    const otp = raw.otp;
    const phone = this.getPhoneE164();
    if (!qid || !phone || !otp) return;

    this.loading = true;
    this.otpService.verifyOtp(qid, phone, otp)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: res => {
          this.auth.externalLogin(res).subscribe(success => {
            if (success) this.ref.close(true);
            else {
              this.notifier.error(
                this.translate.instant('auth.login.qatarResidentDialog.errorDescription'),
                this.translate.instant('auth.login.qatarResidentDialog.errorTitle'),
              );
            }
          });
        }
      });
  }

  close(): void {
    this.ref.close(this.config.data?.returnValue ?? null);
  }

  private qatarPhoneValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value as QatarPhoneNumber | null;
    if (!value) return null;

    const countryCode = value.countryCode?.toUpperCase?.();
    const isQatarDialCode = value.dialCode === '+974' || value.e164Number?.startsWith('+974');

    return countryCode === CountryISO.Qatar.toUpperCase() || isQatarDialCode ? null : { nonQatar: true };
  }

  private getPhoneE164(): string | null {
    const value = this.form.controls.phoneNumber.value as QatarPhoneNumber | null;
    return value?.e164Number ?? null;
  }

  private getPhoneDisplay(): string | null {
    const value = this.form.controls.phoneNumber.value as QatarPhoneNumber | null;
    return value?.internationalNumber ?? value?.e164Number ?? null;
  }
}
