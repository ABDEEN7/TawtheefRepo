import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { WizardStepComponent } from '../base/wizard-step.component';
import { FormBuilder, FormGroup } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { JobLookupService } from '../../../services/job-lookup.service';
import { GUID } from '../../../../../../../shared/types/guid.type';

@Component({
  selector: 'app-review-step',
  standalone: false,
  templateUrl: './review-step.component.html',
  styleUrls: ['./review-step.component.scss']
})
export class ReviewStepComponent extends WizardStepComponent implements OnInit {
  @Output() editStep = new EventEmitter<number>();

  protected jobService = inject(JobService);
  protected lookupService = inject(JobLookupService);
  private fb = inject(FormBuilder);


  readonly form: FormGroup = this.fb.group({});

  readonly STEP_NUMBERS = {
    OVERVIEW: 1,
    QUALIFICATIONS: 2,
    RESPONSIBILITIES: 3,
    CONDITIONS: 4,
    SKILLS: 5,
    ATTACHMENTS: 6,
    BENEFITS: 7
  };



  ngOnInit(): void {
  }

  isValid(): boolean {
    return true;
  }

  goToStep(stepNumber: number): void {
    this.editStep.emit(stepNumber);
  }

  getSkillName(skillId: GUID): string {
    return this.lookupService.skills().find(s => s.id === skillId)?.name ?? '';
  }

  hasOverview(): boolean {
    return !!this.jobService.getCurrentJob()?.overviewAr?.trim();
  }

  hasQualifications(): boolean {
    return !!this.jobService.getCurrentJob()?.qualificationsDescriptionAr?.trim() ||
      (this.jobService.getCurrentJob()?.degrees?.length || 0) > 0;
  }

  hasResponsibilities(): boolean {
    return (this.jobService.getCurrentJob()?.responsibilities?.length || 0) > 0;
  }

  hasConditions(): boolean {
    return (this.jobService.getCurrentJob()?.conditions?.length || 0) > 0;
  }

  hasSkills(): boolean {
    return (this.jobService.getCurrentJob()?.skills?.length || 0) > 0;
  }

  hasAttachments(): boolean {
    return (this.jobService.getCurrentJob()?.requiredAttachments?.length || 0) > 0;
  }

  hasBenefits(): boolean {
    return !!this.jobService.getCurrentJob()?.benefitsAr?.trim();
  }

  getDegreeNames(): string {
    if (!this.jobService.getCurrentJob()?.degrees?.length) return '';

    const degreeNames = this.jobService.getCurrentJob()?.degrees?.map(degree =>
      this.lookupService.degrees().find(d => d.id === degree.degreeId)?.name || ''
    ).filter(name => name);

    return (degreeNames && degreeNames.length > 0) ? degreeNames.join(', ') : '';
  }

  formatHtmlContent(text: string | null | undefined): string {
    if (!text) return '';
    return text.replace(/\n/g, '<br>');
  }
}
