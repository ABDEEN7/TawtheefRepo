import {CommonModule} from '@angular/common';
import {Component, computed, inject, OnInit, signal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {Select} from 'primeng/select';
import {InputSwitch} from 'primeng/inputswitch';
import {Tooltip} from 'primeng/tooltip';
import {PaginationComponent} from '../../../../shared/components/pagination/pagination.component';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {MajorsSkillsManagementService} from './services/majors-skills-management.service';
import {MajorSkillFiltersModel} from './models/major-skill-filters.model';
import {NotificationService} from '../../../../core/services/notification.service';
import {MajorFiltersModel} from './models/major-filters.model';
import {SkillFiltersModel} from './models/skill-filters.model';
import {Lang, LanguageService} from '../../../../core/services/language.service';
import {PaginationMetadata} from '../../../../core/models/pagination-metadata.model';

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
    InputSwitch,
    Tooltip,
    PaginationComponent,
    I18nNamespaceDirective
  ],
})
export class MajorsSkillsManagement implements OnInit {
  private managementService = inject(MajorsSkillsManagementService);
  private notification = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  activeTab = signal<'mapping' | 'mainMajors' | 'subMajors' | 'skills'>('mapping');

  majorSkills = this.managementService.majorSkills;
  majorSkillsMeta = this.managementService.majorSkillsMeta;

  skills = this.managementService.skills;
  skillsMeta = this.managementService.skillsMeta;

  mainMajors = this.managementService.mainMajors;
  subMajors = this.managementService.subMajors;
  skillTypes = this.managementService.skillTypes;

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
    const {pageNumber, pageSize} = this.mainMajorFilters();
    const start = (pageNumber - 1) * pageSize;
    return this.mainMajors().slice(start, start + pageSize);
  });

  subMajorsPage = computed(() => {
    const {pageNumber, pageSize} = this.subMajorFilters();
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

  loadSkillTypes() {
    this.managementService.loadSkillTypes().subscribe({
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.LOAD_ERROR'))
    });
  }

  loadMainMajors() {
    this.managementService.loadMainMajors(this.mainMajorFilters()).subscribe({
      next: () => {
        if (!this.majorSkillFilters().parentMajorId && this.mainMajors().length > 0) {
          const firstId = this.mainMajors()[0].id;
          this.majorSkillFilters.update(f => ({...f, parentMajorId: firstId}));
          this.subMajorFilters.update(f => ({...f, parentMajorId: firstId}));
          this.loadSubMajors();
        }
      },
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.LOAD_ERROR'))
    });
  }

  loadSubMajors() {
    const parentId = this.subMajorFilters().parentMajorId || this.majorSkillFilters().parentMajorId || '';
    if (!parentId) {
      this.subMajorsMeta();
      return;
    }

    this.managementService.loadSubMajors(parentId).subscribe({
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.LOAD_ERROR'))
    });
  }

  loadMajorSkills() {
    this.managementService.loadMajorSkills(this.majorSkillFilters()).subscribe({
      next: () => this.syncPagination(this.majorSkillsMeta(), this.majorSkillFilters),
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.LOAD_ERROR'))
    });
  }

  loadSkills() {
    this.managementService.loadSkills(this.skillFilters()).subscribe({
      next: () => this.syncPagination(this.skillsMeta(), this.skillFilters),
      error: () => this.notification.error(this.translate.instant('MAJORS_SKILLS.LOAD_ERROR'))
    });
  }

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
    this.managementService.updateMajorSkill({id, isSkillRequired, isActive}).subscribe({
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

  onMajorSkillPageChange(page: number) {
    this.majorSkillFilters.update(f => ({...f, pageNumber: page}));
    this.loadMajorSkills();
  }

  onMainMajorsPageChange(page: number) {
    this.mainMajorFilters.update(f => ({...f, pageNumber: page}));
  }

  onSubMajorsPageChange(page: number) {
    this.subMajorFilters.update(f => ({...f, pageNumber: page}));
  }

  onSkillsPageChange(page: number) {
    this.skillFilters.update(f => ({...f, pageNumber: page}));
    this.loadSkills();
  }

  onMajorSkillFilterChange() {
    this.majorSkillFilters.update(f => ({...f, pageNumber: 1}));
    this.loadMajorSkills();
  }

  onSkillFilterChange() {
    this.skillFilters.update(f => ({...f, pageNumber: 1}));
    this.loadSkills();
  }

  onMainMajorSearchChange() {
    this.mainMajorFilters.update(f => ({...f, pageNumber: 1}));
    this.loadMainMajors();
  }

  onSubMajorSearchChange() {
    this.subMajorFilters.update(f => ({...f, pageNumber: 1}));
    this.loadSubMajors();
  }

  updateSelectedParentMajor(value: string) {
    this.majorSkillFilters.update(f => ({...f, parentMajorId: value, subMajorId: ''}));
    this.subMajorFilters.update(f => ({...f, parentMajorId: value, pageNumber: 1}));
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
    if (!meta) {
      return;
    }
    filterSignal.update((f: any) => ({
      ...f,
      pageNumber: meta.currentPage,
      pageSize: meta.pageSize
    }));
  }
}
