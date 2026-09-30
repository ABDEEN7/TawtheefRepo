import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TabsModule } from 'primeng/tabs';
import { TooltipModule } from 'primeng/tooltip';

import { AuthService } from '../../../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../../../core/constants/permissions';
import { routes } from '../../../../../../../routes/routes';
import { PaginationComponent } from '../../../../../../../shared/components/pagination/pagination.component';
import {
  APPOINTMENT_STATUS_LABELS,
  APPOINTMENT_STATUS_PILL,
  INTERVIEW_TYPE_LABELS,
  SCHEDULE_STATUS_LABELS,
  SCHEDULE_STATUS_PILL,
} from '../../../interview-schedule/models/enums';
import {
  ATTENDANCE_STATUS_LABELS,
  ATTENDANCE_STATUS_PILL,
  AttendanceStatus,
  OPERATIONAL_ISSUE_STATUS_LABELS,
  OPERATIONAL_ISSUE_STATUS_PILL,
  OPERATIONAL_ISSUE_TYPE_LABELS,
  OperationalIssueStatus,
} from '../../../interview-evaluation/models/enums';
import {
  FINAL_DECISION_LABELS,
  FINAL_DECISION_PILL,
  RESULT_REPORT_STATUS_LABELS,
  RESULT_REPORT_STATUS_PILL,
} from '../../../interview-result-report/models/enums';
import { InterviewDashboardFacade } from '../../interview-dashboard.facade';
import { InterviewDashboardStore } from '../../interview-dashboard.store';
import { AttendanceView, DecisionView, ReportTab } from '../../models/interview-dashboard.model';

@Component({
  selector: 'app-dashboard-reports',
  templateUrl: './dashboard-reports.html',
  styleUrl: './dashboard-reports.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    TabsModule,
    TableModule,
    SelectModule,
    TooltipModule,
    PaginationComponent,
  ],
})
export class DashboardReportsComponent {
  readonly store = inject(InterviewDashboardStore);
  readonly facade = inject(InterviewDashboardFacade);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  // Label/pill maps owned by the source modules (their i18n namespaces are loaded by the page).
  readonly SCHEDULE_STATUS_LABELS: Record<number, string> = SCHEDULE_STATUS_LABELS;
  readonly SCHEDULE_STATUS_PILL: Record<number, string> = SCHEDULE_STATUS_PILL;
  readonly APPOINTMENT_STATUS_LABELS: Record<number, string> = APPOINTMENT_STATUS_LABELS;
  readonly APPOINTMENT_STATUS_PILL: Record<number, string> = APPOINTMENT_STATUS_PILL;
  readonly INTERVIEW_TYPE_LABELS: Record<number, string> = INTERVIEW_TYPE_LABELS;
  readonly ATTENDANCE_STATUS_LABELS: Record<number, string> = ATTENDANCE_STATUS_LABELS;
  readonly ATTENDANCE_STATUS_PILL: Record<number, string> = ATTENDANCE_STATUS_PILL;
  readonly OPERATIONAL_ISSUE_TYPE_LABELS: Record<number, string> = OPERATIONAL_ISSUE_TYPE_LABELS;
  readonly OPERATIONAL_ISSUE_STATUS_LABELS: Record<number, string> =
    OPERATIONAL_ISSUE_STATUS_LABELS;
  readonly OPERATIONAL_ISSUE_STATUS_PILL: Record<number, string> = OPERATIONAL_ISSUE_STATUS_PILL;
  readonly FINAL_DECISION_LABELS: Record<number, string> = FINAL_DECISION_LABELS;
  readonly FINAL_DECISION_PILL: Record<number, string> = FINAL_DECISION_PILL;
  readonly RESULT_REPORT_STATUS_LABELS: Record<number, string> = RESULT_REPORT_STATUS_LABELS;
  readonly RESULT_REPORT_STATUS_PILL: Record<number, string> = RESULT_REPORT_STATUS_PILL;
  readonly AttendanceStatus = AttendanceStatus;

  readonly tabs = computed(() =>
    this.store.availableTabs().map((tab) => ({
      value: tab,
      labelKey: `INTERVIEW_DASHBOARD.REPORTS.TABS.${tab.toUpperCase()}`,
    })),
  );

  readonly attendanceOptions = [
    { value: AttendanceView.Present, label: ATTENDANCE_STATUS_LABELS[1] },
    { value: AttendanceView.Late, label: ATTENDANCE_STATUS_LABELS[4] },
    { value: AttendanceView.NoShow, label: ATTENDANCE_STATUS_LABELS[2] },
    { value: AttendanceView.Withdrew, label: ATTENDANCE_STATUS_LABELS[3] },
    { value: AttendanceView.NotRecorded, label: 'INTERVIEW_DASHBOARD.LEGEND.NOT_RECORDED' },
  ];

  readonly decisionChips = [
    { value: null, labelKey: 'INTERVIEW_DASHBOARD.REPORTS.ALL' },
    { value: DecisionView.CandidateForHiringProcess, labelKey: FINAL_DECISION_LABELS[1] },
    { value: DecisionView.WaitingList, labelKey: FINAL_DECISION_LABELS[2] },
    { value: DecisionView.Rejected, labelKey: FINAL_DECISION_LABELS[3] },
    { value: DecisionView.NeedsAction, labelKey: FINAL_DECISION_LABELS[4] },
    { value: DecisionView.NoShow, labelKey: FINAL_DECISION_LABELS[5] },
    { value: DecisionView.Pending, labelKey: 'INTERVIEW_DASHBOARD.LEGEND.PENDING_DECISION' },
  ];

  readonly issueStatusOptions = [
    {
      value: OperationalIssueStatus.Open,
      label: OPERATIONAL_ISSUE_STATUS_LABELS[OperationalIssueStatus.Open],
    },
    {
      value: OperationalIssueStatus.Resolved,
      label: OPERATIONAL_ISSUE_STATUS_LABELS[OperationalIssueStatus.Resolved],
    },
    {
      value: OperationalIssueStatus.Waived,
      label: OPERATIONAL_ISSUE_STATUS_LABELS[OperationalIssueStatus.Waived],
    },
  ];

  private readonly canOpenSchedules = computed(() =>
    this.auth.hasPermission([
      Permissions.InterviewSchedule.View,
      Permissions.InterviewSchedule.Manage,
    ]),
  );
  private readonly canOpenSessions = computed(() =>
    this.auth.hasPermission([
      Permissions.InterviewEvaluation.View,
      Permissions.InterviewEvaluation.Manage,
    ]),
  );
  readonly canOpenResults = computed(() =>
    this.auth.hasPermission([
      Permissions.InterviewResultReport.View,
      Permissions.InterviewResultReport.Manage,
    ]),
  );
  readonly canOpenExecution = computed(() => this.canOpenSchedules() || this.canOpenSessions());

  onTabChange(value: string | number | undefined): void {
    if (typeof value === 'string') this.facade.setTab(value as ReportTab);
  }

  // Deep links reuse each module's own URL contract (?schedule / ?appointment) and its permission.
  openSchedule(scheduleId: string): void {
    if (this.canOpenSchedules())
      this.router.navigate([routes.portal.interviews], { queryParams: { schedule: scheduleId } });
    else if (this.canOpenSessions())
      this.router.navigate([routes.portal.startInterview], {
        queryParams: { schedule: scheduleId },
      });
  }

  openAppointment(scheduleId: string, appointmentId: string): void {
    if (this.canOpenSessions())
      this.router.navigate([routes.portal.startInterview], {
        queryParams: { schedule: scheduleId, appointment: appointmentId },
      });
    else this.openSchedule(scheduleId);
  }

  openReport(scheduleId: string): void {
    if (this.canOpenResults())
      this.router.navigate([routes.portal.approveInterview], {
        queryParams: { schedule: scheduleId },
      });
  }

  name(ar: string | null | undefined, en: string | null | undefined): string {
    return this.store.localized(ar, en) || '—';
  }
}
