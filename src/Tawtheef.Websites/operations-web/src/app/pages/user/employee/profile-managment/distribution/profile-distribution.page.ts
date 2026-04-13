import { CommonModule } from '@angular/common';
import {
  Component,
  DestroyRef,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  Subject,
  debounceTime,
  distinctUntilChanged,
  finalize,
  map,
  switchMap,
  tap, defer,
} from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { AvatarModule } from 'primeng/avatar';
import { BadgeModule } from 'primeng/badge';
import { ProgressBarModule } from 'primeng/progressbar';
import { Select } from 'primeng/select';
import { Ripple } from 'primeng/ripple';
import { Tooltip } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';

import { ProfileDistributionService } from './services/profile-distribution.service';
import {
  AutoAssignRequest,
  DistributionEmployee,
  DistributionFile,
  DistributionProfilesFilters,
  DistributionResult,
  EmployeeAvailability,
  ManualAssignRequest,
  ReassignRequest,
} from './models/profile-distribution.models';

import { ProfileStatusNumber } from '../../../../../core/enums/lookups.enum';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { DistributionDialogResult } from './models/profile-distribution.dialogs';
import { ManualAssignDialog } from './dialogs/manual-assign-dialog/manual-assign-dialog';
import { AutoAssignDialog } from './dialogs/auto-assign-dialog/auto-assign-dialog';
import { AuthService } from '../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../core/constants/permissions';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { PaginationMetadata } from '../../../../../core/models/pagination-metadata.model';
import { PaginationComponent } from '../../../../../shared/components/pagination/pagination.component';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import { NotificationService } from '../../../../../core/services/notification.service';
import {SortEvent} from 'primeng/api';
import {ProfileApprovalListFilter} from '../approval-list/models/profile-approval.models';

@Component({
  selector: 'app-profile-distribution-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule,
    TableModule,
    InputTextModule,
    ButtonModule,
    TagModule,
    DialogModule,
    InputNumberModule,
    AvatarModule,
    BadgeModule,
    PaginationComponent,
    I18nNamespaceDirective,
    Select,
    Ripple,
    Tooltip,
    ProgressBarModule,
  ],
  providers: [DialogService],
  templateUrl: './profile-distribution.page.html',
  styleUrl: './profile-distribution.page.scss',
})
export class ProfileDistributionPage implements OnInit {
  private api = inject(ProfileDistributionService);
  private dialogService = inject(DialogService);
  private authService = inject(AuthService);
  private notifications = inject(NotificationService);
  private translate = inject(TranslateService);
  private destroyRef = inject(DestroyRef);

  // state
  files = signal<DistributionFile[]>([]);
  employees = signal<DistributionEmployee[]>([]);
  targetEntities = signal<dropdownOptionsModel[]>([]);
  loading = signal(false);
  paginationMetadata = signal<PaginationMetadata | null>(null);

  statusFilter = signal<ProfileStatusNumber | 'all'>('all');
  search = signal('');
  targetEntityId = signal<string>('');
  hasOtherFilter = signal<'all' | 'yes' | 'no'>('all');
  selectedIds = signal<Set<string>>(new Set());
  pageNumber = signal(1);
  pageSize = signal(10);
  sortBy = signal<string | null>(null);
  sortDirection = signal<'asc' | 'desc' | null>(null);

  manualEmployeeId = signal<string>('');
  autoEmployeeIds = signal<Set<string>>(new Set());
  autoLimit = signal<number | null>(null);

  // streams
  private searchChanges$ = new Subject<string>();
  private query$ = new Subject<{ force?: boolean }>();
  // computed
  readonly selectedFiles = computed(() =>
    this.files().filter(file => this.selectedIds().has(file.profileId))
  );

  readonly displayedFiles = computed(() => this.files());

  readonly kpis = computed(() => {
    const displayedFiles = this.displayedFiles();
    const totalCount = this.paginationMetadata()?.totalCount ?? this.files().length;

    return {
      total: totalCount,
      filtered: totalCount,
      submitted: displayedFiles.filter(file => file.status === ProfileStatusNumber.Submitted).length,
      underReview: displayedFiles.filter(file => file.status === ProfileStatusNumber.UnderReview).length,
      needsChanges: displayedFiles.filter(file => file.status === ProfileStatusNumber.RequiresUpdate).length,
    };
  });

  readonly availableEmployees = computed(() =>
    this.employees().filter(e => e.isActive && e.availability === EmployeeAvailability.Available)
  );

  readonly statusOptions: { value: ProfileStatusNumber | 'all'; label: string }[] = [
    { value: 'all', label: 'distribution.filters.statusAll' },
    { value: ProfileStatusNumber.Submitted, label: 'distribution.filters.statusSubmitted' },
    { value: ProfileStatusNumber.UnderReview, label: 'distribution.filters.statusUnderReview' },
    { value: ProfileStatusNumber.RequiresUpdate, label: 'distribution.filters.statusNeedsChanges' },
  ];

  readonly yesNoOptions: { value: 'all' | 'yes' | 'no'; label: string }[] = [
    { value: 'all', label: 'distribution.filters.statusAll' },
    { value: 'yes', label: 'common.yes' },
    { value: 'no', label: 'common.no' }
  ];

  readonly rowsPerPageOptions = [10, 20, 50];

  ngOnInit(): void {
    this.setupSearchListener();
    this.setupQueryPipeline();

    // load static/slow-changing data once
    this.loadEmployees();
    this.loadTargetEntities();

    // initial table load
    this.loadData();
  }

  // ✅ Main query trigger (no duplication, no race conditions)
  loadData(force = false): void {
    this.query$.next({ force });
  }

  onSort(event: SortEvent): void {
    if (!event.field || !event.order) return;

    const newDir = event.order === 1 ? 'asc' : 'desc';

    if (this.sortBy() === event.field && this.sortDirection() === newDir) return; // ✅ ignore duplicate

    this.sortBy.set(event.field);
    this.sortDirection.set(newDir);
    this.pageNumber.set(1);
    this.loadData();
  }

  private buildFilters(): DistributionProfilesFilters {
    const filters: DistributionProfilesFilters = {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
    };

    const status = this.statusFilter();
    if (status !== 'all') filters.status = status;

    const term = this.search().trim();
    if (term) filters.searchTerm = term;

    const entityId = this.targetEntityId();
    if (entityId) filters.targetEntityId = entityId;

    const sortBy = this.sortBy();
    const sortDir = this.sortDirection();
    if (sortBy && sortDir) {
      filters.sortBy = sortBy;
      filters.sortDirection = sortDir;
    }

    const hasOther = this.hasOtherFilter();
    if (hasOther !== 'all') filters.hasOtherSpecialization = hasOther === 'yes';

    return filters;
  }

  private filtersKey(f: DistributionProfilesFilters): string {
    // stable key for dedupe
    return [
      f.pageNumber,
      f.pageSize,
      f.status ?? 'all',
      f.searchTerm ?? '',
      f.targetEntityId ?? '',
      f.hasOtherSpecialization?.toString() ?? 'all',
      f.sortBy ?? '',
      f.sortDirection ?? '',
    ].join('|');
  }

  private setupQueryPipeline(): void {
    this.query$
      .pipe(
        map(({ force }) => {
          const filters = this.buildFilters();
          return { filters, key: this.filtersKey(filters), force: !!force };
        }),
        // ✅ اسمح بالمرور إذا force=true حتى لو نفس الـ key
        distinctUntilChanged((a, b) => !b.force && a.key === b.key),

        switchMap(({ filters }) =>
          defer(() => {
            this.loading.set(true);
            return this.api.getFiles(filters).pipe(
              finalize(() => this.loading.set(false))
            );
          })
        ),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: (response: PaginatedResult<DistributionFile>) => {
          this.files.set(response.items);
          this.paginationMetadata.set(response.metadata);
          this.pageNumber.set(response.metadata.currentPage);
          this.pageSize.set(response.metadata.pageSize);
        },
      });
  }
  private loadEmployees(): void {
    this.api
      .getEmployees()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: employees => this.employees.set(employees ?? []),
      });
  }

  loadTargetEntities(): void {
    this.api
      .getTargetEntities()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: entities => this.targetEntities.set(entities ?? []),
      });
  }

  // pagination
  onPageChange(page: number): void {
    this.pageNumber.set(page);
    this.loadData();
  }

  onPageSizeChange(size: number): void {
    this.pageSize.set(size);
    this.pageNumber.set(1);
    this.loadData();
  }

  // filters
  onStatusChange(value: ProfileStatusNumber | 'all'): void {
    this.statusFilter.set(value);
    this.applySearch();
  }

  onTargetEntityChange(value: string): void {
    this.targetEntityId.set(value);
    this.applySearch();
  }

  onHasOtherChange(value: 'all' | 'yes' | 'no'): void {
    this.hasOtherFilter.set(value);
    this.applySearch();
  }

  // search input
  onSearchChange(value: string): void {
    // keep raw input for UI binding, but the listener will normalize
    this.search.set(value);
    this.searchChanges$.next(value);
  }

  private setupSearchListener(): void {
    this.searchChanges$
      .pipe(
        map(v => (v ?? '').trim()),
        // optional: ignore very short terms (uncomment if you want)
        // map(v => (v.length < 2 ? '' : v)),
        debounceTime(1000),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(normalized => {
        this.search.set(normalized);
        this.applySearch();
      });
  }

  // selection
  onSelectionChange(selection: DistributionFile[]): void {
    this.selectedIds.set(new Set(selection.map(item => item.profileId)));
  }

  clearSelection(): void {
    this.selectedIds.set(new Set());
  }

  clearSearch(): void {
    this.search.set('');
    this.statusFilter.set('all');
    this.targetEntityId.set('');
    this.hasOtherFilter.set('all');
    this.applySearch();
  }

  // resets pagination + triggers query
  applySearch(): void {
    this.pageNumber.set(1);
    this.clearSelection();
    this.loadData();
  }

  // dialogs
  openManualDialog(profileId?: string): void {
    if (!this.canManageDistribution()) return;

    if (profileId) this.selectedIds.set(new Set([profileId]));

    const ids = Array.from(this.selectedIds());
    if (ids.length === 0) {
      this.notifyNoSelection();
      return;
    }

    this.dialogService
      .open(ManualAssignDialog, {
        header: this.translate.instant('distribution.dialog.manual.title'),
        width: '520px',
        modal: true,
        draggable: false,
        dismissableMask: false,
        data: {
          employees: this.employees(),
          selectedProfileIds: ids,
          initialEmployeeId: this.manualEmployeeId() || null,
        },
      })
      ?.onClose.pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((res: DistributionDialogResult) => {
        if (!res || res.kind !== 'manual') return;
        this.assignManual(res.payload);
      });
  }

  openAutoDialog(): void {
    if (!this.canManageDistribution()) return;

    const ids = Array.from(this.selectedIds());
    if (ids.length === 0) {
      this.notifyNoSelection();
      return;
    }

    this.dialogService
      .open(AutoAssignDialog, {
        header: this.translate.instant('distribution.dialog.auto.title'),
        width: '640px',
        modal: true,
        draggable: false,
        dismissableMask: false,
        data: {
          employees: this.employees(),
          selectedProfileIds: ids,
          initialLimit: this.autoLimit(),
          initialEmployeeIds: Array.from(this.autoEmployeeIds()),
        },
      })
      ?.onClose.pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((res: DistributionDialogResult) => {
        if (!res || res.kind !== 'auto') return;
        this.assignAuto(res.payload);
      });
  }

  // actions
  private assignManual(payload: ManualAssignRequest): void {
    if (!this.canManageDistribution()) return;

    this.loading.set(true);
    this.api
      .assignManually(payload)
      .pipe(finalize(() => this.loading.set(false)), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: result => this.handleResult(result.assignedCount, result),
      });
  }

  private assignAuto(payload: AutoAssignRequest): void {
    if (!this.canManageDistribution()) return;

    this.loading.set(true);
    this.api
      .assignAutomatically(payload)
      .pipe(finalize(() => this.loading.set(false)), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: result => this.handleResult(result.assignedCount, result),
      });
  }

  private reassignManual(payload: ManualAssignRequest): void {
    if (!this.canManageDistribution()) return;

    const request: ReassignRequest = {
      mode: 'manual',
      profileIds: payload.profileIds,
      employeeId: payload.employeeId,
    };

    this.loading.set(true);
    this.api
      .reassign(request)
      .pipe(finalize(() => this.loading.set(false)), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: result => this.handleResult(result.assignedCount, result),
      });
  }

  private reassignAuto(payload: AutoAssignRequest): void {
    if (!this.canManageDistribution()) return;

    const request: ReassignRequest = {
      mode: 'auto',
      profileIds: payload.profileIds ?? [],
      employeeIds: payload.employeeIds,
      perEmployeeCount: payload.perEmployeeCount,
    };

    this.loading.set(true);
    this.api
      .reassign(request)
      .pipe(finalize(() => this.loading.set(false)), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: result => this.handleResult(result.assignedCount, result),
      });
  }

  private handleResult(_assigned: number, result: DistributionResult): void {
    // update employees availability/loads returned by API
    this.employees.set(result.employees ?? this.employees());
    this.clearSelection();

    // ✅ لازم force=true عشان يتجاوز distinctUntilChanged
    this.loadData(true);
  }
  // helpers
  canManageDistribution(): boolean {
    return this.authService.hasPermission(Permissions.ProfileDistribution.Manage);
  }

  statusSeverity(
    status: ProfileStatusNumber
  ): 'info' | 'warn' | 'success' | 'danger' | 'secondary' {
    switch (status) {
      case ProfileStatusNumber.UnderReview:
        return 'info';
      case ProfileStatusNumber.RequiresUpdate:
        return 'warn';
      case ProfileStatusNumber.Approved:
        return 'success';
      case ProfileStatusNumber.Rejected:
      case ProfileStatusNumber.Cancelled:
        return 'danger';
      default:
        return 'secondary';
    }
  }

  statusLabel(status: ProfileStatusNumber): string {
    switch (status) {
      case ProfileStatusNumber.Submitted:
        return 'distribution.status.submitted';
      case ProfileStatusNumber.UnderReview:
        return 'distribution.status.underReview';
      case ProfileStatusNumber.RequiresUpdate:
        return 'distribution.status.needsChanges';
      case ProfileStatusNumber.Approved:
        return 'distribution.status.approved';
      case ProfileStatusNumber.Rejected:
        return 'distribution.status.rejected';
      case ProfileStatusNumber.Cancelled:
        return 'distribution.status.cancelled';
      default:
        return '------';
    }
  }

  private notifyNoSelection(): void {
    const message = this.translate.instant('distribution.errors.noProfilesSelected');
    this.notifications.warn(message);
  }

  protected readonly Number = Number;
}
