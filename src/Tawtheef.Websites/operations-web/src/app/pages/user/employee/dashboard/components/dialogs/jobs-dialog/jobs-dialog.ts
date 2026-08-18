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
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ChartData } from 'chart.js';
import { ChartModule } from 'primeng/chart';
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

@Component({
  selector: 'app-dashboard-jobs-dialog',
  standalone: true,
  imports: [CommonModule, ChartModule, TableModule, TranslatePipe, I18nNamespaceDirective],
  templateUrl: './jobs-dialog.html',
  styleUrl: './jobs-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class JobsDialog implements OnInit {
  @ViewChild('chart', { read: ElementRef }) private chart?: ElementRef<HTMLElement>;
  readonly kpis = input.required<JobKpis>();
  readonly breakdown = input.required<JobBreakdown>();
  readonly filters = input.required<OperationsDashboardFilters>();
  readonly canExport = input.required<boolean>();
  readonly closed = output<void>();
  private readonly api = inject(OperationsDashboardService);
  private readonly chartExport = inject(DashboardChartExportService);
  private readonly files = inject(FileUtilsService);
  private readonly translate = inject(TranslateService);
  readonly latestJobs = signal<LatestJob[]>([]);
  readonly exportInProgress = signal(false);
  readonly chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    cutout: '72%',
    plugins: { legend: { display: false } },
  };

  private readonly colors = [
    '#8A1538',
    '#488ADA',
    '#FFB547',
    '#2F8A3A',
    '#D9182D',
    '#94DDBF',
    '#6C4BB6',
    '#64748B',
    '#9CA3AF',
  ];

  readonly items = computed(() =>
    this.breakdown().byStatus.map((item, index) => ({
      jobStatusId: item.jobStatusId,
      label: item.label,
      count: item.count,
      color: this.colors[index % this.colors.length],
    })),
  );

  readonly chartData = computed<ChartData<'doughnut'>>(() => this.buildChart(this.items()));
  readonly rows = computed(() =>
    this.latestJobs().map((job) => ({
      ...job,
      workflow: [
        {
          labelKey: 'dashboard.workflow.invitations',
          value: job.invitationsSent,
          color: '#488ADA',
        },
        { labelKey: 'dashboard.workflow.applicants', value: job.candidatesCount, color: '#2F8A3A' },
      ],
      actionKey: this.actionKey(job.status),
    })),
  );

  ngOnInit(): void {
    this.api.getLatestJobs(this.filters()).subscribe((rows) => this.latestJobs.set(rows));
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
          host: this.chart?.nativeElement,
          filename: this.translate.instant('dashboard.export.files.jobStatus'),
          title: this.translate.instant('dashboard.modals.jobs.statusTitle'),
          totalLabel: this.translate.instant('dashboard.common.total'),
          total: this.kpis().totalJobs,
          items: this.items(),
          direction: this.translate.currentLang === 'ar' ? 'rtl' : 'ltr',
        },
      ]);
    } finally {
      this.exportInProgress.set(false);
    }
  }

  
  private buildChart(items: DashboardChartExportItem[]): ChartData<'doughnut'> {
    return items.every((item) => item.count === 0)
      ? {
          labels: [this.translate.instant('common.chart.noData')],
          datasets: [{ data: [1], backgroundColor: ['#E5E7EB'], borderWidth: 0 }],
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
  private actionKey(status: string): string {
    const value = status.toLowerCase();
    return value.includes('pendingapproval')
      ? 'dashboard.latestJobs.actions.review'
      : value.includes('need')
        ? 'dashboard.latestJobs.actions.update'
        : value.includes('published')
          ? 'dashboard.latestJobs.actions.followInvitations'
          : 'dashboard.latestJobs.actions.none';
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
}
