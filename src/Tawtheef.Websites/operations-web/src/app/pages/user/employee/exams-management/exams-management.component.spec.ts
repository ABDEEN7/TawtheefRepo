import { TestBed } from '@angular/core/testing';
import { BehaviorSubject, of } from 'rxjs';
import { LanguageService } from '../../../../core/services/language.service';
import { ExamsManagementComponent } from './exams-management.component';
import { ExamsService } from './services/exams.service';

describe('Exams creation date range', () => {
  let component: ExamsManagementComponent;
  let service: jasmine.SpyObj<ExamsService>;

  beforeEach(() => {
    service = jasmine.createSpyObj<ExamsService>('ExamsService', [
      'list', 'getStatuses', 'getSpecializations'
    ]);
    service.list.and.returnValue(of({ items: [], metadata: {
      totalCount: 0, pageSize: 10, currentPage: 1, totalPages: 0,
      hasPreviousPage: false, hasNext: false
    } }));
    service.getStatuses.and.returnValue(of([]));
    service.getSpecializations.and.returnValue(of([]));
    TestBed.configureTestingModule({ providers: [
      { provide: ExamsService, useValue: service },
      { provide: LanguageService, useValue: {
        get: () => 'en', current$: new BehaviorSubject('en')
      } }
    ] });
    component = TestBed.runInInjectionContext(() => new ExamsManagementComponent());
    component.ngOnInit();
    service.list.calls.reset();
  });

  it('sends both local calendar dates and resets pagination', () => {
    component.onPageChange(3);
    component.createdFrom = new Date(2026, 8, 1);
    component.createdTo = new Date(2026, 8, 7);
    component.applyFilters();
    expect(service.list).toHaveBeenCalledWith(jasmine.objectContaining({
      pageNumber: 1, createdFrom: '2026-09-01', createdTo: '2026-09-07'
    }));
    expect(component.filters()).not.toEqual(jasmine.objectContaining({ createdDate: jasmine.anything() }));
  });

  it('supports Created From without Created To', () => {
    component.createdFrom = new Date(2026, 8, 1);
    component.applyFilters();
    expect(component.filters().createdFrom).toBe('2026-09-01');
    expect(component.filters().createdTo).toBeUndefined();
  });

  it('supports Created To without Created From', () => {
    component.createdTo = new Date(2026, 8, 7);
    component.applyFilters();
    expect(component.filters().createdFrom).toBeUndefined();
    expect(component.filters().createdTo).toBe('2026-09-07');
  });

  it('allows the same day even when Date values contain different times', () => {
    component.createdFrom = new Date(2026, 8, 7, 20);
    component.createdTo = new Date(2026, 8, 7, 1);
    component.applyFilters();
    expect(component.invalidCreatedRange()).toBeFalse();
    expect(service.list).toHaveBeenCalled();
  });

  it('blocks a reversed range and allows a corrected range', () => {
    component.createdFrom = new Date(2026, 8, 8);
    component.createdTo = new Date(2026, 8, 7);
    component.applyFilters();
    expect(component.invalidCreatedRange()).toBeTrue();
    expect(service.list).not.toHaveBeenCalled();
    component.createdTo = new Date(2026, 8, 9);
    component.applyFilters();
    expect(component.invalidCreatedRange()).toBeFalse();
    expect(service.list).toHaveBeenCalled();
  });

  it('clears both dates and their active filter count while retaining page size', () => {
    component.onPageSizeChange(20);
    component.createdFrom = new Date(2026, 8, 1);
    component.createdTo = new Date(2026, 8, 7);
    expect(component.activeAdvancedFilterCount()).toBe(2);
    component.clearFilters();
    expect(component.createdFrom).toBeNull();
    expect(component.createdTo).toBeNull();
    expect(component.activeAdvancedFilterCount()).toBe(0);
    expect(component.filters().pageSize).toBe(20);
    expect(component.filters().createdFrom).toBeUndefined();
    expect(component.filters().createdTo).toBeUndefined();
  });
});
