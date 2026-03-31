import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { JobDetailsService } from '../services/job-details.service';

@Component({
  selector: 'app-job-skills',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (skills().length > 0) {
      <ul>
        @for (skill of skills(); track $index) {
          <li>{{ skill }}</li>
        }
      </ul>
    }
  `
})
export class JobSkillsComponent {
  private detailsService = inject(JobDetailsService);
  job = this.detailsService.job;

  skills = computed(() => {
    if (!this.job()?.skills?.length) return [];
    return this.job()!.skills!
      .filter(skill => skill.showToApplicants)
      .map(skill => skill.skill.name);
  });
}
