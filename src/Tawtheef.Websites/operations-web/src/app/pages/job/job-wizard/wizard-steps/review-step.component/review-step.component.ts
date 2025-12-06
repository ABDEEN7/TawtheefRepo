import { Component, inject, OnInit } from '@angular/core';
import { WizardStepComponent } from '../base/wizard-step.component';
import { FormBuilder, FormGroup } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { Job } from '../../../models/job.model';
import { JobLookupService } from '../../../services/job-lookup.service';
import { GUID } from '../../../../../shared/types/guid.type';

@Component({
  selector: 'app-review-step',
  standalone: false,
  templateUrl: './review-step.component.html',
  styleUrls: ['./review-step.component.scss']
})
export class ReviewStepComponent extends WizardStepComponent implements OnInit {
  private jobService = inject(JobService);
  protected lookupService = inject(JobLookupService);
  private fb = inject(FormBuilder);

  jobData!: Job;
  
  readonly form: FormGroup = this.fb.group({});
  
  readonly STEP_NUMBERS = {
    OVERVIEW: 2,
    QUALIFICATIONS: 3,
    RESPONSIBILITIES: 4,
    CONDITIONS: 5,
    SKILLS: 6,
    ATTACHMENTS: 7,
    BENEFITS: 8
  };

  ngOnInit(): void {
    const currentJob = this.jobService.getCurrentJob();
    if (currentJob) {
      this.setJobData(currentJob);
    }
  }

  isValid(): boolean {
    return true;
  }

  setJobData(job: Job): void {
    this.jobData = job;
  }

  // Navigation to edit specific step (this should bubble up to parent wizard)
  goToStep(stepNumber: number): void {
    // This will be handled by emitting an event
    // The parent wizard component should listen for this event
  }

  getSkillName(skillId: GUID): string {
    return this.lookupService.skills().find(s => s.id === skillId)?.name || 'مهارة غير معروفة';
  }

  hasOverview(): boolean {
    return !!this.jobData?.overviewAr?.trim();
  }

  hasQualifications(): boolean {
    return !!this.jobData?.qualificationsDescriptionAr?.trim() || 
           (this.jobData?.degrees?.length || 0) > 0;
  }

  hasResponsibilities(): boolean {
    return (this.jobData?.responsibilities?.length || 0) > 0;
  }

  hasConditions(): boolean {
    return (this.jobData?.conditions?.length || 0) > 0;
  }

  hasSkills(): boolean {
    return (this.jobData?.skills?.length || 0) > 0;
  }

  hasAttachments(): boolean {
    return (this.jobData?.requiredAttachments?.length || 0) > 0;
  }

  hasBenefits(): boolean {
    return !!this.jobData?.benefitsAr?.trim();
  }

  getDegreeNames(): string {
    if (!this.jobData?.degrees?.length) return 'غير محدد';
    
    const degreeNames = this.jobData.degrees.map(degree => 
      this.lookupService.degrees().find(d => d.id === degree.degreeId)?.name || ''
    ).filter(name => name);
    
    return degreeNames.length > 0 ? degreeNames.join(', ') : 'غير محدد';
  }

  formatHtmlContent(text: string | undefined): string {
    if (!text) return '';
    return text.replace(/\n/g, '<br>');
  }
}