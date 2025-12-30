// majors-skills-management.store.ts
import { DestroyRef, Injectable, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslateService } from '@ngx-translate/core';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';

import { NotificationService } from '../../../../core/services/notification.service';
import { LanguageService, Lang } from '../../../../core/services/language.service';

import { MajorsSkillsManagementService } from './services/majors-skills-management.service';
import { EndpointsService } from '../../../../core/http/endpoints.service';

import { PaginationMetadata } from '../../../../core/models/pagination-metadata.model';
import { PaginatedResult } from '../../../../core/models/paginated-result.model';
import { dropdownOptionsModel } from '../../../../shared/models/dropdown-options.model';

import { MajorSkillListItemModel } from './models/major-skill-list-item.model';
import { MajorSkillFiltersModel } from './models/major-skill-filters.model';
import { MajorSkillDetailsModel } from './models/major-skill-details.model';

import { MajorFiltersModel } from './models/major-filters.model';
import { MajorListItemModel } from './models/major-list-item.model';

import { SkillFiltersModel } from './models/skill-filters.model';
import { SkillListItemModel } from './models/skill-list-item.model';

import { ParentMajorInfoDialogComponent } from './dialogs/parent-major-info.dialog';
import {PaginatedRequest} from '../../../../core/models/paginated-request.model';
import { UpsertSkillDialogComponent } from "./dialogs/upsert-skill.dialog";
import {UpsertMajorDialogComponent} from './dialogs/upsert-major.dialog';
import {UpsertMajorSkillDialogComponent} from './dialogs/upsert-major-skill.dialog';

type TabKey = 'mapping' | 'mainMajors' | 'subMajors' | 'skills';

@Injectable()
export class MajorsSkillsManagementStore {
  private destroyRef = inject(DestroyRef);

  private api = inject(MajorsSkillsManagementService);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private dialog = inject(DialogService);

  public endpoints = inject(EndpointsService);

  // ===== UI State =====
  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');
  activeTab = signal<TabKey>('mapping');

  // ===== Lookups =====
  skillTypes = signal<dropdownOptionsModel[] | null>(null);

  // ===== Mapping Data =====
  majorSkills = signal<MajorSkillListItemModel[]>([]);
  majorSkillsMeta = signal<PaginationMetadata | null>(null);

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

  // ===== Lifecycle =====
  init() {
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(lang => this.currentLang.set(lang));

    this.loadSkillTypes();
    this.loadMainMajors();
    this.loadMajorSkills();
    this.loadSkills();
  }

  switchTab(tab: TabKey) {
    this.activeTab.set(tab);
    if (tab === 'subMajors') this.loadSubMajors();
  }

  // ===================== Loaders =====================
  loadMajorSkills() {
    this.api.getMajorSkills(this.majorSkillFilters()).subscribe({
      next: res => {
        this.majorSkills.set(res.items ?? []);
        this.majorSkillsMeta.set(res.metadata);
      },
      error: () => this.toast('MAJORS_SKILLS.LOAD_ERROR', true)
    });
  }

  loadMainMajors() {
    this.api.getMainMajors(this.mainMajorFilters()).subscribe({
      next: res => {
        this.mainMajorsResult.set(res);

        // auto-select first parentMajor for mapping/subMajors (optional)
        const selected = this.majorSkillFilters().parentMajorId;
        const first = this.mainMajorsItems()[0]?.id;
        if (!selected && first) {
          this.patch(this.majorSkillFilters, { parentMajorId: first, subMajorId: '', pageNumber: 1 });
          this.patch(this.subMajorFilters, { parentMajorId: first, pageNumber: 1 });
        }

        this.loadSubMajors();
        this.loadMajorSkills();
      },
      error: () => this.toast('MAJORS_SKILLS.LOAD_ERROR', true)
    });
  }

  loadSubMajors() {
    const parentId = this.subMajorFilters().parentMajorId || this.majorSkillFilters().parentMajorId;
    if (!parentId) {
      this.subMajorsResult.set(null);
      return;
    }

    this.api.getSubMajorsPaged({ ...this.subMajorFilters(), parentMajorId: parentId }).subscribe({
      next: res => this.subMajorsResult.set(res),
      error: () => this.toast('MAJORS_SKILLS.LOAD_ERROR', true)
    });
  }

  loadSkills() {
    this.api.getSkills(this.skillFilters()).subscribe({
      next: res => {
        const mapped = (res.items ?? []).map(s => ({
          ...s,
          isActive: (s as any).isActive ?? s.additionalData?.isActive ?? true
        }));
        this.skills.set(mapped);
        this.skillsMeta.set(res.metadata);
      },
      error: () => this.toast('MAJORS_SKILLS.LOAD_ERROR', true)
    });
  }

  loadSkillTypes() {
    this.api.getSkillsPageForTypes().subscribe({
      next: res => this.skillTypes.set(res),
      error: () => this.toast('MAJORS_SKILLS.LOAD_ERROR', true)
    });
  }

  // ===================== Mapping Actions =====================
  toggleMajorSkillActive(id: string, isActive: boolean) {
    this.api.changeMajorSkillActivation(id, isActive).subscribe({
      next: () => { this.toast('MAJORS_SKILLS.STATUS_UPDATED'); this.loadMajorSkills(); },
      error: () => this.toast('MAJORS_SKILLS.UPDATE_FAILED', true),
    });
  }

  changeMajorSkillRequirement(id: string, isSkillRequired: boolean, isActive: boolean) {
    this.api.updateMajorSkill({ id, isSkillRequired, isActive }).subscribe({
      next: () => { this.toast('MAJORS_SKILLS.SAVE_SUCCESS'); this.loadMajorSkills(); },
      error: () => this.toast('MAJORS_SKILLS.UPDATE_FAILED', true),
    });
  }

  // ===================== Majors Actions =====================
  toggleMajorActive(id: string, isActive: boolean) {
    this.api.changeMajorActivation(id, isActive).subscribe({
      next: () => { this.toast('MAJORS_SKILLS.STATUS_UPDATED'); this.loadMainMajors(); this.loadSubMajors(); },
      error: () => this.toast('MAJORS_SKILLS.UPDATE_FAILED', true),
    });
  }

  // ===================== Skills Actions =====================
  toggleSkillActive(id: string, isActive: boolean) {
    this.api.changeSkillActivation(id, isActive).subscribe({
      next: () => { this.toast('MAJORS_SKILLS.STATUS_UPDATED'); this.loadSkills(); },
      error: () => this.toast('MAJORS_SKILLS.UPDATE_FAILED', true),
    });
  }

  // ===================== Dialogs =====================
  openParentMajorInfo() {
    const parentId = this.majorSkillFilters().parentMajorId;
    const parent = this.mainMajorsItems().find(x => x.id === parentId);

    this.dialog.open(ParentMajorInfoDialogComponent, {
      header: this.translate.instant('MAJORS_SKILLS.PARENT_MAJOR_INFO'),
      width: '520px',
      modal: true,
      dismissableMask: true,
      data: { parentMajor: parent ?? null }
    });
  }

  openCreateMajorSkill() {
    this.dialog.open(UpsertMajorSkillDialogComponent, {
      header: this.translate.instant('MAJORS_SKILLS.CREATE_MAPPING'),
      width: '720px',
      modal: true,
      dismissableMask: true,
      data: {
        mode: 'create',
        skillTypes: this.skillTypes() ?? [],
        parentMajorId: this.majorSkillFilters().parentMajorId,
        subMajorId: this.majorSkillFilters().subMajorId
      }
    })?.onClose.subscribe((payload?: Partial<MajorSkillDetailsModel>) => {
      if (!payload) return;
      this.api.createMajorSkill(payload).subscribe({
        next: () => { this.toast('MAJORS_SKILLS.SAVE_SUCCESS'); this.loadMajorSkills(); },
        error: () => this.toast('MAJORS_SKILLS.UPDATE_FAILED', true),
      });
    });
  }

  openEditMajorSkill(id: string) {
    this.api.getMajorSkillDetails(id).subscribe({
      next: details => {
        this.dialog.open(UpsertMajorSkillDialogComponent, {
          header: this.translate.instant('MAJORS_SKILLS.EDIT_MAPPING'),
          width: '720px',
          modal: true,
          dismissableMask: true,
          data: {
            mode: 'edit',
            model: details,
            skillTypes: this.skillTypes() ?? []
          }
        })?.onClose.subscribe((payload?: Partial<MajorSkillDetailsModel>) => {
          if (!payload) return;
          this.api.updateMajorSkill(payload).subscribe({
            next: () => { this.toast('MAJORS_SKILLS.SAVE_SUCCESS'); this.loadMajorSkills(); },
            error: () => this.toast('MAJORS_SKILLS.UPDATE_FAILED', true),
          });
        });
      },
      error: () => this.toast('MAJORS_SKILLS.LOAD_ERROR', true)
    });
  }

  openCreateMajor(parentId: string | null) {
    this.dialog.open(UpsertMajorDialogComponent, {
      header: this.translate.instant(parentId ? 'MAJORS_SKILLS.CREATE_SUB_MAJOR' : 'MAJORS_SKILLS.CREATE_MAIN_MAJOR'),
      width: '640px',
      modal: true,
      dismissableMask: true,
      data: { mode: 'create', parentId }
    })?.onClose.subscribe((payload?: any) => {
      if (!payload) return;
      this.api.createMajor(payload).subscribe({
        next: () => { this.toast('MAJORS_SKILLS.SAVE_SUCCESS'); this.loadMainMajors(); this.loadSubMajors(); },
        error: () => this.toast('MAJORS_SKILLS.UPDATE_FAILED', true),
      });
    });
  }

  openEditMajor(model: MajorListItemModel) {
    this.dialog.open(UpsertMajorDialogComponent, {
      header: this.translate.instant('MAJORS_SKILLS.EDIT_MAJOR'),
      width: '640px',
      modal: true,
      dismissableMask: true,
      data: { mode: 'edit', model }
    })?.onClose.subscribe((payload?: any) => {
      if (!payload) return;
      this.api.updateMajor(payload).subscribe({
        next: () => { this.toast('MAJORS_SKILLS.SAVE_SUCCESS'); this.loadMainMajors(); this.loadSubMajors(); },
        error: () => this.toast('MAJORS_SKILLS.UPDATE_FAILED', true),
      });
    });
  }

  openCreateSkill() {
    this.dialog.open(UpsertSkillDialogComponent, {
      header: this.translate.instant('MAJORS_SKILLS.CREATE_SKILL'),
      width: '720px',
      modal: true,
      dismissableMask: true,
      data: { mode: 'create', skillTypes: this.skillTypes() ?? [] }
    })?.onClose.subscribe((payload?: any) => {
      if (!payload) return;
      this.api.createSkill(payload).subscribe({
        next: () => { this.toast('MAJORS_SKILLS.SAVE_SUCCESS'); this.loadSkills(); },
        error: () => this.toast('MAJORS_SKILLS.UPDATE_FAILED', true),
      });
    });
  }

  openEditSkill(model: SkillListItemModel) {
    this.dialog.open(UpsertSkillDialogComponent, {
      header: this.translate.instant('MAJORS_SKILLS.EDIT_SKILL'),
      width: '720px',
      modal: true,
      dismissableMask: true,
      data: { mode: 'edit', model, skillTypes: this.skillTypes() ?? [] }
    })?.onClose.subscribe((payload?: any) => {
      if (!payload) return;
      this.api.updateSkill(payload).subscribe({
        next: () => { this.toast('MAJORS_SKILLS.SAVE_SUCCESS'); this.loadSkills(); },
        error: () => this.toast('MAJORS_SKILLS.UPDATE_FAILED', true),
      });
    });
  }

  // ===================== Filter & Paging helpers =====================
  setMajorSkillSearch(v: string) { this.patch(this.majorSkillFilters, { search: v, pageNumber: 1 }); this.loadMajorSkills(); }
  setMajorSkillType(v: string)   { this.patch(this.majorSkillFilters, { skillTypeId: v, pageNumber: 1 }); this.loadMajorSkills(); }
  setMajorSkillSubMajor(v: string) { this.patch(this.majorSkillFilters, { subMajorId: v, pageNumber: 1 }); this.loadMajorSkills(); }
  setMajorSkillParent(v: string) {
    this.patch(this.majorSkillFilters, { parentMajorId: v, subMajorId: '', pageNumber: 1 });
    this.patch(this.subMajorFilters, { parentMajorId: v, pageNumber: 1 });
    this.loadSubMajors();
    this.loadMajorSkills();
  }
  setMajorSkillActiveOnly(checked: boolean) {
    this.patch(this.majorSkillFilters, { isActive: checked ? true : null, pageNumber: 1 });
    this.loadMajorSkills();
  }

  setMainMajorSearch(v: string) { this.patch(this.mainMajorFilters, { search: v, pageNumber: 1 }); this.loadMainMajors(); }
  setSubMajorSearch(v: string)  { this.patch(this.subMajorFilters, { search: v, pageNumber: 1 }); this.loadSubMajors(); }
  setSubMajorParent(v: string)  {
    this.patch(this.subMajorFilters, { parentMajorId: v, pageNumber: 1 });
    this.patch(this.majorSkillFilters, { parentMajorId: v, subMajorId: '', pageNumber: 1 });
    this.loadSubMajors();
    this.loadMajorSkills();
  }

  setSkillSearch(v: string) { this.patch(this.skillFilters, { search: v, pageNumber: 1 }); this.loadSkills(); }
  setSkillType(v: string)   { this.patch(this.skillFilters, { skillTypeId: v, pageNumber: 1 }); this.loadSkills(); }

  onMappingLazy(first: number, rows: number) {
    this.patch(this.majorSkillFilters, { pageNumber: Math.floor(first / rows) + 1, pageSize: rows });
    this.loadMajorSkills();
  }
  onMainMajorsLazy(first: number, rows: number) {
    this.patch(this.mainMajorFilters, { pageNumber: Math.floor(first / rows) + 1, pageSize: rows });
    this.loadMainMajors();
  }
  onSubMajorsLazy(first: number, rows: number) {
    this.patch(this.subMajorFilters, { pageNumber: Math.floor(first / rows) + 1, pageSize: rows });
    this.loadSubMajors();
  }
  onSkillsLazy(first: number, rows: number) {
    this.patch(this.skillFilters, { pageNumber: Math.floor(first / rows) + 1, pageSize: rows });
    this.loadSkills();
  }

  // ===================== Internal =====================
  private toast(key: string, isError = false) {
    const msg = this.translate.instant(key);
    isError ? this.notify.error(msg) : this.notify.success(msg);
  }

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
  onMainMajorsPageChange(page: number) {
    this.patch(this.mainMajorFilters, { pageNumber: page });
    this.loadMainMajors();
  }

  onSkillsPageChange(page: number) {
    this.patch(this.skillFilters, { pageNumber: page });
    this.loadSkills();
  }
  onMajorSkillPageChange(page: number) {
    this.patch(this.majorSkillFilters, { pageNumber: page });
    this.loadMajorSkills();
  }

  onSubMajorsPageChange(page: number) {
    this.patch(this.subMajorFilters, { pageNumber: page });
    this.loadSubMajors();
  }

  onMajorSkillPageSizeChange(pageSize: number) {
    this.patch(this.majorSkillFilters, { pageSize, pageNumber: 1 });
    this.loadMajorSkills();
  }

  onMainMajorsPageSizeChange(pageSize: number) {
    this.patch(this.mainMajorFilters, { pageSize, pageNumber: 1 });
    this.loadMainMajors();
  }
  onSubMajorsPageSizeChange(pageSize: number) {
    this.patch(this.subMajorFilters, { pageSize, pageNumber: 1 });
    this.loadSubMajors();
  }
  onSkillsPageSizeChange(pageSize: number) {
    this.patch(this.skillFilters, { pageSize, pageNumber: 1 });
    this.loadSkills();
  }
}
