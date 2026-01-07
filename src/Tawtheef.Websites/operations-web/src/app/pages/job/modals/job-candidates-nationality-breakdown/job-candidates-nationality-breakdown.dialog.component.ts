import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { JobLookupService } from '../../services/job-lookup.service';
import { JobCandidateNationalityPercentage } from '../../models/job-candidates-filter-settings.model';
import { GUID } from '../../../../shared/types/guid.type';

@Component({
  selector: 'app-job-candidates-nationality-breakdown-dialog',
  templateUrl: './job-candidates-nationality-breakdown.dialog.component.html',
  styleUrl: './job-candidates-nationality-breakdown.dialog.component.scss',
  standalone: false,
})
export class JobCandidatesNationalityBreakdownDialogComponent implements OnInit {
  private dialogRef = inject(DynamicDialogRef);
  private dialogConfig = inject(DynamicDialogConfig);
  private formBuilder = inject(FormBuilder);
  lookupsService = inject(JobLookupService);

  candidateTypeId!: GUID;

  form = this.formBuilder.group({
    breakdowns: this.formBuilder.array([]),
  });

  get breakdownControls(): FormArray {
    return this.form.get('breakdowns') as FormArray;
  }

  ngOnInit(): void {
    const data = this.dialogConfig.data ?? {};
    this.candidateTypeId = data.candidateTypeId;
    const breakdowns: JobCandidateNationalityPercentage[] = data.breakdowns ?? [];

    if (breakdowns.length === 0) {
      this.addBreakdown();
      return;
    }

    breakdowns.forEach((breakdown) => {
      this.breakdownControls.push(
        this.formBuilder.group({
          nationalityId: [breakdown.nationalityId, Validators.required],
          percentage: [breakdown.percentage, [Validators.min(0), Validators.max(100)]],
        })
      );
    });
  }

  addBreakdown(): void {
    this.breakdownControls.push(
      this.formBuilder.group({
        nationalityId: [null, Validators.required],
        percentage: [0, [Validators.min(0), Validators.max(100)]],
      })
    );
  }

  removeBreakdown(index: number): void {
    this.breakdownControls.removeAt(index);
  }

  save(): void {
    if (this.form.invalid) return;

    const breakdowns = this.breakdownControls.value.map(
      (item: { nationalityId: GUID; percentage: number }) => ({
        candidateTypeId: this.candidateTypeId,
        nationalityId: item.nationalityId,
        percentage: Number(item.percentage ?? 0),
      })
    );

    this.dialogRef.close(breakdowns);
  }

  close(): void {
    this.dialogRef.close();
  }
}
