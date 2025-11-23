import { Component, inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { JobLookupService } from '../../../services/job-lookup.service';

@Component({
  selector: 'app-review-step',
  standalone: false,
  templateUrl: './review-step.component.html',
  styleUrls: ['./review-step.component.scss']
})
export class ReviewStepComponent implements WizardStepComponent {
  fb = inject(FormBuilder);
  jobService = inject(JobService);
  lookupsService = inject(JobLookupService)

  jobSignal = this.jobService.currentJob;

  readonly form = this.fb.group({});

  downloadPdf() {
    window.print();
  }

  isValid(): boolean {
    return true;
  }
}
