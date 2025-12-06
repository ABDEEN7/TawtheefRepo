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
  
  // Form definition
  readonly form = this.fb.group({
    jobSkills: this.fb.array([])
  }) as FormGroup;

  ngOnInit() {
    // Load skills based on major when job data is available
    const currentJob = this.jobService.getCurrentJob();
    if (currentJob) {
      this.setJobData(currentJob);
      
      // Load skills for the selected major
      if (currentJob.majorId) {
        this.lookupsService.loadSkillsByMajor(currentJob.majorId);
      }
    }
    
    // Subscribe to form changes
    this.form.valueChanges.subscribe(() => {
      this.updateJobData();
    });
  }

  // Get skills form array
  get jobSkillsArray(): FormArray {
    return this.form.get('jobSkills') as FormArray;
  }

  // Get specific skill form group
  getSkillGroup(index: number): FormGroup {
    return this.jobSkillsArray.at(index) as FormGroup;
  }

  addSkill() {
    const skillGroup = this.fb.group({
      skillId: ['', [Validators.required]],
      showToApplicants: [true]
    });
    
    this.jobSkillsArray.push(skillGroup);
  }

  removeSkill(i: number) {
    this.jobSkillsArray.removeAt(i);
  }

  setJobData(job: Job): void {
    this.jobData = job;
    this.jobSkillsArray.clear();
    
    if (job.skills?.length) {
      job.skills.forEach((skill: JobSkill) => {
        const skillGroup = this.fb.group({
          skillId: [skill.skillId, [Validators.required]],
          showToApplicants: [skill.showToApplicants || false]
        });
        
        this.jobSkillsArray.push(skillGroup);
      });
    }
  }

  isValid() { 
    // Skills are optional, but if added, they must be valid
    if (this.jobSkillsArray.length === 0) {
      return true; // No skills is okay
    }
    
    return this.form.valid;
  }

  private updateJobData(): void {
    const skills = this.jobSkillsArray.controls
      .filter(control => {
        const group = control as FormGroup;
        return group.get('skillId')?.value; // Only include skills with a selected skill
      })
      .map(control => {
        const group = control as FormGroup;
        return {
          skillId: group.get('skillId')?.value || '' as GUID,
          showToApplicants: group.get('showToApplicants')?.value || false
        };
      });
    
    this.jobService.updateCurrentJobSkills(skills);
  }

  // Helper to check if a skill is selected
  isSkillSelected(index: number): boolean {
    const group = this.getSkillGroup(index);
    return !!group.get('skillId')?.value;
  }

  // Get skill name for display
  getSkillName(skillId: GUID): string {
    const skill = this.lookupsService.skills().find(s => s.id === skillId);
    return skill?.name || '';
  }
}