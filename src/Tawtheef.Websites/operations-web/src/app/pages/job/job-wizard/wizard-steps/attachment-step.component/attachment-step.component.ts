import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormArray, Validators, FormGroup, AbstractControl } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { Job } from '../../../models/job.model';
import { debounceTime, filter, Subject, takeUntil } from 'rxjs';
import { MessageService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-attachment-step',
  standalone: false,
  templateUrl: './attachment-step.component.html',
  styleUrls: ['./attachment-step.component.scss'],
})
export class AttachmentStepComponent extends WizardStepComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  protected jobService = inject(JobService);
  private messageService = inject(MessageService);
  private transaltionService = inject(TranslateService);
  
  jobData!: Job;
  
  newAttachment = {
    titleAr: '',
    titleEn: '',
    isMandatory: false
  };
  
  readonly form: FormGroup = this.fb.group({
    attachments: this.fb.array([])
  });
  
  private readonly destroy$ = new Subject<void>();
  disabled: boolean = false;

  ngOnInit(): void {
    const currentJob = this.jobService.getCurrentJob();
    if (currentJob) {
      this.setJobData(currentJob);
    }
    
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

  get attachmentsArray(): FormArray {
    return this.form.get('attachments') as FormArray;
  }

  getAttachmentGroup(index: number): FormGroup {
    return this.attachmentsArray.at(index) as FormGroup;
  }

  setJobData(job: Job,disable:boolean =false): void {
    this.jobData = job;
    this.attachmentsArray.clear();
    
    if (job.requiredAttachments?.length) {
      job.requiredAttachments.forEach(attachment => {
        this.addAttachmentToForm(attachment);
      });
    }

    this.disabled =disable
  }

  addAttachment(): void {
    const textAr = this.newAttachment.titleAr.trim();
    const textEn = this.newAttachment.titleEn.trim();

    if (!textAr && !textEn) {
       this.messageService.add({
          severity: 'error',
          summary: 'No data entered',
          detail: this.transaltionService.instant('JOB_WIZARD.STEPS.DUPLICATE_ENTRY_ERROR'),
        });
      return;
    }

    const isDuplicate = this.attachmentsArray.controls.some((control: AbstractControl) => {
      const group = control as FormGroup;
      return group.get('titleAr')?.value === textAr && group.get('titleEn')?.value === textEn;
    });
    
    if (isDuplicate) {
       this.messageService.add({
          severity: 'error',
          summary: 'Duplicate Entry',
          detail: this.transaltionService.instant('JOB_WIZARD.STEPS.DUPLICATE_ENTRY_ERROR'),
        });
      return;
    }

      const attachmentGroup = this.fb.group({
        titleAr: [this.newAttachment.titleAr, [Validators.required, Validators.maxLength(200)]],
        titleEn: [this.newAttachment.titleEn, [Validators.maxLength(200)]],
      });    
      
    this.attachmentsArray.push(attachmentGroup);
    this.resetNewAttachment();
    this.updateJobData();
  }

  removeAttachment(index: number): void {
    this.attachmentsArray.removeAt(index);
    this.updateJobData();
  }

  isValid(): boolean {
    if (this.attachmentsArray.length === 0) {
      return true;
    }
    
    return this.form.valid && this.attachmentsArray.valid;
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

  hasArabicTitle(index: number): boolean {
    const group = this.getAttachmentGroup(index);
    const titleAr = group.get('titleAr')?.value?.trim();
    return !!titleAr;
  }

  private resetNewAttachment(): void {
    this.newAttachment = {
      titleAr: '',
      titleEn: '',
      isMandatory: false
    };
  }

  isFieldValid(groupIndex: number, fieldName: string): boolean {
    const group = this.getAttachmentGroup(groupIndex);
    const field = group.get(fieldName);
    return field ? field.valid && (field.dirty || field.touched) : false;
  }
}