import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { TranslateModule } from '@ngx-translate/core';
import {ProfileApprovalItem, ReviewStatus} from '../../../../approval-list/models/profile-approval.models';
import {ItemInlineReviewComponent} from '../../item-inline-review/item-inline-review';

@Component({
  selector: 'app-profile-approval-qualifications-section',
  standalone: true,
  imports: [CommonModule, TranslateModule, CardModule, ButtonModule, TagModule, ItemInlineReviewComponent],
  templateUrl: './qualifications-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class QualificationsSectionComponent {
  @Input() qualifications: any[] | null = null;
  @Input() reviewItems: ProfileApprovalItem[] = [];
  @Output() viewFile = new EventEmitter<string>();
  @Output() review = new EventEmitter<{
    reviewItemId: string;
    status: ReviewStatus;
    note?: string;
  }>();
  preview(url?: string | null): void {
    if (url) {
      this.viewFile.emit(url);
    }
  }
  getItemReview(id: string) {
    return this.reviewItems?.find(r => r.entityId === id);
  }
}
