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
  if (details.endTime!.getTime() <= details.startTime!.getTime()) return ['TIME_RANGE'];

  return [];
}
