import { Component, inject, OnInit } from '@angular/core';
import { WizardStepComponent } from '../base/wizard-step.component';
import { FormBuilder, Validators, FormArray, FormGroup } from '@angular/forms';
import { Job } from '../../../models/job.model';
import { JobService } from '../../../services/job.service';
import { JobSkill } from '../../../models/job-skill.model';
import { JobLookupService } from '../../../services/job-lookup.service';
import { GUID } from '../../../../../shared/types/guid.type';

@Component({
  selector: 'app-skills-step',
  standalone: false,
  templateUrl: './skills-step.component.html',
  styleUrls: ['./skills-step.component.scss'],
})
export class SkillsStepComponent extends WizardStepComponent implements OnInit {
  private fb = inject(FormBuilder);
  protected jobService = inject(JobService);
  protected lookupsService = inject(JobLookupService);

  jobData!: Job;

  readonly form = this.fb.group({
    jobSkills: this.fb.array([]),
  }) as FormGroup;
  disabled: boolean = false;

  ngOnInit() {
    const currentJob = this.jobService.getCurrentJob();
    if (currentJob) {
      if (currentJob.majorId) {
        this.lookupsService.loadSkillsByMajor(currentJob.majorId);
      }
    }

    this.form.valueChanges.subscribe(() => {
      this.updateJobData();
    });
  }

  get jobSkillsArray(): FormArray {
    return this.form.get('jobSkills') as FormArray;
  }

  getSkillGroup(index: number): FormGroup {
    return this.jobSkillsArray.at(index) as FormGroup;
  }

  addSkill() {
    const skillGroup = this.fb.group({
      skillId: ['', [Validators.required]],
      showToApplicants: [true],
    });

    this.jobSkillsArray.push(skillGroup);
  }

  removeSkill(i: number) {
    this.jobSkillsArray.removeAt(i);
  }

  setJobData(job: Job, disable: boolean = false): void {
    this.jobData = job;
    this.jobSkillsArray.clear();

    if (job.skills?.length) {
      job.skills.forEach((skill: JobSkill) => {
        const skillGroup = this.fb.group({
          skillId: [skill.skillId, [Validators.required]],
          showToApplicants: [skill.showToApplicants || false],
        });

        this.jobSkillsArray.push(skillGroup);
      });
    }

    this.disabled =disable

  }

  isValid() {
    if (this.jobSkillsArray.length === 0) {
      return true;
    }

    return this.form.valid;
  }

  private updateJobData(): void {
    const skills = this.jobSkillsArray.controls
      .filter((control) => {
        const group = control as FormGroup;
        return group.get('skillId')?.value;
      })
      .map((control) => {
        const group = control as FormGroup;
        return {
          skillId: group.get('skillId')?.value || ('' as GUID),
          showToApplicants: group.get('showToApplicants')?.value || false,
        };
      });

    this.jobService.updateCurrentJobSkills(skills);
  }

  isSkillSelected(index: number): boolean {
    const group = this.getSkillGroup(index);
    return !!group.get('skillId')?.value;
  }

  getSkillName(skillId: GUID): string {
    const skill = this.lookupsService.skills().find((s) => s.id === skillId);
    return skill?.name || '';
  }
}
