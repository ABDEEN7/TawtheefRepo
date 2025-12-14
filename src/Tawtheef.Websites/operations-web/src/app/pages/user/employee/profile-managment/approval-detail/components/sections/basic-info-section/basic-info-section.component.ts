import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TranslateModule } from '@ngx-translate/core';
import {ProfileApprovalData} from '../../../../approval-list/models/profile-approval.models';

@Component({
  selector: 'app-profile-approval-basic-info-section',
  standalone: true,
  imports: [CommonModule, TranslateModule, CardModule, ButtonModule],
  templateUrl: './basic-info-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class BasicInfoSectionComponent {
  @Input({ required: true }) profile!: ProfileApprovalData;
  @Output() viewFile = new EventEmitter<string>();

  hasValue(value: unknown): boolean {
    return value !== null && value !== undefined && `${value}`.toString().trim() !== '';
  }

  hasBasicFiles(): boolean {
    const b = this.profile?.basicInformation;
    return !!(
      b?.resumeAttachment ||
      b?.nationalCard ||
      b?.birthdayCertificate ||
      b?.marriageCertificate ||
      b?.residenceAddressCertificate ||
      b?.sponsorCard
    );
  }

  preview(url?: string | null): void {
    if (url) {
      this.viewFile.emit(url);
    }
  }
}
