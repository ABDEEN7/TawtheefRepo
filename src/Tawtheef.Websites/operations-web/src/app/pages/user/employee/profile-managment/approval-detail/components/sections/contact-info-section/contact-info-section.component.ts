import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TranslateModule } from '@ngx-translate/core';
import {ProfileApprovalData} from '../../../../approval-list/models/profile-approval.models';
import {Ripple} from 'primeng/ripple';

@Component({
  selector: 'app-profile-approval-contact-info-section',
  standalone: true,
  imports: [CommonModule, TranslateModule, CardModule, ButtonModule, Ripple],
  templateUrl: './contact-info-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class ContactInfoSectionComponent {
  @Input({ required: true }) profile!: ProfileApprovalData;
  @Output() viewFile = new EventEmitter<string>();

  contactFields(): { label: string; value: unknown }[] {
    const fields = [
      { label: 'profileApproval.detail.snapshot.email', value: this.profile.basicInformation.email },
      { label: 'profileApproval.detail.snapshot.phoneNumber', value: this.profile.basicInformation.phoneNumber },
      { label: 'profileApproval.detail.snapshot.interviewLocation', value: this.profile.basicInformation.interviewLocation },
      { label: 'profileApproval.detail.snapshot.residenceCountry', value: this.profile.basicInformation.residenceCountry },
      { label: 'profileApproval.detail.snapshot.address', value: this.profile.basicInformation.address },
    ];

    return fields.filter(field => this.hasValue(field.value));
  }

  hasValue(value: unknown): boolean {
    return value !== null && value !== undefined && `${value}`.toString().trim() !== '';
  }

  hasBasicFiles(): boolean {
    const b = this.profile?.basicInformation;
    return !!(
      b?.residenceAddress?.certificate
    );
  }

  preview(url?: string | null): void {
    if (url) {
      this.viewFile.emit(url);
    }
  }
}
