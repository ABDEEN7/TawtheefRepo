import { CommonModule } from '@angular/common';
import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { SortEvent } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelect } from 'primeng/multiselect';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import {
  catchError,
  debounceTime,
  defer,
  distinctUntilChanged,
  EMPTY,
  finalize,
  forkJoin,
  map,
  Subject,
  switchMap,
} from 'rxjs';
import { Permissions } from '../../../../../core/constants/permissions';
import { ProfileStatusNumber } from '../../../../../core/enums/lookups.enum';
import { PaginationMetadata } from '../../../../../core/models/pagination-metadata.model';
import { routes } from '../../../../../routes/routes';
import { PageFiltersComponent } from '../../../../../shared/components/page-filters/page-filters.component';
import { PaginationComponent } from '../../../../../shared/components/pagination/pagination.component';
import { HasPermissionDirective } from '../../../../../shared/directives/has-permission.directive';
import { I18nNamespaceDirective } from '../../../../../shared/directives/i18n-namespace.directive';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import {
  ProfileApprovalCandidateSource,
  ProfileApprovalListFilter,
  ProfileApprovalListItem,
  ReviewStatus,
} from './models/profile-approval.models';
import { ProfileApprovalService } from './services/profile-approval.service';

@Component({
  selector: 'app-profile-approval-list-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    TranslateModule,
    I18nNamespaceDirective,
    TableModule,
    TagModule,
    MultiSelect,
    ButtonModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    PaginationComponent,
    PageFiltersComponent,
    HasPermissionDirective,
  ],
  templateUrl: './profile-approval-list.page.html',
  styleUrl: './profile-approval-list.page.scss',
})
export class ProfileApprovalListPage implements OnInit {
  private readonly api = inject(ProfileApprovalService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly Permissions = Permissions;
  items = signal<ProfileApprovalListItem[]>([]);
  meta = signal<PaginationMetadata>({
    currentPage: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 1,
    hasNext: false,
    hasPreviousPage: false,
  });
  targetEntities = signal<dropdownOptionsModel[]>([]);
  candidateTypes = signal<dropdownOptionsModel[]>([]);
  loading = signal(false);
  showAdvancedFilters = signal(false);
  searchTerm = signal('');
  filters = signal<ProfileApprovalListFilter>({
    pageNumber: 1,
    pageSize: 10,
    sortBy: 'createdDate',
    sortDirection: 'desc',
  });

  readonly statusOptions = [
    { label: 'profileApproval.status.changes', value: ReviewStatus.ChangesRequested },
    { label: 'profileApproval.status.pending', value: ReviewStatus.Pending },
  ];
  readonly candidateSourceOptions = [
    {
      label: 'profileApproval.list.source.ministerOffice',
      value: ProfileApprovalCandidateSource.MinisterOffice,
    },
    {
      label: 'profileApproval.list.source.kawaader',
      value: ProfileApprovalCandidateSource.Kawaader,
    },
  ];

  readonly totalItems = computed(() => this.meta().totalCount);
  readonly currentPage = computed(() => this.filters().pageNumber);
  readonly pageSize = computed(() => this.filters().pageSize);
  readonly activeFilterCount = computed(() => {
    const filters = this.filters();
    return [
      filters.search?.trim(),
      filters.statuses?.length,
      filters.targetEntityIds?.length,
      filters.candidateTypeIds?.length,
      filters.candidateSources?.length,
    ].filter(Boolean).length;
  });
  readonly advancedExpanded = computed(
    () => this.showAdvancedFilters() || this.hasActiveAdvancedFilters(),
  );

  private readonly searchChanges$ = new Subject<string>();
  private readonly listRequests$ = new Subject<boolean>();

  ngOnInit(): void {
    this.setupRequestOrchestration();
    this.loadLookups();
    this.requestList();
  }

  onSearchChange(value: string): void {
    this.searchTerm.set(value);
    this.searchChanges$.next(value);
  }

  onStatusesChange(value: ReviewStatus[] | null): void {
    this.updateArrayFilter('statuses', value);
  }
  onTargetEntitiesChange(value: string[] | null): void {
    this.updateArrayFilter('targetEntityIds', value);
  }
  onCandidateTypesChange(value: string[] | null): void {
    this.updateArrayFilter('candidateTypeIds', value);
  }
  onCandidateSourcesChange(value: ProfileApprovalCandidateSource[] | null): void {
    this.updateArrayFilter('candidateSources', value);
  }

  toggleAdvancedFilters(): void {
    if (!this.hasActiveAdvancedFilters()) this.showAdvancedFilters.update((expanded) => !expanded);
  }

  clearFilters(): void {
    const pageSize = this.filters().pageSize;
    this.searchTerm.set('');
    this.searchChanges$.next('');
    this.showAdvancedFilters.set(false);
    this.filters.set({ pageNumber: 1, pageSize, sortBy: 'createdDate', sortDirection: 'desc' });
    this.requestList(true);
  }

  refresh(): void {
    this.requestList(true);
  }

  onSort(event: SortEvent): void {
    if (!event.field) return;
    const sortMap: Record<string, ProfileApprovalListFilter['sortBy']> = {
      targetEntity: 'targetEntityId',
      submittedAtUtc: 'createdDate',
    };
    const sortBy = sortMap[event.field];
    if (!sortBy) return;
    this.filters.update((filters) => ({
      ...filters,
      sortBy,
      sortDirection: event.order === 1 ? 'asc' : 'desc',
      pageNumber: 1,
    }));
    this.requestList();
  }

  onPageChanged(pageNumber: number): void {
    this.filters.update((filters) => ({ ...filters, pageNumber }));
    this.requestList();
  }

  onPageSizeChanged(pageSize: number): void {
    this.filters.update((filters) => ({ ...filters, pageSize, pageNumber: 1 }));
    this.requestList();
  }

  statusClass(status?: ReviewStatus): string {
    switch (status) {
      case ReviewStatus.Approved:
        return 'soft';
      case ReviewStatus.Rejected:
        return 'danger';
      case ReviewStatus.ChangesRequested:
        return 'warning';
      default:
        return '';
    }
  }

  statusLabel(status?: ReviewStatus): string {
    switch (status) {
      case ReviewStatus.Approved:
        return 'profileApproval.status.approved';
      case ReviewStatus.Rejected:
        return 'profileApproval.status.rejected';
      case ReviewStatus.ChangesRequested:
        return 'profileApproval.status.changes';
      default:
        return 'profileApproval.status.pending';
    }
  }

  openProfile(row: ProfileApprovalListItem): void {
    if (!row.userProfileId) return;
    const url =
      row.profileStatus === ProfileStatusNumber.Approved
        ? routes.portal.approvalProfileChanges(row.userProfileId)
        : routes.portal.approvalProfileReview(row.userProfileId);
    void this.router.navigate([url]);
  }

  private setupRequestOrchestration(): void {
    this.searchChanges$
      .pipe(
        map((value) => (value ?? '').trim()),
        debounceTime(400),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((search) => {
        if ((this.filters().search ?? '') === search) return;
        this.filters.update((filters) => ({
          ...filters,
          search: search || undefined,
          pageNumber: 1,
        }));
        this.requestList();
      });

    this.listRequests$
      .pipe(
        map((force) => {
          const filters = this.buildRequest();
          return { filters, key: JSON.stringify(filters), force };
        }),
        distinctUntilChanged((previous, current) => !current.force && previous.key === current.key),
        switchMap(({ filters }) =>
          defer(() => {
            this.loading.set(true);
            return this.api.getProfiles(filters).pipe(
              catchError(() => EMPTY),
              finalize(() => this.loading.set(false)),
            );
          }),
        ),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((response) => {
        this.items.set(response.items ?? []);
        if (response.metadata) this.meta.set(response.metadata);
      });
  }

  private loadLookups(): void {
    forkJoin({
      targetEntities: this.api.getTargetEntities(),
      candidateTypes: this.api.getCandidateTypes(),
    })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(({ targetEntities, candidateTypes }) => {
        this.targetEntities.set(targetEntities ?? []);
        this.candidateTypes.set(candidateTypes ?? []);
      });
  }

  private requestList(force = false): void {
    this.listRequests$.next(force);
  }

  private updateArrayFilter<
    K extends
      | 'statuses'
      | 'targetEntityIds'
      | 'candidateTypeIds'
      | 'candidateSources',
  >(key: K, values: NonNullable<ProfileApprovalListFilter[K]> | null): void {
    this.filters.update((filters) => ({
      ...filters,
      [key]: values?.length ? [...values] : undefined,
      pageNumber: 1,
    }));
    this.requestList();
  }

  private buildRequest(): ProfileApprovalListFilter {
    const filters = this.filters();
    return {
      pageNumber: filters.pageNumber,
      pageSize: filters.pageSize,
      ...(filters.sortBy ? { sortBy: filters.sortBy } : {}),
      ...(filters.sortDirection ? { sortDirection: filters.sortDirection } : {}),
      ...(filters.search?.trim() ? { search: filters.search.trim() } : {}),
      ...(filters.statuses?.length ? { statuses: [...filters.statuses] } : {}),
      ...(filters.targetEntityIds?.length ? { targetEntityIds: [...filters.targetEntityIds] } : {}),
      ...(filters.candidateTypeIds?.length
        ? { candidateTypeIds: [...filters.candidateTypeIds] }
        : {}),
      ...(filters.candidateSources?.length
        ? { candidateSources: [...filters.candidateSources] }
        : {}),
    };
  }

  private hasActiveAdvancedFilters(): boolean {
    const filters = this.filters();
    return Boolean(
      filters.candidateTypeIds?.length ||
      filters.candidateSources?.length,
    );
  }
}
