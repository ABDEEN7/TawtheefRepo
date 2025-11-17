import {Component, inject, OnInit, signal} from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { DialogService } from 'primeng/dynamicdialog';
import { ResidentsModalComponent } from '../../../modals/residents-modal/residents-modal.component';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import {Job} from '../../../models/job.model';
import {debounceTime, filter} from 'rxjs';
import {JobQuotas} from '../../../models/job-quotas.models';

@Component({
  selector: 'app-quotas-step',
  standalone: false,
  providers: [DialogService],
  templateUrl: './quotas-step.component.html',
  styleUrls: ['./quotas-step.component.scss']
})
export class QuotasStepComponent implements WizardStepComponent,OnInit {
  fb = inject(FormBuilder);
  jobService = inject(JobService);
  dialogService = inject(DialogService);

  readonly form = this.fb.group({
    qatariCitizens: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
    qatarMother: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
    nonQatariSpouse: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
    gcc: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
    quGrads: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
    residents: [0, [Validators.required, Validators.min(0), Validators.max(100)]]
  });

  totalQuota = signal(0);

  get residentsBreakdown() {
    return this.jobService.currentJob().quotas.residentsBreakdown || [];
  }

  ngOnInit() {
    this.setJobData(this.jobService.currentJob());

    this.form.valueChanges.pipe(
      debounceTime(300),
      filter(() => this.form.valid) // Only update service when valid
    ).subscribe(value => {
      this.calculateTotal();
        this.jobService.updateCurrentJobQuotas(value as JobQuotas);
    });
  }
  openModal() {
    const ref = this.dialogService.open(ResidentsModalComponent, {
      width: '1000px',
      modal: true,
      styleClass: 'custom-bootstrap-dialog',
      data: {
        target: this.form.controls.residents.value,
        breakdown: this.residentsBreakdown
      }
    });

    ref?.onClose.subscribe(result => {
      if (result) this.jobService.updateCurrentJobQuotas({ residentsBreakdown: result });
    });
  }

  isValid(): boolean {
    this.form.markAllAsTouched();
    return this.form.valid && this.totalQuota() === 100;
  }

  setJobData(currentJob: Job) {
    if(currentJob.quotas){
      this.form.patchValue({
        qatariCitizens: currentJob.quotas.qatariCitizens ?? 0,
        qatarMother: currentJob.quotas.qatarMother ?? 0,
        nonQatariSpouse: currentJob.quotas.nonQatariSpouse ?? 0,
        gcc: currentJob.quotas.gcc ?? 0,
        quGrads: currentJob.quotas.quGrads ?? 0,
        residents: currentJob.quotas.residents ?? 0
      });
      this.calculateTotal();
      this.form.updateValueAndValidity();
    }
  }

  private calculateTotal(): void {
    const values = this.form.value;
    const total = Object.values(values).reduce<number>((sum, val) => sum + Number(val ?? 0), 0);
    this.totalQuota.set(total);
  }
}
