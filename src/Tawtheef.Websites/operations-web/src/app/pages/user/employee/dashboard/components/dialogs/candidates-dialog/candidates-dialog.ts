import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, ElementRef, ViewChild, computed, inject, input, output, signal } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { finalize } from 'rxjs';
import { ChartData } from 'chart.js';
import { ChartModule } from 'primeng/chart';
import { I18nNamespaceDirective } from '../../../../../../../shared/directives/i18n-namespace.directive';
import { FileUtilsService } from '../../../../../../../core/utils/file-utils';
import { ProfileBreakdown } from '../../../models/dashboard-candidates.model';
import { DashboardKpis } from '../../../models/dashboard-overview.model';
import { OperationsDashboardFilters } from '../../../models/dashboard-filters.model';
import { OperationsDashboardService } from '../../../services/operations-dashboard.service';
import { DashboardChartExportItem, DashboardChartExportService } from '../../../services/dashboard-chart-export.service';

@Component({
  selector: 'app-dashboard-candidates-dialog',
  standalone: true,
  imports: [CommonModule, ChartModule, TranslatePipe, I18nNamespaceDirective],
  templateUrl: './candidates-dialog.html',
  styleUrl: './candidates-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CandidatesDialog {
  @ViewChild('statusChart', { read: ElementRef }) private statusChart?: ElementRef<HTMLElement>;
  @ViewChild('typeChart', { read: ElementRef }) private typeChart?: ElementRef<HTMLElement>;

  readonly breakdown = input.required<ProfileBreakdown>();
  readonly kpis = input.required<DashboardKpis>();
  readonly filters = input.required<OperationsDashboardFilters>();
  readonly canExport = input.required<boolean>();
  readonly closed = output<void>();

  private readonly dashboardService = inject(OperationsDashboardService);
  private readonly chartExport = inject(DashboardChartExportService);
  private readonly fileUtils = inject(FileUtilsService);
  private readonly translate = inject(TranslateService);
  private readonly languageChange = toSignal(this.translate.onLangChange, { initialValue: null });
  readonly exportInProgress = signal(false);
  readonly chartOptions = { responsive: true, maintainAspectRatio: false, cutout: '72%', plugins: { legend: { display: false } } };
  private readonly colors = ['#8A1538', '#488ADA', '#FFB547', '#2F8A3A', '#D9182D', '#94DDBF', '#6C4BB6'];

  readonly statusItems = computed(() => { this.languageChange(); return this.breakdown().byStatus.map((item, index) => ({
    label: this.translate.instant(`dashboard.status.${item.status}`), count: item.count, color: this.colors[index % this.colors.length]
  })); });
  readonly typeItems = computed(() =>
  this.breakdown().byCandidateType.map((item, index) => ({
    label: item.label,
    count: item.count,
    color: this.colors[index % this.colors.length]
  }))
);
  readonly statusChartData = computed<ChartData<'doughnut'>>(() => this.chartData(this.statusItems()));
  readonly typeChartData = computed<ChartData<'doughnut'>>(() => this.chartData(this.typeItems()));
  readonly typeTotal = computed(() => this.typeItems().reduce((total, item) => total + item.count, 0));

  exportList(): void {
    if (!this.canExport() || this.exportInProgress()) return;
    this.exportInProgress.set(true);
    this.dashboardService.exportList('Candidates', this.filters())
      .pipe(finalize(() => this.exportInProgress.set(false)))
      .subscribe(response => this.downloadResponse(response, 'Candidates.xlsx'));
  }

  exportCandidateStatusChart(): Promise<void> {
    return this.exportChart(
      this.statusChart,
      'dashboard.export.files.candidateStatus',
      'dashboard.modals.candidates.statusTitle',
      this.kpis().totalProfiles,
      this.statusItems()
    );
  }

  exportCandidateTypesChart(): Promise<void> {
    return this.exportChart(
      this.typeChart,
      'dashboard.export.files.candidateTypes',
      'dashboard.candidates.typeTitle',
      this.typeTotal(),
      this.typeItems()
    );
  }

  private async exportChart(
    host: ElementRef<HTMLElement> | undefined,
    filenameKey: string,
    titleKey: string,
    total: number,
    items: DashboardChartExportItem[]
  ): Promise<void> {
    if (!this.canExport() || this.exportInProgress()) return;
    this.exportInProgress.set(true);
    try {
      await this.chartExport.download([this.exportSpec(host, filenameKey, titleKey, total, items)]);
    } finally {
      this.exportInProgress.set(false);
    }
  }

  private chartData(items: DashboardChartExportItem[]): ChartData<'doughnut'> {
    return items.length === 0
      ? { labels: [this.translate.instant('common.chart.noData')], datasets: [{ data: [1], backgroundColor: ['#E5E7EB'], borderWidth: 0 }] }
      : { labels: items.map(item => item.label), datasets: [{ data: items.map(item => item.count), backgroundColor: items.map(item => item.color), borderWidth: 0, hoverOffset: 8 }] };
  }

  private exportSpec(host: ElementRef<HTMLElement> | undefined, filenameKey: string, titleKey: string, total: number, items: DashboardChartExportItem[]) {
    return { host: host?.nativeElement, filename: this.translate.instant(filenameKey), title: this.translate.instant(titleKey),
      totalLabel: this.translate.instant('dashboard.common.total'), total, items,
      direction: this.translate.currentLang === 'ar' ? 'rtl' as const : 'ltr' as const };
  }

  private downloadResponse(response: import('@angular/common/http').HttpResponse<Blob>, fallback: string): void {
    const disposition = response.headers.get('content-disposition') ?? '';
    const encoded = /filename\*=UTF-8''([^;]+)/i.exec(disposition)?.[1];
    const plain = /filename="?([^";]+)"?/i.exec(disposition)?.[1];
    this.fileUtils.downloadBlob(response.body ?? new Blob(), encoded ? decodeURIComponent(encoded) : plain ?? fallback);
  }
}
