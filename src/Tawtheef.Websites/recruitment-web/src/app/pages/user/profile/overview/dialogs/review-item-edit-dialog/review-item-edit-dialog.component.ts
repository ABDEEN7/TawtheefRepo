import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ButtonDirective } from 'primeng/button';
import { Tag } from 'primeng/tag';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { TooltipModule } from 'primeng/tooltip';

import {
  MyProfileReviewNoteDto,
  ProfileSectionEnum,
  ReviewStatusEnum,
  ReviewTargetTypeEnum,
} from '../../models/profile-overview.model';
import {I18nNamespaceDirective} from '../../../../../../shared/directives/i18n-namespace.directive';

interface DialogData {
  note: MyProfileReviewNoteDto;
  section: ProfileSectionEnum;
  sectionLabel: string;
  fileUrl?: string | null;
  canEdit: boolean;
}

@Component({
  selector: 'app-review-item-edit-dialog',
  standalone: true,
  imports: [CommonModule, TranslateModule, ButtonDirective, Tag, TooltipModule, I18nNamespaceDirective],
  templateUrl: './review-item-edit-dialog.component.html',
  styleUrls: ['./review-item-edit-dialog.component.scss'],
})
export class ReviewItemEditDialogComponent {
  private readonly translate = inject(TranslateService);
  private readonly config = inject(DynamicDialogConfig<DialogData>);
  private readonly ref = inject(DynamicDialogRef);

  protected readonly ReviewStatusEnum = ReviewStatusEnum;
  protected readonly ReviewTargetTypeEnum = ReviewTargetTypeEnum;
  protected readonly note = this.config.data?.note;

  get sectionLabel(): string {
    return this.config.data?.sectionLabel ?? '';
  }

  get canEdit(): boolean {
    return !!this.config.data?.canEdit;
  }

  get fileUrl(): string | null {
    return this.config.data?.fileUrl ?? null;
  }

  statusSeverity(status: ReviewStatusEnum): 'warn' | 'danger' | 'success' | 'info' {
    switch (status) {
      case ReviewStatusEnum.NeedsCorrection:
        return 'warn';
      case ReviewStatusEnum.Rejected:
        return 'danger';
      case ReviewStatusEnum.Approved:
        return 'success';
      default:
        return 'info';
    }
  }

  statusLabelKey(status: ReviewStatusEnum): string {
    if (status === ReviewStatusEnum.NeedsCorrection) return 'profileOverview.reviewStatus.needsCorrection';
    if (status === ReviewStatusEnum.Rejected) return 'profileOverview.reviewStatus.rejected';
    if (status === ReviewStatusEnum.Approved) return 'profileOverview.reviewStatus.approved';
    return 'profileOverview.reviewStatus.other';
  }

  close(): void {
    this.ref.close();
  }

  edit(): void {
    if (!this.canEdit) return;
    this.ref.close({ section: this.config?.data?.section });
  }

  preview(): void {
    if (!this.fileUrl) return;
    this.ref.close({ preview: this.fileUrl });
  }

  targetHintKey(targetType?: ReviewTargetTypeEnum): string {
    if (targetType === ReviewTargetTypeEnum.Attachment) {
      return 'profileOverview.reviewItemDialog.fileHint';
    }
    if (targetType === ReviewTargetTypeEnum.Row) {
      return 'profileOverview.reviewItemDialog.rowHint';
    }
    return 'profileOverview.reviewItemDialog.genericHint';
  }
}
