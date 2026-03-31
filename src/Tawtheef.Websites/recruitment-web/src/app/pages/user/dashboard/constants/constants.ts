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
  CANCELLED: 'Cancelled',
  REQUIRES_UPDATE: 'ReturnedAttachment',
  PENDING_ATTACHMENT_APPROVAL: 'PendingAttachmentApproval',
  SUBMITTED: 'Submitted',
  READ: 'Read',
  REJECTED: 'Rejected',
} as const;

// Constants for status pill classes
export const STATUS_PILL_CLASSES: Record<JobStatus, string> = {
  [JOB_INVITATION_STATUSES.NEW_INVITATION]: 'info',
  [JOB_INVITATION_STATUSES.CLOSED]: 'soft',
  [JOB_INVITATION_STATUSES.READ]: 'neutral',
  [JOB_INVITATION_STATUSES.REJECTED]: 'soft',
  [JOB_INVITATION_STATUSES.CANCELLED]: 'soft',
  [JOB_INVITATION_STATUSES.REQUIRES_UPDATE]: 'warning',
  [JOB_INVITATION_STATUSES.PENDING_ATTACHMENT_APPROVAL]: 'info',
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
    showDetails: false,
  },
  [JOB_INVITATION_STATUSES.SUBMITTED]: {
    showApply: false,
    showDetails: false,
  },
  [JOB_INVITATION_STATUSES.REJECTED]: {
    showApply: false,
    showDetails: false,
  },
  [JOB_INVITATION_STATUSES.CLOSED]: {
    showApply: false,
    showDetails: false,
  },
  [JOB_INVITATION_STATUSES.CANCELLED]: {
    showApply: false,
    showDetails: false,
  },
  [JOB_INVITATION_STATUSES.REQUIRES_UPDATE]: {
    showApply: false,
    showDetails: true,
  },
  [JOB_INVITATION_STATUSES.PENDING_ATTACHMENT_APPROVAL]: {
    showApply: false,
    showDetails: true,
  },
  [JOB_INVITATION_STATUSES.READ]: {
    showApply: true,
    showDetails: false
  },
} as const;