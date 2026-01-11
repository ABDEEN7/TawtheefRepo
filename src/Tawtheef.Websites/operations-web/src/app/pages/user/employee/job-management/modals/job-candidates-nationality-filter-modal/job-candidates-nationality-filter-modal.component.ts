import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, Validators } from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef, DialogService } from 'primeng/dynamicdialog';
import { JobLookupService } from '../../services/job-lookup.service';
import { CandidateType } from '../../../../../../core/enums/lookups.enum';
import {
  JobCandidateNationalityPercentage,
  JobCandidateTypePercentage,
} from '../../models/job-candidates-filter-settings.model';
import { JobCandidatesNationalityBreakdownDialogComponent } from '../job-candidates-nationality-breakdown/job-candidates-nationality-breakdown.dialog.component';
import { GUID } from '../../../../../../shared/types/guid.type';

@Component({
  selector: 'app-job-candidates-nationality-filter-dialog',
  templateUrl: './job-candidates-nationality-filter-modal.component.html',
  styleUrl: './job-candidates-nationality-filter-modal.component.scss',
  standalone: false,
})
export class JobCandidatesNationalityFilterModalComponent implements OnInit {
  private dialogRef = inject(DynamicDialogRef);
  private dialogConfig = inject(DynamicDialogConfig);
  private formBuilder = inject(FormBuilder);
  private dialogService = inject(DialogService);
  private lookupsService = inject(JobLookupService);

  candidateTypePercentages: JobCandidateTypePercentage[] = [];
  nationalityPercentages: JobCandidateNationalityPercentage[] = [];

  form = this.formBuilder.group({
    percentages: this.formBuilder.array([]),
  });

  get percentageControls(): FormArray {
    return this.form.get('percentages') as FormArray;
  }

  ngOnInit(): void {
    const data = this.dialogConfig.data ?? {};
    this.candidateTypePercentages = data.candidateTypePercentages ?? [];
    this.nationalityPercentages = data.nationalityPercentages ?? [];

    this.lookupsService.loadCandidateTypes().subscribe((resp) => {
      resp.forEach((candidateType) => {
        const existing = this.candidateTypePercentages.find(
          (item) => item.candidateTypeId === candidateType.id
        );
        this.percentageControls.push(
          this.formBuilder.group({
            candidateTypeId: [candidateType.id],
            percentage: [existing?.percentage ?? 0, [Validators.min(0), Validators.max(100)]],
          })
        );
      });
    });
  }

  getCandidateTypeName(candidateTypeId: GUID): string {
    return (
      this.lookupsService.candidateTypes().find((type) => type.id === candidateTypeId)?.name ?? ''
    );
  }

  hasNationalityBreakdown(candidateTypeId: GUID): boolean {
    const candidateType = this.lookupsService
      .candidateTypes()
      .find((type) => type.id === candidateTypeId);
    return (
      candidateType?.backendName === CandidateType.ResidentQatar ||
      candidateType?.backendName === CandidateType.NonQatari
    );
  }

  getNationalityBreakdownCount(candidateTypeId: GUID): number {
    return this.nationalityPercentages.filter((item) => item.candidateTypeId === candidateTypeId)
      .length;
  }

  openNationalityBreakdown(candidateTypeId: GUID): void {
    const candidateTypeName = this.getCandidateTypeName(candidateTypeId);
    const breakdowns = this.nationalityPercentages.filter(
      (item) => item.candidateTypeId === candidateTypeId
    );

    const ref = this.dialogService.open(JobCandidatesNationalityBreakdownDialogComponent, {
      header: candidateTypeName,
      width: '52rem',
      breakpoints: {
        '1200px': '70vw',
        '992px': '85vw',
        '576px': '96vw',
      },
      contentStyle: { overflow: 'hidden' },
      data: {
        candidateTypeId,
        candidateTypeName,
        breakdowns,
      },
    });

    ref?.onClose.subscribe((result) => {
      if (!result) return;
      this.nationalityPercentages = [
        ...this.nationalityPercentages.filter((item) => item.candidateTypeId !== candidateTypeId),
        ...result,
      ];
    });
  }

  save(): void {
    if (this.form.invalid) return;

    const candidateTypePercentages = this.percentageControls.value.map(
      (item: { candidateTypeId: GUID; percentage: number }) => ({
        candidateTypeId: item.candidateTypeId,
        percentage: Number(item.percentage ?? 0),
      })
    );

    this.dialogRef.close({
      candidateTypePercentages,
      nationalityPercentages: this.nationalityPercentages,
    });
  }

  close(): void {
    this.dialogRef.close();
  }
}
