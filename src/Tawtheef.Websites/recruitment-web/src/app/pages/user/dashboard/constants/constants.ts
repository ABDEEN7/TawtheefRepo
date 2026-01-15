import { ActionConfig } from "../types/action-config.type";
import { InvitationStatus } from "../types/invitation-status.type";
import { JobStatus } from "../types/job-status.type";

// Constants for job types
export const JOB_TYPES = {
  ACADEMIC: 'Academic',
  ADMINISTRATIVE: 'Administrative',
  LABOR: 'Labor'
} as const;


// Constants for job statuses
export const JOB_INVITATION_STATUSES = {
  NEW_INVITATION: 'NewInvitation',
  CLOSED: 'Closed',
  UNDER_REVIEW: 'UnderReview',
  APPROVED: 'Approved',
  READ: 'Read',
  REJECTED: 'Rejected',
  CANCELLED: 'Cancelled',
  REQUIRES_UPDATE: 'RequiresUpdate',
  SUBMITTED: 'Submitted'
} as const;

// Constants for status pill classes
export const STATUS_PILL_CLASSES: Record<JobStatus, string> = {
  [JOB_INVITATION_STATUSES.NEW_INVITATION]: 'info',
  [JOB_INVITATION_STATUSES.CLOSED]: 'neutral',
  [JOB_INVITATION_STATUSES.UNDER_REVIEW]: 'soft',
  [JOB_INVITATION_STATUSES.APPROVED]: 'success',
  [JOB_INVITATION_STATUSES.READ]: 'soft',
  [JOB_INVITATION_STATUSES.REJECTED]: 'neutral',
  [JOB_INVITATION_STATUSES.CANCELLED]: 'neutral',
  [JOB_INVITATION_STATUSES.REQUIRES_UPDATE]: 'soft',
  [JOB_INVITATION_STATUSES.SUBMITTED]: 'success'
} as const;

// Constants for type badge classes
export const TYPE_BADGE_CLASSES = {
  [JOB_TYPES.ACADEMIC]: 'badge-soft academic',
  [JOB_TYPES.ADMINISTRATIVE]: 'badge-soft administrative',
  [JOB_TYPES.LABOR]: 'badge-soft labor'
} as const;

// Action configurations
export const ACTION_CONFIGS: Record<InvitationStatus, ActionConfig> = {
  [JOB_INVITATION_STATUSES.NEW_INVITATION]: {
    showApply: true,
    showView: false,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.SUBMITTED]: {
    showApply: false,
    showView: true,
    showTrack: false,
    showDetails: false,
    showWithdraw: true
  },
  [JOB_INVITATION_STATUSES.UNDER_REVIEW]: {
    showApply: false,
    showView: false,
    showTrack: true,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.REJECTED]: {
    showApply: false,
    showView: false,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.CLOSED]: {
    showApply: false,
    showView: false,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.APPROVED]: {
    showApply: false,
    showView: true,
    showTrack: true,
    showDetails: true,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.CANCELLED]: {
    showApply: false,
    showView: true,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.REQUIRES_UPDATE]: {
    showApply: false,
    showView: true,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
  [JOB_INVITATION_STATUSES.READ]: {
    showApply: true,
    showView: false,
    showTrack: false,
    showDetails: false,
    showWithdraw: false
  },
} as const;