import { StatusTone } from '../../../../shared/components/status-pill/status-pill.component';

export function testSessionStatusTone(status: string): StatusTone {
  switch (status) {
    case 'Draft': return 'neutral';
    case 'PendingApproval':
    case 'Returned': return 'warning';
    case 'Approved': return 'success';
    case 'Ready':
    case 'InProgress': return 'info';
    case 'Closed': return 'neutral';
    case 'Cancelled':
    case 'Rejected': return 'danger';
    default: return 'neutral';
  }
}
