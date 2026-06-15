import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import {
  ProfileApprovalData,
  ProfileApprovalItem,
  ReviewStatus,
  ReviewTargetType,
} from '../../../../approval-list/models/profile-approval.models';
import { Ripple } from 'primeng/ripple';
import { ItemInlineReviewComponent } from '../../item-inline-review/item-inline-review';
import { Tooltip } from 'primeng/tooltip';
import { SponsorType } from '../../../../../../../../core/enums/lookups.enum';

@Component({
  selector: 'app-profile-approval-basic-info-section',
  standalone: true,
  imports: [CommonModule, Tooltip, TranslateModule, CardModule, ButtonModule, Ripple, ItemInlineReviewComponent],
  templateUrl: './basic-info-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class BasicInfoSectionComponent {
  private readonly sectionDataFieldPath = 'SectionData';

  @Input({ required: true }) profile!: ProfileApprovalData;
  @Input() reviewItems: ProfileApprovalItem[] | null = null;
  @Output() viewFile = new EventEmitter<{ url: string; fileName: string }>();
  @Output() reviewItem = new EventEmitter<{ reviewItemId: string; status: ReviewStatus; note?: string }>();
  private translate = inject(TranslateService);

  basicFields(): { label: string; value: unknown }[] {
    const fields = [
      { label: 'profileApproval.detail.snapshot.nationalNumber', value: this.profile.basicInformation.nationalNumber },
      { label: 'profileApproval.detail.snapshot.fullNameAr', value: this.profile.basicInformation.fullNameAr },
      { label: 'profileApproval.detail.snapshot.fullNameEn', value: this.profile.basicInformation.fullNameEn },
      { label: 'profileApproval.detail.snapshot.birthDate', value: this.profile.basicInformation.birthDate },
      { label: 'profileApproval.detail.snapshot.gender', value: this.profile.basicInformation.gender },
      { label: 'profileApproval.detail.snapshot.nationality', value: this.profile.basicInformation.nationality },
      { label: 'profileApproval.detail.snapshot.religion', value: this.profile.basicInformation.religion },
      { label: 'profileApproval.detail.snapshot.maritalStatus', value: this.profile.basicInformation.maritalStatus },
      {
        label: 'profileApproval.detail.snapshot.disability',
        value:
          this.profile.basicInformation.hasDisability === undefined
            ? null
            : this.profile.basicInformation.hasDisability
              ? this.translate.instant('profileApproval.detail.snapshot.disabilityYes')
              : this.translate.instant('profileApproval.detail.snapshot.disabilityNo'),
      },
      { label: 'profileApproval.detail.snapshot.disabilityDetails', value: this.profile.basicInformation.disabilityDetails },

      { label: 'profileApproval.detail.snapshot.idExpiry', value: this.profile.basicInformation.qidExpiry },
      { label: 'profileApproval.detail.snapshot.sponsorType', value: this.profile.basicInformation.sponsorType },
      { label: this.sponsorLabelKey('name'), value: this.profile.basicInformation.sponsorEmployerName },
      { label: this.sponsorLabelKey('number'), value: this.profile.basicInformation.sponsorEmployerNumber },
    ];

    if (!this.isCompanySponsor()) {
      fields.push({
        label: 'profileApproval.detail.snapshot.sponsorQidExpiry',
        value: this.profile.basicInformation.sponsorQidExpiry
      });
    }

    return fields.filter(field => this.hasValue(field.value));
  }

  hasValue(value: unknown): boolean {
    return value !== null &&
      value !== undefined &&
      `${value}`.toString().trim() !== '' &&
      value !== '0001-01-01';
  }

  hasBasicFiles(): boolean {
    const b = this.profile?.basicInformation;
    return !!b?.sponsorCard;
  }

  sponsorCardLabelKey(): string {
    return this.sponsorLabelKey('card');
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

  private sponsorLabelKey(field: 'name' | 'number' | 'card'): string {
    const type = this.sponsorTypeKey();
    if (!type) {
      const fallback: Record<typeof field, string> = {
        name: 'profileApproval.detail.snapshot.sponsorEmployerName',
        number: 'profileApproval.detail.snapshot.sponsorEmployerNumber',
        card: 'profileApproval.detail.snapshot.sponsorCard',
      };
      return fallback[field];
    }

    return `profileApproval.detail.snapshot.sponsor.${type}.${field}`;
  }

  private sponsorTypeKey(): 'individual' | 'company' | null {
    const backendName = this.profile?.basicInformation?.sponsorTypeBackendName;
    if (backendName === SponsorType.Company) return 'company';
    if (backendName === SponsorType.Individual) return 'individual';

    const sponsorType = (this.profile?.basicInformation?.sponsorType ?? '').toLowerCase();
    if (sponsorType.includes('company') || sponsorType.includes('establishment') || sponsorType.includes('منش')) {
      return 'company';
    }
    if (sponsorType.includes('individual') || sponsorType.includes('فرد')) {
      return 'individual';
    }

    return null;
  }

  private isCompanySponsor(): boolean {
    return this.sponsorTypeKey() === 'company';
  }
}
