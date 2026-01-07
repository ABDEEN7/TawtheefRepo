import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AchievementDto, FileRefDto, ProfileStatusDto } from '../../../../../../core/models/auth/auth-response.model';
import { MyProfileReviewNoteDto, ReviewTargetTypeEnum } from '../../../overview/models/profile-overview.model';
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
  @Input() canEdit = false;
  @Input() notes: MyProfileReviewNoteDto[] = [];
  @Input() changesRequest!: FieldChange[];
  @Output() edit = new EventEmitter<void>();
  @Output() refresh = new EventEmitter<void>();

  protected fieldUnderReview(fieldKey: string = ''){
    return this.changesRequest.filter(c => c.field.toLowerCase() === fieldKey.toLowerCase()).length > 0;
  }
  protected onEdit() {
    this.edit.emit();
  }

  protected noteForFile(file: FileRefDto | null | undefined): MyProfileReviewNoteDto | null {
    if (!file?.resourceId) return null;
    return (
      this.notes.find(
        note =>
          note.targetType === ReviewTargetTypeEnum.Attachment &&
          note.resourceId?.toLowerCase() === file.resourceId.toLowerCase()
      ) ?? null
    );
  }

  protected addAchievement() {
    this.lookups.loadAll().subscribe(() => {
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
            },
            error: () => {
              this.notify.error(this.translate.instant('profileView.notifications.saveFailed'));
            }
          });
        });
    });
  }

  protected editAchievement(achievement: AchievementDto) {
    this.lookups.loadAll().subscribe(() => {
      const achievementType =
        this.lookups.achievementTypes().find(type => type.id === achievement.achievementTypeId) ?? null;
      const initialValue = {
        achievementType,
        title: achievement.title ?? '',
        issuingAuthority: achievement.issuingAuthority ?? '',
        country: achievement.country ?? null,
        issueDate: achievement.issueDate ? new Date(achievement.issueDate) : null,
        description: achievement.description ?? '',
        fileName: achievement.attachment?.fileName ?? '',
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
    });
  }

  protected replaceAchievementFile(achievement: AchievementDto, event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) return;

    const payload: Achievement = {
      id: achievement.id,
      achievementType: achievement.achievementTypeId ? { id: achievement.achievementTypeId } : null,
      title: achievement.title ?? '',
      issuingAuthority: achievement.issuingAuthority ?? '',
      country: achievement.country ?? null,
      issueDate: achievement.issueDate ?? '',
      description: achievement.description ?? '',
      relatedToSpecialization: achievement.relatedToSpecialization ?? null,
      file,
      fileName: file.name,
      attachmentId: achievement.attachment?.resourceId ?? null,
    };

    this.profileService.saveAchievementsSection([payload]).subscribe({
      next: () => {
        this.notify.success(this.translate.instant('profileView.notifications.saved'));
        this.refresh.emit();
      },
      error: () => {
        this.notify.error(this.translate.instant('profileView.notifications.saveFailed'));
      }
    });
  }

  protected open(file: FileRefDto | null | undefined) {
    if (!file) return;
    this.fileUtils.previewUrl(file.url ?? '');
  }
}
