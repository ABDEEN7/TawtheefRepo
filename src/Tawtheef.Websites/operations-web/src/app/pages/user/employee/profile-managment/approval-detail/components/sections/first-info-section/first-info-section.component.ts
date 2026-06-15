import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TranslateModule } from '@ngx-translate/core';
import {
  ProfileApprovalData,
  ProfileApprovalItem,
  ReviewStatus,
  ReviewTargetType
} from '../../../../approval-list/models/profile-approval.models';
import { Ripple } from 'primeng/ripple';
import { ItemInlineReviewComponent } from '../../item-inline-review/item-inline-review';
import { Tooltip } from 'primeng/tooltip';

@Component({
  selector: 'app-profile-approval-first-info-section',
  standalone: true,
  imports: [CommonModule, Tooltip, TranslateModule, CardModule, ButtonModule, Ripple, ItemInlineReviewComponent],
  templateUrl: './first-info-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class FirstInfoSectionComponent {
  private readonly sectionDataFieldPath = 'SectionData';

  @Input({ required: true }) profile!: ProfileApprovalData;
  @Input() reviewItems: ProfileApprovalItem[] | null = null;
  @Output() viewFile = new EventEmitter<{ url: string; fileName: string }>();
  @Output() reviewItem = new EventEmitter<{ reviewItemId: string; status: ReviewStatus; note?: string }>();

  firstInfoFields(): { label: string; value: unknown }[] {
    const fields = [
      { label: 'profileApproval.detail.snapshot.candidateType', value: this.profile.basicInformation.candidateType },
      { label: 'profileApproval.detail.snapshot.targetEntity', value: this.profile.basicInformation.targetEntity },
    ];

    return fields.filter(field => this.hasValue(field.value));
  }

  hasValue(value: unknown): boolean {
    return value !== null && value !== undefined && `${value}`.toString().trim() !== '';
  }

  hasBasicFiles(): boolean {
    const b = this.profile?.basicInformation;
    return !!(
      b?.birthdayCertificate ||
      b?.marriageCertificate ||
      b?.nationalCard ||
      b?.resumeAttachment
    );
  }

  preview(url?: string | null, fileName?: string | null): void {
    if (url) {
      this.viewFile.emit({ url, fileName: fileName || '' });
    }
  }

  reviewItemFor(resourceId?: string | null): ProfileApprovalItem | null {
    if (!resourceId) return null;
    return (this.reviewItems ?? []).find(i => i.resourceId === resourceId) ?? null;
  }

  sectionDataReviewItem(): ProfileApprovalItem | null {
    return (this.reviewItems ?? []).find(item =>
      item.targetType === ReviewTargetType.Field &&
      item.fieldPath === this.sectionDataFieldPath
    ) ?? null;
  }
}
