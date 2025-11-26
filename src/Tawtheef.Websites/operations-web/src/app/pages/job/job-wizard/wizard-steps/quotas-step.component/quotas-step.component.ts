import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { DialogService } from 'primeng/dynamicdialog';
import { ResidentsModalComponent } from '../../../modals/residents-modal/residents-modal.component';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { Job } from '../../../models/job.model';
import { debounceTime, filter } from 'rxjs';
import { JobLookupService } from '../../../services/job-lookup.service';
import { JobQuota } from '../../../models/job-quotas.models';
import { ResidentBreakdown } from '../../../models/resident-breakdown.model';

@Component({
  selector: 'app-quotas-step',
  standalone: false,
  providers: [DialogService],
  templateUrl: './quotas-step.component.html',
  styleUrls: ['./quotas-step.component.scss'],
})
export class QuotasStepComponent implements WizardStepComponent, OnInit {
  fb = inject(FormBuilder);
  jobService = inject(JobService);
  dialogService = inject(DialogService);
  lookupsService = inject(JobLookupService);

  readonly form = this.fb.group({
    qatariCitizens: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
    qatarMother: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
    nonQatariSpouse: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
    gcc: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
    quGrads: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
    residents: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
    residentsBreakdowns: this.fb.control<ResidentBreakdown[]>([]),
  });

  totalQuota = signal(0);
  quotasControls = ['qatariCitizens', 'qatarMother', 'nonQatariSpouse', 'gcc', 'quGrads'];

  get residentsBreakdowns() {
    return this.jobService.newJob().quota.residentsBreakdowns || [];
  }

  ngOnInit() {
    this.setJobData(this.jobService.newJob());

    this.form.valueChanges
      .pipe(
        debounceTime(300),
        filter(() => this.form.valid)
      )
      .subscribe((value) => {
        this.calculateTotal();
        if (this.validateResidentsBreakdown() || this.form.valid) 
          this.jobService.updateCurrentJobQuota(value as JobQuota);
      });
  }
  openModal() {
    const residentsValue = this.form.controls.residents.value || 0;

    const ref = this.dialogService.open(ResidentsModalComponent, {
      width: '1000px',
      modal: true,
      styleClass: 'custom-bootstrap-dialog',
      data: {
        target: residentsValue,
        breakdown: this.residentsBreakdowns,
      },
    });

    ref?.onClose.subscribe((result) => {
      if (result) {
        const breakdownTotal = result.reduce(
          (sum: number, item: ResidentBreakdown) => sum + item.percentage,
          0
        );
        const residentsPercentage = this.form.controls.residents.value || 0;
        this.form.controls.residentsBreakdowns.setValue(result);
        if (this.form.valid) {
          this.jobService.updateCurrentJobQuota({ residentsBreakdowns: result });
        }
      }
    });
  }

  isValid(): boolean {
    this.form.markAllAsTouched();
    return this.form.valid && this.totalQuota() === 100;
  }

  setJobData(currentJob: Job) {
    if (currentJob.quota) {
      this.form.patchValue({
        qatariCitizens: currentJob.quota.qatariCitizens ?? 0,
        qatarMother: currentJob.quota.qatarMother ?? 0,
        nonQatariSpouse: currentJob.quota.nonQatariSpouse ?? 0,
        gcc: currentJob.quota.gcc ?? 0,
        quGrads: currentJob.quota.quGrads ?? 0,
        residents: currentJob.quota.residents ?? 0,
      });
      this.calculateTotal();
      this.form.updateValueAndValidity();
    }
  }

  private calculateResidentsBreakdownTotal(): number {
    const breakdown = this.form.value.residentsBreakdowns ?? [];
    return breakdown.reduce((sum: number, item: any) => sum + Number(item.percentage ?? 0), 0);
  }
  private validateResidentsBreakdown(): boolean {
    const residents = Number(this.form.value.residents ?? 0);
    const breakdownTotal = this.calculateResidentsBreakdownTotal();
    return breakdownTotal === residents;
  }
  private calculateTotal(): void {
    
    const { qatariCitizens, qatarMother, nonQatariSpouse, gcc, quGrads, residents } =
      this.form.value;

    const total =
      Number(qatariCitizens ?? 0) +
      Number(qatarMother ?? 0) +
      Number(nonQatariSpouse ?? 0) +
      Number(gcc ?? 0) +
      Number(quGrads ?? 0) +
      Number(residents ?? 0);
    if(residents && this.validateResidentsBreakdown())
    this.totalQuota.set(total);
  }
}
