import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { JobDetailsService } from '../services/job-details.service';

@Component({
  selector: 'app-job-responsibilities',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (responsibilities().length > 0) {
      <ul>
        @for (responsibility of responsibilities(); track $index) {
          <div class="text-area">
            <div [innerHTML]="responsibility"></div>
          </div>
        }
      </ul>
    }
  `
})
export class JobResponsibilitiesComponent {
  private detailsService = inject(JobDetailsService);
  job = this.detailsService.job;

  responsibilities = computed(() => {
    if (!this.job()?.responsibilities?.length) return [];
    return this.job()!.responsibilities!.map((c) => c.text);
  });
}
