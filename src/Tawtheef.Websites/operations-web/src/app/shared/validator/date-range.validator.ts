import {AbstractControl, ValidationErrors, ValidatorFn} from '@angular/forms';

export function DateRangeValidator(fromField: string, toField: string): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const from = control.get(fromField)?.value;
    const to = control.get(toField)?.value;

    if (!from || !to) {
      return null;
    }

    const fromDate = new Date(from);
    const toDate = new Date(to);

    return fromDate > toDate ? { dateRange: true } : null;
  };
}
