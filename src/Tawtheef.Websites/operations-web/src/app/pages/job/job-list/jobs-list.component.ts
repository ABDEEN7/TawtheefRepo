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
import { take } from 'rxjs';
import { DialogHelperService } from '../../../core/services/dialog-helper.service';
import {AuthService} from '../../../core/auth/auth.service';
import {Permissions} from '../../../core/constants/permissions';

@Component({
  selector: 'app-job-list',
  standalone: false,
  templateUrl: './jobs-list.component.html',
  styleUrls: ['./jobs-list.component.scss'],
})
export class JobListComponent implements OnInit {
  private jobService = inject(JobService);
  private router = inject(Router);
  private notificationService = inject(NotificationService);
  private translateService = inject(TranslateService);
  private dialogHelperService = inject(DialogHelperService);
  private authService = inject(AuthService);
  lookupsService = inject(JobLookupService);

  jobs: PaginatedResult<JobResponse> | undefined;
  paginationMetadata: PaginationMetadata | undefined;

  cancelledCount = 0;
  pendingApprovalCount = 0;
  approvedCount = 0;
  draftCount = 0;
  currentPage = signal(1);
  itemsPerPage = 10;

  searchQuery = signal<string>('');
  filterType = signal<GUID | null>(null);
  filterStatus = signal<GUID | null>(null);

readonly jobStatus = JobStatus;

  ngOnInit(): void {
    this.loadJobsWithFilters();
    this.lookupsService.loadJobCategories();

    this.lookupsService
      .loadJobStatus()
      .pipe(take(1))
      .subscribe(() => this.loadStats());
  }

  loadJobsWithFilters() {
    const pagination: PaginatedRequest = {
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage,
      sortBy: 'createdDate',
      sortDirection: 'desc',
    };

    const filter: JobQueryFilter = {
      searchTerm: this.searchQuery() || undefined,
      jobCategoryId: this.filterType() || undefined,
      statusId: this.filterStatus() || undefined,
    };

    this.jobService.getAll(pagination, filter).subscribe({
      next: (paginatedData) => {
        this.jobs = paginatedData;
        this.paginationMetadata = paginatedData.metadata;
      },
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

  canManageJobs(): boolean {
    return this.authService.hasPermission(Permissions.Jobs.Manage);
  }

  canApproveJobs(): boolean {
    return this.authService.hasPermission(Permissions.Jobs.Approve);
  }

  canViewJobs(): boolean {
    return this.authService.hasPermission(Permissions.Jobs.View);
  }

  canManageJobPoints(): boolean {
    return this.authService.hasPermission(Permissions.Jobs.PointsManage);
  }

  editJob(job: JobResponse) {
    if (!this.canManageJobs()) return;
    this.router.navigate([routes.employee.jobEdit, job.id]).then();
  }

  viewJob(job: JobResponse) {
    if (!this.canViewJobs()) return;
    this.router.navigate([routes.employee.jobView, job.id]).then();
  }

  createNewJob() {
    if (!this.canManageJobs()) return;
    this.router.navigate([routes.employee.jobCreate]).then();
  }

  openPointsModal(job: JobResponse) {
    if (!this.canManageJobPoints()) return;
    this.router.navigate([routes.employee.jobPoints,job.id]).then();
  }

  approveJob(job: JobResponse) {
    if (!this.canApproveJobs()) return;
    this.router.navigate([routes.employee.approvalJob, job.id]).then();
  }

  rejectJob(job: JobResponse) {
    if (!this.canApproveJobs()) return;
    const rejectedStatus = this.lookupsService
      .jobStatus()
      .find((s) => s.backendName === this.jobStatus.Rejected);

  if (rejectedStatus) {
    const ref = this.dialogHelperService.openConfirmDialog({
      type: 'submit',
      title: 'JOB_LIST_CONFIRMATIONS_REJECT_JOB',
      description: 'JOB_LIST_CONFIRMATIONS_REJECT_JOB_NOTE',
      cancelText: 'common.cancel',
      confirmText: 'common.confirm',
    });

    ref?.onClose.subscribe((result) => {
      if (!result) return;
      this.jobService.changeStatus(job.id, rejectedStatus.id as GUID).subscribe({
        next: () => {
          this.notificationService.success(
            this.translateService.instant('JOB_LIST_MESSAGES_JOB_REJECTED')
          );
          this.loadJobsWithFilters();
        },
      });
    });
  }
}

  publishJob(job: JobResponse) {
    if (!this.canApproveJobs()) return;
    const publishedStatus = this.lookupsService
      .jobStatus()
      .find((s) => s.backendName === this.jobStatus.Published);

    if (publishedStatus) {
      this.jobService.changeStatus(job.id, publishedStatus.id as GUID).subscribe({
        next: () => {
          this.notificationService.success(
            this.translateService.instant('JOB_LIST_MESSAGES_JOB_PUBLISHED')
          );
          this.loadJobsWithFilters();
        },
      });
    }
  }

  closeJob(job: JobResponse) {
    if (!this.canApproveJobs()) return;
    const closedStatus = this.lookupsService
      .jobStatus()
      .find((s) => s.backendName === this.jobStatus.Closed);

  if (closedStatus) {
    const ref = this.dialogHelperService.openConfirmDialog({
      type: 'submit',
      title: 'JOB_LIST_CONFIRMATIONS_CLOSE_JOB',
      description: 'JOB_LIST_CONFIRMATIONS_CLOSE_JOB_NOTE',
      cancelText: 'common.cancel',
      confirmText: 'common.confirm',
    });

    ref?.onClose.subscribe((result) => {
      if (!result) return;
      this.jobService.changeStatus(job.id, closedStatus.id as GUID).subscribe({
        next: () => {
          this.notificationService.success(
            this.translateService.instant('JOB_LIST_MESSAGES_JOB_CLOSED')
          );
          this.loadJobsWithFilters();
        },
      });
    });
  }
}

  reopenJob(job: JobResponse) {
    const draftStatus = this.lookupsService.jobStatus().find(s =>
      s.backendName === this.jobStatus.Draft
    );

    if (draftStatus) {
      const ref = this.dialogHelperService.openConfirmDialog({
      type: 'submit',
      title: 'JOB_LIST_CONFIRMATIONS_REOPEN_JOB',
      description: 'JOB_LIST_CONFIRMATIONS_REOPEN_JOB_NOTE',
      cancelText: 'common.cancel',
      confirmText: 'common.confirm',
    });

    ref?.onClose.subscribe((result) => {
      if (!result) return;
      this.jobService.changeStatus(job.id, draftStatus.id as GUID).subscribe({
        next: () => {
          this.notificationService.success(
            this.translateService.instant('JOB_LIST_MESSAGES_JOB_REOPENED')
          );
          this.loadJobsWithFilters();
        },
      });
    });
    }
  }

  cancelJob(job: JobResponse) {
    if (!this.canManageJobs()) return;
    const cancelledStatus = this.lookupsService
      .jobStatus()
      .find((s) => s.backendName === this.jobStatus.Cancelled);

  if (cancelledStatus) {
    const ref = this.dialogHelperService.openConfirmDialog({
      type: 'submit',
      title: 'JOB_LIST_CONFIRMATIONS_CANCEL_JOB',
      description: 'JOB_LIST_CONFIRMATIONS_CANCEL_JOB_NOTE',
      cancelText: 'common.cancel',
      confirmText: 'common.confirm',
    });

    ref?.onClose.subscribe((result) => {
      if (!result) return;
      this.jobService.changeStatus(job.id, cancelledStatus.id as GUID).subscribe({
        next: () => {
          this.notificationService.success(
            this.translateService.instant('JOB_LIST_MESSAGES_JOB_CANCELLED')
          );
          this.loadJobsWithFilters();
        },
      });
    });
  }
}

  deleteJob(job: JobResponse) {
    if (!this.canManageJobs()) return;
  const ref = this.dialogHelperService.openConfirmDialog({
    type: 'submit',
    title: 'JOB_LIST_CONFIRMATIONS_DELETE_JOB',
    description: 'JOB_LIST_CONFIRMATIONS_DELETE_JOB_NOTE',
    cancelText: 'common.cancel',
    confirmText: 'common.confirm',
  });

  ref?.onClose.subscribe((result) => {
    if (!result) return;
    this.jobService.delete(job.id).subscribe({
      next: () => {
        this.notificationService.success(
          this.translateService.instant('JOB_LIST_MESSAGES_JOB_DELETED')
        );
        this.loadJobsWithFilters();
      },
    });
  });
}

  getStatusBadgeClass(statusName: string): string {
    const STATUS_BADGE_MAP: Record<string, string> = {
      [JobStatus.Draft]: 'bg-secondary',
      [JobStatus.PendingApproval]: 'bg-warning text-dark',
      [JobStatus.Approved]: 'bg-success',
      [JobStatus.Published]: 'bg-info',
      [JobStatus.Closed]: 'bg-dark',
      [JobStatus.Rejected]: 'bg-danger',
      [JobStatus.Cancelled]: 'bg-secondary',
      [JobStatus.ReadyForAnnouncement]: 'bg-warning text-dark',
    };
    return STATUS_BADGE_MAP[statusName] || 'bg-light text-dark';
  }

  getJobCategoryBadgeClass(categoryName: string): string {
    switch (categoryName?.toLowerCase()) {
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

  private loadStats(): void {
    this.loadCount(JobStatus.Cancelled, (v) => (this.cancelledCount = v));
    this.loadCount(JobStatus.PendingApproval, (v) => (this.pendingApprovalCount = v));
    this.loadCount(JobStatus.Approved, (v) => (this.approvedCount = v));
    this.loadCount(JobStatus.Draft, (v) => (this.draftCount = v));
  }

  private loadCount(status: JobStatus, setter: (v: number) => void): void {
    const statusId = this.lookupsService.getStatusIdByEnum(status);
    if (!statusId) return;

    this.jobService.GetJobsCountByStatus(statusId).subscribe(setter);
  }
}
