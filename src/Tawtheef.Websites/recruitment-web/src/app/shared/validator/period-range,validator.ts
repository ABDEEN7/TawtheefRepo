import {AbstractControl, ValidationErrors} from '@angular/forms';

export function periodRangeValidator(control: AbstractControl): ValidationErrors | null {
  const val = control.value as Date[] | null;
  if (!val || val.length !== 2) {
    return null;
  }

  const [from, to] = val;
  if (!from || !to) return null;

  const fromTime = new Date(from).getTime();
  const toTime   = new Date(to).getTime();

  return fromTime <= toTime ? null : { periodRange: true };
}
