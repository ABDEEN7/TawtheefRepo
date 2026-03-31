import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { JobDetailsService } from '../services/job-details.service';

@Component({
  selector: 'app-job-conditions',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  template: `
    @if (conditions().length > 0) {
      <div class="basic-requirements">
        @for (condition of conditions(); track $index) {
          <div class="text-area">
            <div [innerHTML]="condition"></div>
          </div>
        }
      </div>
    } @else {
      <p class="text-muted">{{ 'JOB_DETAILS.NOT_SPECIFIED' | translate }}</p>
    }
  `
})
export class JobConditionsComponent {
  private detailsService = inject(JobDetailsService);
  job = this.detailsService.job;

  conditions = computed(() => {
    if (!this.job()?.conditions?.length) return [];
    return this.job()!.conditions!.map((c) => c.text);
  });
}
