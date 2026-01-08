import {Component, OnInit, inject, signal, computed} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ButtonModule} from 'primeng/button';
import {DialogService} from 'primeng/dynamicdialog';

import {I18nNamespaceDirective} from '../../../shared/directives/i18n-namespace.directive';
import {JobDetailsService} from './services/job-details.service';
import {NotificationService} from '../../../core/services/notification.service';
import {routes} from '../../../routes/routes';
import {JobTabType} from './enums/job-tab-type';
import {JobApplyConfirmationDialogComponent} from './dialogs/job-apply-confirmation.dialog.component';
import {CandidateInvitationDetailsService} from './services/candidate-invitation-details.service';

@Component({
  selector: 'app-job-details',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe, I18nNamespaceDirective, ButtonModule],
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
    const status = this.invitation()?.invitationStatus?.backendName?.toLowerCase() ?? '';
    return status.includes('approved') || status.includes('applied') || status.includes('submitted');
  });

  routes = routes;

  invitationId = signal<string | null>(null);

  ngOnInit() {
    const invitationId = this.route.snapshot.paramMap.get('invitationId');
    if (!invitationId) return;

    this.invitationId.set(invitationId);
    this.detailsService.loadJobDetails(invitationId).subscribe(resp=>{
      if(resp)
      {
        this.detailsService.changeInvitationStatus(invitationId, 'Read').subscribe();
      }
    });
    this.invitationDetailsService.loadInvitation(invitationId);
  }

  openApplyDialog(): void {
    if (!this.canApply()) return;
    this.dialogService.open(JobApplyConfirmationDialogComponent, {
      header: this.translate.instant('JOB_DETAILS.APPLY_CONFIRM_TITLE'),
      width: '520px',
      contentStyle: { 'border-radius': '12px' }
    })?.onClose.subscribe((confirmed) => {
      if (!confirmed) return;
      this.submitApplication();
    });
  }

  submitApplication(): void {
    const invitationId = this.invitationId();
    if (!invitationId) return;

    this.detailsService.applyInvitation(invitationId).subscribe({
      next: () => {
        this.appliedOverride.set(true);
        this.invitationDetailsService.loadInvitation(invitationId);
        this.notifier.success(this.translate.instant('JOB_DETAILS.APPLY_SUCCESS'));
        this.navigateTo();
      },
      error: () => {
        this.notifier.error(this.translate.instant('JOB_DETAILS.APPLY_ERROR'));
      }
    });
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
  }

  getTabContent(): { id: string; title: string; icon: string } {
    const tabsContent = [
      { id: JobTabType.Overview, title: 'JOB_DETAILS.OVERVIEW', icon: 'fa-file-alt' },
      { id: JobTabType.Responsibilities, title: 'JOB_DETAILS.RESPONSIBILITIES', icon: 'fa-tasks' },
      { id: JobTabType.Conditions, title: 'JOB_DETAILS.CONDITIONS_RESPONSIBILITIES', icon: 'fa-graduation-cap' },
      { id: JobTabType.Skills, title: 'JOB_DETAILS.SKILLS', icon: 'fa-tools' },
      { id: JobTabType.Benefits, title: 'JOB_DETAILS.BENEFITS', icon: 'fa-gift' },
      { id: JobTabType.Attachments, title: 'JOB_DETAILS.REQUIRED_ATTACHMENTS', icon: 'fa-paperclip' }
    ];

    return tabsContent.find(tab => tab.id === this.activeTab) || tabsContent[0];
  }

  canApply(): boolean {
    const status = this.invitation()?.invitationStatus?.backendName?.toLowerCase() ?? '';
    const isRejected = status.includes('reject');
    const isClosed = status.includes('closed') || status.includes('cancel');
    return this.isJobOpen() && !this.hasApplied() && !this.detailsService.applying()
      && !isRejected
      && !isClosed;
  }

  getStatusClass(): string {
    const status = this.job()?.jobStatus?.backendName;
    return status ?? 'Closed';
  }

  getResponsibilities(): string[] {
    if (!this.job()?.responsibilities?.length) return [];
    return this.job()!.responsibilities!.map((c) => c.text);
  }

  getJobConditions(): string[] {
    if (!this.job()?.conditions?.length) return [];
    return this.job()!.conditions!.map((c) => c.text);
  }

  getJobSkills(): string[] {
    if (!this.job()?.skills?.length) return [];
    return this.job()!.skills!
      .filter(skill => skill.showToApplicants)
      .map(skill => skill.skill.name);
  }

  getDegreeRequirements(): string {
    if (!this.job()?.degrees?.length) return '';
    const degreeNames = this.job()!.degrees!.map(degree => degree.degree.name);
    return degreeNames.join(', ') || '';
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
    if (!this.isJobOpen()) return this.translate.instant('JOB_DETAILS.APPLICATION_CLOSED');
    return this.translate.instant('JOB_DETAILS.APPLY');
  }

  canRejectInvitation(): boolean {
    if (this.hasApplied()) return false;
    const status = this.invitation()?.invitationStatus?.backendName?.toLowerCase() ?? '';
    const isPending = status.includes('new') || status.includes('read');
    return isPending;
  }

  rejectInvitation(): void {
    if (!this.canRejectInvitation()) return;
    const invitationId = this.invitationId();
    if (!invitationId) return;

    this.detailsService.changeInvitationStatus(invitationId, 'Rejected').subscribe({
      next: () => {
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
