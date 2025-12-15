import {Component, inject, Input} from '@angular/core';
import {ButtonDirective} from "primeng/button";
import {NgClass, NgForOf} from "@angular/common";
import {TranslatePipe, TranslateService} from "@ngx-translate/core";
import {
  ProfileApprovalDetail,
  ProfileApprovalSection,
  ReviewStatus
} from '../../../../approval-list/models/profile-approval.models';
import {NotificationService} from '../../../../../../core/services/notification.service';

@Component({
  selector: 'app-profile-approval-final-review-section',
  imports: [
    ButtonDirective,
    NgForOf,
    TranslatePipe,
    NgClass
  ],
  templateUrl: './final-review-section.html',
  styleUrl: './final-review-section.scss',
})
export class FinalReviewSection {
  translate = inject(TranslateService);
  messages = inject(NotificationService);

  @Input({ required: true })
  info!: ProfileApprovalDetail;
  finalStatusSummary(info: ProfileApprovalDetail): Record<'approved' | 'rejected' | 'changes' | 'pending', number> {
    const summary = { approved: 0, rejected: 0, changes: 0, pending: 0 };

    info.sections.forEach(sec => {
      const st = sec.sectionReview?.status;

      if (st === ReviewStatus.Approved) summary.approved += 1;
      else if (st === ReviewStatus.Rejected) summary.rejected += 1;
      else if (st === ReviewStatus.ChangesRequested || st === ReviewStatus.NeedsCorrection) summary.changes += 1;
      else summary.pending += 1;
    });

    return summary;
  }
  sectionName(section: number): string {
    switch (section) {
      case 1:
        return 'profileOverview.sections.prerequisites';
      case 2:
        return 'profileOverview.sections.basicInfo';
      case 3:
        return 'profileOverview.sections.contactInfo';
      case 4:
        return 'profileOverview.sections.qualifications';
      case 5:
        return 'profileOverview.sections.experiences';
      case 6:
        return 'profileOverview.sections.training';
      case 7:
        return 'profileOverview.sections.certificates';
      case 8:
        return 'profileOverview.sections.skills';
      case 9:
        return 'profileOverview.sections.languages';
      case 10:
        return 'profileOverview.sections.attachments';
      default:
        return 'profileOverview.sections.finalReview';
    }
  }
  sectionStatusClass(status?: ReviewStatus | null): string {
    switch (status) {
      case ReviewStatus.Approved:
        return 'text-success';
      case ReviewStatus.Rejected:
        return 'text-danger';
      case ReviewStatus.ChangesRequested:
      case ReviewStatus.NeedsCorrection:
        return 'text-warning';
      default:
        return 'text-muted';
    }
  }
  sectionStatusLabel(status?: ReviewStatus | null): string {
    switch (status) {
      case ReviewStatus.Approved:
        return this.translate.instant('profileApproval.status.approved');
      case ReviewStatus.Rejected:
        return this.translate.instant('profileApproval.status.rejected');
      case ReviewStatus.ChangesRequested:
      case ReviewStatus.NeedsCorrection:
        return this.translate.instant('profileApproval.status.changes');
      default:
        return this.translate.instant('profileApproval.status.pending');
    }
  }
  orderedSections(info: ProfileApprovalDetail): ProfileApprovalSection[] {
    return this.sortSections(info.sections);
  }
  private sortSections(sections: ProfileApprovalSection[] | null | undefined): ProfileApprovalSection[] {
    if (!sections?.length) return [];
    sections = [...sections].sort((a, b) => a.section - b.section);
    return sections.filter(sec => sec.sectionReview != null);
  }
  sendApprovalReport(): void {
    this.messages.success(
      `${this.translate.instant('profileApproval.finalReview.reportSentTitle')}: ${this.translate.instant('profileApproval.finalReview.reportSentMessage')}`
    );
  }
}
