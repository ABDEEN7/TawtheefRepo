// password-strength.service.ts
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PasswordStrengthService {
  passwordValue: string = '';
  strengthLevel: string = '';
  passwordInfoMessage: string | null = null;
  passwordInfoColor: string = '';

  private poorRegExp = /[a-z]/;
  private weakRegExp = /(?=.*?[0-9])/;
  private strongRegExp = /(?=.*?[#?!@$%^&*-])/;
  private whitespaceRegExp = /^$|\s+/;

  checkPasswordStrength(password: string): void {
    this.passwordValue = password;
    const passwordLength = password.length;
    const hasPoor = this.poorRegExp.test(password);
    const hasWeak = this.weakRegExp.test(password);
    const hasStrong = this.strongRegExp.test(password);
    const hasWhitespace = this.whitespaceRegExp.test(password);

    if (password === '') {
      this.resetStrength();
      return;
    }

    if (hasWhitespace) {
      this.passwordInfoMessage = 'Whitespaces are not allowed';
      this.passwordInfoColor = 'red';
      this.strengthLevel = '';
      return;
    }

    if (passwordLength < 8) {
      this.strengthLevel = 'poor';
      this.passwordInfoMessage = 'Weak. Must contain at least 8 characters.';
      this.passwordInfoColor = 'red';
    } else if (hasPoor || hasWeak || hasStrong) {
      this.strengthLevel = 'weak';
      this.passwordInfoMessage = 'Average. Must contain at least 1 letter or number.';
      this.passwordInfoColor = '#FFB54A';
    }

    if (passwordLength >= 8 && hasPoor && (hasWeak || hasStrong)) {
      this.strengthLevel = 'strong';
      this.passwordInfoMessage = 'Almost strong. Must contain a special symbol.';
      this.passwordInfoColor = '#1D9CFD';
    }

    if (passwordLength >= 8 && hasPoor && hasWeak && hasStrong) {
      this.strengthLevel = 'heavy';
      this.passwordInfoMessage = 'Awesome! You have a secure password.';
      this.passwordInfoColor = '#159F46';
    }
  }

  resetStrength(): void {
    this.strengthLevel = '';
    this.passwordInfoMessage = null;
  }
}
