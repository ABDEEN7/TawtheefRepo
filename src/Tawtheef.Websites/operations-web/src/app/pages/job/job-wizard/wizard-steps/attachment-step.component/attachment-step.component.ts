import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormArray, Validators, FormGroup } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { Job } from '../../../models/job.model';
import { debounceTime, filter, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-attachment-step',
  standalone: false,
  templateUrl: './attachment-step.component.html',
  styleUrls: ['./attachment-step.component.scss'],
})
export class AttachmentStepComponent extends WizardStepComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  protected jobService = inject(JobService);
  
  jobData!: Job;
  
  // Form definition
  readonly form: FormGroup = this.fb.group({
    attachments: this.fb.array([])
  });
  
  private readonly destroy$ = new Subject<void>();

  ngOnInit(): void {
    // Load initial data if available
    const currentJob = this.jobService.getCurrentJob();
    if (currentJob) {
      this.setJobData(currentJob);
    }
    
    // Subscribe to form changes
    this.form.valueChanges.pipe(
      debounceTime(300),
      filter(() => this.form.valid),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.updateJobData();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // Get attachments form array
  get attachmentsArray(): FormArray {
    return this.form.get('attachments') as FormArray;
  }

  // Get specific attachment form group
  getAttachmentGroup(index: number): FormGroup {
    return this.attachmentsArray.at(index) as FormGroup;
  }

  setJobData(job: Job): void {
    this.jobData = job;
    this.attachmentsArray.clear();
    
    if (job.requiredAttachments?.length) {
      job.requiredAttachments.forEach(attachment => {
        this.addAttachmentToForm(attachment);
      });
    }
  }

  addAttachment(): void {
    const attachmentGroup = this.fb.group({
      titleAr: ['', [Validators.required, Validators.maxLength(200)]],
      titleEn: ['', [Validators.maxLength(200)]],
      isMandatory: [false]
    });
    
    this.attachmentsArray.push(attachmentGroup);
  }

  removeAttachment(index: number): void {
    this.attachmentsArray.removeAt(index);
  }

  isValid(): boolean {
    // Attachments are optional, but if added, they must be valid
    if (this.attachmentsArray.length === 0) {
      return true; // No attachments is okay
    }
    
    return this.form.valid;
  }

  private addAttachmentToForm(attachment: any): void {
    const attachmentGroup = this.fb.group({
      titleAr: [attachment.titleAr || '', [Validators.required, Validators.maxLength(200)]],
      titleEn: [attachment.titleEn || '', [Validators.maxLength(200)]],
      isMandatory: [attachment.isMandatory || false]
    });
    
    this.attachmentsArray.push(attachmentGroup);
  }

  private updateJobData(): void {
    if (this.form.valid) {
      const attachments = this.attachmentsArray.controls.map(control => {
        const group = control as FormGroup;
        return {
          titleAr: group.get('titleAr')?.value || '',
          titleEn: group.get('titleEn')?.value || '',
          isMandatory: group.get('isMandatory')?.value || false
        };
      });
      
      this.jobService.updateCurrentJobAttachments(attachments);
    }
  }

  // Check if attachment has Arabic title
  hasArabicTitle(index: number): boolean {
    const group = this.getAttachmentGroup(index);
    const titleAr = group.get('titleAr')?.value?.trim();
    return !!titleAr;
  }
}