import { CandidateTypeKpis, ProfileBreakdown } from './dashboard-candidates.model';
import { GroupCount } from './dashboard-common.model';
import { OperationsDashboardFilterSnapshot } from './dashboard-filters.model';
import { InvitationKpis } from './dashboard-invitations.model';
import { JobBreakdown, JobKpis } from './dashboard-jobs.model';

export interface DashboardOverview {
  role: string;
  filters: OperationsDashboardFilterSnapshot;
  kpis: DashboardKpis;
  profileBreakdown: ProfileBreakdown;
  candidateTypeKpis: CandidateTypeKpis;
  jobKpis: JobKpis;
  invitationKpis: InvitationKpis;
  jobBreakdown: JobBreakdown;
  taskMonitoring: TaskMonitoring;
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
  pendingProfiles: number;
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
