import { Component, inject, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { JobDetailsService } from '../../services/job-details.service';

@Component({
  selector: 'app-job-side-info',
  standalone: true,
  imports: [CommonModule, TranslatePipe, DatePipe],
  template: `
    <div class="card card-table">
      <div class="card-body">
        <h5 class="mb-4 text-primary">
          <i class="hgi-stroke hgi-information-circle"></i> {{ 'JOB_DETAILS.JOB_INFO' | translate }}
        </h5>
        <div class="info-list-container">
          <div class="info-list-item d-flex align-items-center justify-content-between min-w-0">
            <span class="label me-3 flex-shrink-0">{{ 'JOB_DETAILS.JOB_CATEGORY' | translate }}:</span>
            <span class="value text-truncate" [title]="job()?.jobCategory?.name || ''">
              {{ job()?.jobCategory?.name || ('JOB_DETAILS.NOT_SPECIFIED' | translate) }}
            </span>
          </div>
          <div class="info-list-item d-flex align-items-center justify-content-between min-w-0">
            <span class="label me-3 flex-shrink-0">{{ 'JOB_DETAILS.INVITATION_EXPIRY_DATE' | translate }}:</span>
            <span class="value">{{ job()?.expiresOn ? (job()?.expiresOn | date: 'dd/MM/yyyy') : '-' }}</span>
          </div>
          <div class="info-list-item d-flex align-items-center justify-content-between min-w-0">
            <span class="label me-3 flex-shrink-0">{{ 'JOB_DETAILS.WORK_LOCATION' | translate }}:</span>
            <span class="value text-truncate" [title]="job()?.workLocation?.name || ''">
              {{ job()?.workLocation?.name || ('JOB_DETAILS.NOT_SPECIFIED' | translate) }}
            </span>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./job-side-info.component.scss']
})
export class JobSideInfoComponent {
  private detailsService = inject(JobDetailsService);
  job = this.detailsService.job;

  isAllMandatoryUploaded = computed(() => {
    const job = this.job();
    if (!job || !job.requiredAttachments) return true;
    return job.requiredAttachments
      .filter(a => a.isMandatory)
      .every(a => !!a.attachmentId && !a.isReturned);
  });

  isJobOpen = computed(() => {
    const job = this.job();
    if (!job) return false;
    const status = job.jobStatus?.backendName?.toLowerCase() ?? '';
    const isClosedStatus = status.includes('closed') || status.includes('expired') || status.includes('cancel');
    if (isClosedStatus) return false;
    if (!job.closingDate) return true;
    return new Date(job.closingDate) >= new Date();
  });

  hasApplied = computed(() => {
    const status = this.job()?.invitationStatus?.backendName?.toLowerCase() ?? '';
    return status.includes('applied') || status.includes('submitted') || status.includes('attachmentapproval');
  });

  isReturned = computed(() => {
    const status = this.job()?.invitationStatus?.backendName?.toLowerCase() ?? '';
    return status.includes('returned');
  });
}
