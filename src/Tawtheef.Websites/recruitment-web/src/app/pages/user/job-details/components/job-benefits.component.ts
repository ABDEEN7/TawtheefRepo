import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { JobDetailsService } from '../services/job-details.service';

@Component({
  selector: 'app-job-benefits',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (job()?.benefits) {
      <div class="text-area">
        <div [innerHTML]="job()?.benefits"></div>
      </div>
    }
  `
})
export class JobBenefitsComponent {
  private detailsService = inject(JobDetailsService);
  job = this.detailsService.job;
}
