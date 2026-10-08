import { NgClass } from '@angular/common';
import { Component, input } from '@angular/core';

export type StatusTone =
  | 'neutral'
  | 'secondary'
  | 'info'
  | 'success'
  | 'warning'
  | 'danger'
  | 'primary';

@Component({
  selector: 'app-status-pill',
  standalone: true,
  imports: [NgClass],
  template: `<span class="pill small-pill" [ngClass]="tone()">{{ label() }}</span>`,
})
export class StatusPillComponent {
  readonly label = input.required<string>();
  readonly tone = input.required<StatusTone>();
}
