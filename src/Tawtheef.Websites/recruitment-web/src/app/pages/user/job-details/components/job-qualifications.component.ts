import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { JobDetailsService } from '../services/job-details.service';

@Component({
  selector: 'app-job-qualifications',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  template: `
    <div class="row">
      <div class="col-md-6">
        <h5>{{ 'JOB_DETAILS.QUALIFICATION_REQUIREMENTS' | translate }}</h5>
        <p>{{ degreeRequirements() }}</p>
      </div>
    </div>
    @if (job()?.qualificationDescription) {
    <div class="row mt-4">
      <div class="col-md-6">
        <h5>{{ 'JOB_DETAILS.QUALIFICATIONS' | translate }}</h5>
        <div class="text-area">
          <div [innerHTML]="job()!.qualificationDescription"></div>
        </div>
      </div>
    </div>
    }
  `
})
export class JobQualificationsComponent {
  private detailsService = inject(JobDetailsService);
  job = this.detailsService.job;

  degreeRequirements = computed(() => {
    if (!this.job()?.degrees?.length) return '';
    const degreeNames = this.job()!.degrees!.map(degree => degree.degree.name);
    return degreeNames.join(', ') || '';
  });
}
