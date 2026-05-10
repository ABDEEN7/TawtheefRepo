import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  computed,
  inject,
  signal, OnInit,
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { debounceTime, finalize, Subject, distinctUntilChanged } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import 'chart.js/auto';
import { ChartData, ChartOptions } from 'chart.js';
import { ChartModule } from 'primeng/chart';
import { Tooltip } from 'primeng/tooltip';
import { SortEvent } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { AuthService } from '../../../../core/auth/auth.service';
import { Permissions } from '../../../../core/constants/permissions';
import { OperationsDashboardService } from './services/operations-dashboard.service';
import {
  CandidateStatusSummary,
  CandidateTypeSummary,
  DashboardKpis,
  DashboardOverview,
  EmployeeIndicators,
  EmployeeReviewOutcomes,
  JobKpis,
  JobsSummary,
  LatestJob,
  OperationsDashboardFilters,
  OperationsDashboardResponse,
  TeamPerformanceRow,
} from './models/operations-dashboard.model';
import { PaginatedResult } from '../../../../core/models/paginated-result.model';
import { FontSizeService } from '../../../../core/services/font-size.service';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';

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
  private fontSizeService = inject(FontSizeService);

  readonly fontScale = toSignal(this.fontSizeService.scale$, { initialValue: 1 as number });

  private readonly searchSubject = new Subject<string>();

  readonly loading = signal(false);
  readonly dashboard = signal<OperationsDashboardResponse | null>(null);
  readonly overview = signal<DashboardOverview | null>(null);
  readonly candidateStatus = signal<CandidateStatusSummary | null>(null);
  readonly candidateTypes = signal<CandidateTypeSummary | null>(null);
  readonly jobsSummary = signal<JobsSummary | null>(null);
  readonly latestJobs = signal<LatestJob[]>([]);
  readonly employeeIndicators = signal<EmployeeIndicators | null>(null);
  readonly employeeReviewOutcomes = signal<EmployeeReviewOutcomes | null>(null);

  readonly overviewLoading = signal(false);
  readonly candidateStatusLoading = signal(false);
  readonly candidateTypesLoading = signal(false);
  readonly jobsSummaryLoading = signal(false);
  readonly latestJobsLoading = signal(false);
  readonly employeeIndicatorsLoading = signal(false);
  readonly employeeReviewOutcomesLoading = signal(false);

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
  readonly activeTab = signal<'overview' | 'candidates' | 'jobs' | 'employees'>('overview');
  readonly kpiSkeletonItems = [1, 2, 3, 4, 5, 6, 7, 8];
  readonly miniSkeletonItems = [1, 2, 3, 4];
  readonly tableSkeletonRows = [1, 2, 3, 4, 5];

  readonly teamPerformance = signal<PaginatedResult<TeamPerformanceRow> | null>(null);
  readonly tableLoading = signal(false);
  readonly tableSearch = signal('');

  private filterChanges$ = new Subject<void>();

  readonly lineChartOptions = computed<ChartOptions<'line'>>(() => {
    const scale = this.fontScale();
    const fontSize = 12 * scale;

    return {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { display: false }
      },
      scales: {
        x: {
          grid: { display: false },
          ticks: { font: { size: fontSize } }
        },
        y: {
          beginAtZero: true,
          ticks: {
            precision: 0,
            font: { size: fontSize }
          }
        },
      },
    };
  });

  readonly barChartOptions = computed<ChartOptions<'bar'>>(() => {
    const scale = this.fontScale();
    const fontSize = 12 * scale;

    return {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { display: false }
      },
      scales: {
        x: {
          grid: { display: false },
          ticks: { font: { size: fontSize } }
        },
        y: {
          beginAtZero: true,
          ticks: {
            precision: 0,
            font: { size: fontSize }
          }
        },
      },
    };
  });

  readonly doughnutChartOptions = computed<ChartOptions<'doughnut'>>(() => {
    const scale = this.fontScale();
    const fontSize = 11 * scale;

    return {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom',
          labels: {
            font: { size: fontSize },
            padding: Math.round(15 * scale)
          }
        },
      },
    };
  });

  readonly isHrDashboard = computed(() => (this.overview()?.role ?? this.dashboard()?.role) === 'HrManager');
  readonly isDepartmentManager = computed(() => (this.overview()?.role ?? this.dashboard()?.role) === 'DepartmentManager');

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

  private defaultKpis(): DashboardKpis {
    return {
      totalEmployees: 0,
      activeEmployees: 0,
      totalProfiles: 0,
      newProfilesToday: 0,
      newProfilesThisWeek: 0,
      newProfilesThisMonth: 0,
      approvedProfiles: 0,
      rejectedProfiles: 0,
      inCreationProfiles: 0,
      submittedProfiles: 0,
      underReviewProfiles: 0,
      pendingProfiles: 0,
      returnedProfiles: 0,
      approvalRate: 0,
      rejectionRate: 0,
      averageApprovalHours: 0,
      totalAssignedTasks: 0,
      completedTasks: 0,
      remainingTasks: 0,
      overdueTasks: 0,
      unassignedProfiles: 0,
      followedMinisterOfficeCandidates: 0,
    };
  }

  readonly activeKpis = computed<DashboardKpis>(() => {
    const base = this.dashboard()?.kpis ?? this.defaultKpis();
    const overview = this.overview()?.kpis;
    const candidate = this.candidateStatus();
    const indicators = this.employeeIndicators();
    const outcomes = this.employeeReviewOutcomes();

    return {
      ...base,
      ...(overview ?? {}),
      ...(candidate ? {
        totalProfiles: candidate.totalProfiles,
        inCreationProfiles: candidate.inCreationProfiles,
        submittedProfiles: candidate.submittedProfiles,
        underReviewProfiles: candidate.underReviewProfiles,
        approvedProfiles: candidate.approvedProfiles,
        returnedProfiles: candidate.returnedProfiles,
        rejectedProfiles: candidate.rejectedProfiles,
      } : {}),
      ...(indicators ? {
        activeEmployees: indicators.activeEmployees,
        remainingTasks: indicators.remainingTasks,
        unassignedProfiles: indicators.unassignedProfiles,
        completedTasks: indicators.completedTasks,
      } : {}),
      ...(outcomes ? {
        totalProfiles: outcomes.totalProfiles,
        approvedProfiles: outcomes.approvedProfiles,
        returnedProfiles: outcomes.returnedProfiles,
        unassignedProfiles: outcomes.unassignedProfiles,
        pendingProfiles: outcomes.pendingProfiles,
      } : {}),
    };
  });

  readonly activeJobKpis = computed<JobKpis>(() =>
    this.jobsSummary()?.jobKpis ?? this.overview()?.jobKpis ?? this.dashboard()?.jobKpis ?? {
      totalJobs: 0,
      draftJobs: 0,
      activeJobs: 0,
      pendingReviewJobs: 0,
      approvedJobs: 0,
      rejectedJobs: 0,
      newJobsToday: 0,
      pendingPointConfigurationJobs: 0,
      needPointUpdateJobs: 0,
      pendingPointApprovalJobs: 0,
      needUpdateJobs: 0,
      readyForAnnouncementJobs: 0,
      publishedJobs: 0,
      closedJobs: 0,
      cancelledJobs: 0,
    }
  );

  readonly activeInvitationKpis = computed(() =>
    this.jobsSummary()?.invitationKpis ?? this.overview()?.invitationKpis ?? this.dashboard()?.invitationKpis ?? {
      totalInvitations: 0,
      acceptedInvitations: 0,
      pendingInvitations: 0,
      pendingAttachmentApproval: 0,
    }
  );

  readonly jobDonutSegments = computed(() => {
    const jobKpis = this.activeJobKpis();
    const circumference = 439.82;
    const segments = [
      { key: 'draft', statusKey: 'Draft', value: jobKpis.draftJobs, color: '#c9cbd1' },
      { key: 'pending', statusKey: 'PendingApproval', value: jobKpis.pendingReviewJobs, color: '#e3a72f' },
      { key: 'approved', statusKey: 'PendingPointConfiguration', value: jobKpis.pendingPointConfigurationJobs, color: '#2f9e5b' },
      { key: 'returned', statusKey: 'NeedUpdate', value: jobKpis.needUpdateJobs, color: '#e87532' },
      { key: 'ready', statusKey: 'ReadyForAnnouncement', value: jobKpis.readyForAnnouncementJobs, color: '#f05d23' },
      { key: 'published', statusKey: 'Published', value: jobKpis.publishedJobs, color: '#8a1538' },
      { key: 'closed', statusKey: 'Closed', value: jobKpis.closedJobs, color: '#5f6673' },
    ].filter((segment) => segment.value > 0);
    const total = segments.reduce((sum, segment) => sum + segment.value, 0);

    let offset = 0;
    return segments.map((segment) => {
      const length = total > 0 ? (segment.value / total) * circumference : 0;
      const result = {
        ...segment,
        dasharray: `${length} ${circumference - length}`,
        dashoffset: -offset,
        percentage: total > 0 ? Math.round((segment.value / total) * 100) : 0,
      };
      offset += length;
      return result;
    });
  });

  readonly tableRows = computed<TeamPerformanceRow[]>(() => this.teamPerformance()?.items ?? []);
  readonly tableTotalItems = computed(() => this.teamPerformance()?.metadata?.totalCount ?? 0);
  readonly tableCurrentPage = computed(() => this.tableFilters().pageNumber ?? 1);
  readonly tablePageSize = computed(() => this.tableFilters().pageSize ?? 10);
  readonly tableSortField = computed(() => this.tableFilters().sortBy || '');
  readonly tableSortOrder = computed(() => this.tableFilters().sortDirection === 'desc' ? -1 : 1);
  readonly latestJobRows = computed(() => this.latestJobs().length ? this.latestJobs() : this.dashboard()?.latestJobs ?? []);

  readonly profileTrendChartData = computed<ChartData<'line'>>(() => {
    const points = this.dashboard()?.profileTrend.points ?? [];
    if (points.length === 0) {
      return {
        labels: [this.translate.instant('common.chart.noData')],
        datasets: [
          {
            label: this.translate.instant('common.chart.profiles'),
            data: [0],
            borderColor: '#e5e7eb',
            backgroundColor: 'rgba(229,231,235,0.1)',
            borderWidth: 3,
            fill: true,
            tension: 0.4,
            pointRadius: 4,
            pointBackgroundColor: '#e5e7eb',
          },
        ],
      };
    }
    return {
      labels: points.map((point) => this.translate.instant(point.label)),
      datasets: [
        {
          label: this.translate.instant('common.chart.profiles'),
          data: points.map((point) => point.value),
          borderColor: '#8a1538',
          backgroundColor: 'rgba(138,21,56,0.1)',
          borderWidth: 3,
          fill: true,
          tension: 0.4,
          pointRadius: 4,
          pointBackgroundColor: '#8a1538',
        },
      ],
    };
  });

  readonly taskTrendChartData = computed<ChartData<'line'>>(() => {
    const points = this.dashboard()?.taskCompletionTrend.points ?? [];
    if (points.length === 0) {
      return {
        labels: [this.translate.instant('common.chart.noData')],
        datasets: [
          {
            label: this.translate.instant('common.chart.tasks'),
            data: [0],
            borderColor: '#e5e7eb',
            backgroundColor: 'rgba(229,231,235,0.1)',
            borderWidth: 3,
            fill: true,
            tension: 0.4,
            pointRadius: 4,
            pointBackgroundColor: '#e5e7eb',
          },
        ],
      };
    }
    return {
      labels: points.map((point) => this.translate.instant(point.label)),
      datasets: [
        {
          label: this.translate.instant('common.chart.tasks'),
          data: points.map((point) => point.value),
          borderColor: '#2f9e5b',
          backgroundColor: 'rgba(47,158,91,0.1)',
          borderWidth: 3,
          fill: true,
          tension: 0.4,
          pointRadius: 4,
          pointBackgroundColor: '#2f9e5b',
        },
      ],
    };
  });



  readonly profileStatusChartData = computed<ChartData<'doughnut'>>(() => {
    const items = this.candidateStatus()?.profileBreakdown.byStatus ?? this.dashboard()?.profileBreakdown.byStatus ?? [];
    if (items.length === 0) {
      return {
        labels: [this.translate.instant('common.chart.noData')],
        datasets: [
          {
            data: [1],
            backgroundColor: ['#f3f4f6'],
          },
        ],
      };
    }
    return {
      labels: items.map((item) => this.translate.instant('dashboard.status.' + item.status)),
      datasets: [
        {
          data: items.map((item) => item.count),
          backgroundColor: ['#8a1538', '#2f9e5b', '#e3a72f', '#e87532', '#6c4bb6'],
        },
      ],
    };
  });

  readonly jobStatusChartData = computed<ChartData<'doughnut'>>(() => {
    const items = this.jobsSummary()?.jobBreakdown.byStatus ?? this.dashboard()?.jobBreakdown.byStatus ?? [];
    if (items.length === 0) {
      return {
        labels: [this.translate.instant('common.chart.noData')],
        datasets: [
          {
            data: [1],
            backgroundColor: ['#f3f4f6'],
          },
        ],
      };
    }
    return {
      labels: items.map((item) => this.translate.instant('dashboard.status.' + item.status)),
      datasets: [
        {
          data: items.map((item) => item.count),
          backgroundColor: ['#8a1538', '#2f9e5b', '#e3a72f', '#e87532', '#6c4bb6'],
        },
      ],
    };
  });

  readonly jobDepartmentChartData = computed<ChartData<'bar'>>(() => {
    const items = this.jobsSummary()?.jobBreakdown.byDepartment ?? this.dashboard()?.jobBreakdown.byDepartment ?? [];
    if (items.length === 0) {
      return {
        labels: [this.translate.instant('common.chart.noData')],
        datasets: [
          {
            label: this.translate.instant('dashboard.jobs.title'),
            data: [0],
            backgroundColor: '#f3f4f6',
            borderRadius: 6,
          },
        ],
      };
    }
    return {
      labels: items.map((item) => item.label),
      datasets: [
        {
          label: this.translate.instant('dashboard.jobs.title'),
          data: items.map((item) => item.count),
          backgroundColor: '#8a1538',
          borderRadius: 6,
        },
      ],
    };
  });

  readonly taskStatusChartData = computed<ChartData<'bar'>>(() => {
    const items = this.dashboard()?.taskMonitoring.taskStatusStacked ?? [];
    if (items.length === 0) {
      return {
        labels: [this.translate.instant('common.chart.noData')],
        datasets: [
          {
            label: this.translate.instant('common.chart.tasks'),
            data: [0],
            backgroundColor: '#f3f4f6',
            borderRadius: 6,
          },
        ],
      };
    }
    return {
      labels: items.map((item) => this.translate.instant(item.label)),
      datasets: [
        {
          label: this.translate.instant('common.chart.tasks'),
          data: items.map((item) => item.count),
          backgroundColor: ['#2f9e5b', '#8a1538', '#e87532'],
          borderRadius: 6,
        },
      ],
    };
  });

  readonly smartInsights = computed(() => {
    const kpis = this.activeKpis();
    const jobKpis = this.activeJobKpis();
    const insights: { level: 'warning' | 'info' | 'success'; key: string }[] = [];

    const canReviewProfiles = this.authService.hasPermission(Permissions.ProfileApproval.Review);
    const canManageDistribution = this.authService.hasPermission(Permissions.ProfileDistribution.Manage);
    const canViewMinisterOffice = this.authService.hasPermission(Permissions.MinisterOffice.View);
    const canApproveJobs = this.authService.hasPermission(Permissions.Jobs.Approve);
    const canManageJobs = this.authService.hasPermission(Permissions.Jobs.Edit);

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
  }

  loadDashboard(): void {
    this.loadActiveTab();
  }

  private loadActiveTab(): void {
    switch (this.activeTab()) {
      case 'overview':
        this.loadOverview();
        break;
      case 'candidates':
        this.loadCandidateStatus();
        this.loadCandidateTypes();
        break;
      case 'jobs':
        this.loadJobsSummary();
        this.loadLatestJobs();
        break;
      case 'employees':
        this.loadEmployeeIndicators();
        this.loadEmployeeReviewOutcomes();
        this.loadTeamPerformance();
        break;
    }
  }

  private loadOverview(): void {
    this.overviewLoading.set(true);
    this.dashboardService
      .getOverview(this.dashboardFilters())
      .pipe(finalize(() => this.overviewLoading.set(false)))
      .subscribe({ next: (response) => this.overview.set(response) });
  }

  private loadCandidateStatus(): void {
    this.candidateStatusLoading.set(true);
    this.dashboardService
      .getCandidateStatus(this.dashboardFilters())
      .pipe(finalize(() => this.candidateStatusLoading.set(false)))
      .subscribe({ next: (response) => this.candidateStatus.set(response) });
  }

  private loadCandidateTypes(): void {
    this.candidateTypesLoading.set(true);
    this.dashboardService
      .getCandidateTypes(this.dashboardFilters())
      .pipe(finalize(() => this.candidateTypesLoading.set(false)))
      .subscribe({ next: (response) => this.candidateTypes.set(response) });
  }

  private loadJobsSummary(): void {
    this.jobsSummaryLoading.set(true);
    this.dashboardService
      .getJobsSummary(this.dashboardFilters())
      .pipe(finalize(() => this.jobsSummaryLoading.set(false)))
      .subscribe({ next: (response) => this.jobsSummary.set(response) });
  }

  private loadLatestJobs(): void {
    this.latestJobsLoading.set(true);
    this.dashboardService
      .getLatestJobs(this.dashboardFilters())
      .pipe(finalize(() => this.latestJobsLoading.set(false)))
      .subscribe({ next: (response) => this.latestJobs.set(response) });
  }

  private loadEmployeeIndicators(): void {
    this.employeeIndicatorsLoading.set(true);
    this.dashboardService
      .getEmployeeIndicators(this.dashboardFilters())
      .pipe(finalize(() => this.employeeIndicatorsLoading.set(false)))
      .subscribe({ next: (response) => this.employeeIndicators.set(response) });
  }

  private loadEmployeeReviewOutcomes(): void {
    this.employeeReviewOutcomesLoading.set(true);
    this.dashboardService
      .getEmployeeReviewOutcomes(this.dashboardFilters())
      .pipe(finalize(() => this.employeeReviewOutcomesLoading.set(false)))
      .subscribe({ next: (response) => this.employeeReviewOutcomes.set(response) });
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

  onTableSort(event: SortEvent): void {
    if (!event.field || !event.order) return;

    const sortBy = event.field;
    const sortDirection = event.order === 1 ? 'asc' : 'desc';

    const current = this.tableFilters();
    if (current.sortBy === sortBy &&
      current.sortDirection === sortDirection) {
      return;
    }

    this.tableFilters.update((v) => ({
      ...v,
      sortBy: sortBy,
      sortDirection: sortDirection,
      pageNumber: 1,
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
    this.tableSearch.set(value);
    this.searchSubject.next(value);
  }

  setActiveTab(tab: 'overview' | 'candidates' | 'jobs' | 'employees'): void {
    this.activeTab.set(tab);
    this.loadActiveTab();
  }

  statusCount(status: string): number {
    const normalizedStatus = status.toLowerCase();
    return (this.candidateStatus()?.profileBreakdown.byStatus ?? this.dashboard()?.profileBreakdown.byStatus ?? [])
      .find((item) => item.status.toLowerCase() === normalizedStatus)?.count ?? 0;
  }

  jobStatusCount(status: string): number {
    const normalizedStatus = status.toLowerCase();
    return (this.jobsSummary()?.jobBreakdown.byStatus ?? this.dashboard()?.jobBreakdown.byStatus ?? [])
      .find((item) => item.status.toLowerCase() === normalizedStatus)?.count ?? 0;
  }

  candidateTypeCount(key: string): number {
    const kpis = this.candidateTypes()?.candidateTypeKpis ?? this.dashboard()?.candidateTypeKpis;
    if (kpis) {
      const kpiKey = key as keyof typeof kpis;
      const count = kpis[kpiKey];
      if (typeof count === 'number') return count;
    }

    return (this.candidateTypes()?.byCandidateType ?? this.dashboard()?.profileBreakdown.byCandidateType ?? [])
      .find((item) => item.key === key)?.count ?? 0;
  }

  candidateTypeTotal(): number {
    const kpis = this.candidateTypes()?.candidateTypeKpis ?? this.dashboard()?.candidateTypeKpis;
    if (kpis) return kpis.total;

    return (this.candidateTypes()?.byCandidateType ?? this.dashboard()?.profileBreakdown.byCandidateType ?? [])
      .reduce((total, item) => total + item.count, 0);
  }

  progress(value: number, total: number): number {
    if (!total || total <= 0) return 0;
    return Math.min(100, Math.round((value / total) * 100));
  }

  jobDonutTooltip(segment: { statusKey: string; value: number; percentage: number }): string {
    const label = this.translate.instant('dashboard.status.' + segment.statusKey);
    const unit = this.translate.instant('dashboard.jobs.unit');
    return `${label}: ${segment.value.toLocaleString()} ${unit} (${segment.percentage}%)`;
  }

  jobStatusClass(status: string): string {
    const normalized = status.toLowerCase();
    if (normalized.includes('published')) return 'published';
    if (normalized.includes('pending')) return 'pending';
    if (normalized.includes('need') || normalized.includes('rejected')) return 'returned';
    if (normalized.includes('ready') || normalized.includes('active')) return 'approved';
    if (normalized.includes('closed') || normalized.includes('cancelled')) return 'closed';
    return 'draft';
  }

  jobActionKey(status: string): string {
    const normalized = status.toLowerCase();
    if (normalized.includes('pendingapproval')) return 'dashboard.latestJobs.actions.review';
    if (normalized.includes('need')) return 'dashboard.latestJobs.actions.update';
    if (normalized.includes('published')) return 'dashboard.latestJobs.actions.followInvitations';
    return 'dashboard.latestJobs.actions.none';
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
      fromDateUtc: fromIso,
      toDateUtc: toIso,
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
