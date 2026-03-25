import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { TagModule } from 'primeng/tag';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { TranslateService } from '@ngx-translate/core';
import {
  ProfileApprovalItem, ReviewStatus,
  SpecializationRelationLevel
} from '../../../../approval-list/models/profile-approval.models';
import { ItemInlineReviewComponent } from '../../item-inline-review/item-inline-review';

@Component({
  selector: 'app-profile-approval-training-section',
  standalone: true,
  imports: [CommonModule, TranslateModule, CardModule, ButtonModule, TagModule, SelectModule, FormsModule, ItemInlineReviewComponent],
  templateUrl: './training-section.component.html',
  styleUrls: ['../../../profile-approval-detail.page.scss'],
})
export class TrainingSectionComponent {
  @Input() trainingCourses?: any[] | null = null;
  @Input() reviewItems: ProfileApprovalItem[] | null = null;
  @Output() viewFile = new EventEmitter<{ url: string; fileName: string }>();
  @Output() reviewItem = new EventEmitter<{ reviewItemId: string; status: ReviewStatus; note?: string; specializationRelation?: number | null }>();

  protected readonly relationOptions = [
    { labelKey: 'profileApproval.detail.specializationRelation.Strong', value: SpecializationRelationLevel.Strong },
    { labelKey: 'profileApproval.detail.specializationRelation.Medium', value: SpecializationRelationLevel.Medium },
    { labelKey: 'profileApproval.detail.specializationRelation.Weak', value: SpecializationRelationLevel.Weak },
  ];

  selectedRelations: Record<string, SpecializationRelationLevel | null> = {};

  private translate = inject(TranslateService);

  preview(url?: string | null, fileName?: string | null): void {
    if (url) {
      this.viewFile.emit({ url, fileName: fileName || '' });
    }
  }

  relationLabel(level?: SpecializationRelationLevel | null): string {
    if (level === null || level === undefined) {
      return '-';
    }

    const key = SpecializationRelationLevel[level];
    return this.translate.instant(`profileApproval.detail.specializationRelation.${key}`);
  }

  reviewItemFor(entityId?: string): ProfileApprovalItem | null {
    if (!entityId) return null;
    return (this.reviewItems ?? []).find(i => i.entityId === entityId) ?? null;
  }

  onReview(event: { reviewItemId: string; status: ReviewStatus }, courseId: string): void {
    const relation = this.selectedRelations[courseId];
    if (event.status === ReviewStatus.Approved && (relation === null || relation === undefined)) {
      return;
    }
    this.reviewItem.emit({ ...event, specializationRelation: relation });
  }

  protected readonly ReviewStatus = ReviewStatus;
}
