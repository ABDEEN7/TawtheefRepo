import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { NationalityPreferenceRow } from '../../models/nationality-preference.model';
import { JobLookupService } from '../../services/job-lookup.service';
import { Select } from "primeng/select";
import { InputNumber } from 'primeng/inputnumber';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-job-candidates-nationality-filter-modal',
  templateUrl: './job-candidates-nationality-filter-modal.component.html',
  styleUrls: ['./job-candidates-nationality-filter-modal.component.scss'],
  imports: [Select,InputNumber,TranslatePipe,FormsModule,ReactiveFormsModule],
})
export class JobCandidatesNationalityFilterModalComponent implements OnInit {
  private fb = inject(FormBuilder);
  private ref = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig);

  lookupsService = inject(JobLookupService);

  form!: FormGroup;

  get rows(): FormArray {
    return this.form.get('rows') as FormArray;
  }

  ngOnInit(): void {
    this.lookupsService.loadNationalities?.();

    const initial: NationalityPreferenceRow[] =
      (this.config.data?.nationalityPreferences as NationalityPreferenceRow[])?.length
        ? this.config.data.nationalityPreferences
        : [{ nationalityId: null, percentage: 100 }];

    this.form = this.fb.group({
      rows: this.fb.array(initial.map((r) => this.createRow(r))),
    });
  }

  createRow(row?: NationalityPreferenceRow): FormGroup {
    return this.fb.group({
      nationalityId: [row?.nationalityId ?? null, Validators.required],
      percentage: [
        row?.percentage ?? 100,
        [Validators.required, Validators.min(0), Validators.max(100)],
      ],
    });
  }

  addRow(): void {
    this.rows.push(this.createRow({ nationalityId: null, percentage: 100 }));
  }

  removeRow(index: number): void {
    this.rows.removeAt(index);
    if (this.rows.length === 0) this.addRow();
  }

  close(): void {
    this.ref.close(null);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.value.rows as NationalityPreferenceRow[];

    const seen = new Set<string>();
    const deduped = value.filter((x) => {
      const key = String(x.nationalityId);
      if (!x.nationalityId) return true;
      if (seen.has(key)) return false;
      seen.add(key);
      return true;
    });

    this.ref.close({
      success: true,
      nationalityPreferences: deduped,
    });
  }
}
