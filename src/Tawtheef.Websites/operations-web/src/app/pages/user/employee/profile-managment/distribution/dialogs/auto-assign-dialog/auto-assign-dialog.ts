import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { AvatarModule } from 'primeng/avatar';
import { ToggleSwitch } from 'primeng/toggleswitch';

import { DistributionEmployee } from '../../models/profile-distribution.models';
import { AutoAssignRequest } from '../../models/profile-distribution-assignment.model';
import { EmployeeAvailability } from '../../models/profile-distribution.enums';

type AutoAssignDialogData = {
  employees: DistributionEmployee[];
  selectedProfileIds: string[];
  initialLimit?: number | null;
  initialEmployeeIds?: string[] | null;
};

@Component({
  selector: 'app-auto-assign-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule,
    ButtonModule,
    InputNumberModule,
    AvatarModule,
    ToggleSwitch,
  ],
  templateUrl: './auto-assign-dialog.html',
})
export class AutoAssignDialog {
  private ref = inject(DynamicDialogRef);
  private cfg = inject(DynamicDialogConfig);

  private data = this.cfg.data as AutoAssignDialogData;

  employees = signal<DistributionEmployee[]>(this.data?.employees ?? []);
  selectedCount = signal<number>(this.data?.selectedProfileIds?.length ?? 0);

  limit = signal<number | null>(this.data?.initialLimit ?? null);
  selectedEmployeeIds = signal<Set<string>>(
    new Set(this.data?.initialEmployeeIds ?? [])
  );

  available = (e: DistributionEmployee) =>
    e.isActive && e.availability === EmployeeAvailability.Available;

  pickedCount = computed(() => this.selectedEmployeeIds().size);

  availabilityLabel(value: EmployeeAvailability): string {
    switch (value) {
      case EmployeeAvailability.Available:
        return 'distribution.availability.available';
      case EmployeeAvailability.OnLeave:
        return 'distribution.availability.leave';
      case EmployeeAvailability.Suspended:
        return 'distribution.availability.suspended';
      default:
        return 'distribution.availability.inactive';
    }
  }

  toggleEmployee(employeeId: string): void {
    const set = new Set(this.selectedEmployeeIds());
    set.has(employeeId) ? set.delete(employeeId) : set.add(employeeId);
    this.selectedEmployeeIds.set(set);
  }

  cancel(): void {
    this.ref.close(null);
  }

  confirm(): void {
    const ids = this.data?.selectedProfileIds ?? [];
    const employeeIds = Array.from(this.selectedEmployeeIds());
    if (ids.length === 0 || employeeIds.length === 0) return;

    const payload: AutoAssignRequest = {
      employeeIds,
      profileIds: ids,
      perEmployeeCount: this.limit(),
    };

    this.ref.close({ kind: 'auto', payload });
  }
}
