import { CommonModule } from '@angular/common';
import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { Select } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { debounceTime, distinctUntilChanged, finalize, forkJoin, Subject } from 'rxjs';
import { NotificationService } from '../../../../core/services/notification.service';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { PageFiltersComponent } from '../../../../shared/components/page-filters/page-filters.component';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { dropdownOptionsModel } from '../../../../shared/models/dropdown-options.model';
import { QuestionBankFilters } from './models/question-bank-filters.dto';
import { QuestionBankListItemDto } from './models/question-bank-list-item.dto';
import { QuestionBanksService } from './services/question-banks.service';

@Component({
  selector: 'app-question-banks',
  standalone: true,
  templateUrl: './question-banks.page.html',
  styleUrls: ['./question-banks.page.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    I18nNamespaceDirective,
    PaginationComponent,
    PageFiltersComponent,
    Select,
    TableModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    TagModule
  ]
})
export class QuestionBanksPage implements OnInit {
  private readonly service = inject(QuestionBanksService);
  private readonly notification = inject(NotificationService);
  private readonly translate = inject(TranslateService);
  private readonly language = inject(LanguageService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly searchChanges$ = new Subject<string>();

  readonly questionBanks = signal<QuestionBankListItemDto[]>([]);
  readonly loading = signal(false);
  readonly totalCount = signal(0);
  readonly filters = signal<QuestionBankFilters>({ pageNumber: 1, pageSize: 10 });
  readonly questionBankTypes = signal<dropdownOptionsModel[]>([]);
  readonly managements = signal<dropdownOptionsModel[]>([]);
  readonly jobTitles = signal<dropdownOptionsModel[]>([]);
  readonly stages = signal<dropdownOptionsModel[]>([]);
  readonly currentLang = signal<Lang>(this.language.get());
  readonly advancedFiltersExpanded = signal(false);
  readonly statusOptions = computed(() => {
    this.currentLang();
    return [
      { label: this.translate.instant('QUESTION_BANKS.ACTIVE'), value: true },
      { label: this.translate.instant('QUESTION_BANKS.INACTIVE'), value: false }
    ];
  });

  search = '';
  selectedQuestionBankTypeId?: string;
  selectedManagementId?: string;
  selectedJobTitleId?: string;
  selectedStageId?: string;
  selectedIsActive?: boolean;

  ngOnInit(): void {
    this.searchChanges$
      .pipe(debounceTime(500), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.applyFilters());
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(lang => this.currentLang.set(lang));
    this.loadLookups();
  }

  onSearchChange(): void {
    this.searchChanges$.next(this.search);
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  toggleAdvancedFilters(): void {
    this.advancedFiltersExpanded.update(expanded => !expanded);
  }

  activeAdvancedFilterCount(): number {
    return Number(!!this.selectedManagementId) + Number(!!this.selectedJobTitleId) +
      Number(!!this.selectedStageId) + Number(this.selectedIsActive !== undefined);
  }

  clearFilters(): void {
    this.search = '';
    this.selectedQuestionBankTypeId = undefined;
    this.selectedManagementId = undefined;
    this.selectedJobTitleId = undefined;
    this.selectedStageId = undefined;
    this.selectedIsActive = undefined;
    this.applyFilters();
  }

  onPageChange(pageNumber: number): void {
    this.filters.update(filters => ({ ...filters, pageNumber }));
    this.loadQuestionBanks();
  }

  onPageSizeChange(pageSize: number): void {
    this.filters.update(filters => ({ ...filters, pageNumber: 1, pageSize }));
    this.loadQuestionBanks();
  }

  optionName(option: dropdownOptionsModel): string {
    const localized = this.currentLang() === 'ar'
      ? option.additionalData?.['nameAr']
      : option.additionalData?.['nameEn'];
    return typeof localized === 'string' && localized.trim() ? localized : option.name;
  }

  localizedName(ar?: string | null, en?: string | null): string {
    return (this.currentLang() === 'ar' ? ar : en) || '-';
  }

  private applyFilters(): void {
    this.filters.update(filters => ({
      ...filters,
      pageNumber: 1,
      search: this.search.trim() || undefined,
      questionBankTypeId: this.selectedQuestionBankTypeId,
      managementId: this.selectedManagementId,
      jobTitleId: this.selectedJobTitleId,
      stageId: this.selectedStageId,
      isActive: this.selectedIsActive
    }));
    this.loadQuestionBanks();
  }

  private loadLookups(): void {
    forkJoin({
      questionBankTypes: this.service.getQuestionBankTypes(),
      managements: this.service.getManagements(),
      jobTitles: this.service.getJobTitles(),
      stages: this.service.getStages()
    }).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: lookups => {
        this.questionBankTypes.set(lookups.questionBankTypes);
        this.managements.set(lookups.managements);
        this.jobTitles.set(lookups.jobTitles);
        this.stages.set(lookups.stages);
        this.loadQuestionBanks();
      },
      error: () => this.notification.error(this.translate.instant('QUESTION_BANKS.LOAD_ERROR'))
    });
  }

  private loadQuestionBanks(): void {
    this.loading.set(true);
    this.service.list(this.filters())
      .pipe(finalize(() => this.loading.set(false)), takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: response => {
          this.questionBanks.set(response.items ?? []);
          this.totalCount.set(response.metadata?.totalCount ?? 0);
        },
        error: () => this.notification.error(this.translate.instant('QUESTION_BANKS.LOAD_ERROR'))
      });
  }
}
