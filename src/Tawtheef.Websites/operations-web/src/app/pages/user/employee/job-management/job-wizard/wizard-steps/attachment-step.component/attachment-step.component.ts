import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormArray, Validators, FormGroup, AbstractControl, ValidationErrors } from '@angular/forms';
import { JobService } from '../../../services/job.service';
import { WizardStepComponent } from '../base/wizard-step.component';
import { Job } from '../../../models/job.model';
import { debounceTime, filter, Subject, takeUntil } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { JobTabReviewNoteResponse } from '../../../models/job-tab-review-note-response';
import { JobTabStatus } from '../../../enums/job-tab-status';
import { NotificationService } from '../../../../../../../core/services/notification.service';
import { JobStatus } from '../../../../../../../core/enums/lookups.enum';
import { StringUtils } from '../../../../../../../core/utils/string-utils';

@Component({
  selector: 'app-attachment-step',
  standalone: false,
  templateUrl: './attachment-step.component.html',
  styleUrls: ['./attachment-step.component.scss'],
})
export class AttachmentStepComponent extends WizardStepComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  protected jobService = inject(JobService);
  private notificationService = inject(NotificationService);
  private transaltionService = inject(TranslateService);

  jobData!: Job;
  note: JobTabReviewNoteResponse | null = null;
  newAttachment = {
    titleAr: '',
    titleEn: '',
    isMandatory: false,
  };

  readonly form: FormGroup = this.fb.group({
    attachments: this.fb.array([]),
  }, { validators: [this.duplicateAttachmentTitlesValidator.bind(this)] });

  private readonly destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.form.valueChanges
      .pipe(
        debounceTime(300),
        takeUntil(this.destroy$)
      )
      .subscribe(() => {
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

  override setJobData(job: Job, note: JobTabReviewNoteResponse | null = null): void {
    this.jobData = job;
    this.note = note;
    this.attachmentsArray.clear();

    if (job.requiredAttachments?.length) {
      job.requiredAttachments.forEach((attachment) => {
        this.addAttachmentToForm(attachment);
      });
    }

    if (
      note?.tabStatus !== JobTabStatus.Returned &&
      job.jobStatus?.backendName === JobStatus.NeedUpdate
    ) {
      this.form.disable();
    }
  }

  addAttachment(): void {
    if (this.form.disabled) {
      return;
    }

    const titleAr = this.newAttachment.titleAr?.trim() || '';
    const titleEn = this.newAttachment.titleEn?.trim() || '';

    if (!titleAr && !titleEn) {
      this.notificationService.error(
        this.transaltionService.instant('JOB_WIZARD.STEPS.NO_DATA_ENTERED_ERROR')
      );
      return;
    }

    const isDuplicate = this.hasDuplicateAttachmentTitle(titleAr, titleEn);

    if (isDuplicate) {
      this.notificationService.error(
        this.transaltionService.instant('JOB_WIZARD.STEPS.DUPLICATE_ENTRY_ERROR')
      );
      return;
    }

    const attachmentGroup = this.fb.group({
      titleAr: [titleAr, [Validators.required, Validators.maxLength(200)]],
      titleEn: [titleEn, [Validators.maxLength(200)]],
      isMandatory: [this.newAttachment.isMandatory || false],
    });

    attachmentGroup.get('titleAr')?.markAsTouched();
    attachmentGroup.get('titleEn')?.markAsTouched();

    this.attachmentsArray.push(attachmentGroup);
    this.resetNewAttachment();
    this.updateJobData();

    this.form.updateValueAndValidity();
  }

  removeAttachment(index: number): void {
    this.attachmentsArray.removeAt(index);
    this.updateJobData();
    this.form.updateValueAndValidity();
  }

  isValid(): boolean {
    if (this.attachmentsArray.length === 0) {
      return true;
    }

    if (this.form.disabled) {
      return true;
    }
    return this.form.valid && this.attachmentsArray.valid;
  }

  private addAttachmentToForm(attachment: any): void {
    const attachmentGroup = this.fb.group({
      titleAr: [attachment.titleAr || '', [Validators.required, Validators.maxLength(200)]],
      titleEn: [attachment.titleEn || '', [Validators.maxLength(200)]],
      isMandatory: [attachment.isMandatory || false],
    });

    if (this.form.disabled) {
      attachmentGroup.disable({ emitEvent: false });
    }

    this.attachmentsArray.push(attachmentGroup);
  }

  private updateJobData(): void {
    const attachments = this.attachmentsArray.controls.map((control) => {
      const group = control as FormGroup;
      return {
        titleAr: group.get('titleAr')?.value || '',
        titleEn: group.get('titleEn')?.value || '',
        isMandatory: group.get('isMandatory')?.value || false,
      };
    });

    this.jobService.updateCurrentJobAttachments(attachments);
  }

  hasArabicTitle(index: number): boolean {
    const group = this.getAttachmentGroup(index);
    const titleAr = group.get('titleAr')?.value?.trim();
    return !!titleAr;
  }

  hasDuplicateTitles(): boolean {
    return !!this.form.errors?.['duplicateAttachmentTitle'];
  }

  private resetNewAttachment(): void {
    this.newAttachment = {
      titleAr: '',
      titleEn: '',
      isMandatory: false,
    };
  }

  isFieldValid(groupIndex: number, fieldName: string): boolean {
    const group = this.getAttachmentGroup(groupIndex);
    const field = group.get(fieldName);
    return field ? !(field.invalid && field.touched) : true;
  }

  private hasDuplicateAttachmentTitle(titleAr: string, titleEn: string, excludedIndex: number | null = null): boolean {
    const normalizedTitleAr = this.normalizeTitle(titleAr);
    const normalizedTitleEn = this.normalizeTitle(titleEn);

    return this.attachmentsArray.controls.some((control: AbstractControl, index: number) => {
      if (excludedIndex !== null && index === excludedIndex) {
        return false;
      }

      const group = control as FormGroup;
      const existingTitleAr = this.normalizeTitle(group.get('titleAr')?.value);
      const existingTitleEn = this.normalizeTitle(group.get('titleEn')?.value);

      return (!!normalizedTitleAr && normalizedTitleAr === existingTitleAr) ||
        (!!normalizedTitleEn && normalizedTitleEn === existingTitleEn);
    });
  }

  private duplicateAttachmentTitlesValidator(control: AbstractControl): ValidationErrors | null {
    const attachments = control.get('attachments') as FormArray | null;
    if (!attachments) {
      return null;
    }

    const seenArabicTitles = new Set<string>();
    const seenEnglishTitles = new Set<string>();

    for (const item of attachments.controls) {
      const group = item as FormGroup;
      const titleAr = this.normalizeTitle(group.get('titleAr')?.value);
      const titleEn = this.normalizeTitle(group.get('titleEn')?.value);

      if (titleAr) {
        if (seenArabicTitles.has(titleAr)) {
          return { duplicateAttachmentTitle: true };
        }
        seenArabicTitles.add(titleAr);
      }

      if (titleEn) {
        if (seenEnglishTitles.has(titleEn)) {
          return { duplicateAttachmentTitle: true };
        }
        seenEnglishTitles.add(titleEn);
      }
    }

    return null;
  }

  private normalizeTitle(value: unknown): string {
    return StringUtils.normalize((value ?? '').toString());
  }
}
