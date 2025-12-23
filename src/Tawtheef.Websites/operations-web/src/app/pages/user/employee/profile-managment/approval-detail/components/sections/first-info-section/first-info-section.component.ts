import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TranslateModule } from '@ngx-translate/core';
import {ProfileApprovalData} from '../../../../approval-list/models/profile-approval.models';
import {Ripple} from 'primeng/ripple';

@Component({
  selector: 'app-profile-approval-first-info-section',
  standalone: true,
  imports: [CommonModule, TranslateModule, CardModule, ButtonModule, Ripple],
  templateUrl: './first-info-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class FirstInfoSectionComponent {
  @Input({ required: true }) profile!: ProfileApprovalData;
  @Output() viewFile = new EventEmitter<string>();

  firstInfoFields(): { label: string; value: unknown }[] {
    const fields = [
      { label: 'profileApproval.detail.snapshot.candidateType', value: this.profile.basicInformation.candidateType },
      { label: 'profileApproval.detail.snapshot.targetEntity', value: this.profile.basicInformation.targetEntity },
      { label: 'profileApproval.detail.snapshot.office', value: this.profile.basicInformation.office },
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
      b?.marriageCertificate
    );
  }

  preview(url?: string | null): void {
    if (url) {
      this.viewFile.emit(url);
    }
  }
}
