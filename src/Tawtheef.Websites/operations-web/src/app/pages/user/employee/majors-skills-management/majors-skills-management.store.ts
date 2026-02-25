// majors-skills-management.store.ts
import { Injectable, computed, inject, signal } from '@angular/core';

import { EndpointsService } from '../../../../core/http/endpoints.service';
import { LanguageService, Lang } from '../../../../core/services/language.service';

import { PaginationMetadata } from '../../../../core/models/pagination-metadata.model';
import { PaginatedResult } from '../../../../core/models/paginated-result.model';
import { PaginatedRequest } from '../../../../core/models/paginated-request.model';
import { dropdownOptionsModel } from '../../../../shared/models/dropdown-options.model';

import { MajorSkillListItemModel } from './models/major-skill-list-item.model';
import { MajorSkillFiltersModel } from './models/major-skill-filters.model';
import { MajorFiltersModel } from './models/major-filters.model';
import { MajorListItemModel } from './models/major-list-item.model';

import { SkillFiltersModel } from './models/skill-filters.model';
import { SkillListItemModel } from './models/skill-list-item.model';

export type MajorsSkillsTabKey = 'mapping' | 'mainMajors' | 'subMajors' | 'skills';

@Injectable()
export class MajorsSkillsManagementStore {
  private language = inject(LanguageService);
  public endpoints = inject(EndpointsService);

  mappingInitialized = signal(false);
  mainMajorsInitialized = signal(false);
  subMajorsInitialized = signal(false);
  skillsInitialized = signal(false);

  // ===== UI State =====
  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  activeTab = signal<MajorsSkillsTabKey>('mapping');

  // ===== Lookups =====
  skillTypes = signal<dropdownOptionsModel[] | null>(null);

  // ===== Mapping Data =====
  majorSkillsResult = signal<PaginatedResult<MajorSkillListItemModel> | null>(null);
  majorSkillsItems = computed(() => this.majorSkillsResult()?.items ?? []);
  majorSkillsMeta = computed(() => this.majorSkillsResult()?.metadata ?? this.emptyMeta(this.majorSkillFilters()));

  // ===== Main Majors =====
  mainMajorsResult = signal<PaginatedResult<MajorListItemModel> | null>(null);
  mainMajorsItems = computed(() => this.mainMajorsResult()?.items ?? []);
  mainMajorsMeta = computed(() => this.mainMajorsResult()?.metadata ?? this.emptyMeta(this.mainMajorFilters()));

  // ===== Sub Majors =====
  subMajorsResult = signal<PaginatedResult<MajorListItemModel> | null>(null);
  subMajorsItems = computed(() => this.subMajorsResult()?.items ?? []);
  subMajorsMeta = computed(() => this.subMajorsResult()?.metadata ?? this.emptyMeta(this.subMajorFilters()));

  // ===== Skills =====
  skills = signal<SkillListItemModel[]>([]);
  skillsMeta = signal<PaginationMetadata | null>(null);

  // ===== Filters =====
  majorSkillFilters = signal<MajorSkillFiltersModel>({
    search: '',
    parentMajorId: '',
    subMajorId: '',
    skillTypeId: '',
    isActive: null,
    pageNumber: 1,
    pageSize: 10
  });

  mainMajorFilters = signal<MajorFiltersModel>({
    search: '',
    pageNumber: 1,
    pageSize: 10
  });

  subMajorFilters = signal<MajorFiltersModel>({
    search: '',
    parentMajorId: '',
    pageNumber: 1,
    pageSize: 10
  });

  skillFilters = signal<SkillFiltersModel>({
    search: '',
    skillTypeId: '',
    pageNumber: 1,
    pageSize: 10
  });

  // ===== Derived =====
  parentMajorName = computed(() => {
    const parentId = this.subMajorFilters().parentMajorId || this.majorSkillFilters().parentMajorId;
    if (!parentId) return '-';
    return this.mainMajorsItems().find(m => m.id === parentId)?.name ?? '-';
  });

  /** when mapping filters chooses a sub-major, show info button for its parent */
  showParentMajorInfoButton = computed(() => !!this.majorSkillFilters().subMajorId && !!this.majorSkillFilters().parentMajorId);
  selectedParentMajor = signal<MajorListItemModel | null>(null);

  selectedParentMajorId = computed(() => this.selectedParentMajor()?.id ?? '');
  selectedParentMajorName = computed(() => this.selectedParentMajor()?.name ?? '-');
  // ===== State Writers =====
  setCurrentLang(lang: Lang) {
    this.currentLang.set(lang);
  }

  setActiveTab(tab: MajorsSkillsTabKey) {
    this.activeTab.set(tab);
  }

  setSkillTypes(types: dropdownOptionsModel[]) {
    this.skillTypes.set(types);
  }

  setMajorSkills(result: PaginatedResult<MajorSkillListItemModel>) {
    this.majorSkillsResult.set(result);
  }

  setMainMajorsResult(result: PaginatedResult<MajorListItemModel>) {
    this.mainMajorsResult.set(result);
  }

  setSubMajorsResult(result: PaginatedResult<MajorListItemModel> | null) {
    this.subMajorsResult.set(result);
  }

  setSkills(result: PaginatedResult<SkillListItemModel>) {
    this.skills.set(result.items ?? []);
    this.skillsMeta.set(result.metadata ?? null);
  }

  setMappedSkills(result: PaginatedResult<SkillListItemModel>) {
    const mapped = (result.items ?? []).map(s => ({
      ...s,
      isActive: (s as any).isActive ?? s.additionalData?.isActive ?? true
    }));

    this.skills.set(mapped);
    this.skillsMeta.set(result.metadata ?? null);
  }

  updateMajorSkillFilters(patch: Partial<MajorSkillFiltersModel>) {
    this.patch(this.majorSkillFilters, patch);
  }

  updateMainMajorFilters(patch: Partial<MajorFiltersModel>) {
    this.patch(this.mainMajorFilters, patch);
  }

  updateSubMajorFilters(patch: Partial<MajorFiltersModel>) {
    this.patch(this.subMajorFilters, patch);
  }

  updateSkillFilters(patch: Partial<SkillFiltersModel>) {
    this.patch(this.skillFilters, patch);
  }

  // ===================== Internal =====================
  private patch<T extends object>(sig: { update: (fn: (v: T) => T) => void }, patch: Partial<T>) {
    sig.update(v => ({ ...v, ...patch }));
  }

  private emptyMeta(filters: PaginatedRequest): PaginationMetadata {
    return {
      totalCount: 0,
      pageSize: filters.pageSize ?? 10,
      currentPage: filters.pageNumber ?? 1,
      totalPages: 1,
      hasPreviousPage: (filters.pageNumber ?? 1) > 1,
      hasNext: false,
    };
  }
}
