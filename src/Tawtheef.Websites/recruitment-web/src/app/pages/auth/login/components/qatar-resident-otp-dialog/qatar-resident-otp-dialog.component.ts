import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import {FormBuilder, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import { QatarResidentOtpService } from '../../../../../core/auth/qatar-resident-otp.service';
import { NotificationService } from '../../../../../core/services/notification.service';
import { AuthService } from '../../../../../core/auth/auth.service';
import { finalize } from 'rxjs/operators';
import {I18nNamespaceDirective} from '../../../../../shared/directives/i18n-namespace.directive';
import {InputText} from 'primeng/inputtext';
import {ButtonDirective} from 'primeng/button';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-qatar-resident-otp-dialog',
  templateUrl: './qatar-resident-otp-dialog.component.html',
  styleUrl: './qatar-resident-otp-dialog.component.scss',
  imports: [
    I18nNamespaceDirective,
    TranslatePipe,
    InputText,
    ReactiveFormsModule,
    ButtonDirective,
    FormsModule,
    NgIf
  ],
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

  step: 'identify' | 'otp' = 'identify';
  loading = false;

  readonly requestForm = this.fb.group({
    qid: ['', [Validators.required]],
    phoneNumber: ['', [Validators.required]]
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

    const { qid, phoneNumber } = this.requestForm.getRawValue();
    if (!qid || !phoneNumber) return;

    this.loading = true;
    this.otpService
      .requestOtp(qid, phoneNumber)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: () => {
          this.step = 'otp';
          this.requestForm.disable();
          const sentMsg = this.translate.instant('auth.login.qatarResidentDialog.sent', { phone: phoneNumber });
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

    const { qid, phoneNumber } = this.requestForm.getRawValue();
    const { otp } = this.otpForm.getRawValue();
    if (!qid || !phoneNumber || !otp) return;

    this.loading = true;
    this.otpService
      .verifyOtp(qid, phoneNumber, otp)
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
}
