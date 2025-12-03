import { Component, inject, OnInit } from '@angular/core';
import { WizardStepComponent } from '../base/wizard-step.component';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Job } from '../../../models/job.model';
import { JobService } from '../../../services/job.service';
import { debounceTime, filter } from 'rxjs';

@Component({
  selector: 'app-overview-step',
  standalone:false,
  templateUrl: './overview-step.component.html',
  styleUrl: './overview-step.component.scss',
})
export class OverviewStepComponent implements WizardStepComponent, OnInit {
  private fb = inject(FormBuilder);
  private jobService = inject(JobService);

  

  readonly form = this.fb.group({
    overview: ['', Validators.required],
  });

  ngOnInit(): void {
      this.setJobData(this.jobService.newJob());
  
      this.form.valueChanges.pipe(
        debounceTime(300),
        filter(() => this.form.valid)
      ).subscribe(values => {
        this.jobService.updateCurrentJobOverView(values.overview ?? '');
      });
    }
  
    isValid() {
      return true;
    }
  
    setJobData(currentJob: Job) {
      if (currentJob.description) {
        this.form.patchValue({
          overview: currentJob.overview,
        });
      }
    }

}
