import { Injectable, computed, inject, signal } from '@angular/core';

import { LanguageService, Lang } from '../../../../core/services/language.service';
import { EndpointsService } from '../../../../core/http/endpoints.service';

import { PaginatedResult } from '../../../../core/models/paginated-result.model';
import { PaginationMetadata } from '../../../../core/models/pagination-metadata.model';
import { dropdownOptionsModel } from '../../../../shared/models/dropdown-options.model';

import { SectorListItemModel } from './models/sector-list-item.model';
import { ManagementListItemModel } from './models/management-list-item.model';
import { DepartmentListItemModel } from './models/department-list-item.model';
import { SectorFiltersModel } from './models/sector-filters.model';
import { ManagementFiltersModel } from './models/management-filters.model';
import { DepartmentFiltersModel } from './models/department-filters.model';

export type OrganizationTabKey = 'sectors' | 'managements' | 'departments';

@Injectable()
export class OrganizationStructuresStore {
  private language = inject(LanguageService);
  public endpoints = inject(EndpointsService);

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  activeTab = signal<OrganizationTabKey>('sectors');

  sectorsInitialized = signal(false);
  managementsInitialized = signal(false);
  departmentsInitialized = signal(false);

  sectorLookups = signal<dropdownOptionsModel[]>([]);
  managementLookups = signal<dropdownOptionsModel[]>([]);

  sectorsResult = signal<PaginatedResult<SectorListItemModel> | null>(null);
  managementsResult = signal<PaginatedResult<ManagementListItemModel> | null>(null);
  departmentsResult = signal<PaginatedResult<DepartmentListItemModel> | null>(null);

  sectors = computed(() => this.sectorsResult()?.items ?? []);
  managements = computed(() => this.managementsResult()?.items ?? []);
  departments = computed(() => this.departmentsResult()?.items ?? []);

  sectorMeta = computed(() => this.sectorsResult()?.metadata ?? this.emptyMeta(this.sectorFilters()));
  managementMeta = computed(() => this.managementsResult()?.metadata ?? this.emptyMeta(this.managementFilters()));
  departmentMeta = computed(() => this.departmentsResult()?.metadata ?? this.emptyMeta(this.departmentFilters()));

  sectorFilters = signal<SectorFiltersModel>({
    search: '',
    isActive: null,
    pageNumber: 1,
    pageSize: 10,
  });

  managementFilters = signal<ManagementFiltersModel>({
    search: '',
    isActive: null,
    sectorId: '',
    pageNumber: 1,
    pageSize: 10,
  });

  departmentFilters = signal<DepartmentFiltersModel>({
    search: '',
    isActive: null,
    sectorId: '',
    managementId: '',
    pageNumber: 1,
    pageSize: 10,
  });

  setCurrentLang(lang: Lang) {
    this.currentLang.set(lang);
  }

  setActiveTab(tab: OrganizationTabKey) {
    this.activeTab.set(tab);
  }

  setSectorLookups(options: dropdownOptionsModel[]) {
    this.sectorLookups.set(options);
  }

  setManagementLookups(options: dropdownOptionsModel[]) {
    this.managementLookups.set(options);
  }

  setSectorsResult(result: PaginatedResult<SectorListItemModel>) {
    this.sectorsResult.set(result);
  }

  setManagementsResult(result: PaginatedResult<ManagementListItemModel>) {
    this.managementsResult.set(result);
  }

  setDepartmentsResult(result: PaginatedResult<DepartmentListItemModel>) {
    this.departmentsResult.set(result);
  }

  updateSectorFilters(patch: Partial<SectorFiltersModel>) {
    this.patch(this.sectorFilters, patch);
  }

  updateManagementFilters(patch: Partial<ManagementFiltersModel>) {
    this.patch(this.managementFilters, patch);
  }

  updateDepartmentFilters(patch: Partial<DepartmentFiltersModel>) {
    this.patch(this.departmentFilters, patch);
  }

  private patch<T extends object>(sig: { update: (fn: (v: T) => T) => void }, patch: Partial<T>) {
    sig.update(v => ({ ...v, ...patch }));
  }

  private emptyMeta(filters: { pageNumber?: number; pageSize?: number }): PaginationMetadata {
    return {
      totalCount: 0,
      pageSize: filters.pageSize ?? 10,
      currentPage: filters.pageNumber ?? 1,
      totalPages: 1,
      hasPreviousPage: false,
      hasNext: false,
    };
  }
}
