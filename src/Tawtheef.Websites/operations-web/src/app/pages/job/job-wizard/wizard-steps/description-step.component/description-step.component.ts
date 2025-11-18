import {Component, inject, OnInit} from '@angular/core';
import {FormBuilder, Validators} from '@angular/forms';
import {JobService} from '../../../services/job.service';
import {WizardStepComponent} from '../base/wizard-step.component';
import {Job} from '../../../models/job.model';
import {debounceTime, filter} from 'rxjs';

@Component({
  selector: 'app-description-step',
  standalone: false,
  templateUrl: './description-step.component.html',
  styleUrls: ['./description-step.component.scss']
})
export class DescriptionStepComponent implements WizardStepComponent, OnInit {
  fb = inject(FormBuilder);
  jobService = inject(JobService);

  readonly form = this.fb.group({
    desc: ['', Validators.required],
    benefits: ['', Validators.required]
  });

  ngOnInit(): void {
    this.setJobData(this.jobService.currentJob());

    this.form.valueChanges.pipe(
      debounceTime(300),
      filter(() => this.form.valid) // Only update service when valid
    ).subscribe(values => {
      this.jobService.updateCurrentJobDescription(values.desc ?? '', values.benefits ?? '');
    });
  }

  isValid() {
    return this.form.valid;
  }

  setJobData(currentJob: Job) {
    if (currentJob.description) {
      this.form.patchValue({
        desc: currentJob.description,
        benefits: currentJob.benefits
      });
    }
  }
}
