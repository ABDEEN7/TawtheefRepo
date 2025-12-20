import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { TranslateModule } from '@ngx-translate/core';
import {ItemInlineReviewComponent} from '../../item-inline-review/item-inline-review';
import {ProfileApprovalItem, ReviewStatus} from '../../../../approval-list/models/profile-approval.models';

@Component({
  selector: 'app-profile-approval-certificates-section',
  standalone: true,
  imports: [CommonModule, TranslateModule, CardModule, ButtonModule, TagModule],
  templateUrl: './certificates-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class CertificatesSectionComponent {
  @Input() certificates: any[] | null = null;
  @Output() viewFile = new EventEmitter<string>();

  preview(url?: string | null): void {
    if (url) {
      this.viewFile.emit(url);
    }
  }
}
