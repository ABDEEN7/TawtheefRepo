import { Component, EventEmitter, Input, Output, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgClass, NgForOf, NgIf } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize } from 'rxjs';

import { ButtonDirective } from 'primeng/button';
import { PrimeTemplate } from 'primeng/api';
import { Select } from 'primeng/select';
import { Textarea } from 'primeng/textarea';

import {
  FinalApprovalAction,
  ProfileApprovalDetail,
  ProfileApprovalSection,
  ReviewStatus,
} from '../../../../approval-list/models/profile-approval.models';

import { NotificationService } from '../../../../../../../../core/services/notification.service';
import { ProfileApprovalService } from '../../../../approval-list/services/profile-approval.service';
import { FinalizeProfileApprovalRequest } from '../../../../approval-list/models/profile-approval-finalize.model';

type Summary = Record<'approved' | 'rejected' | 'changes' | 'pending', number>;

@Component({
  selector: 'app-profile-approval-final-review-section',
  standalone: true,
  imports: [
    ButtonDirective,
    NgForOf,
    NgIf,
    NgClass,
    FormsModule,
    TranslatePipe,
    PrimeTemplate,
    Select,
    Textarea,
  ],
  templateUrl: './final-review-section.html',
  styleUrl: './final-review-section.scss',
})
export class FinalReviewSection {
  private readonly translate = inject(TranslateService);
  private readonly notifications = inject(NotificationService);
  private readonly api = inject(ProfileApprovalService);

  // Keep @Input stable but convert to signal for cheap computed usage
  private readonly _info = signal<ProfileApprovalDetail | null>(null);

  @Input({ required: true })
  set info(value: ProfileApprovalDetail) {
    this._info.set(value);
  }
  infoApproval = this._info.asReadonly();

  // IMPORTANT: component needs profileId to finalize; do not rely on internal selectedProfileId.
  @Input({ required: true }) profileId!: string;

  @Output() refresh = new EventEmitter<void>();

  finalAction = signal<FinalApprovalAction | null>(null);
  finalSummary = signal('');
  finalNotes = signal('');
  finalAttachment = signal<File | null>(null);
  submittingFinalAction = signal(false);

  readonly finalActions: Array<{ labelKey: string; value: FinalApprovalAction }> = [
    { labelKey: 'profileApproval.final.actions.ApproveProfile', value: 'ApproveProfile' },
    { labelKey: 'profileApproval.final.actions.NeedsCorrection', value: 'NeedsCorrection' },
    { labelKey: 'profileApproval.final.actions.RejectProfile', value: 'RejectProfile' },
    { labelKey: 'profileApproval.final.actions.BlockProfile', value: 'BlockProfile' },
    { labelKey: 'profileApproval.final.actions.ExceptionalApproval', value: 'ExceptionalApproval' },
  ];

  // Computed data for template
  readonly sectionsOrdered = computed<ProfileApprovalSection[]>(() => {
    const info = this.infoApproval();
    const sections = info?.sections ?? [];
    return [...sections]
      .sort((a, b) => a.section - b.section)
      .filter(s => s.sectionReview != null);
  });

  readonly statusSummary = computed<Summary>(() => {
    const info = this.infoApproval();
    const summary: Summary = { approved: 0, rejected: 0, changes: 0, pending: 0 };

    (info?.sections ?? []).forEach((sec) => {
      const st = sec.sectionReview?.status ?? ReviewStatus.Pending;

      if (st === ReviewStatus.Approved) summary.approved += 1;
      else if (st === ReviewStatus.Rejected) summary.rejected += 1;
      else if (st === ReviewStatus.ChangesRequested || st === ReviewStatus.NeedsCorrection) summary.changes += 1;
      else summary.pending += 1;
    });

    return summary;
  });

  onFinalActionFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.finalAttachment.set(input.files?.[0] ?? null);
  }

  submitFinalDecision(info: ProfileApprovalDetail): void {
    if (!this.profileId) return;

    const action = this.finalAction();
    if (!action) {
      this.notifications.error(this.translate.instant('profileApproval.validation.noteRequired'));
      return;
    }

    const summary = this.finalSummary().trim();
    const notes = this.finalNotes().trim();
    const attachment = this.finalAttachment();

    const stats = this.progressStats(info);
    const correctionTargets = this.needsCorrectionTargets(info);

    if (action === 'ApproveProfile' && (stats.pendingSections > 0 || stats.flaggedSections > 0 || stats.pendingItems > 0)) {
      this.notifications.error(this.translate.instant('profileApproval.validation.cannotApproveProfile'));
      return;
    }

    if ((action === 'RejectProfile' || action === 'ExceptionalApproval') && !attachment) {
      this.notifications.error(
        this.translate.instant(
          action === 'RejectProfile'
            ? 'profileApproval.validation.rejectRequirements'
            : 'profileApproval.validation.exceptionRequirements'
        )
      );
      return;
    }

    if (action === 'NeedsCorrection' && correctionTargets.length === 0) {
      this.notifications.warn(this.translate.instant('profileApproval.validation.correctionTargetsRequired'));
      return;
    }

    if (action !== 'ApproveProfile' && !notes) {
      this.notifications.error(this.translate.instant('profileApproval.validation.noteRequired'));
      return;
    }

    const request: FinalizeProfileApprovalRequest = {
      action,
      summary,
      note: notes,
      needsCorrectionItems: correctionTargets,
    };

    if (action === 'RejectProfile') request.rejectionDocument = attachment!;
    if (action === 'ExceptionalApproval') request.exceptionalFile = attachment!;

    this.submittingFinalAction.set(true);
    this.api
      .finalizeProfile(this.profileId, request)
      .pipe(finalize(() => this.submittingFinalAction.set(false)))
      .subscribe({
        next: () => {
          this.notifications.success(this.translate.instant('profileApproval.final.actionExecuted'));

          this.finalAction.set(null);
          this.finalNotes.set('');
          this.finalSummary.set('');
          this.finalAttachment.set(null);

          this.refresh.emit(); // parent owns reloading
        },
        error: () => this.notifications.error(this.translate.instant('profileApproval.errors.finalize')),
      });
  }

  // UI helpers (prefer returning translation keys)
  sectionNameKey(section: number): string {
    switch (section) {
      case 1: return 'profileOverview.sections.prerequisites';
      case 2: return 'profileOverview.sections.basicInfo';
      case 3: return 'profileOverview.sections.contactInfo';
      case 4: return 'profileOverview.sections.qualifications';
      case 5: return 'profileOverview.sections.experiences';
      case 6: return 'profileOverview.sections.training';
      case 7: return 'profileOverview.sections.certificates';
      case 8: return 'profileOverview.sections.skills';
      case 9: return 'profileOverview.sections.languages';
      case 10: return 'profileOverview.sections.attachments';
      default: return 'profileOverview.sections.finalReview';
    }
  }

  sectionStatusClass(status?: ReviewStatus | null): string {
    switch (status) {
      case ReviewStatus.Approved: return 'text-success';
      case ReviewStatus.Rejected: return 'text-danger';
      case ReviewStatus.ChangesRequested:
      case ReviewStatus.NeedsCorrection: return 'text-warning';
      default: return 'text-muted';
    }
  }

  sectionStatusLabelKey(status?: ReviewStatus | null): string {
    switch (status) {
      case ReviewStatus.Approved: return 'profileApproval.status.approved';
      case ReviewStatus.Rejected: return 'profileApproval.status.rejected';
      case ReviewStatus.ChangesRequested:
      case ReviewStatus.NeedsCorrection: return 'profileApproval.status.changes';
      default: return 'profileApproval.status.pending';
    }
  }

  private needsCorrectionTargets(info: ProfileApprovalDetail): string[] {
    const ids = new Set<string>();
    const sections = (info.sections ?? [])
      .filter(s => s.sectionReview != null)
      .sort((a, b) => a.section - b.section);

    sections.forEach(sec => {
      if (sec.sectionReview && this.isCorrectionStatus(sec.sectionReview.status)) {
        ids.add(sec.sectionReview.reviewItemId);
      }

      (sec.items ?? []).forEach(item => {
        if (this.isCorrectionStatus(item.status)) ids.add(item.reviewItemId);
      });
    });

    return Array.from(ids);
  }

  private isCorrectionStatus(status?: ReviewStatus | null): boolean {
    return (
      status === ReviewStatus.ChangesRequested ||
      status === ReviewStatus.NeedsCorrection ||
      status === ReviewStatus.Rejected
    );
  }

  private progressStats(info: ProfileApprovalDetail): {
    pendingSections: number;
    flaggedSections: number;
    approvedSections: number;
    pendingItems: number;
  } {
    const stats = { pendingSections: 0, flaggedSections: 0, approvedSections: 0, pendingItems: 0 };

    (info.sections ?? []).forEach(sec => {
      const status = sec.sectionReview?.status ?? ReviewStatus.Pending;

      if (status === ReviewStatus.Approved) stats.approvedSections += 1;
      else if (status === ReviewStatus.Rejected || status === ReviewStatus.ChangesRequested || status === ReviewStatus.NeedsCorrection)
        stats.flaggedSections += 1;
      else stats.pendingSections += 1;

      (sec.items ?? []).forEach(item => {
        if (
          item.status === ReviewStatus.Pending ||
          item.status === ReviewStatus.NeedsCorrection ||
          item.status === ReviewStatus.ChangesRequested
        ) {
          stats.pendingItems += 1;
        }
      });
    });

    return stats;
  }
}
