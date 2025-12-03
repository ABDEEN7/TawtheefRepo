import {Component, inject, OnInit} from '@angular/core';
import {FormBuilder, Validators} from '@angular/forms';
import {JobService} from '../../../services/job.service';
import {WizardStepComponent} from '../base/wizard-step.component';
import {Job} from '../../../models/job.model';
import {debounceTime, filter} from 'rxjs';

@Component({
  selector: 'app-benefits-step',
  standalone: false,
  templateUrl: './benefits-step.component.html',
  styleUrls: ['./benefits-step.component.scss']
})
export class BenefitsStepComponent implements WizardStepComponent, OnInit {
  fb = inject(FormBuilder);
  jobService = inject(JobService);

  readonly form = this.fb.group({
    benefits: ['', Validators.required]
  });

  ngOnInit(): void {
    this.setJobData(this.jobService.newJob());

    this.form.valueChanges.pipe(
      debounceTime(300),
      filter(() => this.form.valid)
    ).subscribe(values => {
      this.jobService.updateCurrentJobBenefits(values.benefits || '');
    });
  }

  isValid() {
    return this.form.valid;
  }

  setJobData(currentJob: Job) {
    if (currentJob.description) {
      this.form.patchValue({
        benefits: currentJob.benefits
      });
    }
  }
}
