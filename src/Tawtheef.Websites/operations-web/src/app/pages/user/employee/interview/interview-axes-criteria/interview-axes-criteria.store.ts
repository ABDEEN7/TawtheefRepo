import { Injectable, computed, inject, signal } from '@angular/core';

import { LanguageService, Lang } from '../../../../../core/services/language.service';
import { PaginationMetadata } from '../../../../../core/models/pagination-metadata.model';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';

import { AxisModel } from './models/axis.model';
import { CriterionModel } from './models/criterion.model';
import { AxisFiltersModel } from './models/axis-filters.model';
import { CriterionFiltersModel } from './models/criterion-filters.model';

export type AxesCriteriaTabKey = 'axes' | 'criteria';

export interface AxisOption {
  label: string;
  value: string;
}

@Injectable()
export class InterviewAxesCriteriaStore {
  private language = inject(LanguageService);

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  activeTab = signal<AxesCriteriaTabKey>('axes');

  axesInitialized = signal(false);
  criteriaInitialized = signal(false);

  // Search, status filter and pagination are all applied server-side.
  private axesResult = signal<PaginatedResult<AxisModel> | null>(null);
  private criteriaResult = signal<PaginatedResult<CriterionModel> | null>(null);

  // Full active+inactive axes list (no search/status filter), used for the axis dropdowns only.
  private axisOptionsList = signal<AxisModel[]>([]);

  axisFilters = signal<AxisFiltersModel>({ search: '', isActive: null, pageNumber: 1, pageSize: 10 });
  criterionFilters = signal<CriterionFiltersModel>({ axisId: '', search: '', isActive: null, pageNumber: 1, pageSize: 10 });

  axisOptions = computed<AxisOption[]>(() =>
    this.axisOptionsList().map(a => ({ label: (this.isRtl() ? a.nameAr : (a.nameEn || a.nameAr)), value: a.id }))
  );

  axes = computed(() => this.axesResult()?.items ?? []);
  axisMeta = computed(() => this.axesResult()?.metadata ?? this.emptyMeta(this.axisFilters()));

  criteria = computed(() => this.criteriaResult()?.items ?? []);
  criteriaMeta = computed(() => this.criteriaResult()?.metadata ?? this.emptyMeta(this.criterionFilters()));

  getAxisById(id: string): AxisModel | undefined {
    return this.axisOptionsList().find(a => a.id === id) ?? this.axes().find(a => a.id === id);
  }

  setCurrentLang(lang: Lang) {
    this.currentLang.set(lang);
  }

  setActiveTab(tab: AxesCriteriaTabKey) {
    this.activeTab.set(tab);
  }

  setAxesResult(result: PaginatedResult<AxisModel>) {
    this.axesResult.set(result);
  }

  setAxisOptionsList(axes: AxisModel[]) {
    this.axisOptionsList.set(axes);
  }

  setCriteriaResult(result: PaginatedResult<CriterionModel> | null) {
    this.criteriaResult.set(result);
  }

  updateAxisFilters(patch: Partial<AxisFiltersModel>) {
    this.axisFilters.update(v => ({ ...v, ...patch }));
  }

  updateCriterionFilters(patch: Partial<CriterionFiltersModel>) {
    this.criterionFilters.update(v => ({ ...v, ...patch }));
  }

  private emptyMeta(filters: { pageNumber: number; pageSize: number }): PaginationMetadata {
    return {
      totalCount: 0,
      pageSize: filters.pageSize,
      currentPage: filters.pageNumber,
      totalPages: 1,
      hasPreviousPage: filters.pageNumber > 1,
      hasNext: false,
    };
  }
}
