import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { JobService } from '../services/job.service';
import { JobLookupService } from '../services/job-lookup.service';
import { JobQueryFilter } from '../models/job-query-filter.model';
import { JobResponse } from '../models/job-response-model';
import { GUID } from '../../../../../shared/types/guid.type';
import { TranslateService } from '@ngx-translate/core';
import { debounceTime, distinctUntilChanged, Subject, take } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NotificationService } from '../../../../../core/services/notification.service';
import { DialogHelperService } from '../../../../../core/services/dialog-helper.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { AuthService } from '../../../../../core/auth/auth.service';
import { PaginationMetadata } from '../../../../../core/models/pagination-metadata.model';
import { PaginatedRequest } from '../../../../../core/models/paginated-request.model';
import { JobStatus } from '../../../../../core/enums/lookups.enum';
import { routes } from '../../../../../routes/routes';
import { Permissions } from '../../../../../core/constants/permissions';

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
  private destroyRef = inject(DestroyRef);
  lookupsService = inject(JobLookupService);

  jobs: PaginatedResult<JobResponse> | undefined;
  paginationMetadata: PaginationMetadata | undefined;

  cancelledCount = 0;
  pendingApprovalCount = 0;
  approvedCount = 0;
  draftCount = 0;
  currentPage = signal(1);
  itemsPerPage = signal(10);

  searchQuery = signal<string>('');
  filterType = signal<GUID | null>(null);
  filterStatus = signal<GUID | null>(null);

  readonly jobStatus = JobStatus;
  private searchChanges = new Subject<string>();
  protected readonly Permissions = Permissions;

  ngOnInit(): void {
    this.setupSearchListener();
    this.loadJobsWithFilters();
    this.lookupsService.loadJobCategories().subscribe();

    this.lookupsService
      .loadJobStatus()
      .pipe(take(1))
      .subscribe(() => this.loadStats());
  }

  loadJobsWithFilters() {
    const pagination: PaginatedRequest = {
      pageNumber: this.currentPage(),
      pageSize: this.itemsPerPage(),
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


  onSearchInputChange(value: string) {
    this.searchQuery.set(value ?? '');
    this.searchChanges.next(value ?? '');
  }

  private setupSearchListener() {
    this.searchChanges
      .pipe(debounceTime(1000), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.onFilterChange());
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

  onPageSizeChange(size: number) {
    this.itemsPerPage.set(size);
    this.currentPage.set(1);
    this.loadJobsWithFilters();
  }

  canManageJobs(): boolean {
    return this.authService.hasPermission(Permissions.Jobs.Manage);
  }

  canApproveJobs(): boolean {
    return this.authService.hasPermission([Permissions.Jobs.Manage, Permissions.Jobs.Approve]);
  }

  canViewJobs(): boolean {
    return this.authService.hasPermission([Permissions.Jobs.Manage, Permissions.Jobs.View]);
  }

  canManageJobPoints(): boolean {
    return this.authService.hasPermission([Permissions.Jobs.Manage, Permissions.JobPoints.Manage]);
  }

  canViewJobPoints(): boolean {
    return this.authService.hasPermission([Permissions.Jobs.Manage, Permissions.JobPoints.View]);
  }

  editJob(job: JobResponse) {
    if (!this.canManageJobs()) return;
    this.router.navigate([routes.portal.jobEdit(job.id)]).then();
  }

  viewJob(job: JobResponse) {
    if (!this.canViewJobs()) return;
    this.router.navigate([routes.portal.jobView(job.id)]).then();
  }

  createNewJob() {
    if (!this.canManageJobs()) return;
    this.router.navigate([routes.portal.jobCreate]).then();
  }

  openPointsModal(job: JobResponse, isReadOnly = false) {
    if (!isReadOnly && !this.canManageJobPoints()) return;
    if (isReadOnly && !this.canViewJobPoints()) return;

    this.router
      .navigate([routes.portal.jobPoints(job.id)], {
        queryParams: isReadOnly ? { mode: 'view' } : undefined,
      })
      .then();
  }

  canCopyJob(job: JobResponse): boolean {
    const allowedStatuses = [
      this.jobStatus.Approved,
      this.jobStatus.ReadyForAnnouncement,
      this.jobStatus.Published,
      this.jobStatus.Closed,
      this.jobStatus.Cancelled,
    ];

    return allowedStatuses.includes(job.jobStatus?.backendName as JobStatus);
  }

  copyJob(job: JobResponse) {
    if (!this.canManageJobs() || !this.canCopyJob(job)) return;
    this.router.navigate([routes.portal.jobCreate], {
      queryParams: { copyFrom: job.id },
    }).then();
  }

  approveJob(job: JobResponse) {
    if (!this.canApproveJobs()) return;
    this.router.navigate([routes.portal.approvalJob(job.id)]).then();
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
      [JobStatus.Draft]: 'pill neutral',
      [JobStatus.PendingApproval]: 'pill warning',
      [JobStatus.Approved]: 'pill success',
      [JobStatus.Published]: 'pill info',
      [JobStatus.Closed]: 'pill danger',
      [JobStatus.Rejected]: 'pill danger',
      [JobStatus.Cancelled]: 'pill secondary',
      [JobStatus.ReadyForAnnouncement]: 'pill info',
    };
    return STATUS_BADGE_MAP[statusName] || 'pill neutral';
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

  sendInvitation() {
    if (!this.canManageJobs()) return;
  }

  viewJobCandidate(jobId: GUID) {
    if (!this.canViewJobs()) return;
    const url = routes.portal.jobCandidates(jobId);
    this.router.navigate([url]);
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
