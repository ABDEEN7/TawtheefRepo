import { inject, Injectable } from '@angular/core';
import { FileUtilsService } from '../../../../../core/utils/file-utils';

export interface DashboardChartExportItem {
  label: string;
  count: number;
  color: string;
}

export interface DashboardChartExportSpec {
  filename: string;
  title: string;
  totalLabel?: string;
  total?: number;
  items: DashboardChartExportItem[];
  direction: 'ltr' | 'rtl';
  locale: string;
}

export interface DashboardVisualExportReport {
  filename: string;
  sections: ReadonlyArray<DashboardChartExportSpec>;
}

@Injectable({ providedIn: 'root' })
export class DashboardChartExportService {
  private readonly fileUtils = inject(FileUtilsService);

  async download(charts: ReadonlyArray<DashboardChartExportSpec>): Promise<void> {
    for (const spec of charts) {
      await this.downloadReport({ filename: spec.filename, sections: [spec] });
    }
  }

  async downloadReport(report: DashboardVisualExportReport): Promise<void> {
    if (report.sections.length === 0) return;

    const canvas = document.createElement('canvas');
    canvas.width = 1100;
    canvas.height = Math.max(
      620,
      report.sections.reduce((height, section) => height + this.sectionHeight(section), 40),
    );
    const context = canvas.getContext('2d');
    if (!context) return;

    context.fillStyle = '#ffffff';
    context.fillRect(0, 0, canvas.width, canvas.height);

    let offsetY = 20;
    report.sections.forEach((section) => {
      this.drawSection(context, section, offsetY);
      offsetY += this.sectionHeight(section);
    });

    const blob = await new Promise<Blob | null>((resolve) => canvas.toBlob(resolve, 'image/png'));
    if (blob) await this.fileUtils.downloadBlob(blob, `${report.filename}.png`);
  }

  private sectionHeight(spec: DashboardChartExportSpec): number {
    const headingHeight = spec.total != null && spec.totalLabel ? 135 : 85;
    return Math.max(235, headingHeight + spec.items.length * 42 + 35);
  }

  private drawSection(
    context: CanvasRenderingContext2D,
    spec: DashboardChartExportSpec,
    offsetY: number,
  ): void {
    const textX = spec.direction === 'rtl' ? 1050 : 50;
    const markerX = spec.direction === 'rtl' ? 1025 : 50;
    const firstRowY = offsetY + (spec.total != null && spec.totalLabel ? 135 : 85);
    const numberFormatter = new Intl.NumberFormat(spec.locale, { maximumFractionDigits: 0 });

    context.direction = spec.direction;
    context.textAlign = spec.direction === 'rtl' ? 'right' : 'left';
    context.fillStyle = '#1f2937';
    context.font = 'bold 30px Arial, sans-serif';
    context.fillText(spec.title, textX, offsetY + 35);

    if (spec.total != null && spec.totalLabel) {
      context.font = 'bold 24px Arial, sans-serif';
      context.fillText(
        `${spec.totalLabel}: ${numberFormatter.format(spec.total)}`,
        textX,
        offsetY + 80,
      );
    }

    context.font = '20px Arial, sans-serif';
    const max = Math.max(1, ...spec.items.map((item) => item.count));
    spec.items.forEach((item, index) => {
      const y = firstRowY + index * 42;
      context.fillStyle = item.color;
      context.fillRect(markerX, y - 17, 22, 22);
      context.fillStyle = '#374151';
      context.fillText(
        `${item.label}: ${numberFormatter.format(item.count)}`,
        spec.direction === 'rtl' ? 995 : 82,
        y,
      );
      const width = item.count === 0 ? 0 : Math.max(2, (item.count / max) * 460);
      context.fillStyle = item.color;
      context.fillRect(spec.direction === 'rtl' ? 520 - width : 50, y + 9, width, 9);
    });

    context.fillStyle = '#e5e7eb';
    context.fillRect(50, offsetY + this.sectionHeight(spec) - 15, 1000, 1);
  }
}
