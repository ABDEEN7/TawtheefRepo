import {DestroyRef, inject, Injectable} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {TranslateService} from '@ngx-translate/core';
import {DialogService} from 'primeng/dynamicdialog';

import {NotificationService} from '../../../../core/services/notification.service';
import {LanguageService} from '../../../../core/services/language.service';
import {PaginatedResult} from '../../../../core/models/paginated-result.model';

import {OrganizationStructuresStore, OrganizationTabKey} from './organization-structures.store';
import {OrganizationStructuresService} from './services/organization-structures.service';
import {SectorListItemModel} from './models/sector-list-item.model';
import {ManagementListItemModel} from './models/management-list-item.model';
import {DepartmentListItemModel} from './models/department-list-item.model';

import {UpsertSectorDialogComponent} from './dialogs/upsert-sector.dialog';
import {UpsertManagementDialogComponent} from './dialogs/upsert-management.dialog';
import {UpsertDepartmentDialogComponent} from './dialogs/upsert-department.dialog';

@Injectable()
export class OrganizationStructuresFacade {
  private destroyRef = inject(DestroyRef);
  private store = inject(OrganizationStructuresStore);
  private api = inject(OrganizationStructuresService);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);
  private dialog = inject(DialogService);
  private language = inject(LanguageService);

  init() {
    this.store.setCurrentLang(this.language.get());
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(lang => this.store.setCurrentLang(lang));

    this.loadSectorLookups();
    this.ensureTabLoaded('sectors');
  }

  switchTab(tab: OrganizationTabKey) {
    this.store.setActiveTab(tab);
    this.ensureTabLoaded(tab);
  }

  private ensureTabLoaded(tab: OrganizationTabKey) {
    if (tab === 'sectors' && !this.store.sectorsInitialized()) {
      this.store.sectorsInitialized.set(true);
      this.loadSectors();
      return;
    }

    if (tab === 'managements' && !this.store.managementsInitialized()) {
      this.store.managementsInitialized.set(true);
      this.prepareManagementDefaults();
      return;
    }

    if (tab === 'departments' && !this.store.departmentsInitialized()) {
      this.store.departmentsInitialized.set(true);
      this.prepareDepartmentDefaults();
    }
  }

  // ======== Loaders ========
  loadSectors() {
    this.api.getSectors(this.store.sectorFilters()).subscribe({
      next: res => this.store.setSectorsResult(res)
    });
  }

  loadManagements() {
    const filters = this.store.managementFilters();
    if (!filters.sectorId) {
      this.store.setManagementsResult(this.emptyResult<ManagementListItemModel>(filters));
      return;
    }

    this.api.getManagements(filters).subscribe({
      next: res => this.store.setManagementsResult(res)
    });
  }

  loadDepartments() {
    const filters = this.store.departmentFilters();
    if (!filters.managementId) {
      this.store.setDepartmentsResult(this.emptyResult<DepartmentListItemModel>(filters));
      return;
    }

    this.api.getDepartments(filters).subscribe({
      next: res => this.store.setDepartmentsResult(res)
    });
  }

  loadSectorLookups() {
    this.api.getSectorLookups().subscribe({
      next: res => {
        this.store.setSectorLookups(res);
        const first = res[0]?.id ?? '';
        if (!this.store.managementFilters().sectorId && first) {
          this.store.updateManagementFilters({ sectorId: first });
        }
        if (!this.store.departmentFilters().sectorId && first) {
          this.store.updateDepartmentFilters({ sectorId: first });
        }
        if (this.store.managementsInitialized()) {
          this.prepareManagementDefaults(true);
        }
        if (this.store.departmentsInitialized()) {
          this.prepareDepartmentDefaults(true);
        }
      }
    });
  }

  loadManagementLookups(sectorId: string, updateDepartmentFilters = false, reloadDepartments = false) {
    if (!sectorId) {
      this.store.setManagementLookups([]);
      if (updateDepartmentFilters) {
        this.store.updateDepartmentFilters({ managementId: '' });
      }
      this.store.setManagementsResult(this.emptyResult<ManagementListItemModel>(this.store.managementFilters()));
      if (updateDepartmentFilters) {
        this.store.setDepartmentsResult(this.emptyResult<DepartmentListItemModel>(this.store.departmentFilters()));
      }
      return;
    }

    this.api.getManagementLookups(sectorId).subscribe({
      next: res => {
        this.store.setManagementLookups(res);
        if (updateDepartmentFilters && res.length > 0) {
          const first = res[0]?.id ?? '';
          this.store.updateDepartmentFilters({ managementId: first });
        } else if (updateDepartmentFilters && res.length === 0) {
          this.store.updateDepartmentFilters({ managementId: '' });
        }
        if (reloadDepartments) {
          this.loadDepartments();
        }
      }
    });
  }

  // ======== Filters / Pagination ========
  setSectorSearch(search: string) {
    this.store.updateSectorFilters({ search, pageNumber: 1 });
    this.loadSectors();
  }

  setSectorStatus(status: boolean | null) {
    this.store.updateSectorFilters({ isActive: status, pageNumber: 1 });
    this.loadSectors();
  }

  onSectorPageChange(page: number) {
    this.store.updateSectorFilters({ pageNumber: page });
    this.loadSectors();
  }

  onSectorPageSizeChange(size: number) {
    this.store.updateSectorFilters({ pageSize: size, pageNumber: 1 });
    this.loadSectors();
  }

  setManagementSearch(search: string) {
    this.store.updateManagementFilters({ search, pageNumber: 1 });
    this.loadManagements();
  }

  setManagementStatus(status: boolean | null) {
    this.store.updateManagementFilters({ isActive: status, pageNumber: 1 });
    this.loadManagements();
  }

  setManagementSector(sectorId: string | null) {
    const value = sectorId ?? '';
    this.store.updateManagementFilters({ sectorId: value, pageNumber: 1 });
    this.store.updateDepartmentFilters({ sectorId: value, managementId: '', pageNumber: 1 });
    this.loadManagementLookups(value, true, this.store.departmentsInitialized());
    this.loadManagements();
  }

  onManagementPageChange(page: number) {
    this.store.updateManagementFilters({ pageNumber: page });
    this.loadManagements();
  }

  onManagementPageSizeChange(size: number) {
    this.store.updateManagementFilters({ pageSize: size, pageNumber: 1 });
    this.loadManagements();
  }

  setDepartmentSearch(search: string) {
    this.store.updateDepartmentFilters({ search, pageNumber: 1 });
    this.loadDepartments();
  }

  setDepartmentStatus(status: boolean | null) {
    this.store.updateDepartmentFilters({ isActive: status, pageNumber: 1 });
    this.loadDepartments();
  }

  setDepartmentSector(sectorId: string | null) {
    const value = sectorId ?? '';
    this.store.updateDepartmentFilters({ sectorId: value, managementId: '', pageNumber: 1 });
    this.loadManagementLookups(value, true, true);
  }

  setDepartmentManagement(managementId: string | null) {
    this.store.updateDepartmentFilters({ managementId: managementId ?? '', pageNumber: 1 });
    this.loadDepartments();
  }

  onDepartmentPageChange(page: number) {
    this.store.updateDepartmentFilters({ pageNumber: page });
    this.loadDepartments();
  }

  onDepartmentPageSizeChange(size: number) {
    this.store.updateDepartmentFilters({ pageSize: size, pageNumber: 1 });
    this.loadDepartments();
  }

  // ======== Upsert dialogs ========
  openCreateSector() {
    this.dialog.open(UpsertSectorDialogComponent, {
      header: this.translate.instant('ORG_STRUCTURES.CREATE_SECTOR'),
      data: { mode: 'create' as const },
      dismissableMask: true,
      draggable: false,
    })?.onClose.subscribe(payload => {
      if (!payload) return;
      this.api.createSector(payload).subscribe({
        next: () => {
          this.toast('ORG_STRUCTURES.SUCCESS_SAVE');
          this.loadSectors();
          this.loadSectorLookups();
        }
      });
    });
  }

  openEditSector(sector: SectorListItemModel) {
    this.dialog.open(UpsertSectorDialogComponent, {
      header: this.translate.instant('ORG_STRUCTURES.EDIT_SECTOR'),
      data: { mode: 'edit' as const, model: sector },
      dismissableMask: true,
      draggable: false,
    })?.onClose.subscribe(payload => {
      if (!payload) return;
      this.api.updateSector(sector.id, payload).subscribe({
        next: () => {
          this.toast('ORG_STRUCTURES.SUCCESS_SAVE');
          this.loadSectors();
          this.loadSectorLookups();
        }
      });
    });
  }

  openCreateManagement() {
    this.dialog.open(UpsertManagementDialogComponent, {
      header: this.translate.instant('ORG_STRUCTURES.CREATE_MANAGEMENT'),
      data: { mode: 'create' as const, sectors: this.store.sectorLookups() },
      dismissableMask: true,
      draggable: false,
    })?.onClose.subscribe(payload => {
      if (!payload) return;
      this.api.createManagement(payload).subscribe({
        next: () => {
          this.toast('ORG_STRUCTURES.SUCCESS_SAVE');
          this.prepareManagementDefaults(true);
          this.loadSectorLookups();
        }
      });
    });
  }

  openEditManagement(management: ManagementListItemModel) {
    this.dialog.open(UpsertManagementDialogComponent, {
      header: this.translate.instant('ORG_STRUCTURES.EDIT_MANAGEMENT'),
      data: { mode: 'edit' as const, model: management, sectors: this.store.sectorLookups() },
      dismissableMask: true,
      draggable: false,
    })?.onClose.subscribe(payload => {
      if (!payload) return;
      this.api.updateManagement(management.id, payload).subscribe({
        next: () => {
          this.toast('ORG_STRUCTURES.SUCCESS_SAVE');
          this.prepareManagementDefaults(true);
          this.loadSectorLookups();
        }
      });
    });
  }

  openCreateDepartment() {
    this.dialog.open(UpsertDepartmentDialogComponent, {
      header: this.translate.instant('ORG_STRUCTURES.CREATE_DEPARTMENT'),
      data: {
        mode: 'create' as const,
        sectors: this.store.sectorLookups(),
        managements: this.store.managementLookups(),
      },
      dismissableMask: true,
      draggable: false,
    })?.onClose.subscribe(payload => {
      if (!payload) return;
      this.api.createDepartment(payload).subscribe({
        next: () => {
          this.toast('ORG_STRUCTURES.SUCCESS_SAVE');
          this.prepareDepartmentDefaults(true);
        }
      });
    });
  }

  openEditDepartment(department: DepartmentListItemModel) {
    this.dialog.open(UpsertDepartmentDialogComponent, {
      header: this.translate.instant('ORG_STRUCTURES.EDIT_DEPARTMENT'),
      data: {
        mode: 'edit' as const,
        model: department,
        sectors: this.store.sectorLookups(),
        managements: this.store.managementLookups(),
      },
      dismissableMask: true,
      draggable: false,
    })?.onClose.subscribe(payload => {
      if (!payload) return;
      this.api.updateDepartment(department.id, payload).subscribe({
        next: () => {
          this.toast('ORG_STRUCTURES.SUCCESS_SAVE');
          this.prepareDepartmentDefaults(true);
        }
      });
    });
  }

  // ======== Activation ========
  toggleSectorActive(sector: SectorListItemModel, isActive: boolean) {
    this.api.changeSectorActivation(sector.id, isActive, true).subscribe({
      next: () => {
        this.toast('ORG_STRUCTURES.STATUS_UPDATED');
        this.loadSectors();
        if (this.store.managementsInitialized()) this.prepareManagementDefaults(true);
        if (this.store.departmentsInitialized()) this.prepareDepartmentDefaults(true);
      }
    });
  }

  toggleManagementActive(management: ManagementListItemModel, isActive: boolean) {
    this.api.changeManagementActivation(management.id, isActive, true).subscribe({
      next: () => {
        this.toast('ORG_STRUCTURES.STATUS_UPDATED');
        this.loadManagements();
        if (this.store.departmentsInitialized()) this.prepareDepartmentDefaults(true);
      }
    });
  }

  toggleDepartmentActive(department: DepartmentListItemModel, isActive: boolean) {
    this.api.changeDepartmentActivation(department.id, isActive).subscribe({
      next: () => {
        this.toast('ORG_STRUCTURES.STATUS_UPDATED');
        this.loadDepartments();
      }
    });
  }

  // ======== Helpers ========
  private prepareManagementDefaults(reloadList = false) {
    const sectorId = this.store.managementFilters().sectorId || this.store.sectorLookups()[0]?.id || '';
    if (sectorId) {
      this.store.updateManagementFilters({ sectorId, pageNumber: 1 });
      this.loadManagementLookups(sectorId, true, this.store.departmentsInitialized());
      this.loadManagements();
    }
  }

  private prepareDepartmentDefaults(reloadList = false) {
    const sectorId = this.store.departmentFilters().sectorId || this.store.sectorLookups()[0]?.id || '';
    this.store.updateDepartmentFilters({ sectorId });

    if (sectorId) {
      this.loadManagementLookups(sectorId, true, reloadList || this.store.departmentsInitialized());
    }

    const shouldReload = reloadList || this.store.departmentsInitialized();
    if (shouldReload && !reloadList) {
      this.loadDepartments();
    }
  }

  private toast(key: string, isError = false) {
    const message = this.translate.instant(key);
    if (isError) this.notify.error(message);
    else this.notify.success(message);
  }

  private emptyResult<T>(filters: { pageNumber?: number; pageSize?: number }): PaginatedResult<T> {
    return {
      items: [],
      metadata: {
        totalCount: 0,
        pageSize: filters.pageSize ?? 10,
        currentPage: filters.pageNumber ?? 1,
        totalPages: 1,
        hasPreviousPage: false,
        hasNext: false,
      },
    };
  }
}
