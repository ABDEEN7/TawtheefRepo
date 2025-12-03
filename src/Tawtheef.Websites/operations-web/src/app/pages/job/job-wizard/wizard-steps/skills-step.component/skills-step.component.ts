import { Component, inject, OnInit } from '@angular/core';
import { WizardStepComponent } from '../base/wizard-step.component';
import { FormBuilder, Validators, FormArray } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { MessageService } from 'primeng/api';
import { debounceTime, filter } from 'rxjs';
import { Job } from '../../../models/job.model';
import { JobService } from '../../../services/job.service';
import { JobSkill } from '../../../models/job-skill.model';
import { JobLookupService } from '../../../services/job-lookup.service';
import { Lookups } from '../../../../../core/models/lookups.model';

@Component({
  selector: 'app-skills-step',
  standalone: false,
  templateUrl: './skills-step.component.html',
  styleUrls: ['./skills-step.component.scss'],
})
export class SkillsStepComponent implements WizardStepComponent, OnInit {
  fb = inject(FormBuilder);
  jobService = inject(JobService);
  messageService = inject(MessageService);
  translationService = inject(TranslateService);
  lookupsService = inject(JobLookupService)

  skillsOptions : Lookups[] = this.lookupsService.skills();
  skills: JobSkill[] = [];
  readonly form = this.fb.group({
    jobSkills: this.fb.array<any>([], [Validators.required])
  });

  get items() { return this.form.controls.jobSkills as FormArray; }

  ngOnInit() {
    this.setJobData(this.jobService.newJob());

    this.form.valueChanges.pipe(
      debounceTime(300),
      filter(() => this.form.valid)
    ).subscribe(() => {
      this.jobService.updateCurrentJobSkills(this.items.value);
    });
  }


  createSkillFormGroup(skill: JobSkill = { skillId: undefined, showToApplicants: false }) {
    return this.fb.group({
      skillId: [skill.skillId, [Validators.required]],
      showToApplicants: [skill.showToApplicants || false]
    });
  }

  addSkill() {
    this.items.push(this.createSkillFormGroup());
  }

  removeSkill(i: number) {
    this.items.removeAt(i);
  }

  setJobData(job: Job): void {
    if (job.skills && job.skills.length > 0) {
      this.items.clear();
      job.skills.forEach((jobSkill: JobSkill) => {
        this.items.push(this.createSkillFormGroup(jobSkill));
      });
      this.form.updateValueAndValidity();
    }
  }

  isValid() { 
    return true; 
  }
}