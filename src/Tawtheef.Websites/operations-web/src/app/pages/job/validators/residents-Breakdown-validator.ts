import { AbstractControl, ValidationErrors } from '@angular/forms';

export function residentsBreakdownValidator(control: AbstractControl): ValidationErrors | null {
  const residents = Number(control.get('residents')?.value ?? 0);
  const breakdowns = control.get('residentsBreakdowns')?.value ?? [];

  if (residents === 0) return null;

  const breakdownTotal = breakdowns.reduce((sum: number, item: any) => sum + Number(item.percentage ?? 0), 0);

  return breakdownTotal === residents ? null : { residentsMismatch: true };
}
