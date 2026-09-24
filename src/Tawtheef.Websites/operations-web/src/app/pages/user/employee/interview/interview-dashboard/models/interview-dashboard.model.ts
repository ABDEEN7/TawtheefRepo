import { AppointmentStatus, InterviewType, ScheduleStatus } from '../../interview-schedule/models/enums';
import {
  AttendanceStatus,
  OperationalIssueStatus,
  OperationalIssueType,
} from '../../interview-evaluation/models/enums';
import { FinalDecision, ResultReportStatus } from '../../interview-result-report/models/enums';

export type DatePreset = 'all' | 'today' | 'week' | 'month' | 'custom';

// Mirrors InterviewDashboardFilter (dates as yyyy-MM-dd in the viewer's timezone; the service adds
// utcOffsetMinutes so the server can map those days onto UTC appointment times).
export interface InterviewDashboardFilters {
  fromDate?: string;
  toDate?: string;
  jobId?: string;
  committeeId?: string;
  scheduleId?: string;
  interviewType?: InterviewType;
  scheduleStatus?: ScheduleStatus;
  resultReportStatus?: ResultReportStatus;
  finalDecision?: FinalDecision;
}

export interface InterviewDashboardCount {
  key: number;
  count: number;
}

export interface InterviewDashboardOverview {
  sections: { templates: boolean; committees: boolean; execution: boolean; results: boolean };
  templates: { active: number; approved: number } | null;
  committees: { total: number; approved: number; byStatus: InterviewDashboardCount[] } | null;
  schedules: {
    total: number;
    scheduled: number;
    inProgress: number;
    closed: number;
    byStatus: InterviewDashboardCount[];
  } | null;
  candidates: {
    scheduled: number;
    present: number;
    late: number;
    noShow: number;
    withdrew: number;
    attendanceNotRecorded: number;
    interviewsCompleted: number;
    evaluationsCompleted: number;
    rescheduled: number;
    cancelled: number;
    byAppointmentStatus: InterviewDashboardCount[];
    byInterviewType: InterviewDashboardCount[];
  } | null;
  issues: {
    total: number;
    open: number;
    openBlocking: number;
    resolved: number;
    waived: number;
    byType: { type: OperationalIssueType; open: number; closed: number }[];
  } | null;
  results: {
    candidates: number;
    qualified: number;
    notQualified: number;
    candidateForHiringProcess: number;
    waitingList: number;
    rejected: number;
    needsAction: number;
    noShow: number;
    pendingDecision: number;
    reportsByStatus: InterviewDashboardCount[];
  } | null;
  activity: {
    granularity: 'Day' | 'Month';
    points: { period: string; scheduled: number; evaluationsCompleted: number; noShow: number }[];
  } | null;
}

export interface InterviewDashboardScheduleRow {
  scheduleId: string;
  titleAr: string;
  titleEn: string | null;
  jobNameAr: string | null;
  jobNameEn: string | null;
  committeeNameAr: string | null;
  committeeNameEn: string | null;
  status: ScheduleStatus;
  firstAppointmentAt: string | null;
  lastAppointmentAt: string | null;
  candidates: number;
  present: number;
  noShow: number;
  evaluationsCompleted: number;
  openIssues: number;
  reportCode: string | null;
  reportStatus: ResultReportStatus | null;
}

export interface InterviewDashboardCandidateRow {
  appointmentId: string;
  scheduleId: string;
  candidateNameAr: string | null;
  candidateNameEn: string | null;
  jobNameAr: string | null;
  jobNameEn: string | null;
  scheduleTitleAr: string;
  scheduleTitleEn: string | null;
  startAt: string;
  interviewType: InterviewType;
  status: AppointmentStatus;
  attendanceStatus: AttendanceStatus | null;
  submittedEvaluations: number;
  requiredEvaluations: number;
}

export interface InterviewDashboardResultRow {
  candidateId: string;
  scheduleId: string;
  reportCode: string;
  reportStatus: ResultReportStatus;
  candidateNameAr: string | null;
  candidateNameEn: string | null;
  jobNameAr: string | null;
  jobNameEn: string | null;
  scheduleTitleAr: string;
  scheduleTitleEn: string | null;
  finalScore: number;
  qualificationScore: number | null;
  isQualified: boolean;
  finalDecision: FinalDecision | null;
  decidedAt: string | null;
}

export interface InterviewDashboardIssueRow {
  issueId: string;
  scheduleId: string;
  appointmentId: string;
  issueType: OperationalIssueType;
  status: OperationalIssueStatus;
  isBlocking: boolean;
  description: string | null;
  candidateNameAr: string | null;
  candidateNameEn: string | null;
  scheduleTitleAr: string;
  scheduleTitleEn: string | null;
  createdDate: string;
  resolvedAt: string | null;
}

// Mirrors InterviewDashboardLookupKind / InterviewDashboardLookupDto.
export enum InterviewDashboardLookupKind {
  Job = 1,
  Committee = 2,
  Schedule = 3,
}

export interface InterviewDashboardLookup {
  id: string;
  nameAr: string;
  nameEn: string | null;
  hintAr: string | null;
  hintEn: string | null;
}

// Table-local quick filters (mirror InterviewDashboardAttendanceView / InterviewDashboardDecisionView).
export enum AttendanceView {
  Present = 1,
  NoShow = 2,
  Withdrew = 3,
  Late = 4,
  NotRecorded = 5,
}

export enum DecisionView {
  CandidateForHiringProcess = 1,
  WaitingList = 2,
  Rejected = 3,
  NeedsAction = 4,
  NoShow = 5,
  Pending = 6,
}

export type ReportTab = 'schedules' | 'candidates' | 'results' | 'issues';
