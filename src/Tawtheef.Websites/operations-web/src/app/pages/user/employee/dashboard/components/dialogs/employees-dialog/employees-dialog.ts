import { CommonModule } from '@angular/common';
import { HttpResponse } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
  computed,
  inject,
  input,
  output,
  signal,
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ChartData } from 'chart.js';
import { SortEvent } from 'primeng/api';
import { ChartModule } from 'primeng/chart';
import { TableModule } from 'primeng/table';
import { finalize } from 'rxjs';

import { PaginatedResult } from '../../../../../../../core/models/paginated-result.model';
import { FileUtilsService } from '../../../../../../../core/utils/file-utils';
import { PaginationComponent } from '../../../../../../../shared/components/pagination/pagination.component';
import { I18nNamespaceDirective } from '../../../../../../../shared/directives/i18n-namespace.directive';

import {
  employeeAssignmentTranslationKey,
  TeamPerformanceRow,
} from '../../../models/dashboard-employees.model';
import { OperationsDashboardFilters } from '../../../models/dashboard-filters.model';
import {
  DashboardKpis,
  TaskMonitoring,
} from '../../../models/dashboard-overview.model';
import {
  DashboardChartExportItem,
  DashboardChartExportService,
} from '../../../services/dashboard-chart-export.service';
import { OperationsDashboardService } from '../../../services/operations-dashboard.service';

@Component({
  selector: 'app-dashboard-employees-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ChartModule,
    TableModule,
    TranslatePipe,
    I18nNamespaceDirective,
    PaginationComponent,
  ],
  templateUrl: './employees-dialog.html',
  styleUrl: './employees-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmployeesDialog implements OnInit {
  @ViewChild('chart', { read: ElementRef })
  private chart?: ElementRef<HTMLElement>;

  readonly kpis = input.required<DashboardKpis>();
  readonly monitoring = input.required<TaskMonitoring>();
  readonly filters = input.required<OperationsDashboardFilters>();
  readonly canExport = input.required<boolean>();

  readonly closed = output<void>();

  private readonly api = inject(OperationsDashboardService);
  private readonly chartExport = inject(DashboardChartExportService);
  private readonly files = inject(FileUtilsService);
  private readonly translate = inject(TranslateService);

  private readonly languageChange = toSignal(
    this.translate.onLangChange,
    { initialValue: null },
  );

  readonly teamPerformance =
    signal<PaginatedResult<TeamPerformanceRow> | null>(null);

  readonly tableFilters = signal<OperationsDashboardFilters>({
    pageNumber: 1,
    pageSize: 10,
    search: '',
    sortBy: '',
    sortDirection: 'asc',
  });

  readonly exportInProgress = signal(false);

  readonly rows = computed(
    () => this.teamPerformance()?.items ?? [],
  );

  readonly totalItems = computed(
    () => this.teamPerformance()?.metadata?.totalCount ?? 0,
  );

  readonly currentPage = computed(
    () =>
      this.teamPerformance()?.metadata?.currentPage ??
      this.tableFilters().pageNumber ??
      1,
  );

  readonly itemsPerPage = computed(
    () =>
      this.teamPerformance()?.metadata?.pageSize ??
      this.tableFilters().pageSize ??
      10,
  );

  readonly chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    cutout: '72%',
    plugins: {
      legend: {
        display: false,
      },
    },
  };

  readonly items = computed(() => {
    const colors = [
      '#488ADA',
      '#2F8A3A',
      '#FFB547',
      '#D9182D',
      '#94DDBF',
    ];

    return this.monitoring().taskStatusStacked.map(
      (item, index) => ({
        labelKey: employeeAssignmentTranslationKey(item.label),
        count: item.count,
        color: colors[index % colors.length],
      }),
    );
  });

  readonly employeeChartItems =
    computed<DashboardChartExportItem[]>(() => {
      this.languageChange();

      return [
        {
          label: this.translate.instant(
            'dashboard.kpi.totalEmployees',
          ),
          count: this.kpis().totalEmployees,
          color: '#488ADA',
        },
      ];
    });

  readonly chartData = computed<ChartData<'doughnut'>>(() => {
    const items = this.employeeChartItems();

    return items.every((item) => item.count === 0)
      ? {
          labels: [
            this.translate.instant('common.chart.noData'),
          ],
          datasets: [
            {
              data: [1],
              backgroundColor: ['#E5E7EB'],
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
              hoverOffset: 8,
            },
          ],
        };
  });

  ngOnInit(): void {
    this.loadTeamPerformance();
  }

  loadTeamPerformance(): void {
    this.api
      .getTeamPerformance({
        ...this.filters(),
        ...this.tableFilters(),
      })
      .subscribe((result) => {
        this.teamPerformance.set(result);

        if (result.metadata) {
          this.tableFilters.update((filters) => ({
            ...filters,
            pageNumber: result.metadata.currentPage,
            pageSize: result.metadata.pageSize,
          }));
        }
      });
  }

  onPageChange(pageNumber: number): void {
    if (pageNumber === this.currentPage()) return;

    this.tableFilters.update((filters) => ({
      ...filters,
      pageNumber,
    }));

    this.loadTeamPerformance();
  }

  onPageSizeChange(pageSize: number): void {
    this.tableFilters.update((filters) => ({
      ...filters,
      pageNumber: 1,
      pageSize,
    }));

    this.loadTeamPerformance();
  }

  onSort(event: SortEvent): void {
    if (!event.field) return;

    this.tableFilters.update((filters) => ({
      ...filters,
      pageNumber: 1,
      sortBy: event.field,
      sortDirection: event.order === -1 ? 'desc' : 'asc',
    }));

    this.loadTeamPerformance();
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
      .pipe(
        finalize(() => this.exportInProgress.set(false)),
      )
      .subscribe((response) =>
        this.downloadResponse(response),
      );
  }

  async exportCharts(): Promise<void> {
    if (!this.canExport() || this.exportInProgress()) return;

    this.exportInProgress.set(true);

    try {
      await this.chartExport.download([
        {
          host: this.chart?.nativeElement,
          filename: this.translate.instant(
            'dashboard.export.files.employeeAssignments',
          ),
          title: this.translate.instant(
            'dashboard.modals.employees.statusTitle',
          ),
          totalLabel: this.translate.instant(
            'dashboard.common.total',
          ),
          total: this.kpis().totalEmployees,
          items: this.employeeChartItems(),
          direction:
            this.translate.currentLang === 'ar'
              ? 'rtl'
              : 'ltr',
        },
      ]);
    } finally {
      this.exportInProgress.set(false);
    }
  }

  private downloadResponse(
    response: HttpResponse<Blob>,
  ): void {
    const disposition =
      response.headers.get('content-disposition') ?? '';

    const encoded =
      /filename\*=UTF-8''([^;]+)/i.exec(disposition)?.[1];

    const plain =
      /filename="?([^";]+)"?/i.exec(disposition)?.[1];

    this.files.downloadBlob(
      response.body ?? new Blob(),
      encoded
        ? decodeURIComponent(encoded)
        : (plain ?? 'Employees.xlsx'),
    );
  }
}