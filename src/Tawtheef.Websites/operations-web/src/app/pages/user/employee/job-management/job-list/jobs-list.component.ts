import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { MenuItem } from 'primeng/api';
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

export interface JobAction {
  label: string;
  icon: string;
  command: () => void;
  tooltip?: string;
  danger?: boolean;
}

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
  pendingPointConfigurationCount = 0;
  pendingPointApprovalCount = 0;
  draftCount = 0;
  currentPage = signal(1);
  itemsPerPage = signal(10);

  searchQuery = signal<string>('');
  filterType = signal<GUID | null>(null);
  filterStatus = signal<GUID | null>(null);
  filterGender = signal<GUID | null>(null);
  filterManagement = signal<GUID | null>(null);
  filterSector = signal<GUID | null>(null);
  filterDepartment = signal<GUID | null>(null);

  readonly jobStatus = JobStatus;
  private searchChanges = new Subject<string>();
  protected readonly Permissions = Permissions;
  activeActions: MenuItem[] = [];
  showMoreFilters = signal(false);

  activeFiltersCount = computed(() => {
    let count = 0;
    if (this.filterGender()) count++;
    if (this.filterSector()) count++;
    if (this.filterManagement()) count++;
    if (this.filterDepartment()) count++;
    return count;
  });

  ngOnInit(): void {
    this.setupSearchListener();
    this.loadJobsWithFilters();
    this.lookupsService.loadJobCategories().subscribe();
    this.lookupsService.loadGenders().subscribe();
    this.lookupsService.loadAll(); // Load all lookups for filters

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
      genderId: this.filterGender() || undefined,
      managementId: this.filterManagement() || undefined,
      sectorId: this.filterSector() || undefined,
      departmentId: this.filterDepartment() || undefined,
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

  onSectorChange(sectorId: GUID | null) {
    this.filterSector.set(sectorId);
    this.filterManagement.set(null);
    this.filterDepartment.set(null);
    if (sectorId) {
      this.lookupsService.loadManagementsBySector(sectorId);
    } else {
      this.lookupsService.resetManagements();
      this.lookupsService.resetDepartments();
    }
    this.onFilterChange();
  }

  onManagementChange(managementId: GUID | null) {
    this.filterManagement.set(managementId);
    this.filterDepartment.set(null);
    if (managementId) {
      this.lookupsService.loadDepartmentsByManagement(managementId);
    } else {
      this.lookupsService.resetDepartments();
    }
    this.onFilterChange();
  }

  refresh() {
    this.loadJobsWithFilters();
    this.loadStats();
  }

  clearFilters() {
    this.searchQuery.set('');
    this.filterType.set(null);
    this.filterStatus.set(null);
    this.filterGender.set(null);
    this.filterManagement.set(null);
    this.filterSector.set(null);
    this.filterDepartment.set(null);

    this.lookupsService.resetManagements();
    this.lookupsService.resetDepartments();

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
      this.jobStatus.PendingPointConfiguration,
      this.jobStatus.PendingPointApproval,
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
      [JobStatus.NeedUpdate]: 'pill warning',
      [JobStatus.PendingApproval]: 'pill warning',
      [JobStatus.PendingPointConfiguration]: 'pill info',
      [JobStatus.PendingPointApproval]: 'pill warning',
      [JobStatus.Published]: 'pill info',
      [JobStatus.Closed]: 'pill danger',
      [JobStatus.Rejected]: 'pill danger',
      [JobStatus.Cancelled]: 'pill secondary',
      [JobStatus.ReadyForAnnouncement]: 'pill info',
      [JobStatus.Active]: 'pill success',
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

  getGenderBadgeClass(genderName: string): string {
    switch (genderName?.toLowerCase()) {
      case 'male':
      case 'ذكر':
        return 'pill info';
      case 'female':
      case 'أنثى':
        return 'pill danger';
      default:
        return 'pill secondary';
    }
  }

  get currentLang(): string {
    return this.translateService.currentLang || 'ar';
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
    this.loadCount(JobStatus.PendingPointConfiguration, (v) => (this.pendingPointConfigurationCount = v));
    this.loadCount(JobStatus.PendingPointApproval, (v) => (this.pendingPointApprovalCount = v));
    this.loadCount(JobStatus.Draft, (v) => (this.draftCount = v));
  }

  private loadCount(status: JobStatus, setter: (v: number) => void): void {
    const statusId = this.lookupsService.getStatusIdByEnum(status);
    if (!statusId) return;

    this.jobService.GetJobsCountByStatus(statusId).subscribe(setter);
  }

  getJobActions(job: JobResponse): JobAction[] {
    const actions: JobAction[] = [];

    // 1. Point config
    if (job.jobStatus?.backendName === this.jobStatus.PendingPointConfiguration && this.canManageJobPoints()) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_POINTS_CONFIG',
        icon: 'hgi hgi-stroke hgi-solar-system',
        command: () => this.openPointsModal(job),
      });
    }

    // Point approval (view for now)
    if (job.jobStatus?.backendName === this.jobStatus.PendingPointApproval && this.canApproveJobs()) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_POINTS_REVIEW',
        icon: 'hgi hgi-stroke hgi-checkmark-badge-03',
        command: () => this.openPointsModal(job),
      });
    }

    // 2. Point view
    if (
      (job.jobStatus?.backendName === this.jobStatus.Published ||
        job.jobStatus?.backendName === this.jobStatus.ReadyForAnnouncement) &&
      this.canViewJobPoints()) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_POINTS_VIEW',
        icon: 'hgi hgi-stroke hgi-solar-system-01',
        command: () => this.openPointsModal(job, true),
      });
    }

    // 3. Copy & View
    const backendName = job.jobStatus?.backendName;
    if (
      backendName !== this.jobStatus.Draft &&
      backendName !== this.jobStatus.NeedUpdate &&
      this.canViewJobs()
    ) {
      if (this.canCopyJob(job)) {
        actions.push({
          label: 'JOB_LIST_BUTTONS_COPY',
          icon: 'hgi hgi-stroke hgi-copy-01',
          command: () => this.copyJob(job),
        });
      }
      actions.push({
        label: 'JOB_LIST_BUTTONS_VIEW',
        icon: 'hgi hgi-stroke hgi-view',
        command: () => this.viewJob(job),
      });
    }

    // 4. Edit
    if (
      (backendName === this.jobStatus.Draft || backendName === this.jobStatus.NeedUpdate) &&
      this.canManageJobs()
    ) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_EDIT',
        icon: 'hgi hgi-stroke hgi-pencil-edit-02',
        command: () => this.editJob(job),
      });
    }

    // 5. Approve / Reject
    if (backendName === this.jobStatus.PendingApproval && this.canApproveJobs()) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_APPROVE',
        icon: 'hgi hgi-stroke hgi-tick-02',
        command: () => this.approveJob(job),
      });
      actions.push({
        label: 'JOB_LIST_BUTTONS_REJECT',
        icon: 'hgi hgi-stroke hgi-cancel-01',
        command: () => this.rejectJob(job),
      });
    }

    // 6. Publish
    if (backendName === this.jobStatus.ReadyForAnnouncement && this.canApproveJobs()) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_PUBLISH',
        icon: 'hgi hgi-stroke hgi-upload-01',
        command: () => this.publishJob(job),
      });
    }

    // 7. Close
    if (backendName === this.jobStatus.Published && this.canApproveJobs()) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_CLOSE',
        icon: 'hgi hgi-stroke hgi-square-lock-01',
        command: () => this.closeJob(job),
      });
    }

    // 8. Cancel
    if (
      (backendName === this.jobStatus.Published ||
        backendName === this.jobStatus.Draft ||
        backendName === this.jobStatus.ReadyForAnnouncement) &&
      this.canManageJobs()
    ) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_CANCEL',
        icon: 'hgi hgi-stroke hgi-unavailable',
        command: () => this.cancelJob(job),
      });
    }

    // 9. Reopen
    if (backendName === this.jobStatus.Rejected && this.canApproveJobs()) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_REOPEN',
        icon: 'hgi hgi-stroke hgi-checkmark-circle-02',
        command: () => this.reopenJob(job),
      });
    }

    // 10. Delete
    if (
      backendName !== this.jobStatus.Published &&
      backendName !== this.jobStatus.PendingPointConfiguration &&
      backendName !== this.jobStatus.PendingPointApproval &&
      this.canManageJobs()
    ) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_DELETE',
        icon: 'hgi hgi-stroke hgi-delete-02',
        command: () => this.deleteJob(job),
        danger: true,
      });
    }

    // 11. View Candidates
    if (backendName === this.jobStatus.Published && this.canViewJobs()) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_VIEW_CANDIDATE',
        icon: 'hgi hgi-stroke hgi-user-multiple',
        command: () => this.viewJobCandidate(job.id),
      });
    }

    return actions;
  }

  showMoreActions(event: Event, actions: JobAction[], menu: any) {
    this.activeActions = actions.map((a) => ({
      label: this.translateService.instant(a.label),
      icon: a.icon,
      command: () => a.command(),
      styleClass: a.danger ? 'text-danger' : '',
    }));
    menu.toggle(event);
  }
}
