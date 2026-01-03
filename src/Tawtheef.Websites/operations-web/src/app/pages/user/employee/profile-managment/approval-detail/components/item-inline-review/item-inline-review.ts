import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ReviewStatus } from '../../../approval-list/models/profile-approval.models';
import { ButtonDirective } from 'primeng/button';
import { TranslatePipe } from '@ngx-translate/core';
import {NgClass, NgIf} from '@angular/common';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-item-inline-review',
  standalone: true,
  templateUrl: './item-inline-review.html',
  styleUrl: './item-inline-review.scss',
  imports: [
    ButtonDirective,
    TranslatePipe,
    NgClass,
    TooltipModule,
    NgIf,
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

  getClassStatus(): string {
    switch (this.status) {
      case ReviewStatus.Approved:
        return 'status-approved';
      case ReviewStatus.ChangesRequested:
        return 'status-changes';
      case ReviewStatus.Rejected:
        return 'status-rejected';
      default:
        return 'status-pending';
    }
  }

  getStatusKey(): string {
    switch (this.status) {
      case ReviewStatus.Approved:
        return 'approved';
      case ReviewStatus.ChangesRequested:
        return 'changes';
      case ReviewStatus.Rejected:
        return 'rejected';
      default:
        return 'pending';
    }
  }

  getStatusIcon(): string {
    switch (this.status) {
      case ReviewStatus.Approved:
        return 'pi pi-check-circle text-success';
      case ReviewStatus.ChangesRequested:
        return 'pi pi-exclamation-circle text-warning';
      case ReviewStatus.Rejected:
        return 'pi pi-ban text-danger';
      default:
        return 'pi pi-hourglass text-secondary';
    }
  }
}
