import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpResponse } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { ChartData, ChartOptions, TooltipItem } from 'chart.js';
import { SortEvent } from 'primeng/api';
import { ChartModule } from 'primeng/chart';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import {
  Subject,
  catchError,
  debounceTime,
  distinctUntilChanged,
  finalize,
  map,
  of,
  switchMap,
  tap,
} from 'rxjs';

import { PaginatedResult } from '../../../../../../../core/models/paginated-result.model';
import { FileUtilsService } from '../../../../../../../core/utils/file-utils';
import { PaginationComponent } from '../../../../../../../shared/components/pagination/pagination.component';
import { PageFiltersComponent } from '../../../../../../../shared/components/page-filters/page-filters.component';
import { I18nNamespaceDirective } from '../../../../../../../shared/directives/i18n-namespace.directive';

import {
  employeeAssignmentTranslationKey,
  TeamPerformanceRow,
} from '../../../models/dashboard-employees.model';
import { OperationsDashboardFilters } from '../../../models/dashboard-filters.model';
import { DashboardKpis, TaskMonitoring } from '../../../models/dashboard-overview.model';
import {
  DashboardChartExportItem,
  DashboardChartExportService,
} from '../../../services/dashboard-chart-export.service';
import { OperationsDashboardService } from '../../../services/operations-dashboard.service';
import {
  DashboardChartColors,
  employeeWorkloadColor,
} from '../../../constants/dashboard-chart-colors';

interface TeamPerformanceRequest {
  filters: OperationsDashboardFilters;
  key: string;
}
interface EmployeesDialogData {
  kpis: DashboardKpis;
  monitoring: TaskMonitoring;
  filters: OperationsDashboardFilters;
  canExport: boolean;
}
type TeamPerformanceSortValue =
  | 'Name:asc'
  | 'AssignedTasks:desc'
  | 'CompletedTasks:desc'
  | 'RemainingTasks:desc'
  | 'OverdueTasks:desc';

interface TeamPerformanceSortOption {
  value: TeamPerformanceSortValue;
  labelKey: string;
}

@Component({
  selector: 'app-dashboard-employees-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ChartModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    SelectModule,
    TranslatePipe,
    I18nNamespaceDirective,
    PaginationComponent,
    PageFiltersComponent,
  ],
  templateUrl: './employees-dialog.html',
  styleUrl: './employees-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmployeesDialog implements OnInit {
  private readonly config = inject(DynamicDialogConfig<EmployeesDialogData>);
  readonly kpis = computed<DashboardKpis>(() => this.config.data.kpis);
  readonly monitoring = computed<TaskMonitoring>(() => this.config.data.monitoring);
  readonly filters = computed<OperationsDashboardFilters>(() => this.config.data.filters);
  readonly canExport = computed<boolean>(() => this.config.data.canExport);

  private readonly api = inject(OperationsDashboardService);
  private readonly chartExport = inject(DashboardChartExportService);
  private readonly files = inject(FileUtilsService);
  private readonly translate = inject(TranslateService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly requests = new Subject<TeamPerformanceRequest>();
  private readonly searches = new Subject<string>();

  private readonly languageChange = toSignal(this.translate.onLangChange, { initialValue: null });

  readonly teamPerformance = signal<PaginatedResult<TeamPerformanceRow> | null>(null);
  readonly teamPerformanceLoading = signal(false);
  readonly searchValue = signal('');
  readonly selectedSort = signal<TeamPerformanceSortValue>('Name:asc');
  readonly sortOptions: TeamPerformanceSortOption[] = [
    { value: 'Name:asc', labelKey: 'dashboard.modals.table.employee' },
    { value: 'AssignedTasks:desc', labelKey: 'dashboard.table.assignedTasks' },
    { value: 'CompletedTasks:desc', labelKey: 'dashboard.teamPerformance.finalizedReviews' },
    { value: 'RemainingTasks:desc', labelKey: 'dashboard.teamPerformance.remainingWorkload' },
    { value: 'OverdueTasks:desc', labelKey: 'dashboard.teamPerformance.overdueWorkload' },
  ];

  readonly tableFilters = signal<OperationsDashboardFilters>({
    pageNumber: 1,
    pageSize: 10,
    search: '',
    sortBy: '',
    sortDirection: 'asc',
  });

  readonly exportInProgress = signal(false);

  readonly rows = computed(() => this.teamPerformance()?.items ?? []);
  readonly metricMaxima = computed(() => this.rows().reduce(
    (maxima, employee) => ({
      completed: Math.max(maxima.completed, employee.completedTasks),
      remaining: Math.max(maxima.remaining, employee.remainingTasks),
      overdue: Math.max(maxima.overdue, employee.overdueTasks),
    }),
    { completed: 0, remaining: 0, overdue: 0 },
  ));

  readonly totalItems = computed(() => this.teamPerformance()?.metadata?.totalCount ?? 0);

  readonly currentPage = computed(
    () => this.teamPerformance()?.metadata?.currentPage ?? this.tableFilters().pageNumber ?? 1,
  );

  readonly itemsPerPage = computed(
    () => this.teamPerformance()?.metadata?.pageSize ?? this.tableFilters().pageSize ?? 10,
  );

  readonly chartOptions = computed<ChartOptions<'bar'>>(() => {
    this.languageChange();
    const numberFormatter = new Intl.NumberFormat('en-US', { maximumFractionDigits: 0 });
    const compactFormatter = new Intl.NumberFormat('en-US', {
      notation: 'compact',
      maximumFractionDigits: 1,
    });
    return {
      responsive: true,
      maintainAspectRatio: false,
      indexAxis: 'y',
      plugins: {
        legend: { display: false },
        tooltip: { callbacks: { label: (item: TooltipItem<'bar'>) => numberFormatter.format(Number(item.raw)) } },
      },
      layout: { padding: { top: 8, right: 12, bottom: 4, left: 12 } },
      scales: {
        x: {
          beginAtZero: true,
          stacked: false,
          ticks: { padding: 8, font: { size: 13 }, callback: value => compactFormatter.format(Number(value)) },
        },
        y: {
          stacked: false,
          grid: { display: false },
          ticks: { padding: 10, autoSkip: false, font: { size: 14, weight: 600 } },
        },
      },
    };
  });

  readonly items = computed(() =>
    this.monitoring().taskStatusStacked.map((item) => ({
      labelKey: employeeAssignmentTranslationKey(item.label),
      count: item.count,
      color: employeeWorkloadColor(item.label),
    })),
  );

  readonly employeeWorkloadChartItems = computed<DashboardChartExportItem[]>(() => {
    this.languageChange();

    return this.items().map((item) => ({
      label: this.translate.instant(item.labelKey),
      count: item.count,
      color: item.color,
    }));
  });

  readonly chartData = computed<ChartData<'bar'>>(() => {
    const items = this.employeeWorkloadChartItems();

    return items.every((item) => item.count === 0)
      ? {
          labels: [this.translate.instant('common.chart.noData')],
          datasets: [
            {
              data: [0],
              backgroundColor: [DashboardChartColors.noData],
              borderWidth: 0,
            },
          ],
        }
      : {
          labels: items.map((item) => item.label),
          datasets: [
            {
              data: items.map((item) => item.count),
              backgroundColor: items.map((item) => item.color),

              borderWidth: 0,
              borderRadius: 7,
              borderSkipped: false,

              maxBarThickness: 72,
              minBarLength: 7,

              categoryPercentage: 0.55,
              barPercentage: 0.8,
            },
          ],
        };
  });

  ngOnInit(): void {
    this.requests
      .pipe(
        distinctUntilChanged((previous, current) => previous.key === current.key),
        tap(() => this.teamPerformanceLoading.set(true)),
        switchMap((request) =>
          this.api.getTeamPerformance(request.filters).pipe(
            map((result) => ({ result })),
            catchError(() => of({ result: null })),
          ),
        ),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(({ result }) => {
        this.teamPerformanceLoading.set(false);
        if (!result) return;

        this.teamPerformance.set(result);
        if (result.metadata) {
          this.tableFilters.update((filters) => ({
            ...filters,
            pageNumber: result.metadata.currentPage,
            pageSize: result.metadata.pageSize,
          }));
        }
      });

    this.searches
      .pipe(
        debounceTime(400),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((search) => {
        this.tableFilters.update((filters) => ({ ...filters, pageNumber: 1, search }));
        this.requestTeamPerformance();
      });

    this.translate.onLangChange
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(({ lang }) => this.requestTeamPerformance(lang));

    this.requestTeamPerformance();
  }

  onPageChange(pageNumber: number): void {
    if (pageNumber === this.currentPage()) return;

    this.tableFilters.update((filters) => ({
      ...filters,
      pageNumber,
    }));

    this.requestTeamPerformance();
  }

  onPageSizeChange(pageSize: number): void {
    if (pageSize === this.itemsPerPage()) return;

    this.tableFilters.update((filters) => ({
      ...filters,
      pageNumber: 1,
      pageSize,
    }));

    this.requestTeamPerformance();
  }

  onSort(event: SortEvent): void {
    if (!event.field) return;

    const sortDirection = event.order === -1 ? 'desc' : 'asc';
    const current = this.tableFilters();
    if (current.sortBy === event.field && current.sortDirection === sortDirection) return;

    this.tableFilters.update((filters) => ({
      ...filters,
      pageNumber: 1,
      sortBy: event.field,
      sortDirection,
    }));

    this.requestTeamPerformance();
  }

  onSortSelection(value: TeamPerformanceSortValue): void {
    this.selectedSort.set(value);
    const [field, direction = 'asc'] = value.split(':');
    if (!field) return;
    this.onSort({ field, order: direction === 'desc' ? -1 : 1 });
  }

  metricScale(value: number, metric: 'completed' | 'remaining' | 'overdue'): number {
    const maximum = Math.max(this.metricMaxima()[metric], 1);
    return Math.min(100, Math.max(0, (value / maximum) * 100));
  }

  onSearchChange(value: string): void {
    this.searchValue.set(value);
    this.searches.next(value);
  }

  exportList(): void {
    if (!this.canExport() || this.exportInProgress()) return;

    this.exportInProgress.set(true);

    const tableFilters = this.tableFilters();

    this.api
      .exportList('Employees', {
        ...this.filters(),
        search: tableFilters.search,
        sortBy: tableFilters.sortBy,
        sortDirection: tableFilters.sortDirection,
      })
      .pipe(finalize(() => this.exportInProgress.set(false)))
      .subscribe((response) => this.downloadResponse(response));
  }

  async exportCharts(): Promise<void> {
    if (!this.canExport() || this.exportInProgress()) return;

    this.exportInProgress.set(true);

    try {
      await this.chartExport.download([
        {
          filename: this.translate.instant('dashboard.export.files.employeeAssignments'),
          title: this.translate.instant('dashboard.modals.employees.statusTitle'),
          items: this.employeeWorkloadChartItems(),
          direction: this.translate.currentLang === 'ar' ? 'rtl' : 'ltr',
          locale: 'en-US',
        },
      ]);
    } finally {
      this.exportInProgress.set(false);
    }
  }

  private downloadResponse(response: HttpResponse<Blob>): void {
    const disposition = response.headers.get('content-disposition') ?? '';

    const encoded = /filename\*=UTF-8''([^;]+)/i.exec(disposition)?.[1];

    const plain = /filename="?([^";]+)"?/i.exec(disposition)?.[1];

    this.files.downloadBlob(
      response.body ?? new Blob(),
      encoded ? decodeURIComponent(encoded) : (plain ?? 'Employees.xlsx'),
    );
  }

  private requestTeamPerformance(language = this.translate.currentLang): void {
    const filters = {
      ...this.filters(),
      ...this.tableFilters(),
    };

    this.requests.next({
      filters,
      key: JSON.stringify({ ...filters, language }),
    });
  }
}
