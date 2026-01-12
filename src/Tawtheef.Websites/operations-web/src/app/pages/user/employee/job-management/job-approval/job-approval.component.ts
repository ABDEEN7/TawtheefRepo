import { ChangeDetectorRef, Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { JobService } from '../services/job.service';
import { GUID } from '../../../../../shared/types/guid.type';
import { JobLookupService } from '../services/job-lookup.service';
import { JobResponse } from '../models/job-response-model';
import { JobStatus } from '../../../../../core/enums/lookups.enum';
import { JobTabReviewNote } from '../models/job-tab-review-note';
import { TranslateService } from '@ngx-translate/core';
import { JobTabStatus } from '../enums/job-tab-status';
import { DialogHelperService } from '../../../../../core/services/dialog-helper.service';
import { routes } from '../../../../../routes/routes';
import { JobTabType } from '../enums/job-tab-type';
import { JobTabReviewNoteResponse } from '../models/job-tab-review-note-response';
import { NotificationService } from '../../../../../core/services/notification.service';
import { FileUtilsService } from '../../../../../core/utils/file-utils';
import { DialogService } from 'primeng/dynamicdialog';
import { JobReviewAttachment } from '../models/job-review-attachment';
import { JobReviewAttachmentsModalComponent } from '../modals/job-review-attachments-modal/job-review-attachments-modal.component';
import { JobReviewResponse } from '../models/job-review-response';
import { AuthService } from '../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../core/constants/permissions';
import { catchError, debounceTime, EMPTY, filter, Subject, switchMap, takeUntil } from 'rxjs';
type JobApprovalTab = JobTabType | 'Review';

@Component({
  selector: 'app-job-approval.component',
  standalone: false,
  templateUrl: './job-approval.component.html',
  styleUrls: ['./job-approval.component.scss'],
})

export class JobApprovalComponent implements OnInit, OnDestroy {
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
  private destroy$ = new Subject<void>();
  private reviewDraft$ = new Subject<JobTabReviewNote>();
  private suspendDraftSave = false;

  job!: JobResponse;
  id!: GUID;
  jobStatus = JobStatus;
  tabStatus = Object.values(JobTabStatus);
  tabType = JobTabType;

  jobReviewAttachments: JobReviewAttachment[] = [];

  tabNotes: JobTabReviewNote[] = [];
  reviewHistory: JobTabReviewNoteResponse[] = [];
  reviewTabId: JobApprovalTab = 'Review';
  activeTab: JobApprovalTab = JobTabType.BasicData;
  reviewForm!: FormGroup;

  tabsContent: { id: JobApprovalTab; title: string; icon: string }[] = [
    { id: JobTabType.BasicData, title: 'JOB_APPROVAL.BASIC_DATA', icon: 'fa-clipboard-list' },
    { id: JobTabType.Overview, title: 'JOB_APPROVAL.OVERVIEW', icon: 'fa-file-alt' },
    { id: JobTabType.Qualifications, title: 'JOB_APPROVAL.QUALIFICATIONS', icon: 'fa-graduation-cap' },
    { id: JobTabType.Conditions, title: 'JOB_APPROVAL.CONDITIONS', icon: 'fa-clipboard-check' },
    { id: JobTabType.Responsibilities, title: 'JOB_APPROVAL.RESPONSIBILITIES', icon: 'fa-info-circle' },
    { id: JobTabType.Skills, title: 'JOB_APPROVAL.SKILLS', icon: 'fa-tools' },
    { id: JobTabType.Benefits, title: 'JOB_APPROVAL.BENEFITS', icon: 'fa-gift' },
    { id: JobTabType.Attachments, title: 'JOB_APPROVAL.JOB_ATTACHMENTS', icon: 'fa-solid fa-file' },
    { id: this.reviewTabId, title: 'JOB_APPROVAL.JOB_REVIEW', icon: 'fa-eye' },
  ];

  private reviewTabs: JobTabType[] = [
    JobTabType.BasicData,
    JobTabType.Overview,
    JobTabType.Qualifications,
    JobTabType.Conditions,
    JobTabType.Responsibilities,
    JobTabType.Skills,
    JobTabType.Benefits,
    JobTabType.Attachments,
  ];

  ngOnInit() {
    this.initializeForm();
    this.initializeTabNotes();
    this.id = this.route.snapshot.paramMap.get('id') as GUID;
    this.loadJobById();
    this.lookupsService.loadJobStatus().subscribe();
    this.setupReviewDraftAutoSave();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeForm(): void {
    this.reviewForm = this.fb.group({});
  }

  private initializeTabNotes(): void {
    this.tabNotes = this.reviewTabs.map((tab) => ({ tab, tabStatus: null, note: '' }));

    this.tabNotes.forEach((tabNote, index) => {
      const tabControl = new FormControl(tabNote.tabStatus);
      const noteControl = new FormControl(tabNote.note);

      this.reviewForm.addControl(`tabStatus_${index}`, tabControl);
      this.reviewForm.addControl(`note_${index}`, noteControl);

      tabControl.valueChanges.pipe(takeUntil(this.destroy$)).subscribe((status) => {
        this.onTabStatusChange(index, status);
      });

      noteControl.valueChanges.pipe(takeUntil(this.destroy$)).subscribe((note) => {
        this.tabNotes[index].note = note || '';
      });
    });
  }

  saveCurrentTabReview(): void {
  if (!this.canManageJobs() || !this.job) return;

  const index = this.tabNotes.findIndex(t => t.tab === this.activeTab);
  if (index === -1) return;

  const tabControl = this.reviewForm.get(`tabStatus_${index}`) as FormControl;
  const noteControl = this.reviewForm.get(`note_${index}`) as FormControl;

  tabControl.markAsTouched();
  noteControl.markAsTouched();

  if (tabControl.value === JobTabStatus.Returned) {
    noteControl.setValidators([Validators.required]);
  } else {
    noteControl.clearValidators();
  }
  noteControl.updateValueAndValidity();

  if (tabControl.invalid || noteControl.invalid) {
    this.notificationService.error(
      this.transaltionService.instant('JOB_APPROVAL.VALIDATION_ERROR')
    );
    return;
  }
  this.tabNotes[index].tabStatus = tabControl.value;
  this.tabNotes[index].note = noteControl.value || '';

  const formData = this.buildSingleTabFormData(this.tabNotes[index], false);
  if (!formData) return;

  this.jobService.updateTabReview(formData).subscribe({
    next: () => {
      this.notificationService.success(
        this.transaltionService.instant('common.savedSuccessfully') // replace with your key
      );
    }
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
    this.suspendDraftSave = true;
    review.tabNoteReviews.forEach((tabReview) => {
      const index = this.tabNotes.findIndex(t => t.tab === tabReview.tab);
      if (index !== -1) {
        const tabControl = this.reviewForm.get(`tabStatus_${index}`) as FormControl;
        const noteControl = this.reviewForm.get(`note_${index}`) as FormControl;

        tabControl.setValue(tabReview.tabStatus, { emitEvent: false });
        noteControl.setValue(tabReview.note || '', { emitEvent: false });

        this.tabNotes[index].tabStatus = tabReview.tabStatus;
        this.tabNotes[index].note = tabReview.note || '';
      }
    });
    this.suspendDraftSave = false;
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

  setActiveTab(tab: JobApprovalTab): void {
    this.activeTab = tab;
    this.cdr.detectChanges();
  }

  getTabContent(): { id: JobApprovalTab; title: string; icon: string } {
    return this.tabsContent.find((tab) => tab.id === this.activeTab) || this.tabsContent[0];
  }

  getTabTitle(tab?: JobApprovalTab | null): string {
    if (!tab) return 'JOB_APPROVAL.OVERVIEW';
    return this.tabsContent.find((t) => t.id === tab)?.title || 'JOB_APPROVAL.OVERVIEW';
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

  get reviewedTabsCount(): number {
    return this.tabNotes.filter((note) => note.tabStatus !== null).length;
  }

  get reviewProgressPercent(): number {
    if (!this.tabNotes.length) return 0;
    return Math.round((this.reviewedTabsCount / this.tabNotes.length) * 100);
  }

  getTabIndicatorStatus(tab: JobTabType): JobTabStatus | null {
    return this.tabNotes.find((note) => note.tab === tab)?.tabStatus ?? null;
  }

  getTabIndicatorLabel(tab: JobTabType): string {
    const status = this.getTabIndicatorStatus(tab);
    if (!status) return 'JOB_APPROVAL.REVIEW_PENDING';
    if (status === JobTabStatus.Returned) return 'JOB_APPROVAL.NEEDS_CHANGES';
    return 'JOB_APPROVAL.REVIEWED';
  }

  getTabIndicatorClass(tab: JobTabType): string {
    const status = this.getTabIndicatorStatus(tab);
    if (!status) return 'pending';
    return status === JobTabStatus.Returned ? 'returned' : 'approved';
  }

  isReviewTab(tab: JobApprovalTab): boolean {
    return tab === this.reviewTabId;
  }

  getTabStatusLabel(tabStatus: JobTabStatus | null | undefined): string {
    if (!tabStatus) return 'JOB_APPROVAL.REVIEW_PENDING';
    return `JOB_TAB_STATUS.${tabStatus}`;
  }

  getBasicDataItems(): {
    label: string;
    value: string | null;
    translateValue?: boolean;
    isDate?: boolean;
  }[] {
    if (!this.job) return [];
    return [
      {
        label: 'JOB_APPROVAL.JOB_TITLE',
        value: [this.job.titleAr, this.job.titleEn].filter(Boolean).join(' / '),
      },
      { label: 'JOB_APPROVAL.SECTOR', value: this.job.sector?.name ?? null },
      { label: 'JOB_APPROVAL.MANAGEMENT', value: this.job.management?.name ?? null },
      { label: 'JOB_APPROVAL.DEPARTMENT', value: this.job.department?.name ?? null },
      { label: 'JOB_APPROVAL.JOB_CATEGORY', value: this.job.jobCategory?.name ?? null },
      {
        label: 'JOB_APPROVAL.WORK_LOCATION',
        value: this.job.workLocation?.name ?? null,
        translateValue: true,
      },
      { label: 'JOB_APPROVAL.WORK_TYPE', value: this.job.workType?.name ?? null },
      { label: 'JOB_APPROVAL.GENDER', value: this.job.gender?.name ?? null },
      { label: 'JOB_APPROVAL.MAJOR', value: this.job.major?.name ?? null },
      { label: 'JOB_APPROVAL.SUB_MAJOR', value: this.job.subMajor?.name ?? null },
      {
        label: 'JOB_APPROVAL.YEARS_OF_EXPERIENCE',
        value:
          this.job.yearsOfExperience !== null && this.job.yearsOfExperience !== undefined
            ? `${this.job.yearsOfExperience}`
            : null,
      },
      {
        label: 'JOB_APPROVAL.VACANCIES',
        value:
          this.job.numberOfVacancies !== null && this.job.numberOfVacancies !== undefined
            ? `${this.job.numberOfVacancies}`
            : null,
      },
      {
        label: 'JOB_APPROVAL.CLOSING_DATE',
        value: this.job.closingDate ? new Date(this.job.closingDate).toISOString() : null,
        isDate: true,
      },
      {
        label: 'JOB_APPROVAL.AGE_RANGE',
        value:
          this.job.minimumAge && this.job.maximumAge
            ? `${this.job.minimumAge} - ${this.job.maximumAge}`
            : null,
      },
    ];
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
    if (!this.validateApprovalData()) return;
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
    const formData = this.buildFormData(true);

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

  private buildFormData(includeAttachments: boolean): FormData {
    const formData = new FormData();
    formData.append('JobId', this.job.id);

    const tabsPayload = this.tabNotes
      .filter((t) => t.tabStatus)
      .map((t) => ({
      Tab: t.tab,
      Status: t.tabStatus,
      Note: t.note || '',
    }));

    tabsPayload.forEach((tab, index) => {
      formData.append(`Request.Tabs[${index}].Tab`, tab.Tab);
      formData.append(`Request.Tabs[${index}].Status`, tab.Status?.toString() || '');
      formData.append(`Request.Tabs[${index}].Note`, tab.Note);
    });

    if (includeAttachments) {
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
    }

    return formData;
  }

  private buildSingleTabFormData(
    tabNote: JobTabReviewNote,
    includeAttachments: boolean
  ): FormData | null {
    if (!tabNote.tabStatus) return null;

    const formData = new FormData();
    formData.append('JobId', this.job.id);
    formData.append('Request.Tabs[0].Tab', tabNote.tab);
    formData.append('Request.Tabs[0].Status', tabNote.tabStatus.toString());
    formData.append('Request.Tabs[0].Note', tabNote.note || '');

    if (includeAttachments) {
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
    }

    return formData;
  }

  private handleSuccess(): void {
    this.router.navigate([routes.employee.JobList]);
    if (this.isAllTabsApproved()) {
      this.notificationService.success(
        this.transaltionService.instant('JOB_APPROVAL.SUBMIT_REVIEW_SUCCESS')
      );
    } else {
      this.notificationService.warn(
        this.transaltionService.instant('JOB_APPROVAL.JOB_RETURNED_NEED_UPDATES')
      );
    }
  }

  canManageJobs(): boolean {
    return this.authService.hasPermission(Permissions.Jobs.Manage);
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

  private validateApprovalData(): boolean {
    if (!this.job) return false;
    const hasBasicInfo =
      !!this.job.titleAr?.trim() &&
      !!this.job.titleEn?.trim() &&
      !!this.job.sector?.id &&
      !!this.job.management?.id &&
      !!this.job.department?.id &&
      !!this.job.jobCategory?.id &&
      !!this.job.workLocation?.id &&
      !!this.job.major?.id &&
      !!this.job.workType?.id &&
      this.job.numberOfVacancies > 0 &&
      !!this.job.closingDate &&
      this.job.minimumAge > 0 &&
      this.job.maximumAge > this.job.minimumAge &&
      this.job.yearsOfExperience >= 0;

    const hasOverview =
      !!this.job.overViewAr?.trim() && !!this.job.overViewEn?.trim();
    const hasQualifications =
      !!this.job.qualificationDescriptionAr?.trim() &&
      !!this.job.qualificationDescriptionEn?.trim() &&
      (this.job.degrees?.length || 0) > 0;
    const hasResponsibilities = (this.job.responsibilities?.length || 0) > 0;
    const hasConditions = (this.job.conditions?.length || 0) > 0;
    const hasSkills = (this.job.skills?.length || 0) > 0;

    if (
      !hasBasicInfo ||
      !hasOverview ||
      !hasQualifications ||
      !hasResponsibilities ||
      !hasConditions ||
      !hasSkills
    ) {
      this.notificationService.error(
        this.transaltionService.instant('JOB_APPROVAL.MISSING_REQUIRED_DATA')
      );
      return false;
    }

    return true;
  }

  private setupReviewDraftAutoSave(): void {
    this.reviewDraft$
      .pipe(
        debounceTime(800),
        filter(() => !this.suspendDraftSave),
        filter(() => this.canManageJobs() && !!this.job),
        switchMap((tabNote) => {
          const formData = this.buildSingleTabFormData(tabNote, false);
          if (!formData) return EMPTY;
          return this.jobService.updateTabReview(formData).pipe(
            catchError(() => EMPTY)
          );
        }),
        takeUntil(this.destroy$)
      )
      .subscribe();
  }

  private queueDraftSave(index: number): void {
    if (this.suspendDraftSave || !this.job) return;
    const note = this.tabNotes[index];
    if (!note?.tabStatus) return;
    this.reviewDraft$.next(note);
  }

  preview(file: any): void {
    this.fileUtils.previewUrl(file.url).then(() => {});
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
