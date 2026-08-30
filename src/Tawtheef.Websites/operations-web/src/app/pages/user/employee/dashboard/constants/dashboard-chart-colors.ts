import { InvitationStatus, JobStatus } from "../../../../../core/enums/lookups.enum";

export const DashboardChartColors = {
  success: '#2F8A3A',
  info: '#488ADA',
  warning: '#FFB547',
  danger: '#D9182D',
  update: '#6C4BB6',
  neutral: '#64748B',
  muted: '#9CA3AF',
  noData: '#E5E7EB',
} as const;

export function profileStatusColor(status: string): string {
  switch (status) {
    case 'Approved':
      return DashboardChartColors.success;

    case 'Submitted':
      return DashboardChartColors.warning;

    case 'UnderReview':
      return DashboardChartColors.info;

    case 'RequiresUpdate':
      return DashboardChartColors.update;

    case 'Rejected':
    case 'AdminCancelled':
      return DashboardChartColors.danger;

    case 'InCreation':
      return DashboardChartColors.neutral;

    default:
      return DashboardChartColors.muted;
  }
}

export function jobStatusColor(status: JobStatus): string {
  switch (status) {
    case JobStatus.Published:
      return DashboardChartColors.success;

    case JobStatus.ReadyForAnnouncement:
      return DashboardChartColors.info;

    case JobStatus.PendingApproval:
    case JobStatus.PendingPointConfiguration:
    case JobStatus.PendingPointApproval:
      return DashboardChartColors.warning;

    case JobStatus.NeedUpdate:
    case JobStatus.NeedPointUpdate:
      return DashboardChartColors.update;

    case JobStatus.Rejected:
    case JobStatus.Cancelled:
      return DashboardChartColors.danger;

    case JobStatus.Draft:
    case JobStatus.Closed:
      return DashboardChartColors.neutral;

    default:
      return DashboardChartColors.muted;
  }
}

export function invitationSummaryColor(
  status:
    | 'accepted'
    | 'pending'
    | 'pendingAttachmentApproval'
    | 'expired'
    | 'rejected',
): string {
  switch (status) {
    case 'accepted':
      return DashboardChartColors.success;

    case 'pending':
      return DashboardChartColors.warning;

    case 'pendingAttachmentApproval':
      return DashboardChartColors.info;

    case 'rejected':
      return DashboardChartColors.danger;

    case 'expired':
      return DashboardChartColors.neutral;

    default:
      return DashboardChartColors.muted;
  }
}

export function invitationStatusColor(status: InvitationStatus): string {
  switch (status) {
    case InvitationStatus.ExamEligible:
      return DashboardChartColors.success;

    case InvitationStatus.NewInvitation:
    case InvitationStatus.PendingAttachmentApproval:
      return DashboardChartColors.warning;

    case InvitationStatus.Read:
      return DashboardChartColors.info;

    case InvitationStatus.ReturnedAttachment:
      return DashboardChartColors.update;

    case InvitationStatus.Rejected:
    case InvitationStatus.Cancelled:
      return DashboardChartColors.danger;

    case InvitationStatus.Closed:
    case InvitationStatus.Expired:
      return DashboardChartColors.neutral;

    default:
      return DashboardChartColors.muted;
  }
}

export function employeeWorkloadColor(key: string): string {
  switch (key) {
    case 'ProfilesAwaitingDistribution':
      return DashboardChartColors.warning;

    case 'ActiveReviewWorkload':
      return DashboardChartColors.info;

    case 'CompletedReviews':
      return DashboardChartColors.success;

    default:
      return DashboardChartColors.muted;
  }
}
