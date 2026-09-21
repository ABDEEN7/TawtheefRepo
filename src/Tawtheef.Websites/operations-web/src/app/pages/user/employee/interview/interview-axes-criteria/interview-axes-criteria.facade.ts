import { DestroyRef, inject, Injectable } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslateService } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';

import { NotificationService } from '../../../../../core/services/notification.service';
import { LanguageService } from '../../../../../core/services/language.service';

import { InterviewAxesCriteriaStore, AxesCriteriaTabKey } from './interview-axes-criteria.store';
import { InterviewAxesCriteriaService } from './services/interview-axes-criteria.service';
import { AxisModel } from './models/axis.model';
import { CriterionModel } from './models/criterion.model';

import { UpsertAxisDialogComponent } from './dialogs/upsert-axis.dialog';
import { UpsertCriterionDialogComponent } from './dialogs/upsert-criterion.dialog';

@Injectable()
export class InterviewAxesCriteriaFacade {
  private destroyRef = inject(DestroyRef);
  private store = inject(InterviewAxesCriteriaStore);
  private api = inject(InterviewAxesCriteriaService);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);
  private dialog = inject(DialogService);
  private language = inject(LanguageService);

  private axisSearchChanges$ = new Subject<string>();
  private criterionSearchChanges$ = new Subject<string>();

  init() {
    this.store.setCurrentLang(this.language.get());
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((lang) => this.store.setCurrentLang(lang));

    this.setupSearchListeners();

    this.store.axesInitialized.set(true);
    this.loadAxisOptions();
    this.loadAxes();
  }

  switchTab(tab: AxesCriteriaTabKey) {
    this.store.setActiveTab(tab);

    if (tab === 'criteria' && !this.store.criteriaInitialized()) {
      this.store.criteriaInitialized.set(true);
      const defaultAxisId =
        this.store.criterionFilters().axisId || this.store.axisOptions()[0]?.value || '';
      if (defaultAxisId) {
        this.store.updateCriterionFilters({ axisId: defaultAxisId });
        this.loadCriteria(defaultAxisId);
      }
    }
  }

  private setupSearchListeners() {
    this.axisSearchChanges$
      .pipe(debounceTime(1000), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe((value) => {
        this.store.updateAxisFilters({ search: value, pageNumber: 1 });
        this.loadAxes();
      });

    this.criterionSearchChanges$
      .pipe(debounceTime(1000), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe((value) => {
        this.store.updateCriterionFilters({ search: value, pageNumber: 1 });
        this.loadCriteria();
      });
  }

  // ======== Loaders ========
  // Full active+inactive axes list used by the axis dropdowns (axis filter + create/edit criterion select).
  loadAxisOptions() {
    this.api.getAxisOptions().subscribe({
      next: (result) => {
        this.store.setAxisOptionsList(result.items);
        if (!this.store.criterionFilters().axisId && result.items.length) {
          this.store.updateCriterionFilters({ axisId: result.items[0].id });
        }
      },
    });
  }

  loadAxes() {
    this.api.getAxes(this.store.axisFilters()).subscribe({
      next: (result) => this.store.setAxesResult(result),
    });
  }

  loadCriteria(axisId: string = this.store.criterionFilters().axisId) {
    if (!axisId) {
      this.store.setCriteriaResult(null);
      return;
    }
    this.api.getCriteria({ ...this.store.criterionFilters(), axisId }).subscribe({
      next: (result) => this.store.setCriteriaResult(result),
    });
  }

  // ======== Filters / pagination (server-side) ========
  setAxisSearch(search: string) {
    this.axisSearchChanges$.next(search ?? '');
  }

  setAxisStatus(status: boolean | null) {
    this.store.updateAxisFilters({ isActive: status, pageNumber: 1 });
    this.loadAxes();
  }

  onAxisPageChange(page: number) {
    this.store.updateAxisFilters({ pageNumber: page });
    this.loadAxes();
  }

  onAxisPageSizeChange(size: number) {
    this.store.updateAxisFilters({ pageSize: size, pageNumber: 1 });
    this.loadAxes();
  }

  setCriterionSearch(search: string) {
    this.criterionSearchChanges$.next(search ?? '');
  }

  setCriterionStatus(status: boolean | null) {
    this.store.updateCriterionFilters({ isActive: status, pageNumber: 1 });
    this.loadCriteria();
  }

  setCriterionAxis(axisId: string | null) {
    const value = axisId ?? '';
    this.store.updateCriterionFilters({ axisId: value, search: '', isActive: null, pageNumber: 1 });
    this.loadCriteria(value);
  }

  onCriterionPageChange(page: number) {
    this.store.updateCriterionFilters({ pageNumber: page });
    this.loadCriteria();
  }

  onCriterionPageSizeChange(size: number) {
    this.store.updateCriterionFilters({ pageSize: size, pageNumber: 1 });
    this.loadCriteria();
  }

  // ======== Upsert dialogs ========
  openCreateAxis() {
    this.dialog
      .open(UpsertAxisDialogComponent, {
        header: this.translate.instant('INTERVIEW_AXES_CRITERIA.CREATE_AXIS'),
        data: { mode: 'create' as const },
        dismissableMask: true,
        draggable: false,
        width: '700px',
        style: { 'min-height': '450px' },
      })
      ?.onClose.subscribe((payload) => {
        if (!payload) return;
        this.api.createAxis(payload).subscribe({
          next: () => {
            this.toast('INTERVIEW_AXES_CRITERIA.SUCCESS_SAVE');
            this.loadAxes();
            this.loadAxisOptions();
          },
        });
      });
  }

  openEditAxis(axis: AxisModel) {
    this.dialog
      .open(UpsertAxisDialogComponent, {
        header: this.translate.instant('INTERVIEW_AXES_CRITERIA.EDIT_AXIS'),
        data: { mode: 'edit' as const, model: axis },
        dismissableMask: true,
        draggable: false,
        width: '700px',
        style: { 'min-height': '450px' },
      })
      ?.onClose.subscribe((payload) => {
        if (!payload) return;
        this.api.updateAxis(axis.id, payload).subscribe({
          next: () => {
            this.toast('INTERVIEW_AXES_CRITERIA.SUCCESS_SAVE');
            this.loadAxes();
            this.loadAxisOptions();
          },
        });
      });
  }

  openCreateCriterion() {
    const defaultAxisId = this.store.criterionFilters().axisId;
    this.dialog
      .open(UpsertCriterionDialogComponent, {
        header: this.translate.instant('INTERVIEW_AXES_CRITERIA.CREATE_CRITERION'),
        data: { mode: 'create' as const, axisOptions: this.store.axisOptions(), defaultAxisId },
        dismissableMask: true,
        draggable: false,
        width: '700px',
        style: { 'min-height': '450px' },
      })
      ?.onClose.subscribe((payload) => {
        if (!payload) return;
        this.api.createCriterion(payload).subscribe({
          next: () => {
            this.toast('INTERVIEW_AXES_CRITERIA.SUCCESS_SAVE');
            this.store.updateCriterionFilters({
              axisId: payload.interviewEvaluationAxisId,
              pageNumber: 1,
            });
            this.loadCriteria(payload.interviewEvaluationAxisId);
          },
        });
      });
  }

  openEditCriterion(criterion: CriterionModel) {
    this.dialog
      .open(UpsertCriterionDialogComponent, {
        header: this.translate.instant('INTERVIEW_AXES_CRITERIA.EDIT_CRITERION'),
        data: { mode: 'edit' as const, model: criterion, axisOptions: this.store.axisOptions() },
        dismissableMask: true,
        draggable: false,
        width: '700px',
        style: { 'min-height': '450px' },
      })
      ?.onClose.subscribe((payload) => {
        if (!payload) return;
        this.api.updateCriterion(criterion.id, payload).subscribe({
          next: () => {
            this.toast('INTERVIEW_AXES_CRITERIA.SUCCESS_SAVE');
            this.store.updateCriterionFilters({
              axisId: payload.interviewEvaluationAxisId,
              pageNumber: 1,
            });
            this.loadCriteria(payload.interviewEvaluationAxisId);
          },
        });
      });
  }

  // ======== Activation ========
  toggleAxisActive(axis: AxisModel, isActive: boolean) {
    this.api.changeAxisActivation(axis.id, isActive).subscribe({
      next: () => {
        this.toast('INTERVIEW_AXES_CRITERIA.STATUS_UPDATED');
        this.loadAxes();
        this.loadAxisOptions();
      },
    });
  }

  toggleCriterionActive(criterion: CriterionModel, isActive: boolean) {
    this.api.changeCriterionActivation(criterion.id, isActive).subscribe({
      next: () => {
        this.toast('INTERVIEW_AXES_CRITERIA.STATUS_UPDATED');
        this.loadCriteria(criterion.interviewEvaluationAxisId);
      },
    });
  }

  refreshAxes() {
    this.store.updateAxisFilters({ isActive: null, pageNumber: 1 });
    this.loadAxes();
  }

  refreshCriteria() {
    this.store.updateCriterionFilters({ isActive: null, pageNumber: 1 });
    this.loadCriteria();
  }

  private toast(key: string) {
    this.notify.success(this.translate.instant(key));
  }
}
