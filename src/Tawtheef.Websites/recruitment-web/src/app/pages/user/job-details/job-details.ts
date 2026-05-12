import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { DialogService } from 'primeng/dynamicdialog';
import { TooltipModule } from 'primeng/tooltip';

import { I18nNamespaceDirective } from '../../../shared/directives/i18n-namespace.directive';
import { JobDetailsService } from './services/job-details.service';
import { NotificationService } from '../../../core/services/notification.service';
import { routes } from '../../../routes/routes';
import { JobTabType } from './enums/job-tab-type';
import { JobApplyConfirmationDialogComponent } from './dialogs/job-apply-confirmation.dialog.component';
import { CandidateInvitationDetailsService } from './services/candidate-invitation-details.service';
import { EMPTY, map, startWith, switchMap } from 'rxjs';
import { JobOverviewComponent } from './components/job-overview.component';
import { JobResponsibilitiesComponent } from './components/job-responsibilities.component';
import { JobQualificationsComponent } from './components/job-qualifications.component';
import { JobConditionsComponent } from './components/job-conditions.component';
import { JobSkillsComponent } from './components/job-skills.component';
import { JobBenefitsComponent } from './components/job-benefits.component';
import { JobAttachmentsComponent } from './components/job-attachments.component';
import { JobSideInfoComponent } from './components/job-side-info/job-side-info.component';
import { InvitationStatus } from '../../../core/enums/lookups.enum';

@Component({
  selector: 'app-job-details',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    I18nNamespaceDirective,
    ButtonModule,
    TooltipModule,
    JobOverviewComponent,
    JobResponsibilitiesComponent,
    JobQualificationsComponent,
    JobConditionsComponent,
    JobSkillsComponent,
    JobBenefitsComponent,
    JobAttachmentsComponent,
    JobSideInfoComponent
  ],
  templateUrl: './job-details.html',
  styleUrls: ['./job-details.scss']
})
export class JobDetails implements OnInit {
  detailsService = inject(JobDetailsService);
  invitationDetailsService = inject(CandidateInvitationDetailsService);
  private route = inject(ActivatedRoute);
  private translate = inject(TranslateService);
  private notifier = inject(NotificationService);
  private dialogService = inject(DialogService);
  private router = inject(Router);

  job = this.detailsService.job;
  isLoading = this.detailsService.loading;
  activeTab: string = JobTabType.Overview;
  tabType = JobTabType;

  appliedOverride = signal(false);
  invitation = this.invitationDetailsService.invitation;

  hasApplied = computed(() => {
    if (this.appliedOverride()) return true;
    const status = this.job()?.invitationStatus?.backendName;
    return status === InvitationStatus.ExamEligible ||
      status === InvitationStatus.PendingAttachmentApproval;
  });

  isReturned = computed(() => {
    const status = this.job()?.invitationStatus?.backendName;
    return status === InvitationStatus.ReturnedAttachment;
  });

  routes = routes;

  invitationId = signal<string | null>(null);

  visibleTabs = computed(() => {
    const job = this.job();
    if (!job) return [];

    const tabs = [
      { id: JobTabType.Overview, title: 'JOB_DETAILS.OVERVIEW', icon: 'fa-file-alt', visible: !!job.overView },
      { id: JobTabType.Responsibilities, title: 'JOB_DETAILS.RESPONSIBILITIES', icon: 'fa-tasks', visible: !!job.responsibilities?.length },
      { id: JobTabType.Qualifications, title: 'JOB_DETAILS.QUALIFICATIONS', icon: 'fa-graduation-cap', visible: !!(job.degrees?.length || job.qualificationDescription || job.jobSpecializations?.length) },
      { id: JobTabType.Conditions, title: 'JOB_DETAILS.CONDITIONS', icon: 'fa-clipboard-list', visible: !!job.conditions?.length },
      { id: JobTabType.Skills, title: 'JOB_DETAILS.SKILLS', icon: 'fa-tools', visible: !!job.skills?.some(s => s.showToApplicants) },
      { id: JobTabType.Benefits, title: 'JOB_DETAILS.BENEFITS', icon: 'fa-gift', visible: !!job.benefits },
      { id: JobTabType.Attachments, title: 'JOB_DETAILS.REQUIRED_ATTACHMENTS', icon: 'fa-paperclip', visible: !!job.requiredAttachments?.length }
    ];

    return tabs.filter(tab => tab.visible);
  });

  ngOnInit() {
    const invitationId = this.route.snapshot.paramMap.get('invitationId');
    if (!invitationId) return;

    this.invitationId.set(invitationId);

    this.detailsService.loadJobDetails(invitationId)
      .pipe(
        switchMap(resp => {
          // Handle status update
          const status$ =
            resp?.invitationStatus?.backendName === InvitationStatus.NewInvitation
              ? this.detailsService.changeInvitationStatusRead(invitationId)
              : EMPTY;

          // Run status update, then pass original response forward
          return status$.pipe(map(() => resp), startWith(resp));
        })
      )
      .subscribe(job => {
        // Set active tab
        const visible = this.visibleTabs();
        if (visible.length > 0 && !visible.find(t => t.id === this.activeTab)) {
          this.activeTab = visible[0].id;
        }
      });

    this.invitationDetailsService.loadInvitation(invitationId);
  }

  openApplyDialog(): void {
    if (!this.canApply()) return;
    this.dialogService.open(JobApplyConfirmationDialogComponent, {
      header: this.translate.instant('JOB_DETAILS.APPLY_CONFIRM_TITLE'),
      width: '520px',
      draggable: false,   // ✅ disables dragging
      contentStyle: { 'border-radius': '12px' }
    })?.onClose.subscribe((confirmed) => {
      if (!confirmed) return;
      this.submitApplication();
    });
  }

  submitApplication(): void {
    const invitationId = this.invitationId();
    if (!invitationId) return;

    //TODO: check if all required attachments are uploaded

    this.detailsService.applyInvitation(invitationId).subscribe({
      next: () => {
        this.appliedOverride.set(true);
        this.notifier.success(this.translate.instant('JOB_DETAILS.APPLY_SUCCESS'));
        this.navigateTo();
      },
      error: (err) => {
        this.notifier.error(this.translate.instant('JOB_DETAILS.APPLY_ERROR'));
      }
    });
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
  }

  getTabContent(): { id: string; title: string; icon: string } {
    const visible = this.visibleTabs();
    return visible.find(tab => tab.id === this.activeTab) || visible[0] || { id: '', title: '', icon: '' };
  }

  isAllMandatoryUploaded = computed(() => {
    const job = this.job();
    if (!job || !job.requiredAttachments) return true;
    return job.requiredAttachments
      .filter(a => a.isMandatory)
      .every(a => !!a.attachmentId && !a.isReturned);
  });

  canApply(): boolean {
    const status = this.job()?.invitationStatus?.backendName;
    const isRejected = status === InvitationStatus.Rejected;
    const isClosed = status === InvitationStatus.Closed || status === InvitationStatus.Cancelled;
    const isSubmitted = status === InvitationStatus.ExamEligible || status === InvitationStatus.PendingAttachmentApproval;

    return this.isJobOpen() && !isSubmitted && !this.detailsService.applying()
      && !isRejected
      && !isClosed
      && this.isAllMandatoryUploaded();
  }

  getStatusClass(): string {
    const status = this.job()?.jobStatus?.backendName;
    return status ?? 'Closed';
  }

  isJobOpen(): boolean {
    const status = this.job()?.jobStatus?.backendName?.toLowerCase() ?? '';
    const isClosedStatus =
      status.includes('closed') ||
      status.includes('expired') ||
      status.includes('cancel') ||
      status.includes('suspend') ||
      status.includes('end');
    if (isClosedStatus) return false;
    if (!this.job()?.closingDate) return true;
    const closingDate = new Date(this.job()!.closingDate);
    const today = new Date();
    return closingDate >= today;
  }

  getApplyButtonLabel(): string {
    if (this.hasApplied()) return this.translate.instant('JOB_DETAILS.APPLICATION_SUBMITTED');
    if (this.isReturned()) return this.translate.instant('JOB_DETAILS.RE_SUBMIT');
    if (!this.isJobOpen()) return this.translate.instant('JOB_DETAILS.APPLICATION_CLOSED');
    return this.translate.instant('JOB_DETAILS.APPLY');
  }

  canRejectInvitation(): boolean {
    if (this.hasApplied()) return false;
    const status = this.job()?.invitationStatus?.backendName;
    const isPending = status === InvitationStatus.NewInvitation ||
      status === InvitationStatus.Read;
    return isPending;
  }

  rejectInvitation(): void {
    if (!this.canRejectInvitation()) return;
    const invitationId = this.invitationId();
    if (!invitationId) return;

    this.detailsService.changeInvitationStatusReject(invitationId).subscribe({
      next: () => {
        this.detailsService.loadJobDetails(invitationId).subscribe();
        this.invitationDetailsService.loadInvitation(invitationId);
        this.notifier.success(this.translate.instant('JOB_DETAILS.REJECT_SUCCESS'));
        this.navigateTo();
      },
      error: () => {
        this.notifier.error(this.translate.instant('JOB_DETAILS.REJECT_ERROR'));
      }
    });
  }

  navigateTo() {
    this.router.navigate([routes.user.dashboard]);
  }
}
