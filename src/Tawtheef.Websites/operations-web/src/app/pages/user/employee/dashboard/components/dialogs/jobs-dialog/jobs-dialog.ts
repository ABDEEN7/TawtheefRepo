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
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { TableModule } from 'primeng/table';
import { finalize } from 'rxjs';
import { I18nNamespaceDirective } from '../../../../../../../shared/directives/i18n-namespace.directive';
import { FileUtilsService } from '../../../../../../../core/utils/file-utils';
import { JobBreakdown, JobKpis, LatestJob } from '../../../models/dashboard-jobs.model';
import { OperationsDashboardFilters } from '../../../models/dashboard-filters.model';
import {
  DashboardChartExportItem,
  DashboardChartExportService,
} from '../../../services/dashboard-chart-export.service';
import { OperationsDashboardService } from '../../../services/operations-dashboard.service';
import { InvitationStatus } from '../../../../../../../core/enums/lookups.enum';
import { PaginatedResult } from '../../../../../../../core/models/paginated-result.model';
import { PaginationComponent } from '../../../../../../../shared/components/pagination/pagination.component';
import {
  invitationStatusColor,
  jobStatusColor,
} from '../../../constants/dashboard-chart-colors';
interface JobsDialogData {
  kpis: JobKpis;
  breakdown: JobBreakdown;
  filters: OperationsDashboardFilters;
  canExport: boolean;
}
@Component({
  selector: 'app-dashboard-jobs-dialog',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    TranslatePipe,
    I18nNamespaceDirective,
    PaginationComponent,
  ],
  templateUrl: './jobs-dialog.html',
  styleUrl: './jobs-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class JobsDialog implements OnInit {
  private readonly config = inject(DynamicDialogConfig<JobsDialogData>);
  readonly kpis = computed<JobKpis>(() => this.config.data.kpis);
  readonly breakdown = computed<JobBreakdown>(() => this.config.data.breakdown);
  readonly filters = computed<OperationsDashboardFilters>(() => this.config.data.filters);
  readonly canExport = computed<boolean>(() => this.config.data.canExport);
  private readonly api = inject(OperationsDashboardService);
  private readonly chartExport = inject(DashboardChartExportService);
  private readonly files = inject(FileUtilsService);
  private readonly translate = inject(TranslateService);
  readonly latestJobs = signal<PaginatedResult<LatestJob> | null>(null);
  readonly tableFilters = signal<OperationsDashboardFilters>({
    pageNumber: 1,
    pageSize: 10,
  });
  readonly totalItems = computed(() => this.latestJobs()?.metadata?.totalCount ?? 0);

  readonly pageNumber = computed(() => this.tableFilters().pageNumber ?? 1);

  readonly pageSize = computed(() => this.tableFilters().pageSize ?? 10);
  readonly exportInProgress = signal(false);

  private readonly invitationStatuses = Object.values(InvitationStatus) as InvitationStatus[];

  readonly items = computed(() =>
    this.breakdown().byStatus.map((item) => ({
      jobStatusId: item.jobStatusId,
      label: item.label,
      count: item.count,
      color: jobStatusColor(item.status),
    })),
  );

  readonly rows = computed(() =>
    (this.latestJobs()?.items ?? []).map((job) => {
      const workflowCounts = new Map(
        job.invitationWorkflow.map((item) => [item.status, item.count]),
      );

      return {
        ...job,
        workflow: this.invitationStatuses.map((status) => ({
          status,
          labelKey: `dashboard.status.${status}`,
          value: workflowCounts.get(status) ?? 0,
          color: invitationStatusColor(status),
        })),
      };
    }),
  );
  percent(count: number): number {
    return this.kpis().totalJobs > 0 ? Math.round(count / this.kpis().totalJobs * 1000) / 10 : 0;
  }

  ngOnInit(): void {
    this.loadLatestJobs();
  }

  private loadLatestJobs(): void {
    this.api
      .getLatestJobs({
        ...this.filters(),
        ...this.tableFilters(),
      })
      .subscribe((result) => {
        this.latestJobs.set(result);

        if (result.metadata) {
          this.tableFilters.update((filters) => ({
            ...filters,
            pageNumber: result.metadata.currentPage,
            pageSize: result.metadata.pageSize,
          }));
        }
      });
  }
  exportList(): void {
    this.downloadList('Jobs', 'Jobs.xlsx');
  }
  async exportCharts(): Promise<void> {
    if (!this.canExport() || this.exportInProgress()) return;
    this.exportInProgress.set(true);
    try {
      await this.chartExport.download([
        {
          filename: this.translate.instant('dashboard.export.files.jobStatus'),
          title: this.translate.instant('dashboard.modals.jobs.statusTitle'),
          totalLabel: this.translate.instant('dashboard.common.total'),
          total: this.kpis().totalJobs,
          items: this.items(),
          direction: this.translate.currentLang === 'ar' ? 'rtl' : 'ltr',
          locale: 'en-US',
        },
      ]);
    } finally {
      this.exportInProgress.set(false);
    }
  }

  private downloadList(context: 'Jobs', fallback: string): void {
    if (!this.canExport() || this.exportInProgress()) return;
    this.exportInProgress.set(true);
    this.api
      .exportList(context, this.filters())
      .pipe(finalize(() => this.exportInProgress.set(false)))
      .subscribe((response) => this.downloadResponse(response, fallback));
  }
  private downloadResponse(response: HttpResponse<Blob>, fallback: string): void {
    const disposition = response.headers.get('content-disposition') ?? '';
    const encoded = /filename\*=UTF-8''([^;]+)/i.exec(disposition)?.[1];
    const plain = /filename="?([^";]+)"?/i.exec(disposition)?.[1];
    this.files.downloadBlob(
      response.body ?? new Blob(),
      encoded ? decodeURIComponent(encoded) : (plain ?? fallback),
    );
  }

  onPageChange(pageNumber: number): void {
    if (pageNumber === this.pageNumber()) return;

    this.tableFilters.update((filters) => ({
      ...filters,
      pageNumber,
    }));

    this.loadLatestJobs();
  }

  onPageSizeChange(pageSize: number): void {
    this.tableFilters.update((filters) => ({
      ...filters,
      pageNumber: 1,
      pageSize,
    }));

    this.loadLatestJobs();
  }
}
