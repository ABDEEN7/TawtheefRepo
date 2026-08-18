import { ProfileBreakdown } from './dashboard-candidates.model';
import { GroupCount } from './dashboard-common.model';
import { OperationsDashboardFilterSnapshot } from './dashboard-filters.model';
import { InvitationKpis } from './dashboard-invitations.model';
import { JobBreakdown, JobKpis } from './dashboard-jobs.model';

export interface DashboardOverview {
  role: string;
  filters: OperationsDashboardFilterSnapshot;
  kpis: DashboardKpis;
  profileBreakdown: ProfileBreakdown;
  jobKpis: JobKpis;
  jobBreakdown: JobBreakdown;
  invitationKpis: InvitationKpis;
  kpiTrends: DashboardKpiTrends;
  taskMonitoring: TaskMonitoring;
}

export interface DashboardMetricTrend {
  previousValue: number;
  changePercentage: number | null;
}

export interface DashboardKpiTrends {
  totalProfiles: DashboardMetricTrend;
  approvedProfiles: DashboardMetricTrend;
  underReviewProfiles: DashboardMetricTrend;
  unassignedProfiles: DashboardMetricTrend;
  publishedJobs: DashboardMetricTrend;
  totalInvitations: DashboardMetricTrend;
  acceptedInvitations: DashboardMetricTrend;
}

export interface DashboardKpis {
  totalEmployees: number;
  activeEmployees: number;
  totalProfiles: number;
  newProfilesToday: number;
  newProfilesThisWeek: number;
  newProfilesThisMonth: number;
  approvedProfiles: number;
  rejectedProfiles: number;
  inCreationProfiles: number;
  submittedProfiles: number;
  underReviewProfiles: number;
  returnedProfiles: number;
  approvalRate: number;
  rejectionRate: number;
  averageApprovalHours: number;
  totalAssignedTasks: number;
  completedTasks: number;
  remainingTasks: number;
  overdueTasks: number;
  unassignedProfiles: number;
  followedMinisterOfficeCandidates: number;
}

export interface TaskMonitoring {
  taskStatusStacked: GroupCount[];
}
