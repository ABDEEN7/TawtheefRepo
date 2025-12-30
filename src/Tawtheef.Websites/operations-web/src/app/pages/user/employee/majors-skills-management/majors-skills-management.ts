import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Select } from 'primeng/select';
import { tap } from 'rxjs/operators';

import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';

import { MajorsSkillsManagementService } from './services/majors-skills-management.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { Lang, LanguageService } from '../../../../core/services/language.service';

import { PaginationMetadata } from '../../../../core/models/pagination-metadata.model';
import { dropdownOptionsModel } from '../../../../shared/models/dropdown-options.model';

import { MajorSkillListItemModel } from './models/major-skill-list-item.model';
import { MajorSkillFiltersModel } from './models/major-skill-filters.model';

import { MajorFiltersModel } from './models/major-filters.model';
import { MajorListItemModel } from './models/major-list-item.model';

import { SkillFiltersModel } from './models/skill-filters.model';
import { SkillListItemModel } from './models/skill-list-item.model';
import {ToggleSwitch} from 'primeng/toggleswitch';

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
    PaginationComponent,
    I18nNamespaceDirective,
    ToggleSwitch
  ],
})
export class MajorsSkillsManagement implements OnInit {
  private managementService = inject(MajorsSkillsManagementService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  parentMajorName = computed(() => {
    const parentId = this.subMajorFilters().parentMajorId;
    if (!parentId) return '-';

    const major = this.mainMajors().find(m => m.id === parentId);
    return major?.name ?? '-';
  });
  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  activeTab = signal<'mapping' | 'mainMajors' | 'subMajors' | 'skills'>('mapping');

  // ======= Component State (was in service) =======
  majorSkills = signal<MajorSkillListItemModel[]>([]);
  majorSkillsMeta = signal<PaginationMetadata | null>(null);

  skills = signal<SkillListItemModel[]>([]);
  skillsMeta = signal<PaginationMetadata | null>(null);

  mainMajors = signal<MajorListItemModel[]>([]);
  subMajors = signal<dropdownOptionsModel[]>([]);
  skillTypes = signal<dropdownOptionsModel[]>([]);

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

  // Local pagination (client-side for majors/submajors arrays)
  mainMajorsMeta = computed<PaginationMetadata>(() => this.buildMetadata(
    this.mainMajors().length,
    this.mainMajorFilters().pageNumber,
    this.mainMajorFilters().pageSize
  ));

  subMajorsMeta = computed<PaginationMetadata>(() => this.buildMetadata(
    this.subMajors().length,
    this.subMajorFilters().pageNumber,
    this.subMajorFilters().pageSize
  ));

  mainMajorsPage = computed(() => {
    const { pageNumber, pageSize } = this.mainMajorFilters();
    const start = (pageNumber - 1) * pageSize;
    return this.mainMajors().slice(start, start + pageSize);
  });

  subMajorsPage = computed(() => {
    const { pageNumber, pageSize } = this.subMajorFilters();
    const start = (pageNumber - 1) * pageSize;
    return this.subMajors().slice(start, start + pageSize);
  });

  ngOnInit(): void {
    this.language.current$.subscribe(lang => this.currentLang.set(lang));
    this.loadSkillTypes();
    this.loadMainMajors();
    this.loadMajorSkills();
    this.loadSkills();
  }

  switchTab(tab: 'mapping' | 'mainMajors' | 'subMajors' | 'skills') {
    this.activeTab.set(tab);
    if (tab === 'subMajors' && this.subMajors().length === 0) {
      this.loadSubMajors();
    }
  }

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
        // keep your compatibility mapping here (component concerns)
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
      next: (res) => {
        this.mainMajors.set(res || []);
        if (!this.majorSkillFilters().parentMajorId && this.mainMajors().length > 0) {
          const firstId = this.mainMajors()[0].id;
          this.majorSkillFilters.update(f => ({ ...f, parentMajorId: firstId }));
          this.subMajorFilters.update(f => ({ ...f, parentMajorId: firstId }));
          this.loadSubMajors();
        }
      },
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.LOAD_ERROR'))
    });
  }

  loadSubMajors() {
    const parentId = this.subMajorFilters().parentMajorId || this.majorSkillFilters().parentMajorId || '';
    if (!parentId) {
      this.subMajors.set([]);
      this.subMajorsMeta(); // recompute
      return;
    }

    this.managementService.getSubMajors(parentId).subscribe({
      next: (res) => this.subMajors.set(res || []),
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.LOAD_ERROR'))
    });
  }

  loadSkillTypes() {
    this.managementService.getSkillsPageForTypes(1, 50).pipe(
      tap(res => {
        const uniqueTypes = new Map<string, dropdownOptionsModel>();
        (res.items || []).forEach(skill => {
          const skillTypeId = (skill as any).skillTypeId || skill.additionalData?.skillTypeId;
          const skillTypeName = (skill as any).skillTypeName || skill.additionalData?.skillTypeName;
          if (skillTypeId && !uniqueTypes.has(skillTypeId)) {
            uniqueTypes.set(skillTypeId, {
              id: skillTypeId,
              name: skillTypeName || '',
              backendName: skillTypeName || '',
              description: skillTypeName || '',
              additionalData: null
            } as dropdownOptionsModel);
          }
        });
        this.skillTypes.set(Array.from(uniqueTypes.values()));
      })
    ).subscribe({
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.LOAD_ERROR'))
    });
  }

  // ===== Actions remain the same but call service then refresh =====
  changeMajorSkillStatus(id: string, isActive: boolean) {
    this.managementService.changeMajorSkillActivation(id, isActive).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('MAJORS_SKILLS.STATUS_UPDATED'));
        this.loadMajorSkills();
      },
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.UPDATE_FAILED'))
    });
  }

  changeMajorSkillRequirement(id: string, isSkillRequired: boolean, isActive: boolean) {
    this.managementService.updateMajorSkill({ id, isSkillRequired, isActive }).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('MAJORS_SKILLS.SAVE_SUCCESS'));
        this.loadMajorSkills();
      },
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.UPDATE_FAILED'))
    });
  }

  changeMajorStatus(id: string, isActive: boolean) {
    this.managementService.changeMajorActivation(id, isActive).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('MAJORS_SKILLS.STATUS_UPDATED'));
        this.loadMainMajors();
      },
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.UPDATE_FAILED'))
    });
  }

  changeSkillStatus(id: string, isActive: boolean) {
    this.managementService.changeSkillActivation(id, isActive).subscribe({
      next: () => {
        this.notification.success(this.translate.instant('MAJORS_SKILLS.STATUS_UPDATED'));
        this.loadSkills();
      },
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.UPDATE_FAILED'))
    });
  }

  // ===== Pagination + Filters =====
  onMajorSkillPageChange(page: number) {
    this.majorSkillFilters.update(f => ({ ...f, pageNumber: page }));
    this.loadMajorSkills();
  }

  onMainMajorsPageChange(page: number) {
    this.mainMajorFilters.update(f => ({ ...f, pageNumber: page }));
  }

  onSubMajorsPageChange(page: number) {
    this.subMajorFilters.update(f => ({ ...f, pageNumber: page }));
  }

  onSkillsPageChange(page: number) {
    this.skillFilters.update(f => ({ ...f, pageNumber: page }));
    this.loadSkills();
  }

  onMajorSkillFilterChange() {
    this.majorSkillFilters.update(f => ({ ...f, pageNumber: 1 }));
    this.loadMajorSkills();
  }

  onSkillFilterChange() {
    this.skillFilters.update(f => ({ ...f, pageNumber: 1 }));
    this.loadSkills();
  }

  onMainMajorSearchChange() {
    this.mainMajorFilters.update(f => ({ ...f, pageNumber: 1 }));
    this.loadMainMajors();
  }

  onSubMajorSearchChange() {
    this.subMajorFilters.update(f => ({ ...f, pageNumber: 1 }));
    this.loadSubMajors();
  }

  updateSelectedParentMajor(value: string) {
    this.majorSkillFilters.update(f => ({ ...f, parentMajorId: value, subMajorId: '' }));
    this.subMajorFilters.update(f => ({ ...f, parentMajorId: value, pageNumber: 1 }));
    this.loadSubMajors();
    this.loadMajorSkills();
  }

  private buildMetadata(total: number, currentPage: number, pageSize: number): PaginationMetadata {
    const totalPages = Math.max(1, Math.ceil(total / pageSize));
    return {
      totalCount: total,
      pageSize,
      currentPage,
      totalPages,
      hasPreviousPage: currentPage > 1,
      hasNext: currentPage < totalPages
    };
  }

  private syncPagination(meta: PaginationMetadata | null, filterSignal: any) {
    if (!meta) return;
    filterSignal.update((f: any) => ({
      ...f,
      pageNumber: meta.currentPage,
      pageSize: meta.pageSize
    }));
  }
  setMajorSkillSearch(value: string) {
    this.majorSkillFilters.update(f => ({ ...f, search: value, pageNumber: 1 }));
    this.loadMajorSkills();
  }

  setMajorSkillSubMajor(subMajorId: string) {
    this.majorSkillFilters.update(f => ({ ...f, subMajorId, pageNumber: 1 }));
    this.loadMajorSkills();
  }

  setMajorSkillType(skillTypeId: string) {
    this.majorSkillFilters.update(f => ({ ...f, skillTypeId, pageNumber: 1 }));
    this.loadMajorSkills();
  }

  setMajorSkillActiveOnly(ev: Event) {
    const checked = (ev.target as HTMLInputElement | null)?.checked ?? false;
    this.majorSkillFilters.update(f => ({ ...f, isActive: checked ? true : null, pageNumber: 1 }));
    this.loadMajorSkills();
  }

  setMainMajorSearch(value: string) {
    this.mainMajorFilters.update(f => ({ ...f, search: value, pageNumber: 1 }));
    this.loadMainMajors();
  }

  setSubMajorParent(parentMajorId: string) {
    this.subMajorFilters.update(f => ({ ...f, parentMajorId, pageNumber: 1 }));
    this.updateSelectedParentMajor(parentMajorId); // يحدّث majorSkillFilters + يحمل subMajors + majorSkills
  }

  setSubMajorSearch(value: string) {
    this.subMajorFilters.update(f => ({ ...f, search: value, pageNumber: 1 }));
    this.loadSubMajors();
  }

  setSkillSearch(value: string) {
    this.skillFilters.update(f => ({ ...f, search: value, pageNumber: 1 }));
    this.loadSkills();
  }

  setSkillType(skillTypeId: string) {
    this.skillFilters.update(f => ({ ...f, skillTypeId, pageNumber: 1 }));
    this.loadSkills();
  }
}
