import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { TranslateModule } from '@ngx-translate/core';
import { ItemInlineReviewComponent } from '../../item-inline-review/item-inline-review';
import { ProfileApprovalItem, ReviewStatus } from '../../../../approval-list/models/profile-approval.models';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-profile-approval-qualifications-section',
  standalone: true,
  imports: [CommonModule, TranslateModule, CardModule, ButtonModule, TagModule, ItemInlineReviewComponent, TooltipModule],
  templateUrl: './qualifications-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class QualificationsSectionComponent {
  @Input() qualifications?: any[] | null = null;
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

  isOtherSpec(dto?: any | null): boolean {
    if (!dto) return false;
    const strId = dto.id?.toLowerCase() || '';
    return strId === '7044ac66-706b-4384-bb79-75c71801da8b' || strId === '60705656-fe6c-4f16-8bef-c61dfeca3cb2';
  }
}
