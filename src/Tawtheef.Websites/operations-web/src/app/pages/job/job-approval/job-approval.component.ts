import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { JobService } from '../services/job.service';
import { GUID } from '../../../shared/types/guid.type';
import { JobLookupService } from '../services/job-lookup.service';
import { JobResponse } from '../models/job-response-model';
import { JobStatus } from '../../../core/enums/lookups.enum';
import { JobTabReviewNote } from '../models/job-tab-review-note';
import { TranslateService } from '@ngx-translate/core';
import { JobTabStatus } from '../enums/job-tab-status';
import { DialogHelperService } from '../../../core/services/dialog-helper.service';
import { routes } from '../../../routes/routes';
import { JobTabType } from '../enums/job-tab-type';
import { JobTabReviewNoteResponse } from '../models/job-tab-review-note-response';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-job-approval.component',
  standalone: false,
  templateUrl: './job-approval.component.html',
  styleUrls: ['./job-approval.component.scss'],
})
export class JobApprovalComponent implements OnInit {
  private cdr = inject(ChangeDetectorRef);
  private jobService = inject(JobService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private transaltionService = inject(TranslateService);
  private notificationService = inject(NotificationService);
  private lookupsService = inject(JobLookupService);
  private dialogHelperService = inject(DialogHelperService);
  private fb = inject(FormBuilder);

  job!: JobResponse;
  id!: GUID;
  jobStatus = JobStatus;
  tabStatus = Object.values(JobTabStatus);
  tabType = JobTabType;

  tabAttachments: Record<string, any[]> = {};

  currentTabForm!: FormGroup;

  tabNotes: JobTabReviewNote[] = [];
  reviewHistory: JobTabReviewNoteResponse[] = [];
  activeTab: string = JobTabType.Overview;
  hasApplied: boolean = false;
  isFavorite: boolean = false;

  private tabsContent: { id: string; title: string; icon: string }[] = [
    { id: JobTabType.Overview, title: 'JOB_APPROVAL.OVERVIEW', icon: 'fa-file-alt' },
    { id: JobTabType.Skills, title: 'JOB_APPROVAL.SKILLS', icon: 'fa-tools' },
    { id: JobTabType.Conditions, title: 'JOB_APPROVAL.CONDITIONS', icon: 'fa-graduation-cap' },
    { id: JobTabType.Benefits, title: 'JOB_APPROVAL.BENEFITS', icon: 'fa-gift' },
    {
      id: JobTabType.Responsibilities,
      title: 'JOB_APPROVAL.RESPONSIBILITIES',
      icon: 'fa-info-circle',
    },
    { id: JobTabType.Attachments, title: 'JOB_APPROVAL.JOB_ATTACHMENTS', icon: 'fa-solid fa-file' },
    { id: 'Review', title: 'JOB_APPROVAL.JOB_REVIEW', icon: 'fa-eye' },
  ];

  ngOnInit() {
    this.initializeTabAttachments();
    this.initializeTabNotes();
    this.id = this.route.snapshot.paramMap.get('id') as GUID;
    this.loadJobById();
    this.lookupsService.loadJobStatus();
  }

  private initializeTabAttachments(): void {
    Object.values(JobTabType).forEach((tab) => {
      this.tabAttachments[tab] = [];
    });
  }

  private initializeTabNotes(): void {
    this.tabNotes = [
      { tab: JobTabType.Overview, tabStatus: null, note: '', reviewAttachments: [] },
      { tab: JobTabType.Responsibilities, tabStatus: null, note: '', reviewAttachments: [] },
      { tab: JobTabType.Conditions, tabStatus: null, note: '', reviewAttachments: [] },
      { tab: JobTabType.Skills, tabStatus: null, note: '', reviewAttachments: [] },
      { tab: JobTabType.Benefits, tabStatus: null, note: '', reviewAttachments: [] },
      { tab: JobTabType.Attachments, tabStatus: null, note: '', reviewAttachments: [] },
    ];
  }

  private loadJobById(): void {
    this.jobService.getById(this.id).subscribe({
      next: (job) => {
        this.job = job;
         this.initializeCurrentTabForm();
        this.jobService.getLatestTabReviewNotes(job.id).subscribe({
          next: (tabNotes : JobTabReviewNoteResponse[]) => {
            this.reviewHistory = tabNotes;
            this.cdr.detectChanges();
          },
        });
        this.cdr.detectChanges();
      }
    });
  }

  private initializeCurrentTabForm(): void {
    const currentAttachments = this.tabAttachments[this.activeTab] || [];

    this.currentTabForm = this.fb.group({
      attachments: this.fb.array(
        currentAttachments.map((att) => this.createAttachmentFormGroup(att))
      ),
    });
  }

  private createAttachmentFormGroup(attachment?: any): FormGroup {
    return this.fb.group({
      title: [attachment?.fileName || '', Validators.required],
      file: [attachment?.file || null, Validators.required],
      originalId: [attachment?.id],
      originalFileName: [attachment?.fileName || ''],
    });
  }

  get attachmentsArray(): FormArray {
    return this.currentTabForm.get('attachments') as FormArray;
  }

  setActiveTab(tab: string): void {
    this.saveCurrentTabData();

    this.activeTab = tab;
    this.initializeCurrentTabForm();
    this.cdr.detectChanges();
  }

  private saveCurrentTabData(): void {
    if (!this.currentTabForm) return;

    const formAttachments = this.attachmentsArray.controls.map((control, index) => {
      const formGroup = control as FormGroup;
      const file = formGroup.get('file')?.value;
      const title = formGroup.get('title')?.value;
      const originalId = formGroup.get('originalId')?.value;

      return {
        id: originalId,
        file: file,
        fileName: file.name || title,
      };
    });

    this.tabAttachments[this.activeTab] = formAttachments;

    const tabNote = this.tabNotes.find((t) => t.tab === this.activeTab);
    if (tabNote) {
      tabNote.reviewAttachments = formAttachments;
    }
  }

  getTabContent(): { id: string; title: string; icon: string } {
    return this.tabsContent.find((tab) => tab.id === this.activeTab) || this.tabsContent[0];
  }

  getCurrentTabNote(): JobTabReviewNote | undefined {
    return this.tabNotes.find((t) => t.tab === this.activeTab);
  }

  getStatusClass(): string {
    if (!this.job?.jobStatus?.backendName) return this.jobStatus.Closed;

    switch (this.job.jobStatus.backendName) {
      case JobStatus.Approved:
        return this.jobStatus.Approved;
      case JobStatus.PendingApproval:
        return this.jobStatus.PendingApproval;
      case JobStatus.Draft:
        return this.jobStatus.Draft;
      default:
        return this.jobStatus.Closed;
    }
  }

  getResponsibilities(): string[] {
    if (!this.job?.responsibilities?.length) return [];
    return this.job.responsibilities.map((r) => r.textAr);
  }

  getJobConditions(): { textAr: string; textEn: string }[] {
    if (!this.job?.conditions?.length) return [];
    return this.job.conditions.map((c) => ({
      textAr: c.textAr,
      textEn: c.textEn,
    }));
  }

  getJobSkills(): string[] {
    if (!this.job?.skills?.length) return [];
    return this.job.skills
      .filter((skill) => skill.showToApplicants)
      .map((skill) => skill.skill.name);
  }

  getJobBenefits(): string[] {
    if (!this.job?.benefitsAr) return [];
    return this.job.benefitsAr
      .split(/[\n,]+/)
      .map((benefit) => benefit.trim())
      .filter((b) => b);
  }

  getDegreeRequirements(): string {
    if (!this.job?.degrees?.length) return '';
    const degreeNames = this.job.degrees.map((degree) => degree.degree.name);
    return degreeNames.join(',') || '';
  }

  onFileChange(index: number, event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = input.files;
    if (files && files.length > 0) {
      const file = files[0];
      const row = this.attachmentsArray.at(index) as FormGroup;

      if (!row) return;
      row.get('file')?.setValue(file);
      this.cdr.detectChanges();
    }
  }

  isInvalid(index: number, controlName: string): boolean {
    const row = this.attachmentsArray.at(index) as FormGroup;
    const control = row.get(controlName);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  removeRow(index: number): void {
    this.attachmentsArray.removeAt(index);
    this.cdr.detectChanges();
  }

  addNewRow(): void {
    const newRow = this.createAttachmentFormGroup();
    this.attachmentsArray.push(newRow);
    this.cdr.detectChanges();
  }

  getFile(index: number): File | null {
    const row = this.attachmentsArray.at(index) as FormGroup;
    return row?.get('file')?.value || null;
  }

  previewFile(index: number): void {
    const file = this.getFile(index);
    if (!file) return;

    const fileURL = URL.createObjectURL(file);
    const fileType = file.type;

    if (fileType === 'application/pdf') {
      window.open(fileURL, '_blank');
    } else if (fileType.startsWith('image/')) {
      window.open(fileURL, '_blank');
    } else {
      const link = document.createElement('a');
      link.href = fileURL;
      link.download = file.name;
      link.click();
    }
  }

  submitReview(): void {
    this.saveCurrentTabData();
    const ref = this.dialogHelperService.openConfirmDialog({
      type: 'submit',
      title: 'JOB_APPROVAL.RETURN_CONFIRMATION',
      description: 'JOB_APPROVAL.RETURN_CONFIRM_NOTE',
      cancelText: 'COMMON.CANCEL',
      confirmText: 'COMMON.CONFIRM',
    });

    ref?.onClose.subscribe((result) => {
      if (!result) return;
      this.submitTabReview(this.lookupsService.getStatusIdByEnum(JobStatus.NeedUpdate));
    });
  }

  confirm(): void {
    this.saveCurrentTabData();

    const ref = this.dialogHelperService.openConfirmDialog({
      type: 'submit',
      title: 'JOB_APPROVAL.CONFIRMATION',
      description: 'JOB_APPROVAL.APPROVE_CONFIRM_NOTE',
      cancelText: 'COMMON.CANCEL',
      confirmText: 'COMMON.CONFIRM',
    });

    ref?.onClose.subscribe((result) => {
      if (!result) return;
      this.submitTabReview(this.lookupsService.getStatusIdByEnum(JobStatus.Approved));
    });
  }

  reject(): void {
    this.saveCurrentTabData();

    const ref = this.dialogHelperService.openConfirmDialog({
      type: 'warning',
      title: 'JOB_APPROVAL.REJECT_CONFIRMATION',
      description: 'JOB_APPROVAL.REJECT_CONFIRM_NOTE',
      cancelText: 'COMMON.CANCEL',
      confirmText: 'COMMON.CONFIRM',
    });

    ref?.onClose.subscribe((result) => {
      if (!result) return;
      this.submitTabReview(this.lookupsService.getStatusIdByEnum(JobStatus.Rejected));
    });
  }

  private submitTabReview(newStatusId : GUID): void {
    this.saveCurrentTabData();

    const formData = this.buildFormData();

    this.jobService.submitTabReview(formData).subscribe({
      next: () => {
        this.handleSuccess();
        this.jobService.changeStatus(this.job.id, newStatusId).subscribe();
      }
    });
  }

  private buildFormData(): FormData {
    const formData = new FormData();
    formData.append('JobId', this.job.id);

    const tabsPayload = this.tabNotes.map((t) => ({
      Tab: t.tab,
      Status: t.tabStatus,
      Note: t.note || '',
      AttachmentsJson: JSON.stringify(
        this.tabAttachments[t.tab]?.map((a, idx) => ({
          id: a.id || null,
          fileName: a.fileName ,
          fileIndex: a.file instanceof File ? idx : null,
        })) || []
      ),
    }));

    tabsPayload.forEach((tab, index) => {
      formData.append(`Request.Tabs[${index}].Tab`, tab.Tab);
      formData.append(`Request.Tabs[${index}].Status`, tab.Status?.toString() || '');
      formData.append(`Request.Tabs[${index}].Note`, tab.Note);
      formData.append(`Request.Tabs[${index}].AttachmentsJson`, tab.AttachmentsJson);
    });

    Object.values(JobTabType).forEach((tab) => {
      this.tabAttachments[tab]?.forEach((attachment) => {
        if (attachment.file && attachment.file instanceof File) {
          formData.append(
            'Request.Files',
            attachment.file,
            attachment.fileName || attachment.file.name
          );
        }
      });
    });

    return formData;
  }

  private handleSuccess(): void {
    this.router.navigate([routes.employee.JobList]);
    this.notificationService.success(
      this.transaltionService.instant('JOB_APPROVAL.SUBMIT_REVIEW_SUCCESS')
    );
  }

  isReviewComplete(): boolean {
    return this.tabNotes.every((t) => t.tabStatus !== null && t.tabStatus !== undefined);
  }

  isAllTabsApproved(): boolean {
    return this.tabNotes.every((t) => t.tabStatus === JobTabStatus.Approved);
  }
}
