import { Component, EventEmitter, inject, OnInit, Output, signal } from '@angular/core';
import { WizardStepComponent } from '../base/wizard-step.component';
import { FormBuilder, FormGroup } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { JobLookupService } from '../../../services/job-lookup.service';
import { GUID } from '../../../../../../../shared/types/guid.type';
import { Job } from '../../../models/job.model';
import { JobTabReviewNoteResponse } from '../../../models/job-tab-review-note-response';

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
  job = signal<Job | null>(null);
  isLoading = signal(false);

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

  override onActivate(): void {
    this.loadJob();
  }

  loadJob(): void {
    const jobId = this.jobService.getCurrentJobId();
    if (jobId) {
      this.isLoading.set(true);
      this.jobService.getJobById(jobId).subscribe({
        next: (data) => {
          this.job.set(data);
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      });
    }
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
    return !!this.job()?.overviewAr?.trim();
  }

  hasQualifications(): boolean {
    return !!this.job()?.qualificationsDescriptionAr?.trim() ||
      (this.job()?.degrees?.length || 0) > 0 ||
      (this.job()?.jobSpecializations?.length || 0) > 0;
  }

  hasResponsibilities(): boolean {
    return (this.job()?.responsibilities?.length || 0) > 0;
  }

  hasConditions(): boolean {
    return (this.job()?.conditions?.length || 0) > 0;
  }

  hasSkills(): boolean {
    return (this.job()?.skills?.length || 0) > 0;
  }

  hasAttachments(): boolean {
    return (this.job()?.requiredAttachments?.length || 0) > 0;
  }

  hasBenefits(): boolean {
    return !!this.job()?.benefitsAr?.trim();
  }

  getDegreeNames(): string {
    if (!this.job()?.degrees?.length) return '';

    const degreeNames = this.job()?.degrees?.map(degree =>
      this.lookupService.degrees().find(d => d.id === degree.degreeId)?.name || ''
    ).filter(name => name);

    return (degreeNames && degreeNames.length > 0) ? degreeNames.join(', ') : '';
  }

  formatHtmlContent(text: string | null | undefined): string {
    if (!text) return '';
    return text.replace(/\n/g, '<br>');
  }
}
