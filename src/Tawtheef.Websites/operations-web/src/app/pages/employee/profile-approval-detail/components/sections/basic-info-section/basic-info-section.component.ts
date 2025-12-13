import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { ProfileApprovalData } from '../../../../profile-approval/models/profile-approval.models';

@Component({
  selector: 'app-profile-approval-basic-info-section',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './basic-info-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class BasicInfoSectionComponent {
  @Input({ required: true }) profile!: ProfileApprovalData;
  @Output() viewFile = new EventEmitter<string>();

  hasBasicFiles(): boolean {
    const b = this.profile?.basicInformation;
    return !!(
      b?.resumeAttachment ||
      b?.nationalCard ||
      b?.birthdayCertificate ||
      b?.marriageCertificate ||
      b?.residenceAddressCertificate
    );
  }

  preview(url?: string | null): void {
    if (url) {
      this.viewFile.emit(url);
    }
  }
}
