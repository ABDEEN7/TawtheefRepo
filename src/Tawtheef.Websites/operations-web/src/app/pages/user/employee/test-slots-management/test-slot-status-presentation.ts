import { StatusTone } from '../../../../shared/components/status-pill/status-pill.component';
import { TestSlotStatusIds } from './constants/test-slot-status.ids';

export function testSlotStatusTone(statusId: string): StatusTone {
  switch (statusId.toLowerCase()) {
    case TestSlotStatusIds.ready:
    case TestSlotStatusIds.started:
    case TestSlotStatusIds.rescheduled: return 'info';
    case TestSlotStatusIds.closed: return 'neutral';
    default: return 'neutral';
  }
}
