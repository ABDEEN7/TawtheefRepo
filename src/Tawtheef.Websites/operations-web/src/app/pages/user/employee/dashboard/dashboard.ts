import { CommonModule, DecimalPipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  computed,
  inject,
  signal, OnInit,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { debounceTime, finalize, Subject } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import 'chart.js/auto';
import { ChartData, ChartOptions } from 'chart.js';
import { ChartModule } from 'primeng/chart';
import { Tooltip } from 'primeng/tooltip';
import { DatePicker } from 'primeng/datepicker';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { AuthService } from '../../../../core/auth/auth.service';
import { Permissions } from '../../../../core/constants/permissions';
import { OperationsDashboardService } from './services/operations-dashboard.service';
import {
  DashboardKpis,
  JobBreakdown,
  JobKpis,
  OperationsDashboardFilters,
  OperationsDashboardResponse,
  TeamPerformanceRow,
} from './models/operations-dashboard.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, DecimalPipe, FormsModule, I18nNamespaceDirective, ChartModule, Tooltip, TranslatePipe, DatePicker],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Dashboard implements OnInit {
  private dashboardService = inject(OperationsDashboardService);
  private destroyRef = inject(DestroyRef);
  private translate = inject(TranslateService);
  private authService = inject(AuthService);

  readonly loading = signal(false);
  readonly dashboard = signal<OperationsDashboardResponse | null>(null);

  readonly fromDate = signal<Date | null>(null);
  readonly toDate = signal<Date | null>(null);

  readonly filters = signal<OperationsDashboardFilters>({
    pageNumber: 1,
    pageSize: 10,
    fromDateUtc: this.fromDate()?.toISOString(),
    toDateUtc: this.toDate()?.toISOString()
  });
  readonly searchStatus = signal<string>('');

  private filterChanges$ = new Subject<void>();

  readonly lineChartOptions: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: { legend: { display: false } },
    scales: {
      x: { grid: { display: false } },
      y: { beginAtZero: true, ticks: { precision: 0 } },
    },
  };

  readonly barChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: { legend: { display: false } },
    scales: {
      x: { grid: { display: false } },
      y: { beginAtZero: true, ticks: { precision: 0 } },
    },
  };

  readonly doughnutChartOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'bottom' },
    },
  };

  readonly isHrDashboard = computed(() => this.dashboard()?.role === 'HrManager');
  readonly isDepartmentManager = computed(() => this.dashboard()?.role === 'DepartmentManager');

  readonly hasMinisterOfficePermission = computed(() =>
    this.authService.hasPermission(Permissions.MinisterOffice.View)
  );

  readonly canViewJobStats = computed(() =>
    this.authService.hasPermission(Permissions.Jobs.View)
  );

  readonly canViewProfileStats = computed(() =>
    this.authService.hasPermission(Permissions.ProfileDistribution.View) ||
    this.authService.hasPermission(Permissions.ProfileApproval.Review)
  );

  readonly canViewTeamPerformance = computed(() =>
    this.authService.hasPermission(Permissions.ProfileDistribution.Manage)
  );

  readonly activeKpis = computed<DashboardKpis>(() =>
    this.dashboard()?.kpis ?? {
      totalEmployees: 0,
      activeEmployees: 0,
      totalProfiles: 0,
      newProfilesToday: 0,
      newProfilesThisWeek: 0,
      newProfilesThisMonth: 0,
      approvedProfiles: 0,
      rejectedProfiles: 0,
      pendingProfiles: 0,
      returnedProfiles: 0,
      approvalRate: 0,
      rejectionRate: 0,
      averageApprovalHours: 0,
      totalAssignedTasks: 0,
      remainingTasks: 0,
      overdueTasks: 0,
      followedMinisterOfficeCandidates: 0,
    }
  );

  readonly activeJobKpis = computed<JobKpis>(() =>
    this.dashboard()?.jobKpis ?? {
      totalJobs: 0,
      activeJobs: 0,
      pendingReviewJobs: 0,
      approvedJobs: 0,
      rejectedJobs: 0,
      newJobsToday: 0,
    }
  );

  readonly tableRows = computed<TeamPerformanceRow[]>(() => this.dashboard()?.teamPerformance.items ?? []);

  readonly profileTrendChartData = computed<ChartData<'line'>>(() => ({
    labels: this.dashboard()?.profileTrend.points.map((point) => this.translate.instant(point.label)) ?? [],
    datasets: [
      {
        label: this.translate.instant('common.chart.profiles'),
        data: this.dashboard()?.profileTrend.points.map((point) => point.value) ?? [],
        borderColor: '#2f65d6',
        backgroundColor: 'rgba(47,101,214,0.1)',
        borderWidth: 3,
        fill: true,
        tension: 0.4,
        pointRadius: 4,
        pointBackgroundColor: '#2f65d6',
      },
    ],
  }));

  readonly taskTrendChartData = computed<ChartData<'line'>>(() => ({
    labels: this.dashboard()?.taskCompletionTrend.points.map((point) => this.translate.instant(point.label)) ?? [],
    datasets: [
      {
        label: this.translate.instant('common.chart.tasks'),
        data: this.dashboard()?.taskCompletionTrend.points.map((point) => point.value) ?? [],
        borderColor: '#2b9d76',
        backgroundColor: 'rgba(43,157,118,0.1)',
        borderWidth: 3,
        fill: true,
        tension: 0.4,
        pointRadius: 4,
        pointBackgroundColor: '#2b9d76',
      },
    ],
  }));

  readonly employeeComparisonChartData = computed<ChartData<'bar'>>(() => ({
    labels: this.tableRows().slice(0, 8).map((row) => this.translate.instant(row.name)),
    datasets: [
      {
        label: this.translate.instant('common.chart.completedTasks'),
        data: this.tableRows().slice(0, 8).map((row) => row.completedTasks),
        backgroundColor: '#2b9d76',
        borderRadius: 6,
      },
      {
        label: this.translate.instant('common.chart.remainingTasks'),
        data: this.tableRows().slice(0, 8).map((row) => row.remainingTasks),
        backgroundColor: '#df6d4e',
        borderRadius: 6,
      },
    ],
  }));

  readonly profileStatusChartData = computed<ChartData<'doughnut'>>(() => ({
    labels: this.dashboard()?.profileBreakdown.byStatus.map((item) => this.translate.instant('dashboard.status.' + item.status)) ?? [],
    datasets: [
      {
        data: this.dashboard()?.profileBreakdown.byStatus.map((item) => item.count) ?? [],
        backgroundColor: ['#2f65d6', '#2b9d76', '#f5b342', '#df6d4e', '#9a6bff'],
      },
    ],
  }));

  readonly jobStatusChartData = computed<ChartData<'doughnut'>>(() => ({
    labels: this.dashboard()?.jobBreakdown.byStatus.map((item) => this.translate.instant('dashboard.status.' + item.status)) ?? [],
    datasets: [
      {
        data: this.dashboard()?.jobBreakdown.byStatus.map((item) => item.count) ?? [],
        backgroundColor: ['#4e80ea', '#2b9d76', '#f5b342', '#df6d4e', '#9a6bff'],
      },
    ],
  }));

  readonly jobDepartmentChartData = computed<ChartData<'bar'>>(() => ({
    labels: this.dashboard()?.jobBreakdown.byDepartment.map((item) => item.label) ?? [],
    datasets: [
      {
        label: this.translate.instant('dashboard.jobs.title'),
        data: this.dashboard()?.jobBreakdown.byDepartment.map((item) => item.count) ?? [],
        backgroundColor: '#4e80ea',
        borderRadius: 6,
      },
    ],
  }));

  readonly taskStatusChartData = computed<ChartData<'bar'>>(() => ({
    labels: this.dashboard()?.taskMonitoring.taskStatusStacked.map((item) => this.translate.instant(item.label)) ?? [],
    datasets: [
      {
        label: this.translate.instant('common.chart.tasks'),
        data: this.dashboard()?.taskMonitoring.taskStatusStacked.map((item) => item.count) ?? [],
        backgroundColor: ['#2b9d76', '#4e80ea', '#df6d4e'],
        borderRadius: 6,
      },
    ],
  }));

  readonly smartInsights = computed(() => {
    const kpis = this.activeKpis();
    const jobKpis = this.activeJobKpis();
    const insights: { level: 'warning' | 'info' | 'success'; key: string }[] = [];

    const canReviewProfiles = this.authService.hasPermission(Permissions.ProfileApproval.Review);
    const canManageDistribution = this.authService.hasPermission(Permissions.ProfileDistribution.Manage);
    const canViewMinisterOffice = this.authService.hasPermission(Permissions.MinisterOffice.View);
    const canApproveJobs = this.authService.hasPermission(Permissions.Jobs.Approve);
    const canManageJobs = this.authService.hasPermission(Permissions.Jobs.Manage);

    if (canManageDistribution && kpis.overdueTasks > 0) {
      insights.push({ level: 'warning', key: 'dashboard.insights.overdue' });
    }

    if (canReviewProfiles) {
      if (kpis.rejectionRate > 20) {
        insights.push({ level: 'warning', key: 'dashboard.insights.highRejectionRate' });
      }
      if (kpis.approvalRate >= 75) {
        insights.push({ level: 'success', key: 'dashboard.insights.approvalGood' });
      }

      if (canManageDistribution && kpis.pendingProfiles > kpis.approvedProfiles) {
        insights.push({ level: 'info', key: 'dashboard.insights.pipelinePressure' });
      }
    }

    if (canViewMinisterOffice && kpis.followedMinisterOfficeCandidates > 0) {
      insights.push({ level: 'info', key: 'dashboard.insights.ministerOfficeCandidates' });
    }

    if (canApproveJobs && jobKpis.pendingReviewJobs > 0) {
      insights.push({ level: 'warning', key: 'dashboard.insights.jobsPendingApproval' });
    }

    if (canManageJobs && jobKpis.newJobsToday > 0) {
      insights.push({ level: 'info', key: 'dashboard.insights.jobsNewToday' });
    }

    if (insights.length === 0) {
      if (canManageDistribution) {
        insights.push({ level: 'info', key: 'dashboard.insights.healthy' });
      } else if (kpis.remainingTasks > 0 && kpis.overdueTasks === 0) {
        insights.push({ level: 'success', key: 'dashboard.insights.tasksOnTrack' });
      }
    }

    return insights;
  });

  ngOnInit(): void {
    this.filterChanges$
      .pipe(debounceTime(350), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.loadDashboard());

    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading.set(true);
    this.dashboardService
      .getDashboard(this.filters())
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (response) => this.dashboard.set(response),
      });
  }

  applyFilters(): void {
    this.filters.update((value) => ({
      ...value,
      status: this.searchStatus() || undefined,
      pageNumber: 1,
    }));
    this.filterChanges$.next();
  }

  onStatusChange(value: string): void {
    this.searchStatus.set(value);
    this.applyFilters();
  }

  onDateChange(): void {
    const from = this.fromDate();
    const to = this.toDate();

    this.filters.update(v => ({
      ...v,
      fromDateUtc: from ? from.toISOString() : undefined,
      toDateUtc: to ? to.toISOString() : undefined,
      pageNumber: 1
    }));
    this.filterChanges$.next();
  }

}
