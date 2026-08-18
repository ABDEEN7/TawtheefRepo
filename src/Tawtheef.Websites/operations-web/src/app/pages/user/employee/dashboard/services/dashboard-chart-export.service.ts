import { inject, Injectable } from '@angular/core';
import { FileUtilsService } from '../../../../../core/utils/file-utils';

export interface DashboardChartExportItem {
  label: string;
  count: number;
  color: string;
}

export interface DashboardChartExportSpec {
  host: HTMLElement | undefined;
  filename: string;
  title: string;
  totalLabel: string;
  total: number;
  items: DashboardChartExportItem[];
  direction: 'ltr' | 'rtl';
}

@Injectable({ providedIn: 'root' })
export class DashboardChartExportService {
  private readonly fileUtils = inject(FileUtilsService);

  async download(charts: ReadonlyArray<DashboardChartExportSpec>): Promise<void> {
    for (const spec of charts) {
      const source = this.getRenderedCanvas(spec.host);
      if (!source) continue;

      const canvas = document.createElement('canvas');
      const rowHeight = 42;
      canvas.width = 1100;
      canvas.height = Math.max(620, 150 + spec.items.length * rowHeight);
      const context = canvas.getContext('2d');
      if (!context) continue;

      context.fillStyle = '#ffffff';
      context.fillRect(0, 0, canvas.width, canvas.height);
      context.direction = spec.direction;
      context.textAlign = spec.direction === 'rtl' ? 'right' : 'left';
      context.fillStyle = '#1f2937';
      context.font = 'bold 30px Arial, sans-serif';
      const textX = spec.direction === 'rtl' ? 1050 : 610;
      context.fillText(spec.title, textX, 55);
      context.font = 'bold 24px Arial, sans-serif';
      context.fillText(`${spec.totalLabel}: ${spec.total}`, textX, 100);
      context.drawImage(source, 40, 70, 520, 520);

      context.font = '20px Arial, sans-serif';
      spec.items.forEach((item, index) => {
        const y = 155 + index * rowHeight;
        const markerX = spec.direction === 'rtl' ? 1025 : 610;
        context.fillStyle = item.color;
        context.fillRect(markerX, y - 17, 22, 22);
        context.fillStyle = '#374151';
        context.fillText(`${item.label}: ${item.count}`, spec.direction === 'rtl' ? 1008 : 644, y);
      });

      const blob = await new Promise<Blob | null>(resolve => canvas.toBlob(resolve, 'image/png'));
      if (blob) await this.fileUtils.downloadBlob(blob, `${spec.filename}.png`);
    }
  }

  private getRenderedCanvas(host: HTMLElement | undefined): HTMLCanvasElement | null {
    if (!host || !host.isConnected) return null;
    const canvas = host.querySelector('canvas');
    return canvas instanceof HTMLCanvasElement ? canvas : null;
  }
}
