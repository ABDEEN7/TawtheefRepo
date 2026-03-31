import { Component, input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Reusable status badge component.
 * Maps a backendName to a color-coded pill.
 *
 * Usage:
 *   <app-status-badge [backendName]="'Published'" [label]="status.name" />
 */
@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span class="status-badge" [ngClass]="badgeClass()">
      {{ label() }}
    </span>
  `,
  styles: [`
    .status-badge {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      padding: 4px 12px;
      border-radius: 20px;
      font-size: 12px;
      font-weight: 600;
      line-height: 1.4;
      white-space: nowrap;
      letter-spacing: 0.02em;
    }

    .status-badge::before {
      content: '';
      width: 7px;
      height: 7px;
      border-radius: 50%;
      flex-shrink: 0;
    }

    /* ── Draft / Default ── */
    .badge--default {
      background: rgba(108, 117, 125, 0.12);
      color: #495057;
    }
    .badge--default::before { background: #6c757d; }

    /* ── Published / Open / Active / Applied / Submitted ── */
    .badge--success {
      background: rgba(43, 157, 118, 0.12);
      color: #1a7a5a;
    }
    .badge--success::before { background: #2b9d76; }

    /* ── Needs Update / Pending / Invited ── */
    .badge--warning {
      background: rgba(245, 179, 66, 0.12);
      color: #9a6d00;
    }
    .badge--warning::before { background: #f5b342; }

    /* ── Cancelled / Closed / Rejected ── */
    .badge--danger {
      background: rgba(223, 109, 78, 0.12);
      color: #b94a2e;
    }
    .badge--danger::before { background: #df6d4e; }

    /* ── Info ── */
    .badge--info {
      background: rgba(78, 128, 234, 0.12);
      color: #2f5ac4;
    }
    .badge--info::before { background: #4e80ea; }
  `]
})
export class StatusBadgeComponent {
  backendName = input<string>('');
  label = input<string>('');

  badgeClass = computed(() => {
    const key = (this.backendName() || '').toLowerCase();

    if (key.includes('publish') || key.includes('open') || key.includes('active') ||
        key.includes('appl') || key.includes('submitted'))
      return 'badge--success';

    if (key.includes('pending') || key.includes('need') || key.includes('invite') || key.includes('new'))
      return 'badge--warning';

    if (key.includes('cancel') || key.includes('closed') || key.includes('reject') || key.includes('expired'))
      return 'badge--danger';

    if (key.includes('read') || key.includes('review'))
      return 'badge--info';

    return 'badge--default';
  });
}
