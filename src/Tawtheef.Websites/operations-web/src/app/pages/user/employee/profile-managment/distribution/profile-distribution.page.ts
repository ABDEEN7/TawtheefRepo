import { CommonModule } from '@angular/common';
import {
  Component,
  ChangeDetectionStrategy,
  DestroyRef,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  Subject,
  debounceTime,
  distinctUntilChanged,
  finalize,
  map,
  switchMap,
  defer,
} from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { AvatarModule } from 'primeng/avatar';
import { BadgeModule } from 'primeng/badge';
import { ProgressBarModule } from 'primeng/progressbar';
import { Select } from 'primeng/select';
import { MultiSelectModule } from 'primeng/multiselect';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { Ripple } from 'primeng/ripple';
import { Tooltip } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';

import { ProfileDistributionService } from './services/profile-distribution.service';
import {
  DistributionEmployee,
  DistributionFile,
  DistributionResult,
} from './models/profile-distribution.models';
import {
  DistributionAdvancedFilters,
  DistributionAppliedFilters,
  DistributionProfilesFilters,
} from './models/profile-distribution-filters.model';
import {
  AutoAssignRequest,
  ManualAssignRequest,
} from './models/profile-distribution-assignment.model';
import {
  DistributionAssignmentState,
  EmployeeAvailability,
} from './models/profile-distribution.enums';

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
import { parseFilterYear } from '../../../../../core/utils/year-filter.util';
import {SortEvent} from 'primeng/api';
import { PageFiltersComponent } from '../../../../../shared/components/page-filters/page-filters.component';

const DEFAULT_ADVANCED_FILTERS: DistributionAdvancedFilters = {
  assignmentState: null,
  assignedEmployeeId: null,
  targetEntityId: null,
  candidateTypeIds: [],
  hasOtherSpecialization: null,
  hasOtherUniversity: null,
  isQatarGraduate: false,
  degreeIds: [],
};

const createDefaultFilters = (pageSize = 10): DistributionAppliedFilters => ({
  searchTerm: null,
  statuses: [ProfileStatusNumber.Submitted],
  year: null,
  ...DEFAULT_ADVANCED_FILTERS,
  pageNumber: 1,
  pageSize,
  sortBy: 'CreatedDate',
  sortDirection: 'desc',
});

@Component({
  selector: 'app-profile-distribution-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule,
    TableModule,
    InputTextModule,
    IconFieldModule,
    InputIconModule,
    ButtonModule,
    TagModule,
    DialogModule,
    InputNumberModule,
    AvatarModule,
    BadgeModule,
    PaginationComponent,
    I18nNamespaceDirective,
    Select,
    MultiSelectModule,
    ToggleSwitchModule,
    Ripple,
    Tooltip,
    ProgressBarModule,
    PageFiltersComponent,
  ],
  providers: [DialogService],
  templateUrl: './profile-distribution.page.html',
  styleUrl: './profile-distribution.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfileDistributionPage implements OnInit {
  private api = inject(ProfileDistributionService);
  private dialogService = inject(DialogService);
  private authService = inject(AuthService);
  private notifications = inject(NotificationService);
  private translate = inject(TranslateService);
  private destroyRef = inject(DestroyRef);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  // state
  files = signal<DistributionFile[]>([]);
  employees = signal<DistributionEmployee[]>([]);
  targetEntities = signal<dropdownOptionsModel[]>([]);
  candidateTypes = signal<dropdownOptionsModel[]>([]);
  degrees = signal<dropdownOptionsModel[]>([]);
  loading = signal(false);
  paginationMetadata = signal<PaginationMetadata | null>(null);

  appliedFilters = signal<DistributionAppliedFilters>(createDefaultFilters());
  draftFilters = signal<DistributionAdvancedFilters>({ ...DEFAULT_ADVANCED_FILTERS });
  searchInput = signal('');
  showAdvancedFilters = signal(false);
  selectedIds = signal<Set<string>>(new Set());
  selectedRows = signal<Map<string, DistributionFile>>(new Map());

  manualEmployeeId = signal<string>('');
  autoEmployeeIds = signal<Set<string>>(new Set());
  autoLimit = signal<number | null>(null);

  // streams
  private searchChanges$ = new Subject<string>();
  private query$ = new Subject<{ force?: boolean }>();
  // computed
  readonly selectedFiles = computed(() => Array.from(this.selectedRows().values()));
  readonly pageNumber = computed(() => this.appliedFilters().pageNumber);
  readonly pageSize = computed(() => this.appliedFilters().pageSize);
  readonly activeAdvancedFilterCount = computed(() =>
    this.countAdvancedFilters(this.getAdvancedFilters(this.appliedFilters()))
  );
  readonly hasActiveFilters = computed(() =>
    !!this.appliedFilters().searchTerm ||
    this.appliedFilters().year !== null ||
    this.activeAdvancedFilterCount() > 0 ||
    !this.hasDefaultStatuses(this.appliedFilters().statuses)
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

  readonly statusOptions = [
    { value: ProfileStatusNumber.Submitted, label: 'distribution.status.submitted' },
    { value: ProfileStatusNumber.UnderReview, label: 'distribution.status.underReview' },
    { value: ProfileStatusNumber.Approved, label: 'distribution.status.approved' },
  ];

  readonly assignmentStateOptions = [
    { value: null, label: 'distribution.filters.all' },
    { value: DistributionAssignmentState.Assigned, label: 'distribution.filters.assigned' },
    { value: DistributionAssignmentState.Unassigned, label: 'distribution.filters.unassigned' },
  ];

  readonly yesNoOptions = [
    { value: null, label: 'distribution.filters.all' },
    { value: true, label: 'distribution.filters.yes' },
    { value: false, label: 'distribution.filters.no' },
  ];

  readonly rowsPerPageOptions = [10, 20, 50, 100, 500];

  ngOnInit(): void {
    this.initializeFromUrl();
    this.setupSearchListener();
    this.setupQueryPipeline();

    // load static/slow-changing data once
    this.loadEmployees();
    this.loadTargetEntities();
    this.loadCandidateTypes();
    this.loadDegrees();

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

    const current = this.appliedFilters();
    if (current.sortDirection === newDir) return;
    this.appliedFilters.set({ ...current, sortBy: 'CreatedDate', sortDirection: newDir, pageNumber: 1 });
    this.loadData();
  }

  private toRequest(filters: DistributionAppliedFilters): DistributionProfilesFilters {
    return Object.fromEntries(
      Object.entries(filters).filter(([, value]) => value !== null && value !== '')
    ) as unknown as DistributionProfilesFilters;
  }

  private setupQueryPipeline(): void {
    this.query$
      .pipe(
        map(({ force }) => {
          const filters = this.toRequest(this.appliedFilters());
          return { filters, key: JSON.stringify(filters), force: !!force };
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
          this.appliedFilters.update(filters => ({ ...filters,
            pageNumber: response.metadata.currentPage, pageSize: response.metadata.pageSize }));
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

  private loadCandidateTypes(): void {
    this.api.getCandidateTypes()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ next: values => this.candidateTypes.set(values ?? []) });
  }

  private loadDegrees(): void {
    this.api.getDegrees()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({ next: values => this.degrees.set(values ?? []) });
  }

  // pagination
  onPageChange(page: number): void {
    this.appliedFilters.update(filters => ({ ...filters, pageNumber: page }));
    this.loadData();
  }

  onPageSizeChange(size: number): void {
    this.appliedFilters.update(filters => ({ ...filters, pageSize: size, pageNumber: 1 }));
    this.loadData();
  }

  // search input
  onSearchChange(value: string): void {
    this.searchInput.set(value);
    this.searchChanges$.next(value);
  }

  private setupSearchListener(): void {
    this.searchChanges$
      .pipe(
        map(v => (v ?? '').trim()),
        // optional: ignore very short terms (uncomment if you want)
        // map(v => (v.length < 2 ? '' : v)),
        debounceTime(500),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(normalized => {
        const searchTerm = normalized || null;
        if (this.appliedFilters().searchTerm === searchTerm) return;
        this.appliedFilters.update(filters => ({ ...filters, searchTerm, pageNumber: 1 }));
        this.clearSelection();
        this.loadData();
      });
  }

  toggleAdvancedFilters(): void {
    if (this.showAdvancedFilters()) return this.cancelAdvancedFilters();
    this.draftFilters.set(this.getAdvancedFilters(this.appliedFilters()));
    this.showAdvancedFilters.set(true);
  }

  updateDraftFilter<K extends keyof DistributionAdvancedFilters>(key: K, value: DistributionAdvancedFilters[K]): void {
    this.draftFilters.update(filters => ({ ...filters, [key]: value }));
  }

  onDraftAssignmentStateChange(assignmentState: DistributionAssignmentState | null): void {
    this.draftFilters.update(filters => ({
      ...filters,
      assignmentState,
      assignedEmployeeId: assignmentState === DistributionAssignmentState.Unassigned
        ? null
        : filters.assignedEmployeeId,
    }));
  }

  onDraftAssignedEmployeeChange(assignedEmployeeId: string | null): void {
    this.draftFilters.update(filters => ({
      ...filters,
      assignedEmployeeId,
      assignmentState: assignedEmployeeId ? null : filters.assignmentState,
    }));
  }

  onStatusChange(statuses: ProfileStatusNumber[] | null): void {
    const normalized = statuses?.length ? [...statuses] : [ProfileStatusNumber.Submitted];
    this.appliedFilters.update(filters => ({ ...filters, statuses: normalized, pageNumber: 1 }));
    this.clearSelection();
    this.loadData();
  }

  onDraftQatarGraduateChange(isQatarGraduate: boolean): void {
    this.draftFilters.update(filters => ({
      ...filters,
      isQatarGraduate,
      degreeIds: isQatarGraduate ? filters.degreeIds : [],
    }));
  }

  applyAdvancedFilters(): void {
    const draft = this.normalizeAdvancedFilters(this.draftFilters());
    this.draftFilters.set(draft);
    this.appliedFilters.update(filters => ({
      ...filters,
      ...draft,
      statuses: this.resolveStatuses(filters, draft.assignmentState),
      pageNumber: 1,
    }));
    this.updateUrlFilters(draft.assignmentState, this.appliedFilters().year);
    this.clearSelection();
    this.loadData();
  }

  cancelAdvancedFilters(): void {
    this.draftFilters.set(this.getAdvancedFilters(this.appliedFilters()));
    this.showAdvancedFilters.set(false);
  }

  clearFilters(): void {
    this.appliedFilters.set(createDefaultFilters(this.appliedFilters().pageSize));
    this.draftFilters.set({ ...DEFAULT_ADVANCED_FILTERS });
    this.searchInput.set('');
    this.showAdvancedFilters.set(false);
    this.clearSelection();
    this.updateUrlFilters(null, null);
    this.loadData(true);
  }

  clearYear(): void {
    this.appliedFilters.update(filters => ({ ...filters, year: null, pageNumber: 1 }));
    this.updateUrlFilters(this.appliedFilters().assignmentState, null);
    this.clearSelection();
    this.loadData();
  }

  private getAdvancedFilters(filters: DistributionAppliedFilters): DistributionAdvancedFilters {
    const { assignmentState, assignedEmployeeId, targetEntityId, candidateTypeIds,
      hasOtherSpecialization, hasOtherUniversity, isQatarGraduate, degreeIds } = filters;
    return { assignmentState, assignedEmployeeId, targetEntityId,
      candidateTypeIds: [...candidateTypeIds], hasOtherSpecialization, hasOtherUniversity,
      isQatarGraduate, degreeIds: [...degreeIds] };
  }

  private normalizeAdvancedFilters(filters: DistributionAdvancedFilters): DistributionAdvancedFilters {
    return {
      ...filters,
      candidateTypeIds: [...filters.candidateTypeIds],
      degreeIds: filters.isQatarGraduate ? [...filters.degreeIds] : [],
    };
  }

  private countAdvancedFilters(filters: DistributionAdvancedFilters): number {
    return [
      filters.assignmentState,
      filters.assignedEmployeeId,
      filters.targetEntityId,
      filters.candidateTypeIds.length > 0 ? true : null,
      filters.hasOtherSpecialization,
      filters.hasOtherUniversity,
      filters.isQatarGraduate ? true : null,
    ].filter(value => value !== null && value !== false).length;
  }

  private hasDefaultStatuses(statuses: ProfileStatusNumber[]): boolean {
    return statuses.length === 1 && statuses[0] === ProfileStatusNumber.Submitted;
  }

  private initializeFromUrl(): void {
    const value = this.route.snapshot.queryParamMap.get('assignmentState');
    const assignmentState = Object.values(DistributionAssignmentState)
      .find(state => state.toLowerCase() === value?.toLowerCase());
    const year = parseFilterYear(this.route.snapshot.queryParamMap.get('year'));
    if (!assignmentState && !year) return;

    this.appliedFilters.set({
      ...createDefaultFilters(),
      statuses: assignmentState === DistributionAssignmentState.Unassigned
        ? []
        : [ProfileStatusNumber.Submitted],
      year: year ?? null,
      assignmentState: assignmentState ?? null,
      assignedEmployeeId: null,
    });
    this.draftFilters.set(this.getAdvancedFilters(this.appliedFilters()));
    this.showAdvancedFilters.set(!!assignmentState);
  }

  private resolveStatuses(
    filters: DistributionAppliedFilters,
    assignmentState: DistributionAssignmentState | null
  ): ProfileStatusNumber[] {
    const wasUnassigned = filters.assignmentState === DistributionAssignmentState.Unassigned;
    if (!wasUnassigned && assignmentState === DistributionAssignmentState.Unassigned) return [];
    if (wasUnassigned && assignmentState !== DistributionAssignmentState.Unassigned && !filters.statuses.length)
      return [ProfileStatusNumber.Submitted];
    return filters.statuses;
  }

  private updateUrlFilters(
    assignmentState: DistributionAssignmentState | null,
    year: number | null
  ): void {
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { assignmentState, year },
      queryParamsHandling: 'merge',
      replaceUrl: true,
    });
  }

  // selection
  onSelectionChange(selection: DistributionFile[]): void {
    const currentPageIds = new Set(this.files().map(item => item.profileId));
    const nextSelectedIds = new Set(this.selectedIds());
    const nextSelectedRows = new Map(this.selectedRows());

    // PrimeNG emits only the current page's selected rows. Replace that page's
    // selection while retaining selections made on every other page.
    currentPageIds.forEach(id => {
      nextSelectedIds.delete(id);
      nextSelectedRows.delete(id);
    });
    selection
      .filter(item => currentPageIds.has(item.profileId))
      .forEach(item => {
        nextSelectedIds.add(item.profileId);
        nextSelectedRows.set(item.profileId, item);
      });

    this.selectedIds.set(nextSelectedIds);
    this.selectedRows.set(nextSelectedRows);
  }

  clearSelection(): void {
    this.selectedIds.set(new Set());
    this.selectedRows.set(new Map());
  }

  // dialogs
  openManualDialog(profileId?: string): void {
    if (!this.canManageDistribution()) return;

    if (profileId) {
      const profile = this.files().find(item => item.profileId === profileId);
      this.selectedIds.set(new Set([profileId]));
      this.selectedRows.set(profile ? new Map([[profileId, profile]]) : new Map());
    }

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
