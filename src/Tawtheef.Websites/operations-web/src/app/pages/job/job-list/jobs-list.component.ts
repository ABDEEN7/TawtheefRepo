import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { JobService } from '../services/job.service';
import { JobLookupService } from '../services/job-lookup.service';
import { PaginatedRequest } from '../../../core/models/paginated-request.model';
import { JobQueryFilter } from '../models/job-query-filter.model';
import { JobResponse } from '../models/job-response-model';
import { GUID } from '../../../shared/types/guid.type';
import { PaginationMetadata } from '../../../core/models/pagination-metadata.model';
import { PaginatedResult } from '../../../core/models/paginated-result.model';
import { NotificationService } from '../../../core/services/notification.service';
import { TranslateService } from '@ngx-translate/core';
import { JobStatus } from '../../../core/enums/lookups.enum';
import { routes } from '../../../routes/routes';

@Component({
  selector: 'app-job-list',
  standalone : false,
  templateUrl: './jobs-list.component.html',
  styleUrls: ['./jobs-list.component.scss'],
})
export class JobListComponent implements OnInit {
  private jobService = inject(JobService);
  private router = inject(Router);
  private notificationService = inject(NotificationService);
  private translateService = inject(TranslateService);
  
  lookupsService = inject(JobLookupService)

  jobs: PaginatedResult<JobResponse> | undefined;
  paginationMetadata: PaginationMetadata | undefined;
  
  currentPage = signal(1);
  itemsPerPage = 10;

  searchQuery = signal<string>('');
  filterType = signal<GUID | null>(null);
  filterStatus = signal<GUID | null>(null);

  jobStatus = JobStatus

  ngOnInit() {
    this.loadJobsWithFilters();
    this.lookupsService.loadJobStatus();
    this.lookupsService.loadJobCategories();
  }

  loadJobsWithFilters() {
    const pagination: PaginatedRequest = {
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage,
      sortBy: 'createdDate',
      sortDirection: 'desc'
    };

    const filter: JobQueryFilter = {
      searchTerm: this.searchQuery() || undefined,
      jobCategoryId: this.filterType() || undefined,
      statusId: this.filterStatus() || undefined
    };

    this.jobService.getAll(pagination, filter).subscribe({
      next: (paginatedData) => {
        this.jobs = paginatedData;
        this.paginationMetadata = paginatedData.metadata;
      }
    });
  }

  onFilterChange() {
    this.currentPage.set(1);
    this.loadJobsWithFilters();
  }

  clearFilters() {
    this.searchQuery.set('');
    this.filterType.set(null);
    this.filterStatus.set(null);
    this.currentPage.set(1);
    this.loadJobsWithFilters();
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadJobsWithFilters();
  }

  editJob(job: JobResponse) {
  this.router.navigate([routes.employee.jobEdit, job.id]).then();
  }

  viewJob(job: JobResponse) {
    this.router.navigate([routes.employee.jobView, job.id]).then();
  }

  createNewJob() {
    this.router.navigate([routes.employee.jobCreate]).then();
  }

  openPointsModal(job: JobResponse) {
    this.router.navigate([routes.employee.jobPoints,job.id]).then();
  }

  approveJob(job: JobResponse) {
    this.router.navigate([routes.employee.approvalJob, job.id]).then();
  }

  rejectJob(job: JobResponse) {
    const rejectedStatus = this.lookupsService.jobStatus().find(s => 
      s.backendName === this.jobStatus.Rejected
    );
    
    if (rejectedStatus) {
      if (confirm(this.translateService.instant('JOB_LIST_CONFIRMATIONS_REJECT_JOB'))) {
        this.jobService.reject(job.id, rejectedStatus.id  as GUID).subscribe({
          next: () => {
            this.notificationService.success(
              this.translateService.instant('JOB_LIST_MESSAGES_JOB_REJECTED')
            );
            this.loadJobsWithFilters();
          }
        });
      }
    }
  }

  publishJob(job: JobResponse) {
    const publishedStatus = this.lookupsService.jobStatus().find(s => 
      s.backendName === this.jobStatus.Published
    );
    
    if (publishedStatus) {
      this.jobService.publish(job.id, publishedStatus.id  as GUID).subscribe({
        next: () => {
          this.notificationService.success(
            this.translateService.instant('JOB_LIST_MESSAGES_JOB_PUBLISHED')
          );
          this.loadJobsWithFilters();
        }
      });
    }
  }

  closeJob(job: JobResponse) {
    const closedStatus = this.lookupsService.jobStatus().find(s => 
      s.backendName === this.jobStatus.Closed
    );
    
    if (closedStatus) {
      if (confirm(this.translateService.instant('JOB_LIST_CONFIRMATIONS_CLOSE_JOB'))) {
        this.jobService.close(job.id, closedStatus.id  as GUID).subscribe({
          next: () => {
            this.notificationService.success(
              this.translateService.instant('JOB_LIST_MESSAGES_JOB_CLOSED')
            );
            this.loadJobsWithFilters();
          }
        });
      }
    }
  }

  // reopenJob(job: JobResponse) {
  //   const draftStatus = this.lookupsService.jobStatus().find(s => 
  //     s.backendName === this.jobStatus.Draft
  //   );
    
  //   if (draftStatus) {
  //     this.jobService.changeStatus(job.id, draftStatus.id  as GUID).subscribe({
  //       next: () => {
  //         this.notificationService.success(
  //           this.translateService.instant('JOB_LIST_MESSAGES_JOB_REOPENED')
  //         );
  //         this.loadJobsWithFilters();
  //       },
  //       error: (error) => {
  //         this.notificationService.error(
  //           this.translateService.instant('JOB_LIST_ERRORS_REOPEN_FAILED')
  //         );
  //       }
  //     });
  //   }
  // }

  cancelJob(job: JobResponse) {
    const cancelledStatus = this.lookupsService.jobStatus().find(s => 
      s.backendName === this.jobStatus.Cancelled
    );
    
    if (cancelledStatus) {
      if (confirm(this.translateService.instant('JOB_LIST_CONFIRMATIONS_CANCEL_JOB'))) {
        this.jobService.cancel(job.id, cancelledStatus.id as GUID).subscribe({
          next: () => {
            this.notificationService.success(
              this.translateService.instant('JOB_LIST_MESSAGES_JOB_CANCELLED')
            );
            this.loadJobsWithFilters();
          }
        });
      }
    }
  }

  deleteJob(job: JobResponse) {
    if (confirm(this.translateService.instant('JOB_LIST_CONFIRMATIONS_DELETE_JOB'))) {
      this.jobService.delete(job.id).subscribe({
        next: () => {
          this.notificationService.success(
            this.translateService.instant('JOB_LIST_MESSAGES_JOB_DELETED')
          );
          this.loadJobsWithFilters();
        }
      });
    }
  }

  

  getStatusBadgeClass(statusName: string): string {
    const STATUS_BADGE_MAP: Record<string, string> = {
  [JobStatus.Draft]: 'bg-secondary',
  [JobStatus.PendingApproval]: 'bg-warning text-dark',
  [JobStatus.Approved]: 'bg-success',
  [JobStatus.Published]: 'bg-info',
  [JobStatus.Closed]: 'bg-dark',
  [JobStatus.Rejected]: 'bg-danger',
  [JobStatus.Cancelled]: 'bg-secondary'
};
   return STATUS_BADGE_MAP[statusName] || 'bg-light text-dark';
  }

  getJobCategoryBadgeClass(categoryName: string): string {
    switch(categoryName?.toLowerCase()) {
      case 'academic':
        return 'bg-primary';
      case 'administrative':
        return 'bg-info';
      case 'labor':
        return 'bg-warning text-dark';
      default:
        return 'bg-secondary';
    }
  }
}