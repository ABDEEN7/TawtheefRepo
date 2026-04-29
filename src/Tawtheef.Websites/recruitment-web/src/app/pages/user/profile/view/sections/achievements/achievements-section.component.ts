import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AchievementDto, FileRefDto, ProfileStatusDto } from '../../../../../../core/models/auth/auth-response.model';
import { MyProfileReviewNoteDto, ReviewTargetTypeEnum } from '../../models/profile-overview.model';
import { FieldChange } from '../../utils/detect-change-fields';
import { TooltipModule } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { ProfileLookupsService } from '../../../wizard-profile/services/profile-lookups.service';
import { AchievementModal } from '../../../components/profile-steps/step-achievements/dialogs/achievement.modal';
import { Achievement } from '../../../wizard-profile/models/achievement.model';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';

@Component({
  selector: 'app-profile-achievements-section',
  standalone: true,
  imports: [CommonModule, TranslatePipe, TooltipModule],
  templateUrl: './achievements-section.component.html',
  styleUrls: ['./achievements-section.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileAchievementsSectionComponent {
  private readonly dialogService = inject(DialogService);
  private readonly translate = inject(TranslateService);
  private readonly profileService = inject(ProfileService);
  private readonly notify = inject(NotificationService);
  private readonly lookups = inject(ProfileLookupsService);
  private readonly fileUtils = inject(FileUtilsService);

  @Input() profile: ProfileStatusDto | null = null;
  @Input() canAddAttachment = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Input() changesRequest!: FieldChange[];
  @Input() isProfileApproved!: boolean;
  @Output() edit = new EventEmitter<void>();
  @Output() refresh = new EventEmitter<void>();

  protected achievementsUnderReview(){
    return this.normalizePendingItems().map(cr => {
      return {
        id: cr.Id,
        achievementTypeId: cr.AchievementTypeId,
        achievementType: this.lookups.achievementTypes().find(type => type.id === cr.AchievementTypeId),
        title: cr.Title,
        issuingAuthority: cr.IssuingAuthority,
        countryId: cr.CountryId,
        country: this.lookups.graduationCountry().find(c => c.id === cr.CountryId) ?? null,
        issueDate: cr.IssueDate,
        description: cr.Description,
        attachment: { resourceId: cr.AttachmentResourceId, fileName: cr.FileName } as FileRefDto,
        relatedToSpecialization: cr.RelatedToSpecialization,
      } as AchievementDto;
    });
  }

  protected noteForRaw(achievement: AchievementDto | null | undefined): MyProfileReviewNoteDto | null {
    if (!achievement) return null;
    const rowNote = achievement.id
      ? this.notes.find(
          note =>
            note.targetType === ReviewTargetTypeEnum.Row &&
            note.entityId?.toLowerCase() === achievement.id.toLowerCase()
        ) ?? null
      : null;

    const attachmentNote = achievement.attachment?.resourceId
      ? this.notes.find(
          note =>
            note.targetType === ReviewTargetTypeEnum.Attachment &&
            note.resourceId?.toLowerCase() === achievement.attachment?.resourceId.toLowerCase()
        ) ?? null
      : null;

    return rowNote ?? attachmentNote ?? null;
  }

  protected addAchievement() {
    this.dialogService
    .open(AchievementModal, {
      header: this.translate.instant('profileView.actions.addAchievement'),
      width: '50%',
      contentStyle: { 'max-height': '80vh', overflow: 'auto' },
      baseZIndex: 10000,
      closable: true,
    })
    ?.onClose.subscribe((achievement: Achievement | null) => {
      if (!achievement) return;
      this.profileService.saveAchievementsSection([achievement]).subscribe({
        next: () => {
          this.notify.success(this.translate.instant('profileView.notifications.saved'));
          this.refresh.emit();
        }
      });
    });
  }

  protected editAchievement(achievement: AchievementDto) {const achievementType =
    this.lookups.achievementTypes().find(type => type.id === achievement.achievementTypeId) ?? null;
    const initialValue = {
      achievementType,
      title: achievement.title ?? '',
      issuingAuthority: achievement.issuingAuthority ?? '',
      country: achievement.country ?? null,
      issueDate: achievement.issueDate ? new Date(achievement.issueDate) : null,
      description: achievement.description ?? '',
      fileName: achievement.attachment?.fileName ?? '',
      attachment: achievement.attachment
        ? {
            resourceId: achievement.attachment.resourceId,
            resourceName: achievement.attachment.fileName,
            url: achievement.attachment.url ?? null,
          }
        : null,
      relatedToSpecialization: achievement.relatedToSpecialization ?? null,
    };

    this.dialogService
      .open(AchievementModal, {
        header: this.translate.instant('profileView.actions.editAchievement'),
        width: '50%',
        contentStyle: { 'max-height': '80vh', overflow: 'auto' },
        baseZIndex: 10000,
        closable: true,
        data: {
          initialValue,
          disableFileUpload: true,
          initialId: achievement.id,
          attachmentId: achievement.attachment?.resourceId ?? null,
        },
      })
      ?.onClose.subscribe((result: Achievement | null) => {
      if (!result) return;
      this.profileService.saveAchievementsSection([result]).subscribe({
        next: () => {
          this.notify.success(this.translate.instant('profileView.notifications.saved'));
          this.refresh.emit();
        },
        error: () => {
          this.notify.error(this.translate.instant('profileView.notifications.saveFailed'));
        }
      });
    });
  }

  protected open(file: FileRefDto | null | undefined) {
    if (!file) return;
    this.fileUtils.previewUrl(file.url ?? '');
  }

  private normalizePendingItems(): any[] {
    const items = this.changesRequest.map(change => change.newValue).flatMap(value => {
      if (!value) return [];
      return Array.isArray(value) ? value : [value];
    });
    return items.filter(item => item?.AchievementTypeId || item?.Title);
  }
}
