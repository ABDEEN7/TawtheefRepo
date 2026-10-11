import { StatusTone } from '../../../../shared/components/status-pill/status-pill.component';
import {
  TestSlotCandidateAttendanceStatus,
  TestSlotCandidateAttemptStatus,
  TestSlotCandidateIdentityStatus,
  TestSlotCandidateStatus,
} from '../test-slots-management/constants/test-slot-candidate-backend-status.constants';

export function reportStatusTone(status: string): StatusTone {
  switch (status) {
    case 'Approved': return 'success';
    case 'UnderReview': return 'info';
    case 'Returned': return 'warning';
    case 'Closed': return 'neutral';
    default: return 'neutral';
  }
}

export function candidateResultStatusTone(status: string): StatusTone {
  switch (status) {
    case 'QualifiedForInterview': return 'success';
    case 'RequiresAction': return 'warning';
    case 'NotQualifiedForInterview': return 'neutral';
    case 'NoShow': return 'danger';
    case 'NotCompleted': return 'warning';
    default: return 'neutral';
  }
}

export function attendanceStatusTone(status: string): StatusTone {
  switch (status) {
    case TestSlotCandidateAttendanceStatus.Present: return 'success';
    case TestSlotCandidateAttendanceStatus.NoShow: return 'danger';
    case TestSlotCandidateAttendanceStatus.Pending: return 'neutral';
    default: return 'neutral';
  }
}

export function identityStatusTone(status: string): StatusTone {
  switch (status) {
    case TestSlotCandidateIdentityStatus.Matched: return 'success';
    case TestSlotCandidateIdentityStatus.NotMatched: return 'danger';
    case TestSlotCandidateIdentityStatus.Pending: return 'neutral';
    default: return 'neutral';
  }
}

export function candidateAssignmentStatusTone(status: string): StatusTone {
  switch (status) {
    case TestSlotCandidateStatus.Assigned: return 'neutral';
    case TestSlotCandidateStatus.Authorized:
    case TestSlotCandidateStatus.Completed: return 'success';
    case TestSlotCandidateStatus.Started:
    case TestSlotCandidateStatus.Rescheduled: return 'info';
    case TestSlotCandidateStatus.Cancelled: return 'danger';
    default: return 'neutral';
  }
}

export function attemptStatusTone(status: string | null | undefined): StatusTone {
  switch (status) {
    case TestSlotCandidateAttemptStatus.InProgress: return 'info';
    case TestSlotCandidateAttemptStatus.Completed: return 'success';
    case TestSlotCandidateAttemptStatus.Interrupted:
    case TestSlotCandidateAttemptStatus.TimedOut: return 'warning';
    case TestSlotCandidateAttemptStatus.Cancelled: return 'danger';
    default: return 'neutral';
  }
}

export function attemptPartStatusTone(status: string): StatusTone {
  switch (status) {
    case 'NotStarted': return 'neutral';
    case 'InProgress': return 'info';
    case 'Completed': return 'success';
    case 'Interrupted':
    case 'TimedOut': return 'warning';
    default: return 'neutral';
  }
}
