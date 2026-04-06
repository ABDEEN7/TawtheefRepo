import { CommonModule } from '@angular/common';
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
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
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
  imports: [CommonModule, FormsModule, I18nNamespaceDirective, ChartModule, Tooltip, TranslatePipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Dashboard implements OnInit {
  private dashboardService = inject(OperationsDashboardService);
  private destroyRef = inject(DestroyRef);
  private translate = inject(TranslateService);

  readonly loading = signal(false);
  readonly dashboard = signal<OperationsDashboardResponse | null>(null);

  readonly filters = signal<OperationsDashboardFilters>({ pageNumber: 1, pageSize: 10 });
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
        backgroundColor: 'rgba(47,101,214,0.2)',
        fill: true,
        tension: 0.35,
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
    labels: this.dashboard()?.profileBreakdown.byStatus.map((item) => this.translate.instant(item.status)) ?? [],
    datasets: [
      {
        data: this.dashboard()?.profileBreakdown.byStatus.map((item) => item.count) ?? [],
        backgroundColor: ['#2f65d6', '#2b9d76', '#f5b342', '#df6d4e', '#9a6bff'],
      },
    ],
  }));

  readonly jobStatusChartData = computed<ChartData<'doughnut'>>(() => ({
    labels: this.dashboard()?.jobBreakdown.byStatus.map((item) => this.translate.instant(item.status)) ?? [],
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
    const insights: { level: 'warning' | 'info' | 'success'; key: string }[] = [];

    if (kpis.overdueTasks > 0) insights.push({ level: 'warning', key: 'dashboard.insights.overdue' });
    if (kpis.approvalRate >= 75) insights.push({ level: 'success', key: 'dashboard.insights.approvalGood' });
    if (kpis.pendingProfiles > kpis.approvedProfiles)
      insights.push({ level: 'info', key: 'dashboard.insights.pipelinePressure' });

    if (insights.length === 0) insights.push({ level: 'info', key: 'dashboard.insights.healthy' });

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

}
