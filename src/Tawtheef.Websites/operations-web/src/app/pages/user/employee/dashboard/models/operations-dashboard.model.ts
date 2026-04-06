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
  jobKpis: JobKpis;
  jobBreakdown: JobBreakdown;
  taskMonitoring: TaskMonitoring;
  profileTrend: TrendSeries;
  taskCompletionTrend: TrendSeries;
  topPerformers: PerformanceRank[];
  underPerformers: PerformanceRank[];
}

export interface OperationsDashboardFilterSnapshot {
  fromDateUtc?: string;
  toDateUtc?: string;
  departmentId?: string;
  status?: string;
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
  pendingProfiles: number;
  returnedProfiles: number;
  approvalRate: number;
  rejectionRate: number;
  averageApprovalHours: number;
  totalAssignedTasks: number;
  remainingTasks: number;
  overdueTasks: number;
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
  aging: AgingBucket[];
}

export interface JobKpis {
  totalJobs: number;
  activeJobs: number;
  pendingReviewJobs: number;
  approvedJobs: number;
  rejectedJobs: number;
  newJobsToday: number;
}

export interface JobBreakdown {
  byStatus: StatusCount[];
  byDepartment: GroupCount[];
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

export interface AgingBucket {
  bucket: string;
  count: number;
}

export interface TeamPerformanceRow {
  employeeId: string;
  name: string;
  employeeNumber?: string;
  departmentName?: string;
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
