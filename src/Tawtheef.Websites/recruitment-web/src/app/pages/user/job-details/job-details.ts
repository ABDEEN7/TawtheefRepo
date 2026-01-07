import {Component, OnInit, computed, inject, signal} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ActivatedRoute, RouterLink} from '@angular/router';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ButtonModule} from 'primeng/button';

import {I18nNamespaceDirective} from '../../../shared/directives/i18n-namespace.directive';
import {
  JOB_INVITATION_STATUSES,
  STATUS_PILL_CLASSES,
  TYPE_BADGE_CLASSES
} from '../dashboard/services/candidate-dashboard.service';
import {CandidateInvitationModel} from '../dashboard/models/candidate-invitation.model';
import {CandidateInvitationDetailsService} from './services/candidate-invitation-details.service';
import {NotificationService} from '../../../core/services/notification.service';
import {routes} from '../../../routes/routes';

@Component({
  selector: 'app-job-details',
  standalone: true,
  imports: [CommonModule, TranslatePipe, I18nNamespaceDirective, ButtonModule, RouterLink],
  templateUrl: './job-details.html',
  styleUrls: ['./job-details.scss']
})
export class JobDetails implements OnInit {
  private route = inject(ActivatedRoute);
  private translate = inject(TranslateService);
  private notifier = inject(NotificationService);

  detailsService = inject(CandidateInvitationDetailsService);
  routes = routes;

  invitationId = signal<string | null>(null);
  invitation = this.detailsService.invitation;
  isApplying = this.detailsService.applying;

  canApply = computed(() => {
    const status = this.invitation()?.invitationStatus?.backendName;
    return status === JOB_INVITATION_STATUSES.NEW_INVITATION;
  });

  ngOnInit() {
    const invitationId = this.route.snapshot.paramMap.get('invitationId');
    if (!invitationId) return;

    this.invitationId.set(invitationId);
    this.detailsService.loadInvitation(invitationId);
  }

  applyInvitation(): void {
    const invitationId = this.invitationId();
    if (!invitationId) return;

    this.detailsService.applyInvitation(invitationId).subscribe({
      next: () => {
        this.notifier.success(this.translate.instant('job_details.apply_success'));
        this.detailsService.loadInvitation(invitationId);
      },
      error: () => {
        this.notifier.error(this.translate.instant('job_details.apply_error'));
      }
    });
  }

  getStatusClass(status?: string): string {
    if (!status) return 'status-closed';
    return STATUS_PILL_CLASSES[status as keyof typeof STATUS_PILL_CLASSES] ?? 'status-closed';
  }

  getJobCategoryClass(invitation: CandidateInvitationModel): string {
    return TYPE_BADGE_CLASSES[invitation.jobCategoryBackendName as keyof typeof TYPE_BADGE_CLASSES] ?? '';
  }
}
