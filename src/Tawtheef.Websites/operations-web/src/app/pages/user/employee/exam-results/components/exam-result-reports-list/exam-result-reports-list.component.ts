import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { DatePickerModule } from 'primeng/datepicker';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { catchError, debounceTime, of, Subject, switchMap, tap } from 'rxjs';
import { LanguageService } from '../../../../../../core/services/language.service';
import { portalRoutes } from '../../../../../../routes/portal-routes';
import { PaginationComponent } from '../../../../../../shared/components/pagination/pagination.component';
import { PageFiltersComponent } from '../../../../../../shared/components/page-filters/page-filters.component';
import { I18nNamespaceDirective } from '../../../../../../shared/directives/i18n-namespace.directive';
import {
  ExamResultReportFilters,
  ExamResultReportListItem,
} from '../../models/exam-result-report.models';
import { ExamResultReportsService } from '../../services/exam-result-reports.service';
import { TooltipModule } from 'primeng/tooltip';
import { StatusPillComponent } from '../../../../../../shared/components/status-pill/status-pill.component';
import { reportStatusTone } from '../../exam-result-status-presentation';

@Component({
  selector: 'app-exam-result-reports-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    DatePickerModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    TableModule,
    TooltipModule,
    StatusPillComponent,
    PaginationComponent,
    PageFiltersComponent,
    I18nNamespaceDirective,
  ],
  templateUrl: './exam-result-reports-list.component.html',
  styleUrl: './exam-result-reports-list.component.scss',
})
export class ExamResultReportsListComponent implements OnInit {
  readonly reportStatusTone = reportStatusTone;
  private readonly service = inject(ExamResultReportsService);
  private readonly language = inject(LanguageService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly requests$ = new Subject<void>();
  private readonly searchChanges$ = new Subject<void>();

  readonly reports = signal<ExamResultReportListItem[]>([]);
  readonly loading = signal(false);
  readonly loadError = signal(false);
  readonly totalItems = signal(0);
  readonly advancedExpanded = signal(false);
  readonly filters = signal<ExamResultReportFilters>({
    pageNumber: 1,
    pageSize: 10,
    language: this.language.get(),
  });

  search = '';
  createdFrom: Date | null = null;
  createdTo: Date | null = null;

  ngOnInit(): void {
    this.requests$
      .pipe(
        switchMap(() => {
          this.loading.set(true);
          this.loadError.set(false);
          return this.service.list(this.filters()).pipe(
            catchError(() => {
              this.loadError.set(true);
              return of(null);
            }),
          );
        }),
        tap(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((response) => {
        this.reports.set(response?.items ?? []);
        this.totalItems.set(response?.metadata?.totalCount ?? 0);
      });

    this.searchChanges$
      .pipe(debounceTime(500), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.applyFilters());

    this.language.current$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((language) => {
      this.filters.update((filters) => ({ ...filters, language }));
      this.requests$.next();
    });
  }

  onSearchChange(): void {
    this.searchChanges$.next();
  }

  applyFilters(): void {
    if (this.invalidCreatedRange()) return;
    this.filters.update((filters) => ({
      ...filters,
      pageNumber: 1,
      search: this.search.trim() || undefined,
      createdFrom: this.toDateFilter(this.createdFrom),
      createdTo: this.toDateFilter(this.createdTo),
    }));
    this.requests$.next();
  }

  clearFilters(): void {
    this.search = '';
    this.createdFrom = null;
    this.createdTo = null;
    this.applyFilters();
  }

  invalidCreatedRange(): boolean {
    const from = this.toDateFilter(this.createdFrom);
    const to = this.toDateFilter(this.createdTo);
    return !!from && !!to && from > to;
  }

  onPageChange(pageNumber: number): void {
    this.filters.update((filters) => ({ ...filters, pageNumber }));
    this.requests$.next();
  }

  onPageSizeChange(pageSize: number): void {
    this.filters.update((filters) => ({ ...filters, pageNumber: 1, pageSize }));
    this.requests$.next();
  }

  open(reportId: string): void {
    void this.router.navigateByUrl(portalRoutes.examResultDetails(reportId));
  }

  private toDateFilter(date: Date | null): string | undefined {
    if (!date) return undefined;
    const year = String(date.getFullYear()).padStart(4, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
