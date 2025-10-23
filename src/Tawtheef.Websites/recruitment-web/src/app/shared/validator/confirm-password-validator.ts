import {AbstractControl, ValidationErrors, ValidatorFn} from "@angular/forms";

export class ConfirmPasswordValidator {
  static MatchPassword: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    const password = control.get('newPassword');
    const confirmPassword = control.get('confirmPassword');

    if (!password || !confirmPassword) {
      return null;
    }

    return password.value === confirmPassword.value ? null : {passwordMismatch: true};
  };
}
