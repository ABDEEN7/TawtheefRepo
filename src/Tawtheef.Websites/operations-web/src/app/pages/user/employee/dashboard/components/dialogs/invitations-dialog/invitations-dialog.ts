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
import { ChartModule } from 'primeng/chart';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { finalize } from 'rxjs';

import { PaginatedResult } from '../../../../../../../core/models/paginated-result.model';
import { FileUtilsService } from '../../../../../../../core/utils/file-utils';
import { PaginationComponent } from '../../../../../../../shared/components/pagination/pagination.component';
import { I18nNamespaceDirective } from '../../../../../../../shared/directives/i18n-namespace.directive';
import {
  DashboardChartColors,
  invitationSummaryColor,
} from '../../../constants/dashboard-chart-colors';
import { InvitationKpis, LatestInvitation } from '../../../models/dashboard-invitations.model';
import { OperationsDashboardFilters } from '../../../models/dashboard-filters.model';
import {
  DashboardChartExportItem,
  DashboardChartExportService,
} from '../../../services/dashboard-chart-export.service';
import { OperationsDashboardService } from '../../../services/operations-dashboard.service';
import { InvitationSource } from '../../../../../../../core/enums/invitation-source.enum';

@Component({
  selector: 'app-dashboard-invitations-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ChartModule,
    TableModule,
    TagModule,
    TranslatePipe,
    I18nNamespaceDirective,
    PaginationComponent,
  ],
  templateUrl: './invitations-dialog.html',
  styleUrl: './invitations-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InvitationsDialog implements OnInit {
  readonly InvitationSource = InvitationSource;
  @ViewChild('chart', { read: ElementRef })
  private chart?: ElementRef<HTMLElement>;

  readonly kpis = input.required<InvitationKpis>();
  readonly filters = input.required<OperationsDashboardFilters>();
  readonly canExport = input.required<boolean>();

  readonly closed = output<void>();

  private readonly api = inject(OperationsDashboardService);
  private readonly chartExport = inject(DashboardChartExportService);
  private readonly files = inject(FileUtilsService);
  private readonly translate = inject(TranslateService);

  private readonly languageChange = toSignal(this.translate.onLangChange, { initialValue: null });

  readonly invitations = signal<PaginatedResult<LatestInvitation> | null>(null);

  readonly tableFilters = signal<OperationsDashboardFilters>({
    pageNumber: 1,
    pageSize: 10,
  });

  readonly exportInProgress = signal(false);

  readonly rows = computed(() => this.invitations()?.items ?? []);

  readonly totalItems = computed(() => this.invitations()?.metadata?.totalCount ?? 0);

  readonly pageNumber = computed(() => this.tableFilters().pageNumber ?? 1);

  readonly pageSize = computed(() => this.tableFilters().pageSize ?? 10);

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
    const invitations = this.kpis();

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

  readonly chartData = computed<ChartData<'doughnut'>>(() => {
    const items = this.localizedItems();

    return items.every((item) => item.count === 0)
      ? {
          labels: [this.translate.instant('common.chart.noData')],
          datasets: [
            {
              data: [1],
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
              hoverOffset: 8,
            },
          ],
        };
  });

  ngOnInit(): void {
    this.loadLatestInvitations();
  }

  onPageChange(pageNumber: number): void {
    if (pageNumber === this.pageNumber()) return;

    this.tableFilters.update((filters) => ({
      ...filters,
      pageNumber,
    }));

    this.loadLatestInvitations();
  }

  onPageSizeChange(pageSize: number): void {
    this.tableFilters.update((filters) => ({
      ...filters,
      pageNumber: 1,
      pageSize,
    }));

    this.loadLatestInvitations();
  }

  exportList(): void {
    if (!this.canExport() || this.exportInProgress()) return;

    this.exportInProgress.set(true);

    this.api
      .exportList('Invitations', this.filters())
      .pipe(finalize(() => this.exportInProgress.set(false)))
      .subscribe((response) => this.downloadResponse(response));
  }

  async exportCharts(): Promise<void> {
    if (!this.canExport() || this.exportInProgress()) return;

    this.exportInProgress.set(true);

    try {
      await this.chartExport.download([
        {
          host: this.chart?.nativeElement,
          filename: this.translate.instant('dashboard.export.files.invitationStatus'),
          title: this.translate.instant('dashboard.modals.invitations.statusTitle'),
          totalLabel: this.translate.instant('dashboard.common.total'),
          total: this.kpis().totalInvitations,
          items: this.localizedItems(),
          direction: this.translate.currentLang === 'ar' ? 'rtl' : 'ltr',
        },
      ]);
    } finally {
      this.exportInProgress.set(false);
    }
  }

  private loadLatestInvitations(): void {
    this.api
      .getLatestInvitations({
        ...this.filters(),
        ...this.tableFilters(),
      })
      .subscribe((result) => {
        this.invitations.set(result);

        if (result.metadata) {
          this.tableFilters.update((filters) => ({
            ...filters,
            pageNumber: result.metadata.currentPage,
            pageSize: result.metadata.pageSize,
          }));
        }
      });
  }

  private localizedItems(): DashboardChartExportItem[] {
    this.languageChange();

    return this.items().map((item) => ({
      ...item,
      label: this.translate.instant(item.labelKey),
    }));
  }

  private downloadResponse(response: HttpResponse<Blob>): void {
    const disposition = response.headers.get('content-disposition') ?? '';

    const encoded = /filename\*=UTF-8''([^;]+)/i.exec(disposition)?.[1];

    const plain = /filename="?([^";]+)"?/i.exec(disposition)?.[1];

    this.files.downloadBlob(
      response.body ?? new Blob(),
      encoded ? decodeURIComponent(encoded) : (plain ?? 'Invitations.xlsx'),
    );
  }
}
