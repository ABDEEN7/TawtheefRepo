import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TranslateModule } from '@ngx-translate/core';
import {
  ProfileApprovalData,
  ProfileApprovalItem,
  ReviewStatus,
  ReviewTargetType,
} from '../../../../approval-list/models/profile-approval.models';
import { Ripple } from 'primeng/ripple';
import { ItemInlineReviewComponent } from '../../item-inline-review/item-inline-review';
import { Tooltip } from 'primeng/tooltip';

@Component({
  selector: 'app-profile-approval-contact-info-section',
  standalone: true,
  imports: [CommonModule, Tooltip, TranslateModule, CardModule, ButtonModule, Ripple, ItemInlineReviewComponent],
  templateUrl: './contact-info-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class ContactInfoSectionComponent {
  private readonly sectionDataFieldPath = 'SectionData';

  @Input({ required: true }) profile!: ProfileApprovalData;
  @Input() reviewItems: ProfileApprovalItem[] | null = null;
  @Output() viewFile = new EventEmitter<{ url: string; fileName: string }>();
  @Output() reviewItem = new EventEmitter<{ reviewItemId: string; status: ReviewStatus; note?: string }>();

  contactFields(): { label: string; value: unknown }[] {
    const fields = [
      { label: 'profileApproval.detail.snapshot.email', value: this.profile.basicInformation.email },
      { label: 'profileApproval.detail.snapshot.phoneNumber', value: this.profile.basicInformation.phoneNumber },
      { label: 'profileApproval.detail.snapshot.interviewLocation', value: this.profile.basicInformation.interviewLocation },
      { label: 'profileApproval.detail.snapshot.residenceCountry', value: this.profile.basicInformation.residenceCountry },
      { label: 'profileApproval.detail.snapshot.address', value: this.profile.basicInformation.address },

      { label: 'profileApproval.detail.snapshot.buildingNo', value: this.profile.basicInformation.residenceAddress?.naBuilding },
      { label: 'profileApproval.detail.snapshot.streetNo', value: this.profile.basicInformation.residenceAddress?.naStreet },
      { label: 'profileApproval.detail.snapshot.zoneNo', value: this.profile.basicInformation.residenceAddress?.naZone },
      { label: 'profileApproval.detail.snapshot.unitNo', value: this.profile.basicInformation.residenceAddress?.naUnit },
    ];

    return fields.filter(field => this.hasValue(field.value));
  }

  hasValue(value: unknown): boolean {
    return value !== null && value !== undefined && `${value}`.toString().trim() !== '';
  }

  hasBasicFiles(): boolean {
    const b = this.profile?.basicInformation;
    return !!(
      b?.residenceAddress?.residenceAddressCertificate
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
