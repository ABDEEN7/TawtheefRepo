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
import { debounceTime, finalize, Subject, distinctUntilChanged } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import 'chart.js/auto';
import { ChartData, ChartOptions } from 'chart.js';
import { ChartModule } from 'primeng/chart';
import { Tooltip } from 'primeng/tooltip';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { AuthService } from '../../../../core/auth/auth.service';
import { Permissions } from '../../../../core/constants/permissions';
import { OperationsDashboardService } from './services/operations-dashboard.service';
import {
  DashboardKpis,
  JobKpis,
  OperationsDashboardFilters,
  OperationsDashboardResponse,
  TeamPerformanceRow,
} from './models/operations-dashboard.model';
import { PaginatedResult } from '../../../../core/models/paginated-result.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, I18nNamespaceDirective, ChartModule, Tooltip, TranslatePipe, TableModule, InputTextModule, PaginationComponent],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Dashboard implements OnInit {
  private dashboardService = inject(OperationsDashboardService);
  private destroyRef = inject(DestroyRef);
  private translate = inject(TranslateService);
  private authService = inject(AuthService);

  private readonly searchSubject = new Subject<string>();

  readonly loading = signal(false);
  readonly dashboard = signal<OperationsDashboardResponse | null>(null);

  readonly fromDate = signal<Date | null>(new Date(new Date().setDate(new Date().getDate() - 30)));
  readonly toDate = signal<Date | null>(new Date());

  readonly dashboardFilters = signal<OperationsDashboardFilters>({
    status: ''
  });

  readonly tableFilters = signal<OperationsDashboardFilters>({
    pageNumber: 1,
    pageSize: 10,
    search: '',
    sortBy: '',
    sortDirection: 'asc'
  });

  readonly searchStatus = signal<string>('');

  readonly teamPerformance = signal<PaginatedResult<TeamPerformanceRow> | null>(null);
  readonly tableLoading = signal(false);

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

  readonly tableRows = computed<TeamPerformanceRow[]>(() => this.teamPerformance()?.items ?? []);

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
      .subscribe(() => {
        this.loadDashboard();
        this.loadTeamPerformance();
      });

    this.searchSubject
      .pipe(
        debounceTime(400),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe((value) => {
        this.tableFilters.update((v) => ({
          ...v,
          search: value,
          pageNumber: 1,
        }));
        this.loadTeamPerformance();
      });

    this.loadDashboard();
    if (this.canViewTeamPerformance()) {
      this.loadTeamPerformance();
    }
  }

  loadDashboard(): void {
    this.loading.set(true);
    this.dashboardService
      .getDashboard(this.dashboardFilters())
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (response) => {
          this.dashboard.set(response);
        },
      });
  }

  loadTeamPerformance(): void {
    if (!this.canViewTeamPerformance()) return;
    this.tableLoading.set(true);

    // Merge common filters with table filters
    const merged = { ...this.dashboardFilters(), ...this.tableFilters() };

    this.dashboardService
      .getTeamPerformance(merged)
      .pipe(finalize(() => this.tableLoading.set(false)))
      .subscribe({
        next: (response) => {
          this.teamPerformance.set(response);
        },
      });
  }

  onTableLazyLoad(event: TableLazyLoadEvent): void {
    const sortBy = event.sortField as string || '';
    const sortDirection = event.sortOrder === 1 ? 'asc' : 'desc';

    const current = this.tableFilters();
    if (current.sortBy === sortBy &&
      current.sortDirection === sortDirection) {
      return;
    }

    this.tableFilters.update((v) => ({
      ...v,
      sortBy: sortBy,
      sortDirection: sortDirection,
    }));

    this.loadTeamPerformance();
  }

  onPageChange(page: number): void {
    this.tableFilters.update(v => ({ ...v, pageNumber: page }));
    this.loadTeamPerformance();
  }

  onPageSizeChange(size: number): void {
    this.tableFilters.update(v => ({ ...v, pageSize: size, pageNumber: 1 }));
    this.loadTeamPerformance();
  }

  onSearchChange(value: string): void {
    this.searchSubject.next(value);
  }

  applyFilters(): void {
    const nextStatus = this.searchStatus() || undefined;
    if (this.dashboardFilters().status === nextStatus) return;

    this.dashboardFilters.update((value) => ({
      ...value,
      status: nextStatus,
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
    const current = this.dashboardFilters();

    const fromIso = from ? from.toISOString() : undefined;
    const toIso = to ? to.toISOString() : undefined;

    if (current.fromDateUtc === fromIso && current.toDateUtc === toIso) return;

    this.dashboardFilters.update(v => ({
      ...v,
    }));
    this.filterChanges$.next();
  }

  onFromDateChange(value: string | null): void {
    const nextDate = value ? new Date(value) : null;
    this.fromDate.set(nextDate);
    this.onDateChange();
  }

  onToDateChange(value: string | null): void {
    const nextDate = value ? new Date(value) : null;
    this.toDate.set(nextDate);
    this.onDateChange();
  }

}
