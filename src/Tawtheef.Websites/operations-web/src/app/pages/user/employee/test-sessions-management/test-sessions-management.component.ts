import { DatePipe, CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { DatePickerModule } from 'primeng/datepicker';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { Select } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { Ripple } from 'primeng/ripple';
import { Tooltip } from 'primeng/tooltip';
import { catchError, debounceTime, finalize, forkJoin, of, Subject, switchMap } from 'rxjs';
import { Permissions } from '../../../../core/constants/permissions';
import { LanguageService } from '../../../../core/services/language.service';
import { portalRoutes } from '../../../../routes/portal-routes';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { PageFiltersComponent } from '../../../../shared/components/page-filters/page-filters.component';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { dropdownOptionsModel } from '../../../../shared/models/dropdown-options.model';
import {
  TestSessionFilters,
  TestSessionListItemDto,
  TestSessionLookupsDto,
  TEST_SESSION_STATUS_IDS,
} from './models/test-session-list-item.dto';
import { TestSessionsService } from './services/test-sessions.service';

@Component({
  selector: 'app-test-sessions-management',
  standalone: true,
  templateUrl: './test-sessions-management.component.html',
  styleUrl: './test-sessions-management.component.scss',
  imports: [
    CommonModule,
    DatePipe,
    FormsModule,
    TranslatePipe,
    ButtonModule,
    DatePickerModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    Select,
    TableModule,
    TagModule,
    Ripple,
    Tooltip,
    PaginationComponent,
    PageFiltersComponent,
    HasPermissionDirective,
    I18nNamespaceDirective,
  ],
})
export class TestSessionsManagementComponent implements OnInit {
  protected readonly Permissions = Permissions;
  private readonly service = inject(TestSessionsService);
  private readonly language = inject(LanguageService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly filterChanges$ = new Subject<void>();
  private readonly listRequests$ = new Subject<void>();

  readonly sessions = signal<TestSessionListItemDto[]>([]);
  readonly loading = signal(false);
  readonly loadError = signal(false);
  readonly lookupError = signal(false);
  readonly totalItems = signal(0);
  readonly advancedFiltersExpanded = signal(false);
  readonly lookups = signal<TestSessionLookupsDto>(this.emptyLookups());
  readonly filters = signal<TestSessionFilters>({
    pageNumber: 1,
    pageSize: 10,
    language: this.language.get(),
  });

  searchText = '';
  selectedExamId?: string;
  selectedStatusId?: string;
  selectedJobId?: string;
  selectedRoomId?: string;
  selectedPeriod?: string;
  selectedNationalityId?: string;
  selectedGenderId?: string;
  selectedFromDate: Date | null = null;
  selectedToDate: Date | null = null;

  ngOnInit(): void {
    this.listRequests$
      .pipe(
        switchMap(() => {
          this.loading.set(true);
          this.loadError.set(false);
          return this.service.list(this.filters()).pipe(
            catchError(() => {
              this.loadError.set(true);
              return of(null);
            }),
            finalize(() => this.loading.set(false)),
          );
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((response) => {
        this.sessions.set(response?.items ?? []);
        this.totalItems.set(response?.metadata?.totalCount ?? 0);
      });

    this.filterChanges$
      .pipe(debounceTime(500), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.applyFilters());

    this.language.current$
      .pipe(
        switchMap((language) => {
          this.filters.update((filters) => ({ ...filters, language }));
          this.listRequests$.next();
          this.lookupError.set(false);
          return forkJoin({ lookups: this.service.lookups(language, this.selectedRoomId) }).pipe(
            catchError(() => {
              this.lookupError.set(true);
              return of({ lookups: this.emptyLookups() });
            }),
          );
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(({ lookups }) => this.lookups.set(lookups));
  }

  onSearchChange(): void {
    this.filterChanges$.next();
  }

  applyFilters(): void {
    this.filters.update((filters) => ({
      ...filters,
      pageNumber: 1,
      searchText: this.searchText.trim() || undefined,
      examId: this.selectedExamId,
      statusId: this.selectedStatusId,
      jobId: this.selectedJobId,
      roomId: this.selectedRoomId,
      periodId: this.selectedPeriod,
      nationalityId: this.selectedNationalityId,
      genderId: this.selectedGenderId,
      fromDate: this.toDateFilter(this.selectedFromDate),
      toDate: this.toDateFilter(this.selectedToDate),
    }));
    this.listRequests$.next();
  }

  clearFilters(): void {
    this.searchText = '';
    this.selectedExamId =
      this.selectedStatusId =
      this.selectedJobId =
      this.selectedRoomId =
        undefined;
    this.selectedPeriod = this.selectedNationalityId = this.selectedGenderId = undefined;
    this.selectedFromDate = this.selectedToDate = null;
    this.applyFilters();
  }

  toggleAdvancedFilters(): void {
    this.advancedFiltersExpanded.update((value) => !value);
  }
  createTestSession(): void {
    void this.router.navigateByUrl(portalRoutes.createTestSession);
  }
  canEdit(session: TestSessionListItemDto): boolean {
    const statusId = session.status.id.toLowerCase();

    return (
      statusId === TEST_SESSION_STATUS_IDS.draft || statusId === TEST_SESSION_STATUS_IDS.returned
    );
  }
  editTestSession(session: TestSessionListItemDto): void {
    void this.router.navigateByUrl(portalRoutes.editTestSession(session.id));
  }

  viewTestSession(session: TestSessionListItemDto): void {
    void this.router.navigateByUrl(portalRoutes.viewTestSession(session.id));
  }
  activeAdvancedFilterCount(): number {
    return [
      this.selectedJobId,
      this.selectedRoomId,
      this.selectedPeriod,
      this.selectedNationalityId,
      this.selectedGenderId,
      this.selectedFromDate,
      this.selectedToDate,
    ].filter(Boolean).length;
  }
  onRoomChange(): void {
    this.lookupError.set(false);
    this.service
      .lookups(this.language.get(), this.selectedRoomId)
      .pipe(
        catchError(() => {
          this.lookupError.set(true);
          return of(this.emptyLookups());
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((lookups) => {
        this.lookups.set(lookups);
        if (!lookups.periods.some((period) => period.id === this.selectedPeriod))
          this.selectedPeriod = undefined;
        this.applyFilters();
      });
  }
  onPageChange(pageNumber: number): void {
    this.filters.update((filters) => ({ ...filters, pageNumber }));
    this.listRequests$.next();
  }
  onPageSizeChange(pageSize: number): void {
    this.filters.update((filters) => ({ ...filters, pageNumber: 1, pageSize }));
    this.listRequests$.next();
  }
  private toDateFilter(date: Date | null): string | undefined {
    if (!date) return undefined;
    return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
  }
  private emptyLookups(): TestSessionLookupsDto {
    return {
      exams: [],
      jobs: [],
      rooms: [],
      statuses: [],
      periods: [],
      nationalities: [],
      genders: [],
    };
  }
}
