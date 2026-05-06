import { PaginationMetadata } from '../../../../../core/models/pagination-metadata.model';

export interface OperationsDashboardFilters {
  fromDateUtc?: string;
  toDateUtc?: string;
  departmentId?: string;
  status?: string;
  pageNumber?: number;
  pageSize?: number;
  search?: string;
  sortBy?: string;
  sortDirection?: string;
}

export interface OperationsDashboardResponse {
  role: string;
  filters: OperationsDashboardFilterSnapshot;
  kpis: DashboardKpis;
  profileBreakdown: ProfileBreakdown;
  candidateTypeKpis: CandidateTypeKpis;
  jobKpis: JobKpis;
  invitationKpis: InvitationKpis;
  jobBreakdown: JobBreakdown;
  taskMonitoring: TaskMonitoring;
  profileTrend: TrendSeries;
  taskCompletionTrend: TrendSeries;
  latestJobs: LatestJob[];
  topPerformers: PerformanceRank[];
  underPerformers: PerformanceRank[];
}

export interface OperationsDashboardFilterSnapshot {
  fromDateUtc?: string;
  toDateUtc?: string;
  departmentId?: string;
  status?: string;
}

export interface DashboardOverview {
  role: string;
  filters: OperationsDashboardFilterSnapshot;
  kpis: DashboardKpis;
  jobKpis: JobKpis;
  invitationKpis: InvitationKpis;
}

export interface CandidateStatusSummary {
  totalProfiles: number;
  inCreationProfiles: number;
  submittedProfiles: number;
  underReviewProfiles: number;
  approvedProfiles: number;
  returnedProfiles: number;
  rejectedProfiles: number;
  profileBreakdown: ProfileBreakdown;
}

export interface CandidateTypeSummary {
  candidateTypeKpis: CandidateTypeKpis;
  byCandidateType: CandidateTypeCount[];
}

export interface JobsSummary {
  jobKpis: JobKpis;
  invitationKpis: InvitationKpis;
  jobBreakdown: JobBreakdown;
}

export interface EmployeeIndicators {
  activeEmployees: number;
  remainingTasks: number;
  unassignedProfiles: number;
  completedTasks: number;
}

export interface EmployeeReviewOutcomes {
  totalProfiles: number;
  approvedProfiles: number;
  returnedProfiles: number;
  unassignedProfiles: number;
  pendingProfiles: number;
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

export interface TrendSeries {
  points: TrendPoint[];
}

export interface TrendPoint {
  label: string;
  value: number;
}

export interface ProfileBreakdown {
  byStatus: StatusCount[];
  byDepartment: GroupCount[];
  byPriority: GroupCount[];
  byCandidateType: CandidateTypeCount[];
  aging: AgingBucket[];
}

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

export interface InvitationKpis {
  totalInvitations: number;
  acceptedInvitations: number;
  pendingInvitations: number;
  pendingAttachmentApproval: number;
}

export interface JobBreakdown {
  byStatus: StatusCount[];
  byDepartment: GroupCount[];
}

export interface LatestJob {
  jobId: string;
  jobTitle: string;
  managementName: string;
  status: string;
  candidatesCount: number;
  invitationsSent: number;
}

export interface TaskMonitoring {
  tasksByDepartment: GroupCount[];
  tasksByUrgency: GroupCount[];
  taskStatusStacked: GroupCount[];
}

export interface StatusCount {
  status: string;
  count: number;
}

export interface GroupCount {
  label: string;
  count: number;
}

export interface CandidateTypeCount {
  candidateTypeId?: string;
  key: string;
  label: string;
  count: number;
}

export interface CandidateTypeKpis {
  total: number;
  qatari: number;
  nonQatari: number;
  sonOfQatariMother: number;
  wifeOfQatari: number;
  gcc: number;
  residentQatar: number;
  unknown: number;
}

export interface AgingBucket {
  bucket: string;
  count: number;
}

export interface TeamPerformanceRow {
  employeeId: string;
  name: string;
  employeeNumber?: string;
  departmentName?: string;
  jobDescription?: string;
  assignedTasks: number;
  activeTasks: number;
  completedTasks: number;
  remainingTasks: number;
  overdueTasks: number;
  profilesReviewed: number;
  approvalRate: number;
  rejectionRate: number;
  averageHandlingHours: number;
  averageResponseHours: number;
  workloadRatio: number;
  workloadBalanceIndicator: string;
  productivityScore: number;
}

export interface PaginatedTeamPerformance {
  items: TeamPerformanceRow[];
  metadata: PaginationMetadata;
}

export interface PerformanceRank {
  employeeId: string;
  name: string;
  score: number;
}
