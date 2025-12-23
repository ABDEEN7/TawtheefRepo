import { Component, EventEmitter, Input, Output, signal } from '@angular/core';
import {ReviewStatus} from '../../../approval-list/models/profile-approval.models';
import {ButtonDirective} from 'primeng/button';
import {Textarea} from 'primeng/textarea';
import {TranslatePipe} from '@ngx-translate/core';
import {NgClass, NgIf} from '@angular/common';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-item-inline-review',
  standalone: true,
  templateUrl: './item-inline-review.html',
  styleUrl: './item-inline-review.scss',
  imports: [
    ButtonDirective,
    Textarea,
    TranslatePipe,
    NgIf,
    FormsModule,
    NgClass
  ]
})
export class ItemInlineReviewComponent {
  @Input({ required: true }) reviewItemId!: string;
  @Input() status?: ReviewStatus | null;
  @Input() note?: string | null;

  @Output() review = new EventEmitter<{
    reviewItemId: string;
    status: ReviewStatus;
    note?: string;
  }>();

  noteDraft = signal(this.note ?? '');

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
      note: this.noteDraft().trim(),
    });
  }

  reject(): void {
    this.review.emit({
      reviewItemId: this.reviewItemId,
      status: ReviewStatus.Rejected,
      note: this.noteDraft().trim(),
    });
  }

  needsNote(): boolean {
    return (
      this.status === ReviewStatus.ChangesRequested ||
      this.status === ReviewStatus.Rejected
    );
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
