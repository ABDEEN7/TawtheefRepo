import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { JobDetailsService } from '../services/job-details.service';

@Component({
  selector: 'app-job-overview',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (job()?.overView) {
      <div class="text-area">
        <div [innerHTML]="job()?.overView"></div>
      </div>
    }
  `
})
export class JobOverviewComponent {
  private detailsService = inject(JobDetailsService);
  job = this.detailsService.job;
}
