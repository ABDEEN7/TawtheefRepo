import { DashboardChartColors } from '../../../dashboard/constants/dashboard-chart-colors';
import { AppointmentStatus, InterviewType, ScheduleStatus } from '../../interview-schedule/models/enums';
import { AttendanceStatus } from '../../interview-evaluation/models/enums';
import { FinalDecision, ResultReportStatus } from '../../interview-result-report/models/enums';

// Extends the main Dashboard palette (DashboardChartColors) with the dashboard.scss accent tokens, so
// doughnuts with more states than the base palette still get distinct, on-brand colours.
export const InterviewDashboardColors = {
  ...DashboardChartColors,
  maroon: '#8A1538',
  orange: '#E87532',
  teal: '#1F8A8A',
} as const;

const C = InterviewDashboardColors;

export const SCHEDULE_STATUS_COLOR: Record<ScheduleStatus, string> = {
  [ScheduleStatus.Draft]: C.muted,
  [ScheduleStatus.Proposed]: C.neutral,
  [ScheduleStatus.PendingApproval]: C.warning,
  [ScheduleStatus.Returned]: C.orange,
  [ScheduleStatus.Approved]: C.info,
  [ScheduleStatus.ReadyForExecution]: C.teal,
  [ScheduleStatus.InProgress]: C.update,
  [ScheduleStatus.Closed]: C.success,
  [ScheduleStatus.Cancelled]: C.danger,
};

export const APPOINTMENT_STATUS_COLOR: Record<AppointmentStatus, string> = {
  [AppointmentStatus.Held]: C.muted,
  [AppointmentStatus.Scheduled]: C.info,
  [AppointmentStatus.InInterview]: C.warning,
  [AppointmentStatus.UnderEvaluation]: C.update,
  [AppointmentStatus.Completed]: C.success,
  [AppointmentStatus.Closed]: C.teal,
  [AppointmentStatus.Rescheduled]: C.orange,
  [AppointmentStatus.Cancelled]: C.danger,
};

export const ATTENDANCE_COLOR: Record<AttendanceStatus, string> = {
  [AttendanceStatus.Present]: C.success,
  [AttendanceStatus.Late]: C.warning,
  [AttendanceStatus.NoShow]: C.danger,
  [AttendanceStatus.Withdrew]: C.update,
};

export const FINAL_DECISION_COLOR: Record<FinalDecision, string> = {
  [FinalDecision.CandidateForHiringProcess]: C.success,
  [FinalDecision.WaitingList]: C.warning,
  [FinalDecision.Rejected]: C.danger,
  [FinalDecision.NeedsAction]: C.update,
  [FinalDecision.NoShow]: C.neutral,
};

export const RESULT_REPORT_STATUS_COLOR: Record<ResultReportStatus, string> = {
  [ResultReportStatus.Creating]: C.muted,
  [ResultReportStatus.CommitteeReview]: C.muted,
  [ResultReportStatus.UnderReview]: C.warning,
  [ResultReportStatus.Returned]: C.orange,
  [ResultReportStatus.Approved]: C.success,
  [ResultReportStatus.Closed]: C.teal,
};

export const INTERVIEW_TYPE_COLOR: Record<InterviewType, string> = {
  [InterviewType.InPerson]: C.maroon,
  [InterviewType.Remote]: C.info,
};
