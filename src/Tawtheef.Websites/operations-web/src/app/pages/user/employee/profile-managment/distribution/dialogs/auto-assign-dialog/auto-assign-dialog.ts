import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { debounceTime, defer, distinctUntilChanged, finalize, Subject, switchMap } from 'rxjs';

import { AvatarModule } from 'primeng/avatar';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';

import {
  DistributionEmployee,
  DistributionEmployeeFilters,
} from '../../models/profile-distribution.models';
import { AutoAssignRequest } from '../../models/profile-distribution-assignment.model';
import { EmployeeAvailability } from '../../models/profile-distribution.enums';
import { PaginationMetadata } from '../../../../../../../core/models/pagination-metadata.model';
import { ProfileDistributionService } from '../../services/profile-distribution.service';
import { SearchInputComponent } from '../../../../../../../shared/components/search-input/search-input.component';

type AutoAssignDialogData = {
  selectedProfileIds: string[];
  initialLimit?: number | null;
  initialEmployeeIds?: string[] | null;
};

const AVAILABILITY_META: Record<EmployeeAvailability, { labelKey: string; cssClass: string }> = {
  [EmployeeAvailability.Available]: {
    labelKey: 'distribution.availability.available',
    cssClass: 'success',
  },
  [EmployeeAvailability.OnLeave]: {
    labelKey: 'distribution.availability.leave',
    cssClass: 'warning',
  },
  [EmployeeAvailability.Suspended]: {
    labelKey: 'distribution.availability.suspended',
    cssClass: 'danger',
  },
  [EmployeeAvailability.Inactive]: {
    labelKey: 'distribution.availability.inactive',
    cssClass: 'neutral',
  },
};

const DEFAULT_EMPLOYEE_FILTERS: DistributionEmployeeFilters = {
  pageNumber: 1,
  pageSize: 10,
  sortBy: 'TotalAssigned',
  sortDirection: 'asc',
};

@Component({
  selector: 'app-auto-assign-dialog',
  standalone: true,
  imports: [
    AvatarModule,
    ButtonModule,
    CheckboxModule,
    CommonModule,
    FormsModule,
    InputNumberModule,
    SearchInputComponent,
    SelectModule,
    TableModule,
    TranslateModule,
  ],
  templateUrl: './auto-assign-dialog.html',
  styleUrl: './auto-assign-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AutoAssignDialog {
  private static readonly SEARCH_DEBOUNCE_MS = 350;

  private readonly ref = inject(DynamicDialogRef);
  private readonly cfg = inject(DynamicDialogConfig);
  private readonly api = inject(ProfileDistributionService);
  private readonly data = this.cfg.data as AutoAssignDialogData;
  private readonly searchChanges = new Subject<string>();
  private readonly queryChanges = new Subject<void>();

  readonly employees = signal<DistributionEmployee[]>([]);
  readonly employeePagination = signal<PaginationMetadata | null>(null);
  readonly employeeLoading = signal(false);
  readonly employeeFilters = signal<DistributionEmployeeFilters>({ ...DEFAULT_EMPLOYEE_FILTERS });
  readonly selectedProfileCount = this.data?.selectedProfileIds?.length ?? 0;
  readonly limit = signal<number | null>(this.data?.initialLimit ?? null);
  readonly selectedEmployeeIds = signal<Set<string>>(new Set(this.data?.initialEmployeeIds ?? []));
  readonly searchInput = signal('');

  readonly selectedCount = computed(() => this.selectedEmployeeIds().size);
  readonly selectablePageEmployees = computed(() =>
    this.employees().filter((employee) => this.isAvailable(employee)),
  );
  readonly allPageEmployeesSelected = computed(() => {
    const employees = this.selectablePageEmployees();
    return (
      employees.length > 0 &&
      employees.every((employee) => this.selectedEmployeeIds().has(employee.employeeId))
    );
  });
  readonly somePageEmployeesSelected = computed(() => {
    const employees = this.selectablePageEmployees();
    const selectedCount = employees.filter((employee) =>
      this.selectedEmployeeIds().has(employee.employeeId),
    ).length;
    return selectedCount > 0 && selectedCount < employees.length;
  });
  readonly first = computed(
    () => (this.employeeFilters().pageNumber - 1) * this.employeeFilters().pageSize,
  );

  constructor() {
    this.searchChanges
      .pipe(
        debounceTime(AutoAssignDialog.SEARCH_DEBOUNCE_MS),
        distinctUntilChanged(),
        takeUntilDestroyed(),
      )
      .subscribe((term) => {
        this.updateFilters({ searchTerm: term.trim() || undefined, pageNumber: 1 });
      });

    this.queryChanges
      .pipe(
        switchMap(() =>
          defer(() => {
            this.employeeLoading.set(true);
            return this.api
              .getEmployees(this.employeeFilters())
              .pipe(finalize(() => this.employeeLoading.set(false)));
          }),
        ),
        takeUntilDestroyed(),
      )
      .subscribe((response) => {
        this.employees.set(response.items ?? []);
        this.employeePagination.set(response.metadata);
      });

    this.queryChanges.next();
  }

  onSearch(value: string): void {
    this.searchInput.set(value);
    this.searchChanges.next(value);
  }

  clearFilters(): void {
    this.searchInput.set('');
    this.employeeFilters.set({ ...DEFAULT_EMPLOYEE_FILTERS });
    this.queryChanges.next();
  }

  isAvailable(employee: DistributionEmployee): boolean {
    return employee.isActive && employee.availability === EmployeeAvailability.Available;
  }

  isSelected(employeeId: string): boolean {
    return this.selectedEmployeeIds().has(employeeId);
  }

  setEmployeeSelected(employee: DistributionEmployee, selected: boolean): void {
    if (!this.isAvailable(employee)) return;

    const ids = new Set(this.selectedEmployeeIds());
    selected ? ids.add(employee.employeeId) : ids.delete(employee.employeeId);
    this.selectedEmployeeIds.set(ids);
  }

  setPageSelected(selected: boolean): void {
    const ids = new Set(this.selectedEmployeeIds());
    this.selectablePageEmployees().forEach((employee) => {
      selected ? ids.add(employee.employeeId) : ids.delete(employee.employeeId);
    });
    this.selectedEmployeeIds.set(ids);
  }

  initials(name: string): string {
    return (
      name
        .trim()
        .split(/\s+/)
        .slice(0, 2)
        .map((part) => part.charAt(0))
        .join('')
        .toLocaleUpperCase() || '—'
    );
  }

  availabilityLabel(value: EmployeeAvailability): string {
    return AVAILABILITY_META[value].labelKey;
  }

  availabilityClass(value: EmployeeAvailability): string {
    return AVAILABILITY_META[value].cssClass;
  }

  onTableChange(event: TableLazyLoadEvent): void {
    const pageSize = event.rows ?? this.employeeFilters().pageSize;
    const sortField = Array.isArray(event.sortField) ? event.sortField[0] : event.sortField;
    this.updateFilters({
      pageNumber: Math.floor((event.first ?? 0) / pageSize) + 1,
      pageSize,
      sortBy: sortField || this.employeeFilters().sortBy,
      sortDirection: event.sortOrder === -1 ? 'desc' : 'asc',
    });
  }

  private updateFilters(changes: Partial<DistributionEmployeeFilters>): void {
    this.employeeFilters.update((filters) => ({ ...filters, ...changes }));
    this.queryChanges.next();
  }

  cancel(): void {
    this.ref.close(null);
  }

  confirm(): void {
    const profileIds = this.data?.selectedProfileIds ?? [];
    const employeeIds = Array.from(this.selectedEmployeeIds());
    if (profileIds.length === 0 || employeeIds.length === 0) return;

    const payload: AutoAssignRequest = {
      employeeIds,
      profileIds,
      perEmployeeCount: this.limit(),
    };

    this.ref.close({ kind: 'auto', payload });
  }
}
