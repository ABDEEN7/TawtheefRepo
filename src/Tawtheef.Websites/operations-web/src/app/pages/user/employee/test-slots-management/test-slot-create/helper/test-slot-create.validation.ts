import { testSlotCreateForm } from './test-slot-create.form';

export function validateSlotDetails(
  details: ReturnType<typeof testSlotCreateForm>['value'],
): string[] {
  const requiredValues = [
    details.titleAr?.trim(),
    details.roomId,
    details.slotDate,
    details.startTime,
    details.endTime,
  ];

  if (requiredValues.some(value => !value)) return ['DETAILS'];
  if (isBeforeToday(details.slotDate!)) return ['DATE_RANGE'];
  if (details.endTime!.getTime() <= details.startTime!.getTime()) return ['TIME_RANGE'];

  return [];
}

function isBeforeToday(value: Date): boolean {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const date = new Date(value);
  date.setHours(0, 0, 0, 0);
  return date < today;
}
