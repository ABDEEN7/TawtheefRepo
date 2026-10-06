import { TestSlotCandidateListItemDto } from '../../../../models/test-slot-details.dto';
import {
  TestSlotCandidateAttendanceStatus,
  TestSlotCandidateAttemptStatus,
  TestSlotCandidateIdentityStatus,
  TestSlotCandidateSessionStatus,
  TestSlotCandidateStatus,
} from '../../../../constants/test-slot-candidate-backend-status.constants';

export function canActOnTestSlotCandidate(
  candidate: TestSlotCandidateListItemDto,
  isCurrentUserRoomHead: boolean,
  hasManagePeriodPermission: boolean,
  actingCandidateId: string | null,
): boolean {
  return (
    isCurrentUserRoomHead &&
    hasManagePeriodPermission &&
    (
      [
        TestSlotCandidateSessionStatus.Approved,
        TestSlotCandidateSessionStatus.Ready,
        TestSlotCandidateSessionStatus.InProgress,
      ] as readonly string[]
    ).includes(candidate.sessionStatusBackendName) &&
    candidate.candidateStatusBackendName === TestSlotCandidateStatus.Assigned &&
    actingCandidateId === null
  );
}

export function canMarkTestSlotCandidatePresent(
  candidate: TestSlotCandidateListItemDto,
  canAct: boolean,
): boolean {
  return (
    canAct && candidate.attendanceStatusBackendName === TestSlotCandidateAttendanceStatus.Pending
  );
}

export function canVerifyTestSlotCandidateIdentity(
  candidate: TestSlotCandidateListItemDto,
  canAct: boolean,
): boolean {
  return (
    canAct &&
    candidate.attendanceStatusBackendName === TestSlotCandidateAttendanceStatus.Present &&
    candidate.identityVerificationStatusBackendName === TestSlotCandidateIdentityStatus.Pending
  );
}

export function canAuthorizeTestSlotCandidateEntry(
  candidate: TestSlotCandidateListItemDto,
  canAct: boolean,
): boolean {
  return (
    canAct &&
    candidate.attendanceStatusBackendName === TestSlotCandidateAttendanceStatus.Present &&
    candidate.identityVerificationStatusBackendName === TestSlotCandidateIdentityStatus.Matched
  );
}

export function canReportTestSlotCandidateOperationalIssue(
  candidate: TestSlotCandidateListItemDto,
  isCurrentUserRoomHead: boolean,
  hasManagePeriodPermission: boolean,
  actingCandidateId: string | null,
  authorizingAttemptId: string | null,
): boolean {
  return (
    isCurrentUserRoomHead &&
    hasManagePeriodPermission &&
    candidate.sessionStatusBackendName === TestSlotCandidateSessionStatus.InProgress &&
    candidate.candidateStatusBackendName === TestSlotCandidateStatus.Completed &&
    candidate.testAttemptId !== null &&
    (candidate.attemptStatusBackendName === TestSlotCandidateAttemptStatus.Completed ||
      candidate.attemptStatusBackendName === TestSlotCandidateAttemptStatus.TimedOut) &&
    !candidate.hasOpenInterruption &&
    actingCandidateId === null &&
    authorizingAttemptId === null
  );
}

export function canAuthorizeTestSlotCandidateResume(
  candidate: TestSlotCandidateListItemDto,
  isCurrentUserRoomHead: boolean,
  hasManagePeriodPermission: boolean,
): boolean {
  return (
    candidate.testAttemptId !== null &&
    candidate.sessionStatusBackendName === TestSlotCandidateSessionStatus.InProgress &&
    candidate.attemptStatusBackendName === TestSlotCandidateAttemptStatus.Interrupted &&
    candidate.resumeAllowed &&
    !candidate.resumeAuthorized &&
    isCurrentUserRoomHead &&
    hasManagePeriodPermission
  );
}

export function canRescheduleTestSlotCandidate(
  candidate: TestSlotCandidateListItemDto,
  isCurrentUserRoomHead: boolean,
  hasManagePeriodPermission: boolean,
): boolean {
  return (
    candidate.testAttemptId !== null &&
    candidate.sessionStatusBackendName === TestSlotCandidateSessionStatus.InProgress &&
    candidate.attemptStatusBackendName === TestSlotCandidateAttemptStatus.Interrupted &&
    candidate.rescheduleAllowed &&
    !candidate.resumeAuthorized &&
    isCurrentUserRoomHead &&
    hasManagePeriodPermission
  );
}
