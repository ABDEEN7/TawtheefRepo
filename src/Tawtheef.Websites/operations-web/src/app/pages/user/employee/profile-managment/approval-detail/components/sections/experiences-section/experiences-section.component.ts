import { CommonModule } from '@angular/common';
import {Component, EventEmitter, inject, Input, Output} from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { TranslateModule } from '@ngx-translate/core';
import { TranslateService } from '@ngx-translate/core';
import {
  ProfileApprovalItem, ReviewStatus,
  SpecializationRelationLevel
} from '../../../../approval-list/models/profile-approval.models';
import {ItemInlineReviewComponent} from '../../item-inline-review/item-inline-review';

@Component({
  selector: 'app-profile-approval-experiences-section',
  standalone: true,
  imports: [CommonModule, TranslateModule, CardModule, ButtonModule, TagModule],
  templateUrl: './experiences-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class ExperiencesSectionComponent {
  @Input() experiences: any[] | null = null;
  @Output() viewFile = new EventEmitter<string>();
  private translate = inject(TranslateService);

  preview(url?: string | null): void {
    if (url) {
      this.viewFile.emit(url);
    }
  }

  relationLabel(level?: SpecializationRelationLevel | null): string {
    if (level === null || level === undefined) {
      return '-';
    }

    const key = SpecializationRelationLevel[level];
    return this.translate.instant(`profileApproval.detail.specializationRelation.${key}`);
  }
}
