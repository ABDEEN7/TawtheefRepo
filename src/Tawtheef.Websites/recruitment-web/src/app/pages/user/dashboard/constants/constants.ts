import { InvitationStatus, JobCategory } from "../../../../core/enums/lookups.enum";
import { ActionConfig } from "../types/action-config.type";


// Constants for status pill classes
export const STATUS_PILL_CLASSES: Record<InvitationStatus, string> = {
  [InvitationStatus.NewInvitation]: 'info',
  [InvitationStatus.Closed]: 'soft',
  [InvitationStatus.Cancelled]: 'soft',
  [InvitationStatus.Read]: 'neutral',
  [InvitationStatus.ReturnedAttachment]: 'warning',
  [InvitationStatus.PendingAttachmentApproval]: 'info',
  [InvitationStatus.Rejected]: 'soft',
  [InvitationStatus.ExamEligible]: 'success'
} as const;

// Constants for type badge classes
export const TYPE_BADGE_CLASSES = {
  [JobCategory.Academic]: 'badge-soft academic',
  [JobCategory.Administrative]: 'badge-soft administrative',
  [JobCategory.Labor]: 'badge-soft labor'
} as const;

// Action configurations
export const ACTION_CONFIGS: Record<InvitationStatus, ActionConfig> = {
  [InvitationStatus.NewInvitation]: {
    showApply: true,
    showDetails: false,
  },
  [InvitationStatus.ExamEligible]: {
    showApply: false,
    showDetails: false,
  },
  [InvitationStatus.Rejected]: {
    showApply: false,
    showDetails: false,
  },
  [InvitationStatus.Closed]: {
    showApply: false,
    showDetails: false,
  },
  [InvitationStatus.Cancelled]: {
    showApply: false,
    showDetails: false,
  },
  [InvitationStatus.ReturnedAttachment]: {
    showApply: false,
    showDetails: true,
  },
  [InvitationStatus.PendingAttachmentApproval]: {
    showApply: false,
    showDetails: true,
  },
  [InvitationStatus.Read]: {
    showApply: true,
    showDetails: false
  },
} as const;