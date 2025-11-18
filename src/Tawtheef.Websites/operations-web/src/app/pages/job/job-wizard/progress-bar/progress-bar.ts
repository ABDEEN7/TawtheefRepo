import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-progress-bar',
  standalone: false,
  template: `
    <div class="d-flex align-items-center gap-3">
      <div class="progress" style="width:260px">
        <div class="progress-bar" [style.width.%]="value"></div>
      </div>
      <span class="small-muted">{{ value }}%</span>
    </div>
  `
})
export class ProgressBarComponent {
  @Input() value = 0;
}
