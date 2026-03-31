import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Reusable metric chip for showing a label + value pair with an optional icon.
 *
 * Usage:
 *   <app-metric-chip label="Invited" [value]="120" icon="pi pi-send" variant="info" />
 */
@Component({
  selector: 'app-metric-chip',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="metric-chip" [ngClass]="'metric--' + variant()">
      @if (icon()) {
        <i [class]="icon()" class="metric-icon"></i>
      }
      <span class="metric-value">{{ value() }}</span>
      <span class="metric-label">{{ label() }}</span>
    </div>
  `,
  styles: [`
    .metric-chip {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      padding: 6px 12px;
      border-radius: 8px;
      font-size: 12px;
      font-weight: 500;
      white-space: nowrap;
      transition: transform 0.15s ease;
    }

    .metric-chip:hover {
      transform: translateY(-1px);
    }

    .metric-icon {
      font-size: 13px;
    }

    .metric-value {
      font-weight: 700;
      font-size: 14px;
    }

    .metric-label {
      color: inherit;
      opacity: 0.8;
    }

    /* ── Variants ── */
    .metric--info {
      background: rgba(78, 128, 234, 0.10);
      color: #2f5ac4;
    }

    .metric--success {
      background: rgba(43, 157, 118, 0.10);
      color: #1a7a5a;
    }

    .metric--warning {
      background: rgba(245, 179, 66, 0.10);
      color: #9a6d00;
    }

    .metric--danger {
      background: rgba(223, 109, 78, 0.10);
      color: #b94a2e;
    }

    .metric--neutral {
      background: rgba(108, 117, 125, 0.10);
      color: #495057;
    }

    .metric--purple {
      background: rgba(154, 107, 255, 0.10);
      color: #6f42c1;
    }
  `]
})
export class MetricChipComponent {
  label = input<string>('');
  value = input<number>(0);
  icon = input<string>('');
  variant = input<'info' | 'success' | 'warning' | 'danger' | 'neutral' | 'purple'>('neutral');
}
