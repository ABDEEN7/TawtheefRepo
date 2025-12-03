import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormArray, Validators, FormGroup } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { MessageService } from 'primeng/api';
import { Job } from '../../../models/job.model';
import { RequiredAttachment } from '../../../models/required-attachment.model';
import { debounceTime, filter, Subject, takeUntil } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-attachments-step',
  standalone: false,
  templateUrl: './attachment-step.component.html',
  styleUrls: ['./attachment-step.component.scss'],
})
export class AttachmentStepComponent implements WizardStepComponent, OnInit, OnDestroy {
  // Services
  private readonly fb = inject(FormBuilder);
  private readonly jobService = inject(JobService);
  private readonly messageService = inject(MessageService);
  private readonly translationService = inject(TranslateService);
  
  // Form
  readonly form = this.fb.group({
    attachments: this.fb.array<FormGroup>([])
  });
  
  // State
  private readonly destroy$ = new Subject<void>();

  // Getters
  get attachmentsArray(): FormArray<FormGroup> {
    return this.form.controls.attachments;
  }

  ngOnInit(): void {
    this.initializeForm();
    this.setupFormSubscription();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Initialize form with existing job data
   */
  private initializeForm(): void {
    const job = this.jobService.newJob();
    this.setJobData(job);
  }

  /**
   * Setup form value changes subscription with debounce
   */
  private setupFormSubscription(): void {
    this.form.valueChanges.pipe(
      debounceTime(300),
      filter(() => this.form.valid),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.saveAttachmentsToService();
    });
  }

  /**
   * Populate form with job attachments
   */
  setJobData(job: Job): void {
    this.attachmentsArray.clear();
    
    if (job.requiredAttachments?.length) {
      job.requiredAttachments.forEach(attachment => {
        this.addAttachmentToForm(attachment);
      });
    }
    
    this.form.updateValueAndValidity();
  }

  /**
   * Add new attachment
   */
  addAttachment(): void {
    const newAttachment: RequiredAttachment = {
      title: '',
      isMandatory: false
    };
    
    this.addAttachmentToForm(newAttachment);
  }

  /**
   * Remove attachment at index
   */
  removeAttachment(index: number): void {
    this.attachmentsArray.removeAt(index);
  }

  /**
   * Check if form is valid
   */
  isValid(): boolean {
    // Check if all attachments have valid titles
    return this.form.valid && this.allAttachmentsHaveTitles();
  }

  // Private helper methods
  private createAttachmentFormGroup(attachment: RequiredAttachment): FormGroup {
    return this.fb.group({
      title: [attachment.title, [Validators.required]],
      isMandatory: [attachment.isMandatory || false]
    });
  }

  private addAttachmentToForm(attachment: RequiredAttachment): void {
    const group = this.createAttachmentFormGroup(attachment);
    this.attachmentsArray.push(group);
  }

  private saveAttachmentsToService(): void {
    const attachments: RequiredAttachment[] = this.attachmentsArray.controls.map(control => ({
      title: control.get('title')?.value,
      isMandatory: control.get('isMandatory')?.value || false
    }));
    
    this.jobService.updateCurrentJobAttachments(attachments);
  }

  private allAttachmentsHaveTitles(): boolean {
    return this.attachmentsArray.controls.every(control => {
      const title = control.get('title')?.value?.trim();
      return title && title.length > 0;
    });
  }
}