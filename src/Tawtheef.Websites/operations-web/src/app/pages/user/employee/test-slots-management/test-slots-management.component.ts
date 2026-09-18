import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { DatePickerModule } from 'primeng/datepicker';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { Select } from 'primeng/select';
import { TagModule } from 'primeng/tag';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { catchError, debounceTime, finalize, forkJoin, of, Subject, switchMap } from 'rxjs';
import { LanguageService } from '../../../../core/services/language.service';
import { Permissions } from '../../../../core/constants/permissions';
import { portalRoutes } from '../../../../routes/portal-routes';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { PageFiltersComponent } from '../../../../shared/components/page-filters/page-filters.component';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { RoomListItemDto } from '../rooms-management/models/room-list-item.dto';
import { TestSlotFilters, TestSlotListItemDto } from './models/test-slot-list-item.dto';
import { TestSlotsService } from './services/test-slots.service';

@Component({
  selector: 'app-test-slots-management',
  standalone: true,
  templateUrl: './test-slots-management.component.html',
  styleUrl: './test-slots-management.component.scss',
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    ButtonModule,
    DatePickerModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    Select,
    TagModule,
    TableModule,
    TooltipModule,
    PaginationComponent,
    PageFiltersComponent,
    I18nNamespaceDirective,
    RouterLink,
    HasPermissionDirective,
  ],
})
export class TestSlotsManagementComponent implements OnInit {
  private readonly service = inject(TestSlotsService);
  private readonly language = inject(LanguageService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly filterChanges$ = new Subject<void>();
  private readonly listRequests$ = new Subject<void>();

  readonly testSlots = signal<TestSlotListItemDto[]>([]);
  readonly Permissions = Permissions;
  readonly routes = portalRoutes;
  readonly availableRooms = signal<RoomListItemDto[]>([]);
  readonly loading = signal(false);
  readonly loadError = signal(false);
  readonly lookupError = signal(false);
  readonly totalItems = signal(0);
  readonly advancedFiltersExpanded = signal(false);
  readonly filters = signal<TestSlotFilters>({
    pageNumber: 1,
    pageSize: 10,
    language: this.language.get(),
  });

  searchTerm = '';
  selectedRoomId: string | undefined;
  dateFrom: Date | null = null;
  dateTo: Date | null = null;

  get roomLabel(): 'nameAr' | 'nameEn' {
    return this.language.isRtl ? 'nameAr' : 'nameEn';
  }

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
      .subscribe(response => {
        this.testSlots.set(response?.items ?? []);
        this.totalItems.set(response?.metadata?.totalCount ?? 0);
      });

    this.filterChanges$
      .pipe(debounceTime(500), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.applyFilters());

    this.language.current$
      .pipe(
        switchMap(language => {
          this.filters.update(filters => ({ ...filters, language }));
          this.listRequests$.next();
          this.lookupError.set(false);
          return forkJoin({ availableRooms: this.service.getAvailableRoomsForTestSlot(language) }).pipe(
            catchError(() => {
              this.lookupError.set(true);
              return of({ availableRooms: [] });
            }),
          );
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(lookups => this.availableRooms.set(lookups.availableRooms));
  }

  onTextFilterChange(): void {
    this.filterChanges$.next();
  }

  applyFilters(): void {
    if (this.invalidDateRange()) return;

    this.filters.update(filters => ({
      ...filters,
      pageNumber: 1,
      searchTerm: this.searchTerm.trim() || undefined,
      roomId: this.selectedRoomId,
      dateFrom: this.toDateFilter(this.dateFrom),
      dateTo: this.toDateFilter(this.dateTo),
    }));
    this.listRequests$.next();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.selectedRoomId = undefined;
    this.dateFrom = null;
    this.dateTo = null;
    this.applyFilters();
  }

  toggleAdvancedFilters(): void {
    this.advancedFiltersExpanded.update(expanded => !expanded);
  }

  activeAdvancedFilterCount(): number {
    return Number(!!this.dateFrom) + Number(!!this.dateTo);
  }

  invalidDateRange(): boolean {
    const from = this.toDateFilter(this.dateFrom);
    const to = this.toDateFilter(this.dateTo);
    return !!from && !!to && from > to;
  }

  onPageChange(pageNumber: number): void {
    this.filters.update(filters => ({ ...filters, pageNumber }));
    this.listRequests$.next();
  }

  onPageSizeChange(pageSize: number): void {
    this.filters.update(filters => ({ ...filters, pageNumber: 1, pageSize }));
    this.listRequests$.next();
  }

  private toDateFilter(date: Date | null): string | undefined {
    if (!date) return undefined;
    const year = String(date.getFullYear()).padStart(4, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
