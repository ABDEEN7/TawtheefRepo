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
  private readonly jobService = inject(JobService);
  private readonly router = inject(Router);
  private readonly notificationService = inject(NotificationService);
  private readonly translateService = inject(TranslateService);
  private readonly dialogHelperService = inject(DialogHelperService);
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly lookupService = inject(JobLookupService);

  // --- State Signals ---
  readonly jobs = signal<PaginatedResult<JobResponse> | undefined>(undefined);
  readonly isLoading = signal(false);
  readonly currentPage = signal(1);
  readonly itemsPerPage = signal(10);
  readonly showMoreFilters = signal(false);

  // --- Filter Signals ---
  readonly searchQuery = signal<string>('');
  readonly filterType = signal<GUID | null>(null);
  readonly filterStatus = signal<GUID | null>(null);
  readonly filterGender = signal<GUID | null>(null);
  readonly filterManagement = signal<GUID | null>(null);
  readonly filterSector = signal<GUID | null>(null);
  readonly filterDepartment = signal<GUID | null>(null);

  // --- Statistics Signal ---
  readonly stats = signal({
    cancelled: 0,
    pendingApproval: 0,
    pendingPointConfiguration: 0,
    pendingPointApproval: 0,
    draft: 0
  });

  // --- Computed Permissions ---
  readonly canManage = computed(() => this.authService.hasPermission(Permissions.Jobs.Manage));
  readonly canApprove = computed(() => this.authService.hasPermission(Permissions.Jobs.Approve));
  readonly canView = computed(() => this.authService.hasPermission(Permissions.Jobs.View) || this.canManage());
  readonly canInvite = computed(() => this.authService.hasPermission(Permissions.Jobs.SendInvitation));
  readonly canManagePoints = computed(() => this.authService.hasPermission(Permissions.JobPoints.Manage));
  readonly canViewPoints = computed(() => this.authService.hasPermission(Permissions.JobPoints.View));
  readonly canApprovePoints = computed(() => this.authService.hasPermission(Permissions.JobPoints.Approve));

  readonly activeFiltersCount = computed(() => {
    let count = 0;
    if (this.filterGender()) count++;
    if (this.filterSector()) count++;
    if (this.filterManagement()) count++;
    if (this.filterDepartment()) count++;
    return count;
  });

  // --- Internals ---
  private readonly searchChanges = new Subject<string>();
  protected readonly JobStatusEnum = JobStatus;
  activeActions: MenuItem[] = [];

  ngOnInit(): void {
    this.setupSearchListener();
    this.initializeLookups();
    this.loadData();
  }

  private initializeLookups(): void {
    // Fire and forget lookups that don't block initial load
    this.lookupService.loadJobCategories().subscribe();
    this.lookupService.loadGenders().subscribe();
    this.lookupService.loadAll();

    // Stats depend on status lookups being loaded
    this.lookupService.loadJobStatus()
      .pipe(take(1), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.loadStats());
  }

  loadData(): void {
    this.isLoading.set(true);

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

    this.jobService.getAll(pagination, filter)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data) => {
          this.jobs.set(data);
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      });
  }

  private setupSearchListener(): void {
    this.searchChanges
      .pipe(
        debounceTime(800),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(() => this.onFilterChange());
  }

  onSearchChange(value: string): void {
    this.searchQuery.set(value);
    this.searchChanges.next(value);
  }

  onFilterChange(): void {
    this.currentPage.set(1);
    this.loadData();
  }

  onSectorChange(sectorId: GUID | null): void {
    this.filterSector.set(sectorId);
    this.filterManagement.set(null);
    this.filterDepartment.set(null);

    if (sectorId) {
      this.lookupService.loadManagementsBySector(sectorId);
    } else {
      this.lookupService.resetManagements();
      this.lookupService.resetDepartments();
    }
    this.onFilterChange();
  }

  onManagementChange(managementId: GUID | null): void {
    this.filterManagement.set(managementId);
    this.filterDepartment.set(null);

    if (managementId) {
      this.lookupService.loadDepartmentsByManagement(managementId);
    } else {
      this.lookupService.resetDepartments();
    }
    this.onFilterChange();
  }

  clearFilters(): void {
    this.searchQuery.set('');
    this.filterType.set(null);
    this.filterStatus.set(null);
    this.filterGender.set(null);
    this.filterManagement.set(null);
    this.filterSector.set(null);
    this.filterDepartment.set(null);

    this.lookupService.resetManagements();
    this.lookupService.resetDepartments();
    this.onFilterChange();
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.loadData();
  }

  onPageSizeChange(size: number): void {
    this.itemsPerPage.set(size);
    this.currentPage.set(1);
    this.loadData();
  }

  refresh(): void {
    this.loadData();
    this.loadStats();
  }

  // --- Job Actions ---
  createNewJob(): void {
    this.router.navigate([routes.portal.jobCreate]);
  }

  viewJob(id: GUID): void {
    this.router.navigate([routes.portal.jobView(id)]);
  }

  editJob(id: GUID): void {
    this.router.navigate([routes.portal.jobEdit(id)]);
  }

  copyJob(id: GUID): void {
    this.router.navigate([routes.portal.jobCreate], {
      queryParams: { copyFrom: id }
    });
  }

  approveJob(id: GUID): void {
    this.router.navigate([routes.portal.approvalJob(id)]);
  }

  openPoints(id: GUID, mode: 'view' | 'edit' | 'approve' = 'edit'): void {
    this.router.navigate([routes.portal.jobPoints(id)], {
      queryParams: mode !== 'edit' ? { mode } : undefined
    });
  }

  changeStatus(jobId: GUID, statusEnum: JobStatus, successMsg: string, confirmOptions?: any): void {
    const status = this.lookupService.jobStatus().find(s => s.backendName === statusEnum);
    if (!status) return;

    const executeChange = () => {
      this.jobService.changeStatus(jobId, status.id as GUID).subscribe({
        next: () => {
          this.notificationService.success(this.translateService.instant(successMsg));
          this.refresh();
        }
      });
    };

    if (confirmOptions) {
      this.dialogHelperService.openConfirmDialog({
        type: 'submit',
        cancelText: 'common.cancel',
        confirmText: 'common.confirm',
        ...confirmOptions
      })?.onClose.subscribe(result => result && executeChange());
    } else {
      executeChange();
    }
  }

  deleteJob(id: GUID): void {
    this.dialogHelperService.openConfirmDialog({
      type: 'submit',
      title: 'JOB_LIST_CONFIRMATIONS_DELETE_JOB',
      description: 'JOB_LIST_CONFIRMATIONS_DELETE_JOB_NOTE',
      cancelText: 'common.cancel',
      confirmText: 'common.confirm',
    })?.onClose.subscribe(result => {
      if (result) {
        this.jobService.delete(id).subscribe({
          next: () => {
            this.notificationService.success(this.translateService.instant('JOB_LIST_MESSAGES_JOB_DELETED'));
            this.refresh();
          }
        });
      }
    });
  }

  getJobActions(job: JobResponse): JobAction[] {
    const actions: JobAction[] = [];
    const status = job.jobStatus?.backendName as JobStatus;

    // Cache permissions for speed
    const canManage = this.canManage();
    const canApprove = this.canApprove();
    const canView = this.canView();
    const canInvite = this.canInvite();

    // 1. Basic View/Edit
    if (canView && status !== JobStatus.Draft) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_VIEW',
        icon: 'hgi hgi-stroke hgi-view',
        command: () => this.viewJob(job.id)
      });
    }

    if (canManage && (status === JobStatus.Draft || status === JobStatus.NeedUpdate)) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_EDIT',
        icon: 'hgi hgi-stroke hgi-pencil-edit-02',
        command: () => this.editJob(job.id)
      });
    }

    // 2. Approval Logic
    if (canApprove && status === JobStatus.PendingApproval) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_APPROVE',
        icon: 'hgi hgi-stroke hgi-tick-02',
        command: () => this.approveJob(job.id)
      });
      actions.push({
        label: 'JOB_LIST_BUTTONS_REJECT',
        icon: 'hgi hgi-stroke hgi-cancel-01',
        command: () => this.changeStatus(job.id, JobStatus.Rejected, 'JOB_LIST_MESSAGES_JOB_REJECTED', {
          title: 'JOB_LIST_CONFIRMATIONS_REJECT_JOB',
          description: 'JOB_LIST_CONFIRMATIONS_REJECT_JOB_NOTE'
        })
      });
    }

    // 3. Points Management
    if (status === JobStatus.PendingPointConfiguration && this.canManagePoints()) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_POINTS_CONFIG',
        icon: 'hgi hgi-stroke hgi-solar-system',
        command: () => this.openPoints(job.id, 'edit')
      });
    }

    if (status === JobStatus.PendingPointApproval && (this.canApprovePoints() || canApprove)) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_POINTS_REVIEW',
        icon: 'hgi hgi-stroke hgi-checkmark-badge-03',
        command: () => this.openPoints(job.id, 'approve')
      });
    }

    if (this.canViewPoints() && (status === JobStatus.Published || status === JobStatus.ReadyForAnnouncement)) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_POINTS_VIEW',
        icon: 'hgi hgi-stroke hgi-solar-system-01',
        command: () => this.openPoints(job.id, 'view')
      });
    }

    // 4. Lifecycle Actions
    if (canApprove && status === JobStatus.ReadyForAnnouncement) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_PUBLISH',
        icon: 'hgi hgi-stroke hgi-upload-01',
        command: () => this.changeStatus(job.id, JobStatus.Published, 'JOB_LIST_MESSAGES_JOB_PUBLISHED')
      });
    }

    if (canInvite && status === JobStatus.Published) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_VIEW_CANDIDATE',
        icon: 'hgi hgi-stroke hgi-user-multiple',
        command: () => this.router.navigate([routes.portal.jobCandidates(job.id)])
      });
    }

    if (canApprove && status === JobStatus.Published) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_CLOSE',
        icon: 'hgi hgi-stroke hgi-square-lock-01',
        command: () => this.changeStatus(job.id, JobStatus.Closed, 'JOB_LIST_MESSAGES_JOB_CLOSED', {
          title: 'JOB_LIST_CONFIRMATIONS_CLOSE_JOB',
          description: 'JOB_LIST_CONFIRMATIONS_CLOSE_JOB_NOTE'
        })
      });
    }

    if (canManage && (status === JobStatus.Published || status === JobStatus.Draft || status === JobStatus.ReadyForAnnouncement)) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_CANCEL',
        icon: 'hgi hgi-stroke hgi-unavailable',
        command: () => this.changeStatus(job.id, JobStatus.Cancelled, 'JOB_LIST_MESSAGES_JOB_CANCELLED', {
          title: 'JOB_LIST_CONFIRMATIONS_CANCEL_JOB',
          description: 'JOB_LIST_CONFIRMATIONS_CANCEL_JOB_NOTE'
        })
      });
    }

    if (canApprove && status === JobStatus.Rejected) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_REOPEN',
        icon: 'hgi hgi-stroke hgi-checkmark-circle-02',
        command: () => this.changeStatus(job.id, JobStatus.Draft, 'JOB_LIST_MESSAGES_JOB_REOPENED', {
          title: 'JOB_LIST_CONFIRMATIONS_REOPEN_JOB',
          description: 'JOB_LIST_CONFIRMATIONS_REOPEN_JOB_NOTE'
        })
      });
    }

    // 5. Utility Actions
    if (canManage && this.isStatusCopyable(status)) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_COPY',
        icon: 'hgi hgi-stroke hgi-copy-01',
        command: () => this.copyJob(job.id)
      });
    }

    if (canManage && ![JobStatus.Published, JobStatus.PendingPointConfiguration, JobStatus.PendingPointApproval].includes(status)) {
      actions.push({
        label: 'JOB_LIST_BUTTONS_DELETE',
        icon: 'hgi hgi-stroke hgi-delete-02',
        command: () => this.deleteJob(job.id),
        danger: true
      });
    }

    return actions;
  }

  private isStatusCopyable(status: JobStatus): boolean {
    return [
      JobStatus.PendingPointConfiguration,
      JobStatus.PendingPointApproval,
      JobStatus.ReadyForAnnouncement,
      JobStatus.Published,
      JobStatus.Closed,
      JobStatus.Cancelled
    ].includes(status);
  }

  showMoreActions(event: Event, actions: JobAction[], menu: any): void {
    this.activeActions = actions.map(a => ({
      label: this.translateService.instant(a.label),
      icon: a.icon,
      command: () => a.command(),
      styleClass: a.danger ? 'text-danger' : '',
    }));
    menu.toggle(event);
  }

  getStatusBadgeClass(statusName: string): string {
    const badgeMap: Record<string, string> = {
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
    return badgeMap[statusName] || 'pill neutral';
  }

  private loadStats(): void {
    const trackedStatuses = [
      JobStatus.Cancelled,
      JobStatus.PendingApproval,
      JobStatus.PendingPointConfiguration,
      JobStatus.PendingPointApproval,
      JobStatus.Draft
    ];

    trackedStatuses.forEach(status => {
      const id = this.lookupService.getStatusIdByEnum(status);
      if (id) {
        this.jobService.GetJobsCountByStatus(id).subscribe(count => {
          this.stats.update(current => ({
            ...current,
            [this.mapStatusToStatKey(status)]: count
          }));
        });
      }
    });
  }

  private mapStatusToStatKey(status: JobStatus): string {
    const map: Partial<Record<JobStatus, string>> = {
      [JobStatus.Cancelled]: 'cancelled',
      [JobStatus.PendingApproval]: 'pendingApproval',
      [JobStatus.PendingPointConfiguration]: 'pendingPointConfiguration',
      [JobStatus.PendingPointApproval]: 'pendingPointApproval',
      [JobStatus.Draft]: 'draft'
    };
    return map[status] || '';
  }

  get currentLang(): string {
    return this.translateService.currentLang || 'ar';
  }
}
