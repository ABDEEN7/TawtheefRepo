import { CommonModule } from '@angular/common';
import { HttpResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, ElementRef, OnInit, ViewChild, computed, inject, input, output, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ChartData } from 'chart.js';
import { ChartModule } from 'primeng/chart';
import { TableModule } from 'primeng/table';
import { finalize } from 'rxjs';
import { I18nNamespaceDirective } from '../../../../../../../shared/directives/i18n-namespace.directive';
import { FileUtilsService } from '../../../../../../../core/utils/file-utils';
import { InvitationKpis, LatestInvitation } from '../../../models/dashboard-invitations.model';
import { OperationsDashboardFilters } from '../../../models/dashboard-filters.model';
import { DashboardChartExportItem, DashboardChartExportService } from '../../../services/dashboard-chart-export.service';
import { OperationsDashboardService } from '../../../services/operations-dashboard.service';

@Component({ selector: 'app-dashboard-invitations-dialog', standalone: true,
  imports: [CommonModule, ChartModule, TableModule, TranslatePipe, I18nNamespaceDirective],
  templateUrl: './invitations-dialog.html', styleUrl: './invitations-dialog.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class InvitationsDialog implements OnInit {
  @ViewChild('chart', { read: ElementRef }) private chart?: ElementRef<HTMLElement>;
  readonly kpis = input.required<InvitationKpis>();
  readonly filters = input.required<OperationsDashboardFilters>();
  readonly canExport = input.required<boolean>();
  readonly closed = output<void>();
  private readonly api = inject(OperationsDashboardService);
  private readonly chartExport = inject(DashboardChartExportService);
  private readonly files = inject(FileUtilsService);
  private readonly translate = inject(TranslateService);
  private readonly languageChange = toSignal(this.translate.onLangChange, { initialValue: null });
  readonly latestInvitations = signal<LatestInvitation[]>([]);
  readonly exportInProgress = signal(false);
  readonly chartOptions = { responsive: true, maintainAspectRatio: false, cutout: '72%', plugins: { legend: { display: false } } };
  readonly items = computed(() => {
    const invitations = this.kpis();
    return [
      { labelKey: 'dashboard.legend.invitations.accepted', count: invitations.acceptedInvitations, color: '#2F8A3A' },
      { labelKey: 'dashboard.legend.invitations.pendingResponse', count: invitations.pendingInvitations, color: '#FFB547' },
      { labelKey: 'dashboard.legend.invitations.expired', count: invitations.expiredInvitations, color: '#D9182D' },
      { labelKey: 'dashboard.status.Rejected', count: invitations.rejectedInvitations, color: '#6C4BB6' }
    ];
  });
  readonly chartData = computed<ChartData<'doughnut'>>(() => {
    const items = this.localizedItems();
    return items.every(item => item.count === 0)
      ? { labels: [this.translate.instant('common.chart.noData')], datasets: [{ data: [1], backgroundColor: ['#E5E7EB'], borderWidth: 0 }] }
      : { labels: items.map(item => item.label), datasets: [{ data: items.map(item => item.count), backgroundColor: items.map(item => item.color), borderWidth: 0, hoverOffset: 8 }] };
  });

  ngOnInit(): void { this.api.getLatestInvitations(this.filters()).subscribe(rows => this.latestInvitations.set(rows)); }
  exportList(): void { if (!this.canExport() || this.exportInProgress()) return; this.exportInProgress.set(true);
    this.api.exportList('Invitations', this.filters()).pipe(finalize(() => this.exportInProgress.set(false)))
      .subscribe(response => this.downloadResponse(response)); }
  async exportCharts(): Promise<void> { if (!this.canExport() || this.exportInProgress()) return; this.exportInProgress.set(true);
    try { await this.chartExport.download([{ host: this.chart?.nativeElement,
      filename: this.translate.instant('dashboard.export.files.invitationStatus'), title: this.translate.instant('dashboard.modals.invitations.statusTitle'),
      totalLabel: this.translate.instant('dashboard.common.total'), total: this.kpis().totalInvitations, items: this.localizedItems(),
      direction: this.translate.currentLang === 'ar' ? 'rtl' : 'ltr' }]); } finally { this.exportInProgress.set(false); } }
  private localizedItems(): DashboardChartExportItem[] { this.languageChange(); return this.items().map(item => ({ ...item, label: this.translate.instant(item.labelKey) })); }
  private downloadResponse(response: HttpResponse<Blob>): void { const disposition = response.headers.get('content-disposition') ?? '';
    const encoded = /filename\*=UTF-8''([^;]+)/i.exec(disposition)?.[1]; const plain = /filename="?([^";]+)"?/i.exec(disposition)?.[1];
    this.files.downloadBlob(response.body ?? new Blob(), encoded ? decodeURIComponent(encoded) : plain ?? 'Invitations.xlsx'); }
}
