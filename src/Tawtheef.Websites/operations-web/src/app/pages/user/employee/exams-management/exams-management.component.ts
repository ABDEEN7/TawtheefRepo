import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { portalRoutes } from '../../../../routes/portal-routes';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { DatePickerModule } from 'primeng/datepicker';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { Select } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { catchError, debounceTime, finalize, forkJoin, of, Subject, switchMap } from 'rxjs';
import { Permissions } from '../../../../core/constants/permissions';
import { LanguageService } from '../../../../core/services/language.service';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { PageFiltersComponent } from '../../../../shared/components/page-filters/page-filters.component';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { dropdownOptionsModel } from '../../../../shared/models/dropdown-options.model';
import { ExamFilters, ExamListItemDto } from './models/exam-list-item.dto';
import { ExamsService } from './services/exams.service';

@Component({
  selector: 'app-exams-management',
  standalone: true,
  templateUrl: './exams-management.component.html',
  styleUrl: './exams-management.component.scss',
  imports: [
    CommonModule,
    RouterLink,
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
    TooltipModule,
    PaginationComponent,
    PageFiltersComponent,
    HasPermissionDirective,
    I18nNamespaceDirective,
  ],
})
export class ExamsManagementComponent implements OnInit {
  protected readonly routes = portalRoutes;
  private readonly service = inject(ExamsService);
  private readonly language = inject(LanguageService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly filterChanges$ = new Subject<void>();
  private readonly listRequests$ = new Subject<void>();
  protected readonly Permissions = Permissions;

  readonly exams = signal<ExamListItemDto[]>([]);
  readonly loading = signal(false);
  readonly loadError = signal(false);
  readonly lookupError = signal(false);
  readonly totalItems = signal(0);
  readonly statusOptions = signal<dropdownOptionsModel[]>([]);
  readonly specializationOptions = signal<dropdownOptionsModel[]>([]);
  readonly advancedFiltersExpanded = signal(false);
  readonly filters = signal<ExamFilters>({
    pageNumber: 1,
    pageSize: 10,
    language: this.language.get(),
  });

  search = '';
  selectedSpecializationId: string | undefined;
  selectedStatusId: string | undefined;
  createdFrom: Date | null = null;
  createdTo: Date | null = null;

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
        this.exams.set(response?.items ?? []);
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
          return forkJoin({
            statuses: this.service.getStatuses(language),
            specializations: this.service.getSpecializations(language),
          }).pipe(
            catchError(() => {
              this.lookupError.set(true);
              return of({ statuses: [], specializations: [] });
            }),
          );
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((lookups) => {
        this.statusOptions.set(lookups.statuses);
        this.specializationOptions.set(lookups.specializations);
      });
  }

  onSearchChange(): void {
    this.filterChanges$.next();
  }

  applyFilters(): void {
    if (this.invalidCreatedRange()) return;

    this.filters.update((filters) => ({
      ...filters,
      pageNumber: 1,
      search: this.search.trim() || undefined,
      specializationId: this.selectedSpecializationId,
      statusId: this.selectedStatusId,
      createdFrom: this.toDateFilter(this.createdFrom),
      createdTo: this.toDateFilter(this.createdTo),
    }));
    this.listRequests$.next();
  }

  clearFilters(): void {
    this.search = '';
    this.selectedSpecializationId = undefined;
    this.selectedStatusId = undefined;
    this.createdFrom = null;
    this.createdTo = null;
    this.applyFilters();
  }

  activeAdvancedFilterCount(): number {
    return (
      Number(!!this.selectedSpecializationId) +
      Number(!!this.createdFrom) +
      Number(!!this.createdTo)
    );
  }

  invalidCreatedRange(): boolean {
    const from = this.toDateFilter(this.createdFrom);
    const to = this.toDateFilter(this.createdTo);
    return !!from && !!to && from > to;
  }

  private toDateFilter(date: Date | null): string | undefined {
    if (!date) return undefined;
    const year = String(date.getFullYear()).padStart(4, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  toggleAdvancedFilters(): void {
    this.advancedFiltersExpanded.update((expanded) => !expanded);
  }

  onPageChange(pageNumber: number): void {
    this.filters.update((filters) => ({ ...filters, pageNumber }));
    this.listRequests$.next();
  }

  onPageSizeChange(pageSize: number): void {
    this.filters.update((filters) => ({ ...filters, pageNumber: 1, pageSize }));
    this.listRequests$.next();
  }

  isEditable(exam: ExamListItemDto): boolean {
    return exam.status.backendName === 'Draft' || exam.status.backendName === 'Returned';
  }
}
