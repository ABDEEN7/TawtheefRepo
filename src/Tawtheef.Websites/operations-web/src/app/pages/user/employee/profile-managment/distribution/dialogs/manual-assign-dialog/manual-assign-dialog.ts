import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { Select } from 'primeng/select';

import {
  DistributionEmployeeLookup,
  EmployeeAvailability,
  ManualAssignRequest,
} from '../../models/profile-distribution.models';

type ManualAssignDialogData = {
  employees: DistributionEmployeeLookup[];
  selectedProfileIds: string[];
  initialEmployeeId?: string | null;
};

@Component({
  selector: 'app-manual-assign-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule,
    ButtonModule,
    InputNumberModule,
    Select,
  ],
  templateUrl: './manual-assign-dialog.html',
})
export class ManualAssignDialog {
  private ref = inject(DynamicDialogRef);
  private cfg = inject(DynamicDialogConfig);

  private data = this.cfg.data as ManualAssignDialogData;

  // Only allow active + available employees (same logic as the page)
  availableEmployees = computed(() =>
    (this.data?.employees ?? []).filter(
      e => e.isActive && e.availability === EmployeeAvailability.Available
    )
  );

  selectedCount = signal<number>(this.data?.selectedProfileIds?.length ?? 0);

  employeeId = signal<string>(this.data?.initialEmployeeId ?? '');
  count = signal<number | null>(this.data?.selectedProfileIds?.length ?? null);

  cancel(): void {
    this.ref.close(null);
  }

  confirm(): void {
    const employeeId = this.employeeId();
    const ids = this.data?.selectedProfileIds ?? [];
    if (!employeeId || ids.length === 0) {
      return;
    }

    const c = this.count();
    const payload: ManualAssignRequest = {
      employeeId,
      profileIds: c ? ids.slice(0, c) : ids,
    };

    this.ref.close({ kind: 'manual', payload });
  }
}
