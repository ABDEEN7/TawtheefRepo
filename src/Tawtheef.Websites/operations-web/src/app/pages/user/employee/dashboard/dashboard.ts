import { CommonModule } from '@angular/common';
import { HttpResponse } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ChartData, ChartOptions } from 'chart.js';
import 'chart.js/auto';
import { ChartModule } from 'primeng/chart';
import { MenuItem } from 'primeng/api';
import { Menu } from 'primeng/menu';
import { DialogService, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { catchError, finalize, map, merge, of } from 'rxjs';
import { AuthService } from '../../../../core/auth/auth.service';
import { Permissions } from '../../../../core/constants/permissions';
import { FontSizeService } from '../../../../core/services/font-size.service';
import { FileUtilsService } from '../../../../core/utils/file-utils';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { routes } from '../../../../routes/routes';
import { CandidatesDialog } from './components/dialogs/candidates-dialog/candidates-dialog';
import { EmployeesDialog } from './components/dialogs/employees-dialog/employees-dialog';
import { InvitationsDialog } from './components/dialogs/invitations-dialog/invitations-dialog';
import { JobsDialog } from './components/dialogs/jobs-dialog/jobs-dialog';
import {
  DashboardKpis,
  DashboardMetricTrend,
  DashboardOverview,
} from './models/dashboard-overview.model';
import { JobKpis } from './models/dashboard-jobs.model';
import { OperationsDashboardFilters } from './models/dashboard-filters.model';
import { MainIndicator, QuickAction } from './models/dashboard-ui.model';
import { employeeAssignmentTranslationKey } from './models/dashboard-employees.model';
import {
  DashboardChartExportItem,
  DashboardChartExportService,
} from './services/dashboard-chart-export.service';
import { OperationsDashboardService } from './services/operations-dashboard.service';
import { dashboardDrilldowns } from './navigation/dashboard-drilldown.factory';
import {
  DashboardChartColors,
  employeeWorkloadColor,
  invitationSummaryColor,
  jobStatusColor,
  profileStatusColor,
} from './constants/dashboard-chart-colors';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, I18nNamespaceDirective, ChartModule, Menu, TranslatePipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [DialogService],
})
export class Dashboard implements OnInit {
  private readonly api = inject(OperationsDashboardService);
  private readonly chartExport = inject(DashboardChartExportService);
  private readonly translate = inject(TranslateService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fontSize = inject(FontSizeService);
  private readonly files = inject(FileUtilsService);
  private readonly dialogs = inject(DialogService);
  private readonly languageChange = toSignal(this.translate.onLangChange, { initialValue: null });
  private readonly exportSummaryLabel = this.translationSignal('dashboard.export.summary');
  private readonly exportChartsLabel = this.translationSignal('dashboard.export.charts');

  readonly fontScale = toSignal(this.fontSize.scale$, { initialValue: 1 as number });
  readonly overview = signal<DashboardOverview | null>(null);
  readonly overviewLoading = signal(false);
  readonly yearsLoading = signal(false);
  readonly availableYears = signal<number[]>([]);
  readonly selectedYear = signal(new Date().getUTCFullYear());
  readonly dashboardFilters = signal<OperationsDashboardFilters>({
    year: this.selectedYear(),
    status: '',
  });
  readonly exportInProgress = signal(false);
  readonly kpiSkeletonItems = [1, 2, 3, 4, 5, 6, 7, 8];
  readonly chartOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    maintainAspectRatio: false,
    locale: 'en-US',
    cutout: '72%',
    animation: {
      animateRotate: true,
      animateScale: true,
      duration: 1000,
      easing: 'easeOutQuart',
    },
    plugins: { legend: { display: false }, tooltip: { enabled: true } },
  };

  readonly canExportDashboard = computed(() =>
    this.auth.hasPermission(Permissions.Dashboard.Export),
  );
  readonly canManageDashboard = computed(() =>
    this.auth.hasPermission(Permissions.Dashboard.Manage),
  );
  readonly canViewCandidatesSummary = computed(
    () =>
      this.canManageDashboard() ||
      this.auth.hasPermission([
        Permissions.CandidateUsers.View,
        Permissions.ProfileDistribution.View,
        Permissions.ProfileDistribution.Manage,
        Permissions.ProfileApproval.View,
        Permissions.ProfileApproval.Review,
        Permissions.ProfileApproval.Changes,
      ]),
  );
  readonly canViewJobsSummary = computed(
    () =>
      this.canManageDashboard() ||
      this.auth.hasPermission([
        Permissions.Jobs.View,
        Permissions.Jobs.Edit,
        Permissions.Jobs.Approve,
        Permissions.Jobs.SendInvitation,
        Permissions.Jobs.Cancel,
        Permissions.Jobs.Create,
        Permissions.Jobs.Publish,
        Permissions.Jobs.Delete,
        Permissions.Jobs.Clone,
      ]),
  );
  readonly canViewEmployeesSummary = computed(
    () =>
      this.canManageDashboard() ||
      this.auth.hasPermission([
        Permissions.ProfileDistribution.View,
        Permissions.ProfileDistribution.Manage,
        Permissions.ProfileApproval.View,
        Permissions.ProfileApproval.Review,
        Permissions.ProfileApproval.Changes,
      ]),
  );
  readonly canViewInvitationsSummary = computed(
    () =>
      this.canManageDashboard() ||
      this.auth.hasPermission([Permissions.JobInvitations.View, Permissions.Jobs.SendInvitation]),
  );
  readonly canViewOperationalSummary = computed(
    () =>
      this.canViewCandidatesSummary() ||
      this.canViewJobsSummary() ||
      this.canViewEmployeesSummary() ||
      this.canViewInvitationsSummary(),
  );
  readonly exportActions = computed<MenuItem[]>(() => {
    return [
      {
        label: this.exportSummaryLabel(),
        icon: 'pi pi-file-excel',
        command: () => this.exportDashboardSummary(),
      },
      {
        label: this.exportChartsLabel(),
        icon: 'pi pi-images',
        command: () => void this.exportDashboardCharts(),
      },
    ];
  });

  private readonly quickActionDefinitions: QuickAction[] = [
    {
      key: 'createJob',
      labelKey: 'dashboard.quickActions.createJob',
      icon: 'hgi-task-edit-01',
      route: routes.portal.jobCreate,
      permission: Permissions.Jobs.Edit,
    },
    {
      key: 'sendInvitation',
      labelKey: 'dashboard.quickActions.sendInvitation',
      icon: 'hgi-sent',
      route: routes.portal.jobInvitationSummary,
      permission: Permissions.JobInvitations.View,
    },
    {
      key: 'approveProfiles',
      labelKey: 'dashboard.quickActions.approveProfiles',
      icon: 'hgi-file-validation',
      route: routes.portal.approvalProfile,
      permission: Permissions.ProfileApproval.View,
    },
    {
      key: 'manageUsers',
      labelKey: 'dashboard.quickActions.manageUsers',
      icon: 'hgi-settings-02',
      route: routes.portal.usersManagement,
      permission: Permissions.Users.Manage,
    },
  ];
  readonly quickActions = computed(() =>
    this.quickActionDefinitions.filter((action) =>
      this.auth.hasPermission(action.permission, action.requireAll ?? false),
    ),
  );

  readonly activeKpis = computed<DashboardKpis>(() => this.overview()?.kpis ?? this.defaultKpis());
  readonly activeJobKpis = computed<JobKpis>(
    () => this.overview()?.jobKpis ?? this.defaultJobKpis(),
  );
  readonly activeInvitationKpis = computed(
    () =>
      this.overview()?.invitationKpis ?? {
        totalInvitations: 0,
        acceptedInvitations: 0,
        pendingInvitations: 0,
        pendingAttachmentApproval: 0,
        expiredInvitations: 0,
        rejectedInvitations: 0,
      },
  );
  readonly mainIndicators = computed<MainIndicator[]>(() => {
    const profiles = this.activeKpis();
    const jobs = this.activeJobKpis();
    const invitations = this.activeInvitationKpis();

    const indicators: MainIndicator[] = [
      {
        labelKey: 'dashboard.kpi.totalProfiles',
        value: profiles.totalProfiles,
        icon: 'hgi-file-01',
        color: 'blue',
        trend: this.overview()?.kpiTrends.totalProfiles,
        navigation: dashboardDrilldowns.totalProfiles(this.selectedYear()),
      },
      {
        labelKey: 'dashboard.kpi.approvedProfiles',
        value: profiles.approvedProfiles,
        icon: 'hgi-checkmark-circle-02',
        color: 'green',
        trend: this.overview()?.kpiTrends.approvedProfiles,
        navigation: dashboardDrilldowns.approvedProfiles(this.selectedYear()),
      },
      {
        labelKey: 'dashboard.status.UnderReview',
        value: profiles.underReviewProfiles,
        icon: 'hgi-clock-01',
        color: 'orange',
        trend: this.overview()?.kpiTrends.underReviewProfiles,
        navigation: dashboardDrilldowns.underReviewProfiles(this.selectedYear()),
      },
      {
        labelKey: 'dashboard.kpi.pendingDistributionProfiles',
        value: profiles.unassignedProfiles,
        icon: 'hgi-mail-01',
        color: 'red',
        trend: this.overview()?.kpiTrends.unassignedProfiles,
        navigation: dashboardDrilldowns.waitingDistribution(this.selectedYear()),
      },
      {
        labelKey: 'dashboard.kpi.publishedJobs',
        value: jobs.publishedJobs,
        icon: 'hgi-megaphone-01',
        color: 'purple',
        trend: this.overview()?.kpiTrends.publishedJobs,
        navigation: dashboardDrilldowns.publishedJobs(this.selectedYear()),
      },
      {
        labelKey: 'dashboard.kpi.totalInvitations',
        value: invitations.totalInvitations,
        icon: 'hgi-user-add-01',
        color: 'purple',
        trend: this.overview()?.kpiTrends.totalInvitations,
        navigation: dashboardDrilldowns.totalInvitations(this.selectedYear()),
      },
    ];

    if (this.auth.hasPermission(Permissions.MinisterOffice.View)) {
      indicators.push({
        labelKey: 'dashboard.kpi.ministerOffice',
        value: profiles.followedMinisterOfficeCandidates ?? 0,
        icon: 'hgi-office',
        color: 'blue',
        navigation: {
          route: routes.portal.ministerOfficeManagement,
          requiredPermission: Permissions.MinisterOffice.View,
          requiresDashboardManage: false,
        },
      });
    }

    if (this.auth.hasPermission(Permissions.Kawader.Manage)) {
      indicators.push({
        labelKey: 'dashboard.kpi.KawaderFiles',
        value: profiles.kawaderFiles ?? 0,
        icon: 'hgi-user-multiple',
        color: 'green',
        navigation: {
          route: routes.portal.kawader,
          requiredPermission: Permissions.Kawader.Manage,
          requiresDashboardManage: false,
        },
      });
    }

    return indicators;
  });
  readonly candidateSummaryItems = computed(() => {
    this.languageChange();

    return (this.overview()?.profileBreakdown.byStatus ?? []).map((item) => ({
      label: this.translate.instant(`dashboard.status.${item.status}`),
      count: item.count,
      color: profileStatusColor(item.status),
    }));
  });

  readonly jobsSummaryItems = computed<DashboardChartExportItem[]>(() =>
    (this.overview()?.jobBreakdown.byStatus ?? []).map((item) => ({
      label: item.label,
      count: item.count,
      color: jobStatusColor(item.status),
    })),
  );

  readonly employeeWorkloadItems = computed<DashboardChartExportItem[]>(() => {
    this.languageChange();

    return (this.overview()?.taskMonitoring.taskStatusStacked ?? []).map((item) => ({
      label: this.translate.instant(employeeAssignmentTranslationKey(item.label)),
      count: item.count,
      color: employeeWorkloadColor(item.label),
    }));
  });
  readonly employeeWorkloadTotal = computed(() =>
    this.employeeWorkloadItems().reduce((total, item) => total + item.count, 0),
  );

  readonly invitationsSummaryItems = computed(() => {
    const invitations = this.activeInvitationKpis();

    return [
      {
        labelKey: 'dashboard.legend.invitations.accepted',
        count: invitations.acceptedInvitations,
        color: invitationSummaryColor('accepted'),
      },
      {
        labelKey: 'dashboard.legend.invitations.pendingResponse',
        count: invitations.pendingInvitations,
        color: invitationSummaryColor('pending'),
      },
      {
        labelKey: 'dashboard.status.PendingAttachmentApproval',
        count: invitations.pendingAttachmentApproval,
        color: invitationSummaryColor('pendingAttachmentApproval'),
      },
      {
        labelKey: 'dashboard.legend.invitations.expired',
        count: invitations.expiredInvitations,
        color: invitationSummaryColor('expired'),
      },
      {
        labelKey: 'dashboard.status.Rejected',
        count: invitations.rejectedInvitations,
        color: invitationSummaryColor('rejected'),
      },
    ];
  });
  readonly visibleCandidateChartData = computed<ChartData<'doughnut'>>(() =>
    this.chartData(this.candidateSummaryItems()),
  );
  readonly visibleJobsChartData = computed<ChartData<'doughnut'>>(() =>
    this.chartData(this.jobsSummaryItems()),
  );

  readonly visibleEmployeesChartData = computed<ChartData<'doughnut'>>(() =>
    this.chartData(this.employeeWorkloadItems()),
  );

  readonly visibleInvitationsChartData = computed<ChartData<'doughnut'>>(() =>
    this.chartData(this.localized(this.invitationsSummaryItems())),
  );

  ngOnInit(): void {
    this.yearsLoading.set(true);
    this.api
      .getYears()
      .pipe(
        catchError(() => of<number[]>([])),
        finalize(() => this.yearsLoading.set(false)),
      )
      .subscribe((years) => {
        const currentYear = new Date().getUTCFullYear();
        this.availableYears.set(years);
        this.selectedYear.set(
          years.includes(currentYear) ? currentYear : (years[0] ?? currentYear),
        );
        this.applyYearFilter();
        this.loadOverview();
      });
  }

  onYearChange(year: number): void {
    if (!Number.isInteger(year) || year === this.selectedYear()) return;
    this.selectedYear.set(year);
    this.applyYearFilter();
    this.loadOverview();
  }
  onYearSelectionChange(event: Event): void {
    if (!(event.target instanceof HTMLSelectElement)) return;
    this.onYearChange(Number(event.target.value));
  }

  trendClass(trend?: DashboardMetricTrend): string {
    if (trend?.changePercentage == null || trend.changePercentage === 0) return '';
    return trend.changePercentage > 0 ? 'trend-up' : 'trend-down';
  }

  private applyYearFilter(): void {
    this.dashboardFilters.update((filters) => ({
      ...filters,
      year: this.selectedYear(),
    }));
  }

  private loadOverview(): void {
    this.overviewLoading.set(true);
    this.api
      .getOverview(this.dashboardFilters())
      .pipe(finalize(() => this.overviewLoading.set(false)))
      .subscribe((result) => this.overview.set(result));
  }
  openCandidatesModal(): void {
    const overview = this.overview();
    if (!overview) return;
    this.dialogs.open(
      CandidatesDialog,
      this.dialogConfig('dashboard.modals.candidates.title', 'min(1100px, 92vw)', {
        breakdown: overview.profileBreakdown,
        kpis: overview.kpis,
        filters: this.dashboardFilters(),
        canExport: this.canExportDashboard(),
      }),
    );
  }
  openJobsModal(): void {
    const overview = this.overview();
    if (!overview) return;
    this.dialogs.open(
      JobsDialog,
      this.dialogConfig('dashboard.modals.jobs.title', 'min(1200px, 94vw)', {
        kpis: overview.jobKpis,
        breakdown: overview.jobBreakdown,
        filters: this.dashboardFilters(),
        canExport: this.canExportDashboard(),
      }),
    );
  }
  openEmployeesModal(): void {
    const overview = this.overview();
    if (!overview) return;
    this.dialogs.open(
      EmployeesDialog,
      this.dialogConfig('dashboard.modals.employees.title', 'min(1250px, 95vw)', {
        kpis: overview.kpis,
        monitoring: overview.taskMonitoring,
        filters: this.dashboardFilters(),
        canExport: this.canExportDashboard(),
      }),
    );
  }
  openInvitationsModal(): void {
    const overview = this.overview();
    if (!overview) return;
    this.dialogs.open(
      InvitationsDialog,
      this.dialogConfig('dashboard.modals.invitations.title', 'min(1150px, 92vw)', {
        kpis: overview.invitationKpis,
        filters: this.dashboardFilters(),
        canExport: this.canExportDashboard(),
      }),
    );
  }
  private dialogConfig<TData extends object>(
    headerKey: string,
    width: string,
    data: TData,
  ): DynamicDialogConfig<TData> {
    return {
      header: this.translate.instant(headerKey),
      width,
      modal: true,
      closable: true,
      closeOnEscape: true,
      dismissableMask: true,
      draggable: false,
      styleClass: 'dashboard-dynamic-dialog',
      contentStyle: { padding: '0', 'overflow-y': 'auto', 'max-height': 'calc(100dvh - 8rem)' },
      breakpoints: { '768px': '96vw' },
      data,
    };
  }
  navigateQuickAction(action: QuickAction): void {
    if (this.auth.hasPermission(action.permission, action.requireAll ?? false))
      this.router.navigate([action.route]);
  }
  isIndicatorNavigable(indicator: MainIndicator): boolean {
    return (
      indicator.navigation != null &&
      (indicator.navigation.requiresDashboardManage === false || this.canManageDashboard()) &&
      this.auth.hasPermission(indicator.navigation.requiredPermission)
    );
  }
  navigateToIndicator(indicator: MainIndicator): void {
    if (!this.isIndicatorNavigable(indicator)) return;
    this.router.navigate([indicator.navigation!.route], {
      queryParams: indicator.navigation!.queryParams,
    });
  }
  exportDashboardSummary(): void {
    if (this.exportInProgress()) return;
    this.exportInProgress.set(true);
    this.api
      .exportList('Summary', this.dashboardFilters())
      .pipe(finalize(() => this.exportInProgress.set(false)))
      .subscribe((response) => this.downloadExport(response));
  }

  async exportDashboardCharts(): Promise<void> {
    if (this.exportInProgress()) return;
    this.exportInProgress.set(true);
    try {
      await this.chartExport.download([
        this.exportSpec(
          'dashboard.export.files.candidates',
          'dashboard.operationalSummary.candidates',
          this.activeKpis().totalProfiles,
          this.candidateSummaryItems(),
        ),
        this.exportSpec(
          'dashboard.export.files.jobs',
          'dashboard.operationalSummary.jobs',
          this.activeJobKpis().totalJobs,
          this.jobsSummaryItems(),
        ),
        this.exportSpec(
          'dashboard.export.files.employees',
          'dashboard.operationalSummary.employees',
          this.employeeWorkloadTotal(),
          this.employeeWorkloadItems(),
        ),
        this.exportSpec(
          'dashboard.export.files.invitations',
          'dashboard.operationalSummary.invitations',
          this.activeInvitationKpis().totalInvitations,
          this.localized(this.invitationsSummaryItems()),
        ),
      ]);
    } finally {
      this.exportInProgress.set(false);
    }
  }

  private chartData(items: DashboardChartExportItem[]): ChartData<'doughnut'> {
    this.languageChange();
    const total = items.reduce((sum, item) => sum + item.count, 0);
    return total === 0
      ? {
          labels: [this.translate.instant('common.chart.noData')],
          datasets: [{ data: [1], backgroundColor: [DashboardChartColors.noData], borderWidth: 0 }],
        }
      : {
          labels: items.map((item) => item.label),
          datasets: [
            {
              data: items.map((item) => item.count),
              backgroundColor: items.map((item) => item.color),
              borderWidth: 0,
              hoverOffset: 8,
            },
          ],
        };
  }
  private localized(
    items: { labelKey: string; count: number; color: string }[],
  ): DashboardChartExportItem[] {
    this.languageChange();
    return items.map((item) => ({ ...item, label: this.translate.instant(item.labelKey) }));
  }
  private translationSignal(key: string) {
    return toSignal(
      merge(this.translate.stream(key), this.translate.getStreamOnTranslationChange(key)).pipe(
        map((value) => (typeof value === 'string' ? value : key)),
      ),
      { initialValue: key },
    );
  }
  private exportSpec(
    filenameKey: string,
    titleKey: string,
    total: number | undefined,
    items: DashboardChartExportItem[],
  ) {
    return {
      filename: this.translate.instant(filenameKey),
      title: this.translate.instant(titleKey),
      totalLabel: this.translate.instant('dashboard.common.total'),
      total,
      items,
      direction: this.translate.currentLang === 'ar' ? ('rtl' as const) : ('ltr' as const),
      locale: 'en-US',
    };
  }
  private downloadExport(response: HttpResponse<Blob>): void {
    const disposition = response.headers.get('content-disposition') ?? '';
    const encoded = /filename\*=UTF-8''([^;]+)/i.exec(disposition)?.[1];
    const plain = /filename="?([^";]+)"?/i.exec(disposition)?.[1];
    void this.files.downloadBlob(
      response.body ?? new Blob(),
      encoded ? decodeURIComponent(encoded) : (plain ?? 'dashboard-summary.xlsx'),
    );
  }
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
      kawaderFiles: 0,
    };
  }
  private defaultJobKpis(): JobKpis {
    return {
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
    };
  }
}
