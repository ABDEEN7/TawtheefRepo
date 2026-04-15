import { Component, inject, OnInit } from '@angular/core';
import { WizardStepComponent } from '../base/wizard-step.component';
import { FormBuilder, Validators, FormArray, FormGroup } from '@angular/forms';
import { Job } from '../../../models/job.model';
import { JobService } from '../../../services/job.service';
import { JobSkill } from '../../../models/job-skill.model';
import { JobLookupService } from '../../../services/job-lookup.service';
import { GUID } from '../../../../../../../shared/types/guid.type';
import { JobTabReviewNoteResponse } from '../../../models/job-tab-review-note-response';
import { JobTabStatus } from '../../../enums/job-tab-status';
import { JobStatus } from '../../../../../../../core/enums/lookups.enum';

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
  note: JobTabReviewNoteResponse | null = null;

  readonly form = this.fb.group({
    jobSkills: this.fb.array([]),
  }) as FormGroup;

  ngOnInit() {
    const currentJob = this.jobService.getCurrentJob();
    this.lookupsService.loadSkillsByMajor(currentJob?.subMajorId);

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
      showToApplicants: [true, [Validators.required]],
    });

    this.jobSkillsArray.push(skillGroup);

    // so validation messages can appear after user starts interacting
    this.form.markAsDirty();
  }

  removeSkill(i: number) {
    this.jobSkillsArray.removeAt(i);
    this.form.markAsDirty();
    this.form.updateValueAndValidity();
    this.updateJobData();
  }

  override setJobData(job: Job, note: JobTabReviewNoteResponse | null = null): void {
    this.jobData = job;
    this.note = note;
    this.jobSkillsArray.clear();

    if (job.skills?.length) {
      job.skills.forEach((skill: JobSkill) => {
        const skillGroup = this.fb.group({
          skillId: [skill.skillId, [Validators.required]],
          showToApplicants: [skill.showToApplicants ?? true, [Validators.required]],
        });

        this.jobSkillsArray.push(skillGroup);
      });
    }

    if (note?.tabStatus !== JobTabStatus.Returned && job.jobStatus?.backendName === JobStatus.NeedUpdate) {
      this.form.disable();
    }
  }

  // ✅ NEW RULE: valid ONLY if there is at least 1 skill (unless disabled)
  isValid(): boolean {
    if (this.form.disabled) return true;
    return this.form.valid;
  }

  markTouched() {
    this.form.markAllAsTouched();
    this.form.updateValueAndValidity();
  }

  private updateJobData(): void {
    const skills = this.jobSkillsArray.controls
      .filter((control) => {
        const group = control as FormGroup;
        return !!group.get('skillId')?.value;
      })
      .map((control) => {
        const group = control as FormGroup;
        return {
          skillId: group.get('skillId')?.value || ('' as GUID),
          showToApplicants: !!group.get('showToApplicants')?.value,
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
