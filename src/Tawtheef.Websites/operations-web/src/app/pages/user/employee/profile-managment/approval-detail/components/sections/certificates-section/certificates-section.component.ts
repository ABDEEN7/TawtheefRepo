import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { TranslateModule } from '@ngx-translate/core';
import { ItemInlineReviewComponent } from '../../item-inline-review/item-inline-review';
import { ProfileApprovalItem, ReviewStatus } from '../../../../approval-list/models/profile-approval.models';

@Component({
  selector: 'app-profile-approval-certificates-section',
  standalone: true,
  imports: [CommonModule, TranslateModule, CardModule, ButtonModule, TagModule, ItemInlineReviewComponent],
  templateUrl: './certificates-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class CertificatesSectionComponent {
  @Input() certificates?: any[] | null = null;
  @Input() reviewItems: ProfileApprovalItem[] | null = null;
  @Output() viewFile = new EventEmitter<{ url: string; fileName: string }>();
  @Output() reviewItem = new EventEmitter<{ reviewItemId: string; status: ReviewStatus; note?: string }>();

  preview(url?: string | null, fileName?: string | null): void {
    if (url) {
      this.viewFile.emit({ url, fileName: fileName || '' });
    }
  }

  reviewItemFor(entityId?: string): ProfileApprovalItem | null {
    if (!entityId) return null;
    return (this.reviewItems ?? []).find(i => i.entityId === entityId) ?? null;
  }
}
