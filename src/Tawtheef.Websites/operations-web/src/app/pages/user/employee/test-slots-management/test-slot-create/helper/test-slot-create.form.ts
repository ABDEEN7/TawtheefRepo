import { FormControl, FormGroup, Validators } from '@angular/forms';

export function testSlotCreateForm() {
  return new FormGroup({
    titleAr: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(200)],
    }),
    titleEn: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(200)] }),
    roomId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    slotDate: new FormControl<Date | null>(null, Validators.required),
    startTime: new FormControl<Date | null>(null, Validators.required),
    endTime: new FormControl<Date | null>(null, Validators.required),
  });
}
