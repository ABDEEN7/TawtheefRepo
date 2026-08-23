import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  signal,
} from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { toSignal } from '@angular/core/rxjs-interop';
import { finalize } from 'rxjs';
import { I18nNamespaceDirective } from '../../../../../../../shared/directives/i18n-namespace.directive';
import { FileUtilsService } from '../../../../../../../core/utils/file-utils';
import { ProfileBreakdown } from '../../../models/dashboard-candidates.model';
import { DashboardKpis } from '../../../models/dashboard-overview.model';
import { OperationsDashboardFilters } from '../../../models/dashboard-filters.model';
import { OperationsDashboardService } from '../../../services/operations-dashboard.service';
import {
  DashboardChartExportItem,
  DashboardChartExportSpec,
  DashboardChartExportService,
} from '../../../services/dashboard-chart-export.service';
import {
  profileStatusColor,
} from '../../../constants/dashboard-chart-colors';
interface CandidatesDialogData {
  breakdown: ProfileBreakdown;
  kpis: DashboardKpis;
  filters: OperationsDashboardFilters;
  canExport: boolean;
}

interface CandidateCohortItem {
  key: string;
  count: number;
}
@Component({
  selector: 'app-dashboard-candidates-dialog',
  standalone: true,
  imports: [CommonModule, TranslatePipe, I18nNamespaceDirective],
  templateUrl: './candidates-dialog.html',
  styleUrl: './candidates-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CandidatesDialog {
  private readonly config = inject(DynamicDialogConfig<CandidatesDialogData>);
  readonly breakdown = computed<ProfileBreakdown>(() => this.config.data.breakdown);
  readonly kpis = computed<DashboardKpis>(() => this.config.data.kpis);
  readonly filters = computed<OperationsDashboardFilters>(() => this.config.data.filters);
  readonly canExport = computed<boolean>(() => this.config.data.canExport);
  private readonly dashboardService = inject(OperationsDashboardService);
  private readonly chartExport = inject(DashboardChartExportService);
  private readonly fileUtils = inject(FileUtilsService);
  private readonly translate = inject(TranslateService);
  private readonly languageChange = toSignal(this.translate.onLangChange, { initialValue: null });
  readonly exportInProgress = signal(false);
  private readonly candidateTypeColors = [
    '#D9182D',
    '#FFB547',
    '#94DDBF',
    '#8A1538',
    '#488ADA',
    '#2F8A3A',
    '#6C4BB6',
  ];

  readonly statusItems = computed(() => {
    this.languageChange();

    return this.breakdown().byStatus.map((item) => ({
      label: this.translate.instant(`dashboard.status.${item.status}`),
      count: item.count,
      color: profileStatusColor(item.status),
    }));
  });
  readonly typeItems = computed(() =>
    this.breakdown().byCandidateType.map((item, index) => ({
      label: item.label,
      count: item.count,
      color: this.candidateTypeColors[index % this.candidateTypeColors.length],
    })),
  );
  readonly typeTotal = computed(() =>
    this.typeItems().reduce((total, item) => total + item.count, 0),
  );
  readonly cohortItems = computed<CandidateCohortItem[]>(() => {
    const cohorts = this.breakdown().cohorts;
    const items: Array<[string, number]> = [
      ['registeredKawaderProfiles', cohorts.registeredKawaderProfiles],
      ['registeredMinisterOfficeProfiles', cohorts.registeredMinisterOfficeProfiles],
      ['qatarGraduateProfiles', cohorts.qatarGraduateProfiles],
    ];
    if (cohorts.includeOfficeProfiles) items.push(['officeProfiles', cohorts.officeProfiles]);
    return items.map(([key, count]) => ({ key, count: Number(count) }));
  });
  readonly cohortMaximum = computed(() => Math.max(0, ...this.cohortItems().map(item => item.count)));

  percent(count: number, total: number): number {
    return total > 0 ? Math.round((count / total) * 1000) / 10 : 0;
  }
  cohortWidth(count: number): number {
    return this.cohortMaximum() > 0 ? count / this.cohortMaximum() * 100 : 0;
  }

  exportList(): void {
    if (!this.canExport() || this.exportInProgress()) return;
    this.exportInProgress.set(true);
    this.dashboardService
      .exportList('Candidates', this.filters())
      .pipe(finalize(() => this.exportInProgress.set(false)))
      .subscribe((response) => this.downloadResponse(response, 'Candidates.xlsx'));
  }

  async exportCharts(): Promise<void> {
    if (!this.canExport() || this.exportInProgress()) return;
    this.exportInProgress.set(true);
    try {
      await this.chartExport.downloadReport({
        filename: this.translate.instant('dashboard.export.files.candidates'),
        sections: [
          this.exportSpec(
            'dashboard.modals.candidates.statusTitle',
            this.kpis().totalProfiles,
            this.statusItems(),
          ),
          this.exportSpec('dashboard.candidates.typeTitle', this.typeTotal(), this.typeItems()),
          this.exportSpec(
            'dashboard.cohorts.title',
            undefined,
            this.cohortItems().map((item) => ({
              label: this.translate.instant(`dashboard.cohorts.${item.key}`),
              count: item.count,
              color: '#8A1538',
            })),
          ),
        ],
      });
    } finally {
      this.exportInProgress.set(false);
    }
  }

  private exportSpec(
    titleKey: string,
    total: number | undefined,
    items: DashboardChartExportItem[],
  ): DashboardChartExportSpec {
    return {
      filename: this.translate.instant('dashboard.export.files.candidates'),
      title: this.translate.instant(titleKey),
      totalLabel: this.translate.instant('dashboard.common.total'),
      total,
      items,
      direction: this.translate.currentLang === 'ar' ? ('rtl' as const) : ('ltr' as const),
      locale: 'en-US',
    };
  }

  private downloadResponse(
    response: import('@angular/common/http').HttpResponse<Blob>,
    fallback: string,
  ): void {
    const disposition = response.headers.get('content-disposition') ?? '';
    const encoded = /filename\*=UTF-8''([^;]+)/i.exec(disposition)?.[1];
    const plain = /filename="?([^";]+)"?/i.exec(disposition)?.[1];
    this.fileUtils.downloadBlob(
      response.body ?? new Blob(),
      encoded ? decodeURIComponent(encoded) : (plain ?? fallback),
    );
  }
}
