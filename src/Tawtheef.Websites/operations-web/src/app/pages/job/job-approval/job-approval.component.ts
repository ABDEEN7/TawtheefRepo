import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
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
import { FileUtilsService } from '../../../core/utils/file-utils';
import { DialogService } from 'primeng/dynamicdialog';
import { JobReviewAttachment } from '../models/job-review-attachment';
import { JobReviewAttachmentsModalComponent } from '../modals/job-review-attachments-modal/job-review-attachments-modal.component';
import { JobReviewResponse } from '../models/job-review-response';
import { AuthService } from '../../../core/auth/auth.service';
import { Permissions } from '../../../core/constants/permissions';

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
  private dialogService = inject(DialogService);
  protected fileUtils = inject(FileUtilsService);
  private authService = inject(AuthService);

  job!: JobResponse;
  id!: GUID;
  jobStatus = JobStatus;
  tabStatus = Object.values(JobTabStatus);
  tabType = JobTabType;

  jobReviewAttachments: JobReviewAttachment[] = [];

  tabNotes: JobTabReviewNote[] = [];
  reviewHistory: JobTabReviewNoteResponse[] = [];
  activeTab: string = JobTabType.Overview;
  hasApplied: boolean = false;
  isFavorite: boolean = false;

  reviewForm!: FormGroup;

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
    this.initializeForm();
    this.initializeTabNotes();
    this.id = this.route.snapshot.paramMap.get('id') as GUID;
    this.loadJobById();
    this.lookupsService.loadJobStatus().subscribe();
  }

  private initializeForm(): void {
    this.reviewForm = this.fb.group({});
  }

  private initializeTabNotes(): void {
    this.tabNotes = [
      { tab: JobTabType.Overview, tabStatus: null, note: '' },
      { tab: JobTabType.Responsibilities, tabStatus: null, note: '' },
      { tab: JobTabType.Conditions, tabStatus: null, note: '' },
      { tab: JobTabType.Skills, tabStatus: null, note: '' },
      { tab: JobTabType.Benefits, tabStatus: null, note: '' },
      { tab: JobTabType.Attachments, tabStatus: null, note: '' },
    ];

    this.tabNotes.forEach((tabNote, index) => {
      const tabControl = new FormControl(tabNote.tabStatus);
      const noteControl = new FormControl(tabNote.note);
      
      this.reviewForm.addControl(`tabStatus_${index}`, tabControl);
      this.reviewForm.addControl(`note_${index}`, noteControl);

      tabControl.valueChanges.subscribe((status) => {
        this.onTabStatusChange(index, status);
      });

      noteControl.valueChanges.subscribe((note) => {
        this.tabNotes[index].note = note || '';
      });
    });
  }

  private onTabStatusChange(index: number, status: JobTabStatus | null): void {
    const noteControl = this.reviewForm.get(`note_${index}`) as FormControl;
    
    this.tabNotes[index].tabStatus = status;
    
    if (status === JobTabStatus.Returned) {
      noteControl.setValidators([Validators.required]);
      noteControl.markAsTouched(); 
    } else {
      noteControl.clearValidators();
    }
    
    if (noteControl.value !== null) {
      this.tabNotes[index].note = noteControl.value;
    }
    
    noteControl.updateValueAndValidity();
    this.cdr.detectChanges();
  }

  getCurrentTabNote(): JobTabReviewNote | undefined {
    const currentNote = this.tabNotes.find((t) => t.tab === this.activeTab);
    return currentNote;
  }

  getCurrentTabControls(): { tabControl: FormControl; noteControl: FormControl } | null {
    const index = this.tabNotes.findIndex(t => t.tab === this.activeTab);
    if (index === -1) return null;
    
    return {
      tabControl: this.reviewForm.get(`tabStatus_${index}`) as FormControl,
      noteControl: this.reviewForm.get(`note_${index}`) as FormControl
    };
  }

  hasNoteError(): boolean {
    const controls = this.getCurrentTabControls();
    if (!controls) return false;
    
    return controls.noteControl.invalid && controls.noteControl.touched;
  }

  private loadReviewIntoForm(review: JobReviewResponse): void {
    review.tabNoteReviews.forEach((tabReview) => {
      const index = this.tabNotes.findIndex(t => t.tab === tabReview.tab);
      if (index !== -1) {
        const tabControl = this.reviewForm.get(`tabStatus_${index}`) as FormControl;
        const noteControl = this.reviewForm.get(`note_${index}`) as FormControl;
        
        tabControl.setValue(tabReview.tabStatus);
        noteControl.setValue(tabReview.note || '');
        
        this.tabNotes[index].tabStatus = tabReview.tabStatus;
        this.tabNotes[index].note = tabReview.note || '';
        
        tabControl.markAsTouched();
        noteControl.markAsTouched();
      }
    });
  }

  private loadJobById(): void {
    this.jobService.getById(this.id).subscribe({
      next: (job) => {
        this.job = job;
        this.jobService.getLatestReview(job.id).subscribe({
          next: (review: JobReviewResponse) => {
            this.reviewHistory = review.tabNoteReviews;
            this.loadReviewIntoForm(review);
            this.cdr.detectChanges();
          },
        });
        this.cdr.detectChanges();
      }
    });
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
    this.cdr.detectChanges();
  }

  getTabContent(): { id: string; title: string; icon: string } {
    return this.tabsContent.find((tab) => tab.id === this.activeTab) || this.tabsContent[0];
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

  getResponsibilities(): { textAr: string; textEn: string }[] {
    if (!this.job?.responsibilities?.length) return [];
    return this.job.responsibilities.map((c) => ({
      textAr: c.textAr,
      textEn: c.textEn,
    }));
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

  submitReview(): void {
    if (!this.canManageJobs()) return;
    Object.keys(this.reviewForm.controls).forEach(key => {
      const control = this.reviewForm.get(key);
      control?.markAsTouched();
    });

    if (this.reviewForm.invalid) {
      this.notificationService.error(
        this.transaltionService.instant('JOB_APPROVAL.VALIDATION_ERROR')
      );
      return;
    }

    const ref = this.dialogHelperService.openConfirmDialog({
      type: 'submit',
      title: 'JOB_APPROVAL.RETURN_CONFIRMATION',
      description: 'JOB_APPROVAL.RETURN_CONFIRM_NOTE',
      cancelText: 'common.cancel',
      confirmText: 'common.confirm',
    });

    ref?.onClose.subscribe((result) => {
      if (!result) return;
      this.openAttachmentModal(JobStatus.NeedUpdate);
    });
  }

  confirm(): void {
    if (!this.canManageJobs()) return;
    Object.keys(this.reviewForm.controls).forEach(key => {
      const control = this.reviewForm.get(key);
      control?.markAsTouched();
    });

    if (this.reviewForm.invalid) {
      this.notificationService.error(
        this.transaltionService.instant('JOB_APPROVAL.VALIDATION_ERROR')
      );
      return;
    }

    const ref = this.dialogHelperService.openConfirmDialog({
      type: 'submit',
      title: 'JOB_APPROVAL.CONFIRMATION',
      description: 'JOB_APPROVAL.APPROVE_CONFIRM_NOTE',
      cancelText: 'common.cancel',
      confirmText: 'common.confirm',
    });

    ref?.onClose.subscribe((result) => {
      if (!result) return;
      this.submitTabReview(this.lookupsService.getStatusIdByEnum(JobStatus.Approved));
    });
  }

  reject(): void {
    if (!this.canManageJobs()) return;
    const ref = this.dialogHelperService.openConfirmDialog({
      type: 'warning',
      title: 'JOB_APPROVAL.REJECT_CONFIRMATION',
      description: 'JOB_APPROVAL.REJECT_CONFIRM_NOTE',
      cancelText: 'common.cancel',
      confirmText: 'common.confirm',
    });

    ref?.onClose.subscribe((result) => {
      if (!result) return;
      this.openAttachmentModal(JobStatus.Rejected);
    });
  }

  private openAttachmentModal(status: JobStatus): void {
    if (!this.canManageJobs()) return;
    const dialogRef = this.dialogService.open(JobReviewAttachmentsModalComponent, {
      header: this.transaltionService.instant('JOB_APPROVAL.ADD_REVIEW_ATTACHMENT'),
      width: '900px',
      modal: true,
      data: {
        existingAttachments: [...this.jobReviewAttachments]
      }
    });

    dialogRef?.onClose.subscribe((attachments: JobReviewAttachment[]) => {
      if (attachments) {
        this.jobReviewAttachments = attachments;
        this.submitTabReview(this.lookupsService.getStatusIdByEnum(status));
      }
    });
  }

  private submitTabReview(newStatusId: GUID): void {
    if (!this.canManageJobs()) return;
    const formData = this.buildFormData();

    this.jobService.submitTabReview(formData).subscribe({
      next: () => {
        this.jobService.changeStatus(this.job.id, newStatusId).subscribe({
          next: () => {
            this.handleSuccess();
          },
        });
      },
    });
  }

  private buildFormData(): FormData {
    const formData = new FormData();
    formData.append('JobId', this.job.id);

    const tabsPayload = this.tabNotes.map((t) => ({
      Tab: t.tab,
      Status: t.tabStatus,
      Note: t.note || '',
    }));

    tabsPayload.forEach((tab, index) => {
      formData.append(`Request.Tabs[${index}].Tab`, tab.Tab);
      formData.append(`Request.Tabs[${index}].Status`, tab.Status?.toString() || '');
      formData.append(`Request.Tabs[${index}].Note`, tab.Note);
    });

    const attachmentsJson = JSON.stringify(
      this.jobReviewAttachments.map((a, idx) => ({
        id: a.id || null,
        fileName: a.fileName,
        fileIndex: a.file instanceof File ? idx : null,
      }))
    );
    formData.append('Request.AttachmentsJson', attachmentsJson);

    this.jobReviewAttachments.forEach((attachment) => {
      if (attachment.file && attachment.file instanceof File) {
        formData.append(
          'Request.Files',
          attachment.file,
          attachment.fileName || attachment.file.name
        );
      }
    });

    return formData;
  }

  private handleSuccess(): void {
    this.router.navigate([routes.employee.JobList]);
    if(this.isAllTabsApproved())
    {
    this.notificationService.success(
      this.transaltionService.instant('JOB_APPROVAL.SUBMIT_REVIEW_SUCCESS')
    );
  }else{
  this.notificationService.warn(
      this.transaltionService.instant('JOB_APPROVAL.JOB_RETURNED_NEED_UPDATES')
    );
  }
  }

  canManageJobs(): boolean {
    return this.authService.hasPermission(Permissions.Jobs.Manage);
  }

  canViewJobs(): boolean {
    return this.authService.hasPermission([Permissions.Jobs.Manage, Permissions.Jobs.View]);
  }

  isReviewComplete(): boolean {
    const allStatusesSet = this.tabNotes.every((t, index) => {
      const control = this.reviewForm.get(`tabStatus_${index}`) as FormControl;
      return control.value !== null && control.value !== undefined;
    });

    return allStatusesSet && this.reviewForm.valid;
  }

  isAllTabsApproved(): boolean {
    return this.tabNotes.every((t) => t.tabStatus === JobTabStatus.Approved);
  }

  preview(file: any): void {
    this.fileUtils.previewUrl(file.url).then(() => {});
  }

  getJobReviewAttachments(): JobReviewAttachment[] {
    return this.jobReviewAttachments;
  }

  previewAttachment(attachment: JobReviewAttachment): void {
    if (!attachment.file) return;

    const fileURL = URL.createObjectURL(attachment.file);
    const fileType = attachment.file.type;

    if (fileType === 'application/pdf') {
      window.open(fileURL, '_blank');
    } else if (fileType.startsWith('image/')) {
      window.open(fileURL, '_blank');
    } else {
      const link = document.createElement('a');
      link.href = fileURL;
      link.download = attachment.fileName;
      link.click();
    }
  }
}
