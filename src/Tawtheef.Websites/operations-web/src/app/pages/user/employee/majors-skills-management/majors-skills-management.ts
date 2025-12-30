import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit, signal, DestroyRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Select } from 'primeng/select';
import { ToggleSwitch } from 'primeng/toggleswitch';
import { tap } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';
import { RemoteSelectComponent } from '../../../../shared/components/remote-select/remote-select';

import { MajorsSkillsManagementService } from './services/majors-skills-management.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { Lang, LanguageService } from '../../../../core/services/language.service';
import { EndpointsService } from '../../../../core/http/endpoints.service';

import { PaginationMetadata } from '../../../../core/models/pagination-metadata.model';
import { dropdownOptionsModel } from '../../../../shared/models/dropdown-options.model';

import { MajorSkillListItemModel } from './models/major-skill-list-item.model';
import { MajorSkillFiltersModel } from './models/major-skill-filters.model';

import { MajorFiltersModel } from './models/major-filters.model';
import { MajorListItemModel } from './models/major-list-item.model';

import { SkillFiltersModel } from './models/skill-filters.model';
import { SkillListItemModel } from './models/skill-list-item.model';
import { PaginatedResult } from '../../../../core/models/paginated-result.model';

type TabKey = 'mapping' | 'mainMajors' | 'subMajors' | 'skills';

@Component({
  selector: 'app-majors-skills-management',
  standalone: true,
  templateUrl: './majors-skills-management.html',
  styleUrls: ['./majors-skills-management.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    Select,
    ToggleSwitch,
    PaginationComponent,
    I18nNamespaceDirective,
    RemoteSelectComponent,
  ],
})
export class MajorsSkillsManagement implements OnInit {
  private destroyRef = inject(DestroyRef);

  private managementService = inject(MajorsSkillsManagementService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  public endpoints = inject(EndpointsService);

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  activeTab = signal<TabKey>('mapping');

  // ======= Data =======
  majorSkills = signal<MajorSkillListItemModel[]>([]);
  majorSkillsMeta = signal<PaginationMetadata | null>(null);

  skills = signal<SkillListItemModel[]>([]);
  skillsMeta = signal<PaginationMetadata | null>(null);

  mainMajorsResult = signal<PaginatedResult<MajorListItemModel> | null>(null);
  subMajorsResult = signal<PaginatedResult<MajorListItemModel> | null>(null);

  mainMajorsItems = computed(() => this.mainMajorsResult()?.items ?? []);
  subMajorsItems = computed(() => this.subMajorsResult()?.items ?? []);

  mainMajorsMeta = computed(() => this.mainMajorsResult()?.metadata ?? this.emptyMeta(this.mainMajorFilters()));
  subMajorsMeta = computed(() => this.subMajorsResult()?.metadata ?? this.emptyMeta(this.subMajorFilters()));

  skillTypes = signal<dropdownOptionsModel[] | null>(null);

  // ======= Filters =======
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
    pageSize: 5
  });

  subMajorFilters = signal<MajorFiltersModel>({
    search: '',
    parentMajorId: '',
    pageNumber: 1,
    pageSize: 5
  });

  skillFilters = signal<SkillFiltersModel>({
    search: '',
    skillTypeId: '',
    pageNumber: 1,
    pageSize: 10
  });

  parentMajorName = computed(() => {
    const parentId = this.subMajorFilters().parentMajorId;
    if (!parentId) return '-';
    return this.mainMajorsItems().find(m => m.id === parentId)?.name ?? '-';
  });

  ngOnInit(): void {
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(lang => this.currentLang.set(lang));

    this.loadSkillTypes();
    this.loadMainMajors();     // will auto-pick first parent and load subMajors + majorSkills
    this.loadMajorSkills();
    this.loadSkills();
  }

  switchTab(tab: TabKey) {
    this.activeTab.set(tab);

    // Lazy load sub majors only when needed
    if (tab === 'subMajors') this.loadSubMajors();
  }

  // ===================== Loaders =====================
  loadMajorSkills() {
    this.managementService.getMajorSkills(this.majorSkillFilters()).pipe(
      tap(res => {
        this.majorSkills.set(res.items || []);
        this.majorSkillsMeta.set(res.metadata);
      })
    ).subscribe({
      next: () => this.syncPagination(this.majorSkillsMeta(), this.majorSkillFilters),
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.LOAD_ERROR'))
    });
  }

  loadSkills() {
    this.managementService.getSkills(this.skillFilters()).pipe(
      tap(res => {
        const mapped = (res.items || []).map(s => ({
          ...s,
          isActive: (s as any).isActive ?? s.additionalData?.isActive ?? true,
          skillTypeId: (s as any).skillTypeId ?? s.additionalData?.skillTypeId,
          skillTypeName: (s as any).skillTypeName ?? s.additionalData?.skillTypeName
        }));
        this.skills.set(mapped);
        this.skillsMeta.set(res.metadata);
      })
    ).subscribe({
      next: () => this.syncPagination(this.skillsMeta(), this.skillFilters),
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.LOAD_ERROR'))
    });
  }

  loadMainMajors() {
    this.managementService.getMainMajors(this.mainMajorFilters()).subscribe({
      next: res => {
        this.mainMajorsResult.set(res);

        // auto-select first parentMajor if not selected
        const selected = this.majorSkillFilters().parentMajorId;
        const first = this.mainMajorsItems()[0]?.id;
        if (!selected && first) {
          this.patchSignal(this.majorSkillFilters, { parentMajorId: first, subMajorId: '', pageNumber: 1 });
          this.patchSignal(this.subMajorFilters, { parentMajorId: first, pageNumber: 1 });
        }

        // ensure sub majors + mapping refresh
        this.loadSubMajors();
        this.loadMajorSkills();
      },
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.LOAD_ERROR'))
    });
  }

  loadSubMajors() {
    const parentId = this.subMajorFilters().parentMajorId || this.majorSkillFilters().parentMajorId;
    if (!parentId) {
      this.subMajorsResult.set(null);
      return;
    }

    // IMPORTANT:
    // Ideally your API should be: getSubMajors(filters) => PaginatedResult
    // If you only have getSubMajors(parentId) today, pagination/search will remain limited.
    // Below assumes you have a paginated endpoint; if not, adjust service accordingly.
    this.managementService.getSubMajorsPaged({
      ...this.subMajorFilters(),
      parentMajorId: parentId
    }).subscribe({
      next: res => this.subMajorsResult.set(res),
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.LOAD_ERROR'))
    });
  }

  loadSkillTypes() {
    this.managementService.getSkillsPageForTypes().subscribe({
      next: res => this.skillTypes.set(res),
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.LOAD_ERROR'))
    });
  }

  // ===================== Actions =====================
  changeMajorSkillStatus(id: string, isActive: boolean) {
    this.managementService.changeMajorSkillActivation(id, isActive).subscribe({
      next: () => { this.toast('MAJORS_SKILLS.STATUS_UPDATED'); this.loadMajorSkills(); },
      error: () => this.toast('MAJORS_SKILLS.UPDATE_FAILED', true),
    });
  }

  changeMajorSkillRequirement(id: string, isSkillRequired: boolean, isActive: boolean) {
    this.managementService.updateMajorSkill({ id, isSkillRequired, isActive }).subscribe({
      next: () => { this.toast('MAJORS_SKILLS.SAVE_SUCCESS'); this.loadMajorSkills(); },
      error: () => this.toast('MAJORS_SKILLS.UPDATE_FAILED', true),
    });
  }

  changeMajorStatus(id: string, isActive: boolean) {
    this.managementService.changeMajorActivation(id, isActive).subscribe({
      next: () => { this.toast('MAJORS_SKILLS.STATUS_UPDATED'); this.loadMainMajors(); },
      error: () => this.toast('MAJORS_SKILLS.UPDATE_FAILED', true),
    });
  }

  changeSkillStatus(id: string, isActive: boolean) {
    this.managementService.changeSkillActivation(id, isActive).subscribe({
      next: () => { this.toast('MAJORS_SKILLS.STATUS_UPDATED'); this.loadSkills(); },
      error: () => this.toast('MAJORS_SKILLS.UPDATE_FAILED', true),
    });
  }

  // ===================== Pagination =====================
  onMajorSkillPageChange(page: number) {
    this.patchSignal(this.majorSkillFilters, { pageNumber: page });
    this.loadMajorSkills();
  }

  onMainMajorsPageChange(page: number) {
    this.patchSignal(this.mainMajorFilters, { pageNumber: page });
    this.loadMainMajors();
  }

  onSubMajorsPageChange(page: number) {
    this.patchSignal(this.subMajorFilters, { pageNumber: page });
    this.loadSubMajors();
  }

  onSkillsPageChange(page: number) {
    this.patchSignal(this.skillFilters, { pageNumber: page });
    this.loadSkills();
  }

  // ===================== Filter setters =====================
  setMajorSkillSearch(value: string) {
    this.patchSignal(this.majorSkillFilters, { search: value, pageNumber: 1 });
    this.loadMajorSkills();
  }

  updateSelectedParentMajor(value: string) {
    this.patchSignal(this.majorSkillFilters, { parentMajorId: value, subMajorId: '', pageNumber: 1 });
    this.patchSignal(this.subMajorFilters, { parentMajorId: value, pageNumber: 1 });

    this.loadSubMajors();
    this.loadMajorSkills();
  }

  setMajorSkillSubMajor(subMajorId: string) {
    this.patchSignal(this.majorSkillFilters, { subMajorId, pageNumber: 1 });
    this.loadMajorSkills();
  }

  setMajorSkillType(skillTypeId: string) {
    this.patchSignal(this.majorSkillFilters, { skillTypeId, pageNumber: 1 });
    this.loadMajorSkills();
  }

  setMajorSkillActiveOnly(ev: Event) {
    const checked = (ev.target as HTMLInputElement | null)?.checked ?? false;
    this.patchSignal(this.majorSkillFilters, { isActive: checked ? true : null, pageNumber: 1 });
    this.loadMajorSkills();
  }

  setMainMajorSearch(value: string) {
    this.patchSignal(this.mainMajorFilters, { search: value, pageNumber: 1 });
    this.loadMainMajors();
  }

  setSubMajorParent(parentMajorId: string) {
    this.patchSignal(this.subMajorFilters, { parentMajorId, pageNumber: 1 });
    this.patchSignal(this.majorSkillFilters, { parentMajorId, subMajorId: '', pageNumber: 1 });

    this.loadSubMajors();
    this.loadMajorSkills();
  }

  setSubMajorSearch(value: string) {
    this.patchSignal(this.subMajorFilters, { search: value, pageNumber: 1 });
    this.loadSubMajors();
  }

  setSkillSearch(value: string) {
    this.patchSignal(this.skillFilters, { search: value, pageNumber: 1 });
    this.loadSkills();
  }

  setSkillType(skillTypeId: string) {
    this.patchSignal(this.skillFilters, { skillTypeId, pageNumber: 1 });
    this.loadSkills();
  }

  // ===================== Helpers =====================
  private toast(key: string, isError = false) {
    const msg = this.translate.instant(key);
    isError ? this.notification.error(msg) : this.notification.success(msg);
  }

  private patchSignal<T extends object>(sig: { update: (fn: (v: T) => T) => void }, patch: Partial<T>) {
    sig.update(v => ({ ...v, ...patch }));
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

  private syncPagination(meta: PaginationMetadata | null, filterSignal: { update: (fn: (v: any) => any) => void }) {
    if (!meta) return;
    filterSignal.update(f => ({ ...f, pageNumber: meta.currentPage, pageSize: meta.pageSize }));
  }
}
