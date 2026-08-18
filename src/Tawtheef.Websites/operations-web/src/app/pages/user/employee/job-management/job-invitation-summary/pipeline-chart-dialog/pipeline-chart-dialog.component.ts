import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import 'chart.js/auto';
import { ChartData, ChartOptions } from 'chart.js';
import { ChartModule } from 'primeng/chart';

export interface PipelineChartData {
  jobName: string;
  metrics: { label: string; value: number; color: string }[];
}

@Component({
  selector: 'app-pipeline-chart-dialog',
  standalone: true,
  imports: [CommonModule, ChartModule],
  template: `
    <div class="pipeline-chart-dialog">
      <div>
        <p-chart type="bar" [data]="chartData()" [options]="chartOptions"></p-chart>
      </div>

      <div class="metrics-grid">
        @for (metric of dialogData.metrics; track metric.label) {
          <div class="metric-card">
            <div class="metric-card__value" [style.color]="metric.color">
              {{ metric.value }}
            </div>
            <div class="metric-card__label">
              {{ metric.label }}
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .pipeline-chart-dialog {
      padding: 8px 0;
    }

    .chart-container {
      height: 320px;
      margin-bottom: 24px;
    }

    .metrics-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
      gap: 12px;
    }

    .metric-card {
      text-align: center;
      padding: 16px 12px;
      border-radius: 12px;
      background: #f8f9fa;
      border: 1px solid #e9ecef;
      transition: transform 0.15s ease, box-shadow 0.15s ease;
    }

    .metric-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
    }

    .metric-card__value {
      font-size: 28px;
      font-weight: 700;
      line-height: 1.2;
    }

    .metric-card__label {
      font-size: 12px;
      color: #6c757d;
      margin-top: 4px;
      font-weight: 500;
    }
  `]
})
export class PipelineChartDialogComponent implements OnInit {
  private config = inject(DynamicDialogConfig<PipelineChartData>);
  private ref = inject(DynamicDialogRef);

  dialogData!: PipelineChartData;
  chartData = signal<ChartData<'bar'>>({ labels: [], datasets: [] });

  chartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      tooltip: {
        backgroundColor: '#1e293b',
        titleFont: { size: 13, weight: 'bold' },
        bodyFont: { size: 12 },
        padding: 12,
        cornerRadius: 8,
        displayColors: true,
      }
    },
    scales: {
      x: {
        grid: { display: false },
        ticks: { font: { size: 12, weight: 'bold' } }
      },
      y: {
        beginAtZero: true,
        ticks: { precision: 0, font: { size: 11 } },
        grid: { color: 'rgba(0,0,0,0.05)' }
      }
    }
  };

  ngOnInit(): void {
    this.dialogData = this.config.data!;

    this.chartData.set({
      labels: this.dialogData.metrics.map(m => m.label),
      datasets: [{
        label: this.dialogData.jobName,
        data: this.dialogData.metrics.map(m => m.value),
        backgroundColor: this.dialogData.metrics.map(m => m.color),
        borderRadius: 8,
        barPercentage: 0.6,
        categoryPercentage: 0.7
      }]
    });
  }

  close(): void {
    this.ref.close();
  }
}
