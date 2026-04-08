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
      min-width: 11.25rem;
    }

    .pipeline-track {
      display: flex;
      height: 0.5rem;
      border-radius: 0.25rem;
      overflow: hidden;
      background: #e9ecef;
      gap: 1px;
    }

    .pipeline-segment {
      transition: width 0.4s cubic-bezier(0.4, 0, 0.2, 1);
      min-width: 0.1875rem;
    }

    .pipeline-segment:first-child {
      border-radius: 0.25rem 0 0 0.25rem;
    }

    .pipeline-segment:last-child {
      border-radius: 0 0.25rem 0.25rem 0;
    }

    .pipeline-legend {
      display: flex;
      flex-wrap: wrap;
      gap: 0.375rem 0.75rem;
      margin-top: 0.375rem;
    }

    .legend-item {
      display: flex;
      align-items: center;
      gap: 0.25rem;
      font-size: 0.6875rem;
      color: #6c757d;
    }

    .legend-dot {
      width: 0.375rem;
      height: 0.375rem;
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
