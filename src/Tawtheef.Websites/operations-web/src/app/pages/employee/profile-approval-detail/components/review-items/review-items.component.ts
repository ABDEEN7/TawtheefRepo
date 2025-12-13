import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { ProfileApprovalItem, ReviewStatus } from '../../../profile-approval/models/profile-approval.models';

export type ReviewAction = 'approve' | 'reject' | 'changes';

@Component({
  selector: 'app-profile-review-items',
  standalone: true,
  imports: [CommonModule, TranslateModule, ButtonModule, TagModule],
  templateUrl: './review-items.component.html',
  styleUrls: ['../../profile-approval-detail.page.scss'],
})
export class ReviewItemsComponent {
  @Input() items: ProfileApprovalItem[] | null = [];
  @Input() titleKey = '';
  @Output() review = new EventEmitter<{ item: ProfileApprovalItem; action: ReviewAction }>();
  @Output() viewFile = new EventEmitter<string>();

  protected readonly ReviewStatus = ReviewStatus;

  statusSeverity(status: ReviewStatus | undefined): 'success' | 'danger' | 'info' | 'warning' {
    switch (status) {
      case ReviewStatus.Approved:
        return 'success';
      case ReviewStatus.Rejected:
        return 'danger';
      case ReviewStatus.ChangesRequested:
        return 'warning';
      default:
        return 'info';
    }
  }

  statusLabel(status: ReviewStatus | undefined): string {
    switch (status) {
      case ReviewStatus.Approved:
        return 'profileApproval.status.approved';
      case ReviewStatus.Rejected:
        return 'profileApproval.status.rejected';
      case ReviewStatus.ChangesRequested:
        return 'profileApproval.status.changes';
      default:
        return 'profileApproval.status.pending';
    }
  }

  onReview(item: ProfileApprovalItem, action: ReviewAction): void {
    this.review.emit({ item, action });
  }

  preview(item: ProfileApprovalItem): void {
    if (item.resourceUrl) {
      this.viewFile.emit(item.resourceUrl);
    }
  }
}
