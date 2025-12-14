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
