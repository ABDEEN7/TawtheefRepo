import { StatusTone } from '../../../../shared/components/status-pill/status-pill.component';

export function examStatusTone(status: string): StatusTone {
  switch (status) {
    case 'Draft': return 'neutral';
    case 'PendingApproval': return 'warning';
    case 'Returned': return 'warning';
    case 'Approved': return 'success';
    case 'Cancelled':
    case 'Rejected': return 'danger';
    default: return 'neutral';
  }
}
