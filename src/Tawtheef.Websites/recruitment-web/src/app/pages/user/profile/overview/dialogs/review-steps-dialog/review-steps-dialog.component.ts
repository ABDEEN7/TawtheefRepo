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
        return 'profileOverview.sections.prerequisites';
      case ProfileSectionEnum.Personal:
        return 'profileOverview.sections.personal';
      case ProfileSectionEnum.Contact:
        return 'profileOverview.sections.contact';
      case ProfileSectionEnum.Qualifications:
        return 'profileOverview.sections.qualifications';
      case ProfileSectionEnum.Experience:
        return 'profileOverview.sections.experiences';
      case ProfileSectionEnum.TrainingCourses:
        return 'profileOverview.sections.trainingCourses';
      case ProfileSectionEnum.CertificatesAndAwards:
        return 'profileOverview.sections.certificatesAndAwards';
      case ProfileSectionEnum.Skills:
        return 'profileOverview.sections.skills';
      case ProfileSectionEnum.Languages:
        return 'profileOverview.sections.languages';
      case ProfileSectionEnum.Attachments:
        return 'profileOverview.sections.attachments';
      default:
        return 'common.section';
    }
  }

  statusLabelKey(status: ReviewStatusEnum): string {
    switch (status) {
      case ReviewStatusEnum.NeedsCorrection:
        return 'profileOverview.reviewStatus.needsCorrection';
      case ReviewStatusEnum.Rejected:
        return 'profileOverview.reviewStatus.rejected';
      case ReviewStatusEnum.Approved:
        return 'profileOverview.reviewStatus.approved';
      case ReviewStatusEnum.Pending:
      default:
        return 'profileOverview.reviewStatus.other';
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
    if (!section.notesCount) return this.translate.instant('profileOverview.reviewSteps.noNotes');
    return this.translate.instant('profileOverview.reviewSteps.notesCount', { count: section.notesCount });
  }
}
