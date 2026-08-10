import { StatusCount } from './dashboard-common.model';

export interface JobKpis {
  totalJobs: number;
  draftJobs: number;
  activeJobs: number;
  pendingReviewJobs: number;
  approvedJobs: number;
  rejectedJobs: number;
  newJobsToday: number;
  pendingPointConfigurationJobs: number;
  needPointUpdateJobs: number;
  pendingPointApprovalJobs: number;
  needUpdateJobs: number;
  readyForAnnouncementJobs: number;
  publishedJobs: number;
  closedJobs: number;
  cancelledJobs: number;
}

export interface JobBreakdown {
  byStatus: StatusCount[];
}

export interface LatestJob {
  jobId: string;
  jobTitle: string;
  managementName: string;
  status: string;
  candidatesCount: number;
  invitationsSent: number;
}
