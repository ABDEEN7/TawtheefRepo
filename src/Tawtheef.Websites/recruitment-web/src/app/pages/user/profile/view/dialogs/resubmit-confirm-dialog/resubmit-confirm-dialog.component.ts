import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ButtonDirective } from 'primeng/button';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TranslatePipe } from '@ngx-translate/core';
import { MyProfileReviewChangedItemDto, ReviewTargetTypeEnum } from '../../models/profile-overview.model';

export interface ResubmitConfirmSectionVm {
  section: number;
  labelKey: string;
  items: MyProfileReviewChangedItemDto[];
}

@Component({
  selector: 'app-resubmit-confirm-dialog',
  standalone: true,
  imports: [CommonModule, TranslatePipe, ButtonDirective],
  templateUrl: './resubmit-confirm-dialog.component.html',
  styleUrl: './resubmit-confirm-dialog.component.scss'
})
export class ResubmitConfirmDialogComponent {
  private readonly ref = inject(DynamicDialogRef);
  private readonly config = inject(DynamicDialogConfig);

  protected readonly sections: ResubmitConfirmSectionVm[] = this.config.data?.sections ?? [];
  protected readonly fallbackLabels: string[] = this.config.data?.fallbackLabels ?? [];

  protected itemTypeLabelKey(item: MyProfileReviewChangedItemDto): string {
    switch (item.targetType) {
      case ReviewTargetTypeEnum.Attachment:
        return 'profileOverview.resubmitConfirm.type.attachment';
      case ReviewTargetTypeEnum.Row:
        return 'profileOverview.resubmitConfirm.type.row';
      case ReviewTargetTypeEnum.Field:
        return 'profileOverview.resubmitConfirm.type.field';
      default:
        return 'profileOverview.resubmitConfirm.type.section';
    }
  }

  protected itemTitleKey(item: MyProfileReviewChangedItemDto): string | null {
    if (item.fieldPath === 'SectionData') {
      return 'profileOverview.resubmitConfirm.sectionData';
    }

    return REVIEW_TITLE_TRANSLATION_KEYS[normalizeReviewTitle(item.title)] ??
      REVIEW_TITLE_TRANSLATION_KEYS[normalizeReviewTitle(item.fieldPath)] ??
      REVIEW_TITLE_TRANSLATION_KEYS[normalizeReviewTitle(item.entityName)] ??
      null;
  }

  protected itemTitle(item: MyProfileReviewChangedItemDto): string {
    return item.entityName || item.title;
  }

  protected confirm(): void {
    this.ref.close(true);
  }

  protected cancel(): void {
    this.ref.close(false);
  }
}

function normalizeReviewTitle(value?: string | null): string {
  return (value ?? '').toLowerCase().replace(/[^a-z0-9]/g, '');
}

const REVIEW_TITLE_TRANSLATION_KEYS: Record<string, string> = {
  sectiondata: 'profileOverview.resubmitConfirm.sectionData',
  resume: 'profileOverview.attachments.resume',
  cv: 'profileOverview.attachments.resume',
  resumeattachmentid: 'profileOverview.attachments.resume',
  nationalcard: 'profileOverview.attachments.nationalCard',
  nationalidcard: 'profileOverview.attachments.nationalCard',
  nationalcardid: 'profileOverview.attachments.nationalCard',
  birthcertificate: 'profileOverview.attachments.birthdayCertificate',
  birthdaycertificate: 'profileOverview.attachments.birthdayCertificate',
  birthdaycertificateid: 'profileOverview.attachments.birthdayCertificate',
  marriagecertificate: 'profileOverview.attachments.marriageCertificate',
  marriagecertificateid: 'profileOverview.attachments.marriageCertificate',
  sponsorcard: 'profileOverview.attachments.sponsorCard',
  sponsorcardid: 'profileOverview.attachments.sponsorCard',
  sponsorcardresourceid: 'profileOverview.attachments.sponsorCard',
  nationaladdresscertificate: 'profileOverview.attachments.residenceAddressCertificate',
  nationaladdresscertificateid: 'profileOverview.attachments.residenceAddressCertificate',
  residenceaddresscertificateid: 'profileOverview.attachments.residenceAddressCertificate',
  residenceaddress: 'profileOverview.attachments.residenceAddressCertificate',
  residenceaddresscertificate: 'profileOverview.attachments.residenceAddressCertificate',
  qualification: 'profileOverview.sections.qualifications',
  experience: 'profileOverview.sections.experiences',
  trainingcourse: 'profileOverview.sections.trainingCourses',
  achievement: 'profileOverview.sections.certificatesAndAwards',
  skill: 'profileOverview.sections.skills',
  language: 'profileOverview.sections.languages',
  attachment: 'profileOverview.files.attachment',
  profileadditionalattachment: 'profileOverview.files.attachment',
  additionalattachments: 'profileOverview.files.attachment',
  attachmentid: 'profileOverview.files.attachment',
  attachmentresourceid: 'profileOverview.files.attachment',
  certificateid: 'profileOverview.files.attachment',
};
