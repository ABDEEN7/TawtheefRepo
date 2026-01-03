import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ReviewStatus } from '../../../approval-list/models/profile-approval.models';
import { ButtonDirective } from 'primeng/button';
import { TranslatePipe } from '@ngx-translate/core';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-item-inline-review',
  standalone: true,
  templateUrl: './item-inline-review.html',
  styleUrl: './item-inline-review.scss',
  imports: [
    ButtonDirective,
    TranslatePipe,
    NgClass,
  ]
})
export class ItemInlineReviewComponent {
  @Input({ required: true }) reviewItemId!: string;
  @Input() status?: ReviewStatus | null;
  @Input() note?: string | null;

  @Output() review = new EventEmitter<{
    reviewItemId: string;
    status: ReviewStatus;
  }>();

  approve(): void {
    this.review.emit({
      reviewItemId: this.reviewItemId,
      status: ReviewStatus.Approved,
    });
  }

  requestChanges(): void {
    this.review.emit({
      reviewItemId: this.reviewItemId,
      status: ReviewStatus.ChangesRequested,
    });
  }

  getClassStatus(): string{
    switch (this.status) {
      case ReviewStatus.Approved:
        return 'approved';
      case ReviewStatus.ChangesRequested:
        return 'changes-requested';
      case ReviewStatus.Rejected:
        return 'rejected';
      default:
        return 'pending';
    }
  }
}
