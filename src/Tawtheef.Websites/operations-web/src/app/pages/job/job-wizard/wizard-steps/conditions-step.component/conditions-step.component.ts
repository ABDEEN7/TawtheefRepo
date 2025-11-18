import {Component, inject, OnInit} from '@angular/core';
import {FormBuilder, FormArray, Validators} from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { MessageService } from 'primeng/api';
import {Job} from '../../../models/job.model';
import {debounceTime, filter} from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
@Component({
  selector: 'app-conditions-step',
  standalone: false,
  providers:[MessageService],
  templateUrl: './conditions-step.component.html'
})
export class ConditionsStepComponent implements WizardStepComponent,OnInit {
  fb = inject(FormBuilder);
  jobService = inject(JobService);
  messageService = inject(MessageService);
  translationService = inject(TranslateService)

  readonly form = this.fb.group({
    items: this.fb.array<string>([],[Validators.required])
  });

  get items() { return this.form.controls.items as FormArray; }

  newCond = '';

  ngOnInit() {
    this.setJobData(this.jobService.currentJob());

    this.form.valueChanges.pipe(
      debounceTime(300),
      filter(() => this.form.valid) // Only update service when valid
    ).subscribe(() => {
      this.jobService.updateCurrentJobConditions(this.items.controls.map(c => c.value));
    });
  }

  setJobData(job: Job): void {
    if (job.conditions) {
      this.items.clear();
        job.conditions.forEach(cond  => {
          this.items.push(this.fb.nonNullable.control(cond));
        });
      this.form.updateValueAndValidity();
    }
  }

  add() {
    const value = this.newCond.trim();

    if (!value) {
    this.messageService.add({
      severity: 'warn',
      summary: this.translationService.instant('validation.warning'),
      detail: this.translationService.instant('validation.add_job_cond_detail'),
      life: 2500,
    });
    return;
  }

    this.items.push(this.fb.nonNullable.control(value));
    this.newCond = '';
  }

  remove(i: number) {
    this.items.removeAt(i);
  }

  isValid() { return this.form.valid }

}
