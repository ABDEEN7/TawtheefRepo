import { Component, input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface PipelineSegment {
  label: string;
  value: number;
  color: string;
}

/**
 * Compact horizontal pipeline bar.
 * Takes an array of segments and renders them proportionally.
 *
 * Usage:
 *   <app-pipeline-bar [segments]="pipelineSegments" [total]="totalInvitations" />
 */
@Component({
  selector: 'app-pipeline-bar',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="pipeline-bar-wrapper">
      <div class="pipeline-track">
        @for (seg of normalizedSegments(); track seg.label) {
          @if (seg.percent > 0) {
            <div
              class="pipeline-segment"
              [style.width.%]="seg.percent"
              [style.backgroundColor]="seg.color"
              [title]="seg.label + ': ' + seg.value">
            </div>
          }
        }
      </div>
      <div class="pipeline-legend">
        @for (seg of normalizedSegments(); track seg.label) {
          <div class="legend-item">
            <span class="legend-dot" [style.backgroundColor]="seg.color"></span>
            <span class="legend-label">{{ seg.label }}</span>
            <span class="legend-value">{{ seg.value }}</span>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .pipeline-bar-wrapper {
      min-width: 180px;
    }

    .pipeline-track {
      display: flex;
      height: 8px;
      border-radius: 4px;
      overflow: hidden;
      background: #e9ecef;
      gap: 1px;
    }

    .pipeline-segment {
      transition: width 0.4s cubic-bezier(0.4, 0, 0.2, 1);
      min-width: 3px;
    }

    .pipeline-segment:first-child {
      border-radius: 4px 0 0 4px;
    }

    .pipeline-segment:last-child {
      border-radius: 0 4px 4px 0;
    }

    .pipeline-legend {
      display: flex;
      flex-wrap: wrap;
      gap: 6px 12px;
      margin-top: 6px;
    }

    .legend-item {
      display: flex;
      align-items: center;
      gap: 4px;
      font-size: 11px;
      color: #6c757d;
    }

    .legend-dot {
      width: 6px;
      height: 6px;
      border-radius: 50%;
      flex-shrink: 0;
    }

    .legend-value {
      font-weight: 600;
      color: #343a40;
    }
  `]
})
export class PipelineBarComponent {
  segments = input<PipelineSegment[]>([]);
  total = input<number>(0);

  normalizedSegments = computed(() => {
    const segs = this.segments();
    const t = this.total() || segs.reduce((sum, s) => sum + s.value, 0) || 1;

    return segs.map(s => ({
      ...s,
      percent: Math.max(0, (s.value / t) * 100)
    }));
  });
}
