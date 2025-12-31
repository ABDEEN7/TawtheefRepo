import { CommonModule } from '@angular/common';
import {Component, inject, OnDestroy} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputOtpModule } from 'primeng/inputotp';
import {NotificationService} from '../../../core/services/notification.service';
import {TranslatePipe} from '@ngx-translate/core';

@Component({
  selector: 'app-otp-screen',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    InputOtpModule,
    TranslatePipe,
  ],
  templateUrl: './otp-screen.html',
  styleUrls: ['./otp-screen.scss'],
})
export class OtpScreenComponent implements OnDestroy {
  isRtl = true;
  step: 1 | 2 = 1;

  qid = '';
  phone = '';
  otp = '';

  submitted = false;
  loadingSend = false;
  loadingResend = false;
  loadingVerify = false;

  resendRemainingSec = 0;
  private countdownTimer?: any;

  private notificationService = inject(NotificationService);
  ngOnDestroy(): void {
    this.stopCountdown();
  }

  sendOtp(): void {
    this.submitted = true;

    if (!this.isValidQid(this.qid) || !this.isValidPhone(this.phone)) {
      this.notificationService.warn('Please check QID and phone.', 'Validation',);
      return;
    }

    this.loadingSend = true;

    // TODO: Call API: /auth/send-otp
    setTimeout(() => {
      this.loadingSend = false;

      this.step = 2;
      this.otp = '';
      this.startCountdown(60); // مثال: 60 ثانية

      this.notificationService.success('A verification code has been sent.', 'OTP Sent');
    }, 700);
  }

  resendOtp(): void {
    if (this.resendRemainingSec > 0) return;
    this.loadingResend = true;

    // TODO: Call API: /auth/resend-otp
    setTimeout(() => {
      this.loadingResend = false;
      this.startCountdown(60);
      this.notificationService.success('A new code has been sent.','Resent');
    }, 700);
  }

  verifyOtp(): void {
    if (!this.isOtpComplete()) {
      this.notificationService.warn('Please enter the full code.','OTP');
      return;
    }

    this.loadingVerify = true;

    // TODO: Call API: /auth/verify-otp
    setTimeout(() => {
      this.loadingVerify = false;

      // Example success:
      this.notificationService.success('OTP verified successfully.','Verified');
      // TODO: navigate next (reset password / login / complete registration)
    }, 700);
  }

  editInfo(): void {
    this.step = 1;
    this.stopCountdown();
  }

  cancel(): void {
    this.step = 1;
    this.otp = '';
    this.stopCountdown();
  }

  // Helpers
  isOtpComplete(): boolean {
    return (this.otp ?? '').toString().length === 6;
  }

  maskQid(qid: string): string {
    const v = (qid || '').trim();
    if (v.length < 4) return v;
    return `${v.slice(0, 2)}******${v.slice(-2)}`;
  }

  formatSeconds(sec: number): string {
    const m = Math.floor(sec / 60);
    const s = sec % 60;
    return `${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
  }

  private startCountdown(seconds: number): void {
    this.stopCountdown();
    this.resendRemainingSec = seconds;

    this.countdownTimer = setInterval(() => {
      this.resendRemainingSec--;
      if (this.resendRemainingSec <= 0) {
        this.resendRemainingSec = 0;
        this.stopCountdown();
      }
    }, 1000);
  }

  private stopCountdown(): void {
    if (this.countdownTimer) {
      clearInterval(this.countdownTimer);
      this.countdownTimer = undefined;
    }
  }

  private isValidQid(value: string): boolean {
    const v = (value || '').trim();
    return /^\d{11}$/.test(v);
  }

  private isValidPhone(value: string): boolean {
    const v = (value || '').trim();
    return /^\d{8}$/.test(v);
  }
}
