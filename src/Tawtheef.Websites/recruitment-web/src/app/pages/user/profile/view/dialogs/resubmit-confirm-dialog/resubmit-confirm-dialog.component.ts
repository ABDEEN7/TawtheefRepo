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

    return null;
  }

  protected confirm(): void {
    this.ref.close(true);
  }

  protected cancel(): void {
    this.ref.close(false);
  }
}
