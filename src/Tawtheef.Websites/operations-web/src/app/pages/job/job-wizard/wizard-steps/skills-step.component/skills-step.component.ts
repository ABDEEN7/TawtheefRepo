import {Component, inject, OnInit} from '@angular/core';
import {FormBuilder, FormArray} from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { MessageService } from 'primeng/api';
import {Job} from '../../../models/job.model';
import {debounceTime, filter} from 'rxjs';
@Component({
  selector: 'app-skills-step',
  standalone: false,
  providers: [ MessageService ],
  templateUrl: './skills-step.component.html',
  styleUrls: ['./skills-step.component.scss']
})
export class SkillsStepComponent implements WizardStepComponent,OnInit {
  fb = inject(FormBuilder);
  jobService = inject(JobService);
  messageService = inject(MessageService);

  readonly form = this.fb.group({
    items: this.fb.array<string>([])
  });

  get items() { return this.form.controls.items as FormArray; }

  newSkill = '';

  ngOnInit(): void {
    this.setJobData(this.jobService.currentJob());

    this.form.valueChanges.pipe(
      debounceTime(300),
      filter(() => this.form.valid)
    ).subscribe(() => {
      this.jobService.updateCurrentJobSkills(this.items.controls.map(c => c.value));
    });
  }

  setJobData(job: Job): void {
    if (job.skills) {
      this.items.clear();
      job.skills.forEach(skill  => {
        this.items.push(this.fb.nonNullable.control(skill));
      });
      this.form.updateValueAndValidity();
    }
  }

  add() {
    const value = this.newSkill.trim();

    if (!value) {
      this.messageService.add({
        severity: 'warn',
        summary: 'تنبيه',
        detail: 'الرجاء إدخال شرط قبل الإضافة',
        life: 2500,
      });
      return;
    }

    this.items.push(this.fb.nonNullable.control(value));
    this.newSkill = '';
  }

  remove(i: number) {
    this.items.removeAt(i);
  }

  isValid() { return this.form.valid; }

}
