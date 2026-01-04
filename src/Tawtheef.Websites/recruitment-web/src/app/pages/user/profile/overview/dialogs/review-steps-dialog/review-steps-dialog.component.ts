import { Component, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Tag } from 'primeng/tag';
import { ButtonDirective } from 'primeng/button';
import { DividerModule } from 'primeng/divider';

import {
  MyProfileReviewSectionDto,
  ProfileSectionEnum,
  ReviewStatusEnum,
} from '../../models/profile-overview.model';
import {I18nNamespaceDirective} from '../../../../../../shared/directives/i18n-namespace.directive';

@Component({
  selector: 'app-review-steps-dialog',
  standalone: true,
  imports: [CommonModule, TranslateModule, Tag, ButtonDirective, DividerModule, I18nNamespaceDirective],
  templateUrl: './review-steps-dialog.component.html',
  styleUrls: ['./review-steps-dialog.component.scss'],
})
export class ReviewStepsDialogComponent {
  private readonly translate = inject(TranslateService);
  private readonly config = inject(DynamicDialogConfig<{ sections: MyProfileReviewSectionDto[] }>);
  private readonly ref = inject(DynamicDialogRef);

  protected readonly ReviewStatusEnum = ReviewStatusEnum;
  protected readonly steps = computed(() => this.config?.data?.sections ?? []);

  sectionLabelKey(section: ProfileSectionEnum): string {
    switch (section) {
      case ProfileSectionEnum.Prerequisites:
        return 'profileView.sections.prerequisites';
      case ProfileSectionEnum.Personal:
        return 'profileView.sections.personal';
      case ProfileSectionEnum.Contact:
        return 'profileView.sections.contact';
      case ProfileSectionEnum.Qualifications:
        return 'profileView.sections.qualifications';
      case ProfileSectionEnum.Experience:
        return 'profileView.sections.experiences';
      case ProfileSectionEnum.TrainingCourses:
        return 'profileView.sections.trainingCourses';
      case ProfileSectionEnum.CertificatesAndAwards:
        return 'profileView.sections.certificatesAndAwards';
      case ProfileSectionEnum.Skills:
        return 'profileView.sections.skills';
      case ProfileSectionEnum.Languages:
        return 'profileView.sections.languages';
      case ProfileSectionEnum.Attachments:
        return 'profileView.sections.attachments';
      default:
        return 'common.section';
    }
  }

  statusLabelKey(status: ReviewStatusEnum): string {
    switch (status) {
      case ReviewStatusEnum.NeedsCorrection:
        return 'profileView.reviewStatus.needsCorrection';
      case ReviewStatusEnum.Rejected:
        return 'profileView.reviewStatus.rejected';
      case ReviewStatusEnum.Approved:
        return 'profileView.reviewStatus.approved';
      case ReviewStatusEnum.Pending:
      default:
        return 'profileView.reviewStatus.other';
    }
  }

  statusSeverity(status: ReviewStatusEnum): 'warn' | 'danger' | 'success' | 'info' | 'secondary' {
    switch (status) {
      case ReviewStatusEnum.NeedsCorrection:
        return 'warn';
      case ReviewStatusEnum.Rejected:
        return 'danger';
      case ReviewStatusEnum.Approved:
        return 'success';
      case ReviewStatusEnum.Pending:
      default:
        return 'info';
    }
  }

  close(): void {
    this.ref.close();
  }

  editSection(section: ProfileSectionEnum): void {
    this.ref.close({ section });
  }

  sectionSummary(section: MyProfileReviewSectionDto): string {
    if (!section.notesCount) return this.translate.instant('profileView.reviewSteps.noNotes');
    return this.translate.instant('profileView.reviewSteps.notesCount', { count: section.notesCount });
  }
}
