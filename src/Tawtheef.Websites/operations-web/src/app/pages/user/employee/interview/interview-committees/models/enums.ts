export enum CommitteeStatus {
  Draft = 1,
  PendingApproval = 2,
  Returned = 3,
  Approved = 4,
  Stopped = 5,
  Closed = 6,
  Cancelled = 7,
}

export enum CommitteeRole {
  Chair = 1,
  Evaluator = 2,
  Observer = 3,
  Admin = 4,
}

export enum EvaluationScope {
  AllAxes = 1,
  SelectedAxes = 2,
}

// Mirrors CommitteeConstants.MinimumCommitteeMembers on the backend (Chair included).
export const MINIMUM_COMMITTEE_MEMBERS = 3;

export const COMMITTEE_STATUS_LABELS: Record<CommitteeStatus, string> = {
  [CommitteeStatus.Draft]: 'INTERVIEW_COMMITTEES.STATUS.DRAFT',
  [CommitteeStatus.PendingApproval]: 'INTERVIEW_COMMITTEES.STATUS.PENDING_APPROVAL',
  [CommitteeStatus.Returned]: 'INTERVIEW_COMMITTEES.STATUS.RETURNED',
  [CommitteeStatus.Approved]: 'INTERVIEW_COMMITTEES.STATUS.APPROVED',
  [CommitteeStatus.Stopped]: 'INTERVIEW_COMMITTEES.STATUS.STOPPED',
  [CommitteeStatus.Closed]: 'INTERVIEW_COMMITTEES.STATUS.CLOSED',
  [CommitteeStatus.Cancelled]: 'INTERVIEW_COMMITTEES.STATUS.CANCELLED',
};

export const COMMITTEE_STATUS_PILL: Record<CommitteeStatus, 'neutral' | 'warning' | 'danger' | 'success' | 'info'> = {
  [CommitteeStatus.Draft]: 'neutral',
  [CommitteeStatus.PendingApproval]: 'warning',
  [CommitteeStatus.Returned]: 'danger',
  [CommitteeStatus.Approved]: 'success',
  [CommitteeStatus.Stopped]: 'warning',
  [CommitteeStatus.Closed]: 'info',
  [CommitteeStatus.Cancelled]: 'danger',
};

export const COMMITTEE_ROLE_LABELS: Record<CommitteeRole, string> = {
  [CommitteeRole.Chair]: 'INTERVIEW_COMMITTEES.ROLE.CHAIR',
  [CommitteeRole.Evaluator]: 'INTERVIEW_COMMITTEES.ROLE.EVALUATOR',
  [CommitteeRole.Observer]: 'INTERVIEW_COMMITTEES.ROLE.OBSERVER',
  [CommitteeRole.Admin]: 'INTERVIEW_COMMITTEES.ROLE.ADMIN',
};

// Chair is assigned only through the dedicated chair selector, never from a member row.
export const MEMBER_ROLES: CommitteeRole[] = [CommitteeRole.Evaluator, CommitteeRole.Observer, CommitteeRole.Admin];
