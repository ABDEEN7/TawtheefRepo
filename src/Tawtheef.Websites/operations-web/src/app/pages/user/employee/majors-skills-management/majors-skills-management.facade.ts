import {DestroyRef, inject, Injectable} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {TranslateService} from '@ngx-translate/core';
import {DialogService} from 'primeng/dynamicdialog';

import {NotificationService} from '../../../../core/services/notification.service';
import {LanguageService} from '../../../../core/services/language.service';
import {ConfirmationService} from 'primeng/api';

import {MajorsSkillsManagementService} from './services/majors-skills-management.service';
import {MajorsSkillsManagementStore, MajorsSkillsTabKey} from './majors-skills-management.store';

import {MajorSkillDetailsModel} from './models/major-skill-details.model';
import {MajorListItemModel} from './models/major-list-item.model';
import {SkillListItemModel} from './models/skill-list-item.model';

import {ParentMajorInfoDialogComponent} from './dialogs/parent-major-info.dialog';
import {UpsertSkillDialogComponent} from './dialogs/upsert-skill.dialog';
import {UpsertMajorDialogComponent} from './dialogs/upsert-major.dialog';
import {UpsertMajorSkillDialogComponent} from './dialogs/upsert-major-skill.dialog';

type ActivationHierarchyEntity = 'major' | 'subMajor' | 'skill';

@Injectable()
export class MajorsSkillsManagementFacade {
  private destroyRef = inject(DestroyRef);

  private store = inject(MajorsSkillsManagementStore);
  private api = inject(MajorsSkillsManagementService);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private dialog = inject(DialogService);
  private confirm = inject(ConfirmationService);

  private hierarchyMessages: Record<ActivationHierarchyEntity, string> = {
    major: 'MAJORS_SKILLS.APPLY_ON_MAPPING_MESSAGE_MAJOR',
    subMajor: 'MAJORS_SKILLS.APPLY_ON_MAPPING_MESSAGE_SUB_MAJOR',
    skill: 'MAJORS_SKILLS.APPLY_ON_MAPPING_MESSAGE_SKILL'
  };

  init() {
    this.store.setCurrentLang(this.language.get());

    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(lang => this.store.setCurrentLang(lang));

    // Shared lookup (used by mapping + skills)
    this.loadSkillTypes();

    // Load only the default tab initially (mapping)
    this.ensureTabLoaded('mapping');
  }

  switchTab(tab: MajorsSkillsTabKey) {
    this.store.setActiveTab(tab);
    this.ensureTabLoaded(tab);
  }
  private ensureTabLoaded(tab: MajorsSkillsTabKey) {
    switch (tab) {
      case 'mapping':
        if (this.store.mappingInitialized()) return;
        this.store.mappingInitialized.set(true);
        this.loadMajorSkills();
        return;

      case 'mainMajors':
        if (this.store.mainMajorsInitialized()) return;
        this.store.mainMajorsInitialized.set(true);

        this.loadMainMajors(); // only main majors list
        return;

      case 'subMajors':
        if (this.store.subMajorsInitialized()) return;
        this.store.subMajorsInitialized.set(true);

        // subMajors tab needs main majors to select parent
        this.loadMainMajorsForSubMajorsTab();
        this.loadSubMajors(); // only sub majors list (if parent set)
        return;

      case 'skills':
        if (this.store.skillsInitialized()) return;
        this.store.skillsInitialized.set(true);

        this.loadSkills(); // only skills list
        return;
    }
  }
  // ===================== Loaders =====================

  private loadMainMajorsForMappingDefaults() {
    this.api.getMainMajors(this.store.mainMajorFilters()).subscribe({
      next: res => {
        this.store.setMainMajorsResult(res);

        const selectedParent = this.store.majorSkillFilters().parentMajorId;
        const firstParent = this.store.mainMajorsItems()[0]?.id;

        if (!selectedParent && firstParent) {
          this.store.updateMajorSkillFilters({ parentMajorId: firstParent, subMajorId: '', pageNumber: 1 });
        }
      }
    });
  }
  private loadMainMajorsForSubMajorsTab() {
    this.api.getMainMajors(this.store.mainMajorFilters()).subscribe({
      next: res => {
        this.store.setMainMajorsResult(res);

        const selectedParent = this.store.subMajorFilters().parentMajorId;
        const firstParent = this.store.mainMajorsItems()[0]?.id;

        if (!selectedParent && firstParent) {
          // ONLY update subMajors tab filters
          this.store.updateSubMajorFilters({ parentMajorId: firstParent, pageNumber: 1 });
        }
      }
    });
  }

  loadMajorSkills() {
    this.api.getMajorSkills(this.store.majorSkillFilters()).subscribe({
      next: res => this.store.setMajorSkills(res)
    });
  }
  loadMainMajors() {
    this.api.getMainMajors(this.store.mainMajorFilters()).subscribe({
      next: res => this.store.setMainMajorsResult(res)
    });
  }

  loadSubMajors() {
    const parentId = this.store.subMajorFilters().parentMajorId || this.store.majorSkillFilters().parentMajorId;
    if (!parentId) {
      this.store.setSubMajorsResult(null);
      return;
    }

    this.api.getSubMajorsPaged({ ...this.store.subMajorFilters(), parentMajorId: parentId }).subscribe({
      next: res => this.store.setSubMajorsResult(res)
    });
  }

  loadSkills() {
    this.api.getSkills(this.store.skillFilters()).subscribe({
      next: res => this.store.setMappedSkills(res)
    });
  }

  loadSkillTypes() {
    this.api.getSkillsPageForTypes().subscribe({
      next: res => this.store.setSkillTypes(res)
    });
  }

  // ===================== Mapping Actions =====================
  toggleMajorSkillActive(id: string, isActive: boolean) {
    this.api.changeMajorSkillActivation(id, isActive).subscribe({
      next: () => { this.toast('MAJORS_SKILLS.STATUS_UPDATED'); this.loadMajorSkills(); }
    });
  }

  changeMajorSkillRequirement(id: string, isSkillRequired: boolean, isActive: boolean) {
    this.api.updateMajorSkill({ id, isSkillRequired, isActive }).subscribe({
      next: () => { this.toast('MAJORS_SKILLS.SAVE_SUCCESS'); this.loadMajorSkills(); }
    });
  }

  // ===================== Majors Actions =====================
  toggleMajorActive(major: { id: string; parentId?: string | null }, isActive: boolean) {
    const proceed = (applyOnHierarchy: boolean) => {
      this.api.changeMajorActivation(major.id, isActive, applyOnHierarchy).subscribe({
        next: () => {
          this.toast('MAJORS_SKILLS.STATUS_UPDATED');

          const tab = this.store.activeTab();

          if (tab === 'mainMajors') {
            this.loadMainMajors();
          } else if (tab === 'subMajors') {
            this.loadSubMajors();
          } else if (tab === 'mapping') {
            this.loadMajorSkills();
          } else {
            // do nothing
          }
        }
      });
    };

    if (isActive) this.askApplyOnHierarchy(major.parentId ? 'subMajor' : 'major', proceed);
    else proceed(false);
  }

  // ===================== Skills Actions =====================
  toggleSkillActive(skill: SkillListItemModel, isActive: boolean) {
    const proceed = (applyOnHierarchy: boolean) => {
      this.api.changeSkillActivation(skill.id, isActive, applyOnHierarchy).subscribe({
        next: () => { this.toast('MAJORS_SKILLS.STATUS_UPDATED'); this.loadSkills(); this.loadMajorSkills(); }
      });
    };

    if (isActive) {
      this.askApplyOnHierarchy('skill', proceed);
    } else {
      proceed(false);
    }
  }

  // ===================== Dialogs =====================
  openParentMajorInfo() {
    const parentId = this.store.majorSkillFilters().parentMajorId;
    const parent = this.store.mainMajorsItems().find(x => x.id === parentId);

    this.dialog.open(ParentMajorInfoDialogComponent, {
      header: this.translate.instant('MAJORS_SKILLS.PARENT_MAJOR_INFO'),
      width: '520px',
      modal: true,
      dismissableMask: true,
      draggable: false,
      data: { parentMajor: parent ?? null }
    });
  }

  openCreateMajorSkill() {
    this.dialog.open(UpsertMajorSkillDialogComponent, {
      header: this.translate.instant('MAJORS_SKILLS.CREATE_MAPPING'),
      width: '720px',
      modal: true,
      dismissableMask: true,
      draggable: false,
      data: {
        mode: 'create',
        skillTypes: this.store.skillTypes() ?? [],
        parentMajorId: this.store.majorSkillFilters().parentMajorId,
        subMajorId: this.store.majorSkillFilters().subMajorId
      }
    })?.onClose.subscribe((payload?: Partial<MajorSkillDetailsModel>) => {
      if (!payload) return;
      this.api.createMajorSkill(payload).subscribe({
        next: () => { this.toast('MAJORS_SKILLS.SAVE_SUCCESS'); this.loadMajorSkills(); }
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
          draggable: false,
          data: {
            mode: 'edit',
            model: details,
            skillTypes: this.store.skillTypes() ?? []
          }
        })?.onClose.subscribe((payload?: Partial<MajorSkillDetailsModel>) => {
          if (!payload) return;
          this.api.updateMajorSkill(payload).subscribe({
            next: () => { this.toast('MAJORS_SKILLS.SAVE_SUCCESS'); this.loadMajorSkills(); }
          });
        });
      }
    });
  }

  openCreateMajor(parentId: string | null, parent: string | null) {
    this.dialog.open(UpsertMajorDialogComponent, {
      header: this.translate.instant(parentId ? 'MAJORS_SKILLS.CREATE_SUB_MAJOR' : 'MAJORS_SKILLS.CREATE_MAIN_MAJOR'),
      width: '640px',
      modal: true,
      dismissableMask: true,
      draggable: false,
      data: { mode: 'create', parentId, parent }
    })?.onClose.subscribe((payload?: any) => {
      if (!payload) return;
      this.api.createMajor(payload).subscribe({
        next: () => { this.toast('MAJORS_SKILLS.SAVE_SUCCESS'); this.loadMainMajors(); this.loadSubMajors(); }
      });
    });
  }

  openEditMajor(model: MajorListItemModel) {
    this.dialog.open(UpsertMajorDialogComponent, {
      header: this.translate.instant('MAJORS_SKILLS.EDIT_MAJOR'),
      width: '640px',
      modal: true,
      dismissableMask: true,
      draggable: false,
      data: { mode: 'edit', model }
    })?.onClose.subscribe((payload?: any) => {
      if (!payload) return;
      this.api.updateMajor(payload).subscribe({
        next: () => { this.toast('MAJORS_SKILLS.SAVE_SUCCESS'); this.loadMainMajors(); this.loadSubMajors(); }
      });
    });
  }

  openCreateSkill() {
    this.dialog.open(UpsertSkillDialogComponent, {
      header: this.translate.instant('MAJORS_SKILLS.CREATE_SKILL'),
      width: '720px',
      modal: true,
      dismissableMask: true,
      draggable: false,
      data: { mode: 'create', skillTypes: this.store.skillTypes() ?? [] }
    })?.onClose.subscribe((payload?: any) => {
      if (!payload) return;
      this.api.createSkill(payload).subscribe({
        next: () => { this.toast('MAJORS_SKILLS.SAVE_SUCCESS'); this.loadSkills(); }
      });
    });
  }

  openEditSkill(model: SkillListItemModel) {
    this.dialog.open(UpsertSkillDialogComponent, {
      header: this.translate.instant('MAJORS_SKILLS.EDIT_SKILL'),
      width: '720px',
      modal: true,
      dismissableMask: true,
      draggable: false,
      data: { mode: 'edit', model, skillTypes: this.store.skillTypes() ?? [] }
    })?.onClose.subscribe((payload?: any) => {
      if (!payload) return;
      this.api.updateSkill(payload).subscribe({
        next: () => { this.toast('MAJORS_SKILLS.SAVE_SUCCESS'); this.loadSkills(); }
      });
    });
  }

  // ===================== Filter & Paging helpers =====================
  setMajorSkillSearch(v: string) { this.store.updateMajorSkillFilters({ search: v, pageNumber: 1 }); this.loadMajorSkills(); }
  setMajorSkillType(v: string)   { this.store.updateMajorSkillFilters({ skillTypeId: v, pageNumber: 1 }); this.loadMajorSkills(); }
  setMajorSkillSubMajor(v: string) { this.store.updateMajorSkillFilters({ subMajorId: v, pageNumber: 1 }); this.loadMajorSkills(); }
  setMajorSkillParent(v: string | undefined) {
    this.store.updateMajorSkillFilters({ parentMajorId: v, subMajorId: '', pageNumber: 1 });
    this.loadMajorSkills();
  }
  setMajorSkillActiveOnly(checked: boolean) {
    this.store.updateMajorSkillFilters({ isActive: checked ? true : null, pageNumber: 1 });
    this.loadMajorSkills();
  }

  setMainMajorSearch(v: string) { this.store.updateMainMajorFilters({ search: v, pageNumber: 1 }); this.loadMainMajors(); }
  setSubMajorSearch(v: string)  { this.store.updateSubMajorFilters({ search: v, pageNumber: 1 }); this.loadSubMajors(); }
  setSubMajorParent(parent: MajorListItemModel | null) {
    this.store.selectedParentMajor.set(parent);
    this.store.updateSubMajorFilters({ parentMajorId: parent?.id ?? '', pageNumber: 1 });
    this.loadSubMajors();
  }

  setSkillSearch(v: string) { this.store.updateSkillFilters({ search: v, pageNumber: 1 }); this.loadSkills(); }
  setSkillType(v: string)   { this.store.updateSkillFilters({ skillTypeId: v, pageNumber: 1 }); this.loadSkills(); }

  onMappingLazy(first: number, rows: number) {
    this.store.updateMajorSkillFilters({ pageNumber: Math.floor(first / rows) + 1, pageSize: rows });
    this.loadMajorSkills();
  }
  onMainMajorsLazy(first: number, rows: number) {
    this.store.updateMainMajorFilters({ pageNumber: Math.floor(first / rows) + 1, pageSize: rows });
    this.loadMainMajors();
  }
  onSubMajorsLazy(first: number, rows: number) {
    this.store.updateSubMajorFilters({ pageNumber: Math.floor(first / rows) + 1, pageSize: rows });
    this.loadSubMajors();
  }
  onSkillsLazy(first: number, rows: number) {
    this.store.updateSkillFilters({ pageNumber: Math.floor(first / rows) + 1, pageSize: rows });
    this.loadSkills();
  }

  onMainMajorsPageChange(page: number) {
    this.store.updateMainMajorFilters({ pageNumber: page });
    this.loadMainMajors();
  }

  onSkillsPageChange(page: number) {
    this.store.updateSkillFilters({ pageNumber: page });
    this.loadSkills();
  }
  onMajorSkillPageChange(page: number) {
    this.store.updateMajorSkillFilters({ pageNumber: page });
    this.loadMajorSkills();
  }

  onSubMajorsPageChange(page: number) {
    this.store.updateSubMajorFilters({ pageNumber: page });
    this.loadSubMajors();
  }

  onMajorSkillPageSizeChange(pageSize: number) {
    this.store.updateMajorSkillFilters({ pageSize, pageNumber: 1 });
    this.loadMajorSkills();
  }

  onMainMajorsPageSizeChange(pageSize: number) {
    this.store.updateMainMajorFilters({ pageSize, pageNumber: 1 });
    this.loadMainMajors();
  }
  onSubMajorsPageSizeChange(pageSize: number) {
    this.store.updateSubMajorFilters({ pageSize, pageNumber: 1 });
    this.loadSubMajors();
  }
  onSkillsPageSizeChange(pageSize: number) {
    this.store.updateSkillFilters({ pageSize, pageNumber: 1 });
    this.loadSkills();
  }

  private toast(key: string, isError = false) {
    const msg = this.translate.instant(key);
    isError ? this.notify.error(msg) : this.notify.success(msg);
  }

  private askApplyOnHierarchy(entity: ActivationHierarchyEntity, onDecision: (applyOnHierarchy: boolean) => void) {
    this.confirm.confirm({
      header: this.translate.instant('MAJORS_SKILLS.APPLY_ON_MAPPING_TITLE'),
      message: this.translate.instant(this.hierarchyMessages[entity]),
      acceptLabel: this.translate.instant('MAJORS_SKILLS.APPLY_ON_MAPPING_CONFIRM'),
      rejectLabel: this.translate.instant('MAJORS_SKILLS.APPLY_ON_MAPPING_REJECT'),
      defaultFocus: 'reject',
      accept: () => onDecision(true),
      reject: () => onDecision(false)
    });
  }
}
