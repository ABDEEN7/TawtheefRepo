import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, OnInit } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ChartData, ChartOptions } from 'chart.js';
import 'chart.js/auto';
import { ChartModule } from 'primeng/chart';
import { TooltipModule } from 'primeng/tooltip';
import { map, merge } from 'rxjs';

import { AuthService } from '../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../core/constants/permissions';
import { FontSizeService } from '../../../../../core/services/font-size.service';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { routes } from '../../../../../routes/routes';
import { DashboardChartColors } from '../../dashboard/constants/dashboard-chart-colors';
import {
  INTERVIEW_TYPE_LABELS,
  InterviewType,
  SCHEDULE_STATUS_LABELS,
  ScheduleStatus,
} from '../interview-schedule/models/enums';
import {
  ATTENDANCE_STATUS_LABELS,
  AttendanceStatus,
  OPERATIONAL_ISSUE_TYPE_LABELS,
  OperationalIssueStatus,
} from '../interview-evaluation/models/enums';
import {
  FINAL_DECISION_LABELS,
  FinalDecision,
  RESULT_REPORT_STATUS_LABELS,
  ResultReportStatus,
} from '../interview-result-report/models/enums';
import {
  ATTENDANCE_COLOR,
  FINAL_DECISION_COLOR,
  INTERVIEW_TYPE_COLOR,
  InterviewDashboardColors,
  SCHEDULE_STATUS_COLOR,
} from './constants/interview-dashboard-colors';
import { InterviewDashboardFacade } from './interview-dashboard.facade';
import { InterviewDashboardStore } from './interview-dashboard.store';
import { AttendanceView, DecisionView, ReportTab } from './models/interview-dashboard.model';
import { DashboardFiltersComponent } from './components/dashboard-filters/dashboard-filters';
import { DashboardReportsComponent } from './components/dashboard-reports/dashboard-reports';

interface KpiCard {
  key: string;
  labelKey: string;
  helpKey: string;
  value: number;
  icon: string;
  color: 'blue' | 'green' | 'red' | 'orange' | 'purple';
  noteKey?: string;
  noteParams?: Record<string, unknown>;
  drill?: () => void;
}

interface LegendItem {
  label: string;
  count: number;
  color: string;
}

interface ModuleLink {
  key: string;
  labelKey: string;
  icon: string;
  route: string;
  permission: string[];
}

@Component({
  selector: 'app-interview-dashboard',
  templateUrl: './interview-dashboard.page.html',
  styleUrl: './interview-dashboard.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    I18nNamespaceDirective,
    TranslatePipe,
    ChartModule,
    TooltipModule,
    DashboardFiltersComponent,
    DashboardReportsComponent,
  ],
  providers: [InterviewDashboardStore, InterviewDashboardFacade],
})
export class InterviewDashboardPage implements OnInit {
  readonly store = inject(InterviewDashboardStore);
  readonly facade = inject(InterviewDashboardFacade);
  private readonly translate = inject(TranslateService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fontSize = inject(FontSizeService);

  // The page's namespaces load after first render; anything built with instant() must re-run then.
  private readonly translationTick = toSignal(
    merge(this.translate.onLangChange, this.translate.onTranslationChange).pipe(map(() => Date.now())),
    { initialValue: 0 },
  );

  readonly fontScale = toSignal(this.fontSize.scale$, { initialValue: 1 as number });
  readonly skeletonCards = [1, 2, 3, 4, 5, 6, 7, 8];
  readonly generatedAt = new Date();

  // Label keys come from the modules that own each status (their namespaces are loaded by the page).
  readonly namespaces = [
    'pages/employee/interview-dashboard',
    'pages/employee/interview-schedule',
    'pages/employee/interview-evaluation',
    'pages/employee/interview-result-report',
  ];

  ngOnInit(): void {
    this.facade.init();
  }

  // ---------- KPIs ----------

  readonly mainKpis = computed<KpiCard[]>(() => {
    const o = this.store.overview();
    const cards: KpiCard[] = [];
    const c = o?.candidates;
    if (c) {
      const share = (v: number) => ({ value: percent(v, c.scheduled) });
      cards.push(
        {
          key: 'scheduled',
          labelKey: 'INTERVIEW_DASHBOARD.KPI.SCHEDULED_CANDIDATES',
          helpKey: 'INTERVIEW_DASHBOARD.HELP.SCHEDULED_CANDIDATES',
          value: c.scheduled,
          icon: 'hgi-user-multiple',
          color: 'blue',
          drill: () => this.drill('candidates'),
        },
        {
          key: 'present',
          labelKey: 'INTERVIEW_DASHBOARD.KPI.PRESENT',
          helpKey: 'INTERVIEW_DASHBOARD.HELP.PRESENT',
          value: c.present,
          icon: 'hgi-checkmark-circle-02',
          color: 'green',
          noteKey: 'INTERVIEW_DASHBOARD.KPI.SHARE_OF_SCHEDULED',
          noteParams: share(c.present),
          drill: () => this.drill('candidates', AttendanceView.Present),
        },
        {
          key: 'noShow',
          labelKey: 'INTERVIEW_DASHBOARD.KPI.NO_SHOW',
          helpKey: 'INTERVIEW_DASHBOARD.HELP.NO_SHOW',
          value: c.noShow,
          icon: 'hgi-user-remove-01',
          color: 'red',
          noteKey: 'INTERVIEW_DASHBOARD.KPI.SHARE_OF_SCHEDULED',
          noteParams: share(c.noShow),
          drill: () => this.drill('candidates', AttendanceView.NoShow),
        },
        {
          key: 'evaluated',
          labelKey: 'INTERVIEW_DASHBOARD.KPI.EVALUATIONS_COMPLETED',
          helpKey: 'INTERVIEW_DASHBOARD.HELP.EVALUATIONS_COMPLETED',
          value: c.evaluationsCompleted,
          icon: 'hgi-task-done-01',
          color: 'purple',
          noteKey: 'INTERVIEW_DASHBOARD.KPI.SHARE_OF_SCHEDULED',
          noteParams: share(c.evaluationsCompleted),
          drill: () => this.drill('candidates'),
        },
      );
    }
    const r = o?.results;
    if (r) {
      const share = (v: number) => ({ value: percent(v, r.candidates) });
      cards.push(
        {
          key: 'hiring',
          labelKey: 'INTERVIEW_DASHBOARD.KPI.HIRING',
          helpKey: 'INTERVIEW_DASHBOARD.HELP.HIRING',
          value: r.candidateForHiringProcess,
          icon: 'hgi-user-check-01',
          color: 'green',
          noteKey: 'INTERVIEW_DASHBOARD.KPI.SHARE_OF_RESULTS',
          noteParams: share(r.candidateForHiringProcess),
          drill: () => this.drill('results', DecisionView.CandidateForHiringProcess),
        },
        {
          key: 'waiting',
          labelKey: 'INTERVIEW_DASHBOARD.KPI.WAITING_LIST',
          helpKey: 'INTERVIEW_DASHBOARD.HELP.WAITING_LIST',
          value: r.waitingList,
          icon: 'hgi-hourglass',
          color: 'orange',
          noteKey: 'INTERVIEW_DASHBOARD.KPI.SHARE_OF_RESULTS',
          noteParams: share(r.waitingList),
          drill: () => this.drill('results', DecisionView.WaitingList),
        },
        {
          key: 'rejected',
          labelKey: 'INTERVIEW_DASHBOARD.KPI.REJECTED',
          helpKey: 'INTERVIEW_DASHBOARD.HELP.REJECTED',
          value: r.rejected,
          icon: 'hgi-cancel-circle',
          color: 'red',
          noteKey: 'INTERVIEW_DASHBOARD.KPI.SHARE_OF_RESULTS',
          noteParams: share(r.rejected),
          drill: () => this.drill('results', DecisionView.Rejected),
        },
      );
    }
    const i = o?.issues;
    if (i) {
      cards.push({
        key: 'issues',
        labelKey: 'INTERVIEW_DASHBOARD.KPI.OPEN_ISSUES',
        helpKey: 'INTERVIEW_DASHBOARD.HELP.OPEN_ISSUES',
        value: i.open,
        icon: 'hgi-alert-02',
        color: 'orange',
        noteKey: 'INTERVIEW_DASHBOARD.KPI.BLOCKING_OF_TOTAL',
        noteParams: { blocking: i.openBlocking, total: i.total },
        drill: () => this.drill('issues', OperationalIssueStatus.Open),
      });
    }
    return cards;
  });

  // Preparation & pipeline counters - smaller cards, grouped by stage.
  readonly pipelineKpis = computed<KpiCard[]>(() => {
    const o = this.store.overview();
    const cards: KpiCard[] = [];
    if (o?.templates) {
      cards.push({
        key: 'templates',
        labelKey: 'INTERVIEW_DASHBOARD.KPI.APPROVED_TEMPLATES',
        helpKey: 'INTERVIEW_DASHBOARD.HELP.APPROVED_TEMPLATES',
        value: o.templates.approved,
        icon: 'hgi-doc-02',
        color: 'blue',
        noteKey: 'INTERVIEW_DASHBOARD.KPI.OF_ACTIVE',
        noteParams: { total: o.templates.active },
      });
    }
    if (o?.committees) {
      cards.push({
        key: 'committees',
        labelKey: 'INTERVIEW_DASHBOARD.KPI.APPROVED_COMMITTEES',
        helpKey: 'INTERVIEW_DASHBOARD.HELP.APPROVED_COMMITTEES',
        value: o.committees.approved,
        icon: 'hgi-user-group',
        color: 'purple',
        noteKey: 'INTERVIEW_DASHBOARD.KPI.OF_TOTAL',
        noteParams: { total: o.committees.total },
      });
    }
    if (o?.schedules) {
      cards.push(
        {
          key: 'schedScheduled',
          labelKey: 'INTERVIEW_DASHBOARD.KPI.SCHEDULED_SCHEDULES',
          helpKey: 'INTERVIEW_DASHBOARD.HELP.SCHEDULED_SCHEDULES',
          value: o.schedules.scheduled,
          icon: 'hgi-calendar-03',
          color: 'blue',
          noteKey: 'INTERVIEW_DASHBOARD.KPI.OF_TOTAL',
          noteParams: { total: o.schedules.total },
          drill: () => this.drill('schedules'),
        },
        {
          key: 'schedInProgress',
          labelKey: 'INTERVIEW_DASHBOARD.KPI.IN_PROGRESS_SCHEDULES',
          helpKey: 'INTERVIEW_DASHBOARD.HELP.IN_PROGRESS_SCHEDULES',
          value: o.schedules.inProgress,
          icon: 'hgi-task-edit-01',
          color: 'orange',
          drill: () => this.drill('schedules'),
        },
        {
          key: 'schedClosed',
          labelKey: 'INTERVIEW_DASHBOARD.KPI.CLOSED_SCHEDULES',
          helpKey: 'INTERVIEW_DASHBOARD.HELP.CLOSED_SCHEDULES',
          value: o.schedules.closed,
          icon: 'hgi-file-validation',
          color: 'green',
          drill: () => this.drill('schedules'),
        },
      );
    }
    if (o?.candidates) {
      cards.push(
        {
          key: 'interviewed',
          labelKey: 'INTERVIEW_DASHBOARD.KPI.INTERVIEWS_COMPLETED',
          helpKey: 'INTERVIEW_DASHBOARD.HELP.INTERVIEWS_COMPLETED',
          value: o.candidates.interviewsCompleted,
          icon: 'hgi-user-question-02',
          color: 'purple',
          drill: () => this.drill('candidates'),
        },
        {
          key: 'rescheduled',
          labelKey: 'INTERVIEW_DASHBOARD.KPI.RESCHEDULED',
          helpKey: 'INTERVIEW_DASHBOARD.HELP.RESCHEDULED',
          value: o.candidates.rescheduled,
          icon: 'hgi-refresh',
          color: 'orange',
        },
      );
    }
    if (o?.results) {
      cards.push({
        key: 'needsAction',
        labelKey: 'INTERVIEW_DASHBOARD.KPI.NEEDS_ACTION',
        helpKey: 'INTERVIEW_DASHBOARD.HELP.NEEDS_ACTION',
        value: o.results.needsAction,
        icon: 'hgi-alert-circle',
        color: 'red',
        noteKey: 'INTERVIEW_DASHBOARD.KPI.PENDING_DECISIONS',
        noteParams: { count: o.results.pendingDecision },
        drill: () => this.drill('results', DecisionView.NeedsAction),
      });
    }
    return cards;
  });

  // ---------- charts ----------

  readonly doughnutOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    maintainAspectRatio: false,
    cutout: '72%',
    animation: { animateRotate: true, animateScale: true, duration: 900, easing: 'easeOutQuart' },
    plugins: { legend: { display: false }, tooltip: { enabled: true } },
  };

  readonly scheduleStatusItems = computed<LegendItem[]>(() => {
    this.translationTick();
    const byStatus = this.store.overview()?.schedules?.byStatus ?? [];
    // Draft and Proposed share one label in the Schedule module - merge them instead of listing "Draft" twice.
    const merged = new Map<string, LegendItem>();
    for (const status of Object.values(ScheduleStatus).filter((v) => typeof v === 'number') as ScheduleStatus[]) {
      const count = byStatus.filter((x) => x.key === status).reduce((s, x) => s + x.count, 0);
      const key = SCHEDULE_STATUS_LABELS[status];
      const existing = merged.get(key);
      if (existing) existing.count += count;
      else merged.set(key, { label: this.translate.instant(key), count, color: SCHEDULE_STATUS_COLOR[status] });
    }
    return [...merged.values()].filter((x) => x.count > 0);
  });

  readonly attendanceItems = computed<LegendItem[]>(() => {
    this.translationTick();
    const c = this.store.overview()?.candidates;
    if (!c) return [];
    return [
      this.item(ATTENDANCE_STATUS_LABELS[AttendanceStatus.Present], c.present, ATTENDANCE_COLOR[AttendanceStatus.Present]),
      this.item(ATTENDANCE_STATUS_LABELS[AttendanceStatus.Late], c.late, ATTENDANCE_COLOR[AttendanceStatus.Late]),
      this.item(ATTENDANCE_STATUS_LABELS[AttendanceStatus.NoShow], c.noShow, ATTENDANCE_COLOR[AttendanceStatus.NoShow]),
      this.item(ATTENDANCE_STATUS_LABELS[AttendanceStatus.Withdrew], c.withdrew, ATTENDANCE_COLOR[AttendanceStatus.Withdrew]),
      this.item('INTERVIEW_DASHBOARD.LEGEND.NOT_RECORDED', c.attendanceNotRecorded, DashboardChartColors.muted),
    ];
  });

  readonly decisionItems = computed<LegendItem[]>(() => {
    this.translationTick();
    const r = this.store.overview()?.results;
    if (!r) return [];
    const d = (decision: FinalDecision, count: number) =>
      this.item(FINAL_DECISION_LABELS[decision], count, FINAL_DECISION_COLOR[decision]);
    return [
      d(FinalDecision.CandidateForHiringProcess, r.candidateForHiringProcess),
      d(FinalDecision.WaitingList, r.waitingList),
      d(FinalDecision.Rejected, r.rejected),
      d(FinalDecision.NeedsAction, r.needsAction),
      d(FinalDecision.NoShow, r.noShow),
      this.item('INTERVIEW_DASHBOARD.LEGEND.PENDING_DECISION', r.pendingDecision, DashboardChartColors.muted),
    ];
  });

  readonly reportStatusItems = computed<LegendItem[]>(() => {
    this.translationTick();
    return (this.store.overview()?.results?.reportsByStatus ?? []).map((x) =>
      this.item(RESULT_REPORT_STATUS_LABELS[x.key as ResultReportStatus], x.count, ''),
    );
  });

  readonly typeItems = computed<LegendItem[]>(() => {
    this.translationTick();
    const byType = this.store.overview()?.candidates?.byInterviewType ?? [];
    return [InterviewType.InPerson, InterviewType.Remote].map((type) =>
      this.item(
        INTERVIEW_TYPE_LABELS[type],
        byType.filter((x) => x.key === type).reduce((s, x) => s + x.count, 0),
        INTERVIEW_TYPE_COLOR[type],
      ),
    );
  });


  readonly scheduleChart = computed(() => this.doughnut(this.scheduleStatusItems()));
  readonly attendanceChart = computed(() => this.doughnut(this.attendanceItems()));
  readonly decisionChart = computed(() => this.doughnut(this.decisionItems()));
  readonly typeChart = computed(() => this.doughnut(this.typeItems()));

  readonly hasActivity = computed(() =>
    (this.store.overview()?.activity?.points ?? []).some((p) => p.scheduled + p.evaluationsCompleted + p.noShow > 0),
  );

  readonly activityChart = computed<ChartData<'line'>>(() => {
    this.translationTick();
    const activity = this.store.overview()?.activity;
    const points = activity?.points ?? [];
    const locale = this.store.isRtl() ? 'ar-u-nu-latn' : 'en-GB';
    const format = new Intl.DateTimeFormat(
      locale,
      activity?.granularity === 'Month' ? { month: 'short', year: 'numeric' } : { day: 'numeric', month: 'short' },
    );
    const series = (key: 'scheduled' | 'evaluationsCompleted' | 'noShow', labelKey: string, color: string, fill = false) => ({
      label: this.translate.instant(labelKey),
      data: points.map((p) => p[key]),
      borderColor: color,
      backgroundColor: fill ? `${color}22` : color,
      pointBackgroundColor: color,
      pointRadius: points.length > 31 ? 0 : 3,
      // Monotone keeps the smoothing from overshooting below zero between days with no interviews.
      cubicInterpolationMode: 'monotone' as const,
      fill,
    });
    return {
      labels: points.map((p) => format.format(new Date(`${p.period}T00:00:00`))),
      datasets: [
        series('scheduled', 'INTERVIEW_DASHBOARD.CHARTS.SERIES.SCHEDULED', DashboardChartColors.info, true),
        series('evaluationsCompleted', 'INTERVIEW_DASHBOARD.CHARTS.SERIES.EVALUATED', DashboardChartColors.success),
        series('noShow', 'INTERVIEW_DASHBOARD.CHARTS.SERIES.NO_SHOW', DashboardChartColors.danger),
      ],
    };
  });

  readonly activityOptions = computed<ChartOptions<'line'>>(() => this.axisOptions<'line'>(false));

  readonly issueChart = computed<ChartData<'bar'>>(() => {
    this.translationTick();
    const byType = this.store.overview()?.issues?.byType ?? [];
    return {
      labels: byType.map((x) => this.translate.instant(OPERATIONAL_ISSUE_TYPE_LABELS[x.type])),
      datasets: [
        {
          label: this.translate.instant('INTERVIEW_DASHBOARD.CHARTS.SERIES.OPEN'),
          data: byType.map((x) => x.open),
          backgroundColor: InterviewDashboardColors.orange,
          borderRadius: 6,
          maxBarThickness: 22,
        },
        {
          label: this.translate.instant('INTERVIEW_DASHBOARD.CHARTS.SERIES.CLOSED'),
          data: byType.map((x) => x.closed),
          backgroundColor: DashboardChartColors.success,
          borderRadius: 6,
          maxBarThickness: 22,
        },
      ],
    };
  });

  readonly issueOptions = computed<ChartOptions<'bar'>>(() => this.axisOptions<'bar'>(true));

  // ---------- module shortcuts (same permissions as the sidebar entries) ----------

  private readonly moduleLinkDefinitions: ModuleLink[] = [
    {
      key: 'templates',
      labelKey: 'internal.sidebar.interview-template',
      icon: 'hgi-doc-02',
      route: routes.portal.interviewTemplates,
      permission: [Permissions.InterviewEvaluationTemplate.View, Permissions.InterviewEvaluationTemplate.Manage],
    },
    {
      key: 'committees',
      labelKey: 'internal.sidebar.interview-committee',
      icon: 'hgi-user-group',
      route: routes.portal.interviewCommittees,
      permission: [Permissions.InterviewCommittee.View, Permissions.InterviewCommittee.Manage],
    },
    {
      key: 'interviews',
      labelKey: 'internal.sidebar.interviews',
      icon: 'hgi-user-question-02',
      route: routes.portal.interviews,
      permission: [Permissions.InterviewSchedule.View, Permissions.InterviewSchedule.Manage],
    },
    {
      key: 'start',
      labelKey: 'internal.sidebar.start-interview',
      icon: 'hgi-task-edit-01',
      route: routes.portal.startInterview,
      permission: [Permissions.InterviewEvaluation.View, Permissions.InterviewEvaluation.Manage],
    },
    {
      key: 'approve',
      labelKey: 'internal.sidebar.approve-interview',
      icon: 'hgi-task-done-01',
      route: routes.portal.approveInterview,
      permission: [Permissions.InterviewResultReport.View, Permissions.InterviewResultReport.Manage],
    },
  ];

  readonly moduleLinks = computed(() => this.moduleLinkDefinitions.filter((l) => this.auth.hasPermission(l.permission)));

  openModule(link: ModuleLink): void {
    this.router.navigate([link.route]);
  }

  // ---------- print header ----------

  readonly filterSummary = computed<{ labelKey: string; value: string }[]>(() => {
    this.translationTick();
    const f = this.store.filters();
    const rows: { labelKey: string; value: string }[] = [];
    if (f.fromDate) rows.push({ labelKey: 'INTERVIEW_DASHBOARD.FILTERS.DATE_RANGE', value: `${f.fromDate} → ${f.toDate ?? f.fromDate}` });
    if (f.jobId) rows.push({ labelKey: 'INTERVIEW_DASHBOARD.FILTERS.JOB', value: this.store.jobOption()?.label ?? '—' });
    if (f.committeeId) rows.push({ labelKey: 'INTERVIEW_DASHBOARD.FILTERS.COMMITTEE', value: this.store.committeeOption()?.label ?? '—' });
    if (f.scheduleId) rows.push({ labelKey: 'INTERVIEW_DASHBOARD.FILTERS.SCHEDULE', value: this.store.scheduleOption()?.label ?? '—' });
    if (f.interviewType) rows.push({ labelKey: 'INTERVIEW_DASHBOARD.FILTERS.INTERVIEW_TYPE', value: this.translate.instant(INTERVIEW_TYPE_LABELS[f.interviewType]) });
    if (f.scheduleStatus) rows.push({ labelKey: 'INTERVIEW_DASHBOARD.FILTERS.SCHEDULE_STATUS', value: this.translate.instant(SCHEDULE_STATUS_LABELS[f.scheduleStatus]) });
    if (f.resultReportStatus) rows.push({ labelKey: 'INTERVIEW_DASHBOARD.FILTERS.REPORT_STATUS', value: this.translate.instant(RESULT_REPORT_STATUS_LABELS[f.resultReportStatus]) });
    if (f.finalDecision) rows.push({ labelKey: 'INTERVIEW_DASHBOARD.FILTERS.FINAL_DECISION', value: this.translate.instant(FINAL_DECISION_LABELS[f.finalDecision]) });
    return rows;
  });

  // ---------- helpers ----------

  drill(tab: ReportTab, quick?: number): void {
    if (tab === 'candidates') this.facade.setAttendanceView((quick as AttendanceView) ?? null);
    if (tab === 'results') this.facade.setDecisionView((quick as DecisionView) ?? null);
    if (tab === 'issues') this.facade.setIssueStatusView((quick as OperationalIssueStatus) ?? null);
    this.facade.setTab(tab);
    document.getElementById('interview-dashboard-reports')?.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

  total(items: LegendItem[]): number {
    return items.reduce((s, x) => s + x.count, 0);
  }

  private item(labelKey: string, count: number, color: string): LegendItem {
    return { label: this.translate.instant(labelKey), count, color };
  }

  private doughnut(items: LegendItem[]): ChartData<'doughnut'> {
    const visible = items.filter((x) => x.count > 0);
    return visible.length === 0
      ? {
          labels: [this.translate.instant('common.chart.noData')],
          datasets: [{ data: [1], backgroundColor: [DashboardChartColors.noData], borderWidth: 0 }],
        }
      : {
          labels: visible.map((x) => x.label),
          datasets: [
            {
              data: visible.map((x) => x.count),
              backgroundColor: visible.map((x) => x.color),
              borderWidth: 0,
              hoverOffset: 8,
            },
          ],
        };
  }

  // RTL: the value axis moves to the right and the category/time axis reads right-to-left.
  private axisOptions<T extends 'line' | 'bar'>(horizontal: boolean): ChartOptions<T> {
    const rtl = this.store.isRtl();
    const font = { family: 'Cairo, "Segoe UI", Tahoma, Arial, sans-serif' };
    const grid = { color: '#EEF0F4' };
    const valueAxis = { beginAtZero: true, ticks: { precision: 0, font }, grid, position: rtl ? 'right' : 'left' };
    const categoryAxis = { reverse: rtl, ticks: { font, autoSkip: true, maxRotation: 0 }, grid: { display: false } };
    return {
      responsive: true,
      maintainAspectRatio: false,
      indexAxis: horizontal ? 'y' : 'x',
      interaction: { mode: 'index', intersect: false },
      plugins: {
        legend: { position: 'bottom', rtl, labels: { usePointStyle: true, boxWidth: 8, font } },
        tooltip: { rtl, textDirection: rtl ? 'rtl' : 'ltr', titleFont: font, bodyFont: font },
      },
      scales: horizontal
        ? {
            x: { ...valueAxis, position: 'bottom', reverse: rtl, stacked: true },
            y: { ...categoryAxis, reverse: false, position: rtl ? 'right' : 'left', stacked: true },
          }
        : { x: categoryAxis, y: valueAxis },
    } as unknown as ChartOptions<T>;
  }
}

function percent(value: number, total: number): number {
  return total > 0 ? Math.round((value / total) * 1000) / 10 : 0;
}
