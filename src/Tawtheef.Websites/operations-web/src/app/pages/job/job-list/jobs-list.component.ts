import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { DynamicDialogRef, DialogService } from 'primeng/dynamicdialog';
import { JobService } from '../services/job.service';
import { PointsConfigModalComponent } from '../modals/points-config-modal/points-config-modal.component';
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

@Component({
  selector: 'app-job-list',
  standalone : false,
  templateUrl: './jobs-list.component.html',
  styleUrl: './jobs-list.component.scss',
})
export class JobListComponent implements OnInit {
  private jobService = inject(JobService);
  private router = inject(Router);
  private dialogService = inject(DialogService);
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
    this.lookupsService.loadAll();
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
      },
      error: (error) => {
        this.notificationService.error(
          this.translateService.instant('JOB_LIST_ERRORS_LOAD_JOBS_FAILED')
        );
        console.error('Failed to load jobs:', error);
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
  this.router.navigate([`/jobs/edit`, job.id, 'wizard']).then();
}

  viewJob(job: JobResponse) {
    this.router.navigate([`/jobs/view`, job.id]).then();
  }

  createNewJob() {
    this.router.navigate(['/jobs/create']).then();
  }

  openPointsModal(job: JobResponse) {
    const ref: DynamicDialogRef | null = this.dialogService.open(PointsConfigModalComponent, {
      data: { jobId: job.id, jobTitle: job.titleAr },
      width: '80%',
    });

    ref?.onClose.subscribe((saved: boolean) => {
      if (saved) {
        this.loadJobsWithFilters();
      }
    });
  }

  approveJob(job: JobResponse) {
    const approvedStatus = this.lookupsService.jobStatus().find(s => 
      s.backendName === this.jobStatus.Approved
    );
    
    if (approvedStatus) {
      this.jobService.approve(job.id, approvedStatus.id  as GUID).subscribe({
        next: () => {
          this.notificationService.success(
            this.translateService.instant('JOB_LIST_MESSAGES_JOB_APPROVED')
          );
          this.loadJobsWithFilters();
        },
        error: (error) => {
          this.notificationService.error(
            this.translateService.instant('JOB_LIST_ERRORS_APPROVE_FAILED')
          );
        }
      });
    }
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
          },
          error: (error) => {
            this.notificationService.error(
              this.translateService.instant('JOB_LIST_ERRORS_REJECT_FAILED')
            );
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
        },
        error: (error) => {
          this.notificationService.error(
            this.translateService.instant('JOB_LIST_ERRORS_PUBLISH_FAILED')
          );
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
          },
          error: (error) => {
            this.notificationService.error(
              this.translateService.instant('JOB_LIST_ERRORS_CLOSE_FAILED')
            );
          }
        });
      }
    }
  }

  reopenJob(job: JobResponse) {
    const draftStatus = this.lookupsService.jobStatus().find(s => 
      s.backendName === this.jobStatus.Draft
    );
    
    if (draftStatus) {
      this.jobService.changeStatus(job.id, draftStatus.id  as GUID).subscribe({
        next: () => {
          this.notificationService.success(
            this.translateService.instant('JOB_LIST_MESSAGES_JOB_REOPENED')
          );
          this.loadJobsWithFilters();
        },
        error: (error) => {
          this.notificationService.error(
            this.translateService.instant('JOB_LIST_ERRORS_REOPEN_FAILED')
          );
        }
      });
    }
  }

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
          },
          error: (error) => {
            this.notificationService.error(
              this.translateService.instant('JOB_LIST_ERRORS_CANCEL_FAILED')
            );
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
        },
        error: (error) => {
          this.notificationService.error(
            this.translateService.instant('JOB_LIST_ERRORS_DELETE_FAILED')
          );
        }
      });
    }
  }

  getStatusBadgeClass(statusName: string): string {
    switch(statusName?.toLowerCase()) {
      case this.jobStatus.Draft:
        return 'bg-secondary';
      case this.jobStatus.PendingApproval:
        return 'bg-warning text-dark';
      case this.jobStatus.Approved:
        return 'bg-success';
      case this.jobStatus.Published:
        return 'bg-info';
      case this.jobStatus.Closed:
        return 'bg-dark';
      case this.jobStatus.Rejected:
        return 'bg-danger';
      case this.jobStatus.Cancelled:
        return 'bg-secondary';
      default:
        return 'bg-light text-dark';
    }
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