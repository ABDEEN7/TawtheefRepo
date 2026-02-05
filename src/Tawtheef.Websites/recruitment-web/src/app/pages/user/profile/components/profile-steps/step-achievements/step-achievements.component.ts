import {Component, EventEmitter, inject, Input, isDevMode, OnInit, Output} from '@angular/core';
import {DialogService} from 'primeng/dynamicdialog';
import {TranslateService} from '@ngx-translate/core';
import {ACHIEVEMENT_DIALOG_LIMITS, AchievementModal} from './dialogs/achievement.modal';
import {NotificationService} from '../../../../../../core/services/notification.service';
import {ProfileDataService} from '../../../wizard-profile/services/profile-data.service';
import {ProfileService} from '../../../wizard-profile/services/profile.service';
import {FileUtilsService} from '../../../../../../core/utils/file-utils';
import {createStepValiditySignal} from '../../../wizard-profile/state/profile-step-validity.signal';
import {Achievement} from '../../../wizard-profile/models/achievement.model';

@Component({
  selector: 'app-step-achievements',
  templateUrl: './step-achievements.component.html',
  styleUrl: './step-achievements.component.scss',
  standalone: false
})
export class StepAchievementsComponent implements OnInit {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();
  @Input() submitLabelKey = 'wizard.buttons.next';
  @Input() showBack = true;
  @Input() requireChanges = false;

  ds = inject(ProfileDataService);
  dialog = inject(DialogService);
  translate = inject(TranslateService);
  notify = inject(NotificationService);
  profile = inject(ProfileService);
  fileUtils = inject(FileUtilsService);

  saving = false;
  private lastSubmittedSignature: string | null = null;

  readonly limits = ACHIEVEMENT_DIALOG_LIMITS;

  get step() {
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['achievements'];
  }

  ngOnInit(): void {
    const signature = this.buildSignature(this.ds.state().achievements);
    this.lastSubmittedSignature = signature;
  }

  addAchievement() {
    this.dialog.open(AchievementModal, {
      header: this.translate.instant('wizard.achievements.add'),
      width: '60%',
      contentStyle: { 'max-height': '80vh', overflow: 'auto' },
      baseZIndex: 10000,
      closable: true,
    })?.onClose.subscribe((result: Achievement | null) => {
      if (result) {
        this.ds.addAchievement(result);
      }
    });
  }

  editAchievement(index: number) {
    const achievement = this.ds.state().achievements[index];
    this.dialog.open(AchievementModal, {
      header: this.translate.instant('wizard.achievements.edit'),
      width: '60%',
      contentStyle: { 'max-height': '80vh', overflow: 'auto' },
      baseZIndex: 10000,
      closable: true,
      data: { initialValue: achievement },
    })?.onClose.subscribe((result: Achievement | null) => {
      if (result) {
        this.ds.updateAchievement(index, result);
      }
    });
  }

  removeAchievement(index: number) {
    const achievement = this.ds.state().achievements[index];
    if (achievement?.id) {
      this.profile.deleteAchievement(achievement.id).subscribe({
        next: () => this.ds.delAchievement(index),
        error: (err: any) => {
          if(isDevMode())
            console.error(err);
        },
      });
    } else {
      this.ds.delAchievement(index);
    }
  }

  preview(achievement: Achievement, ev?: Event) {
    ev?.stopPropagation();
    if (achievement.file) {
      this.fileUtils.previewBlob(achievement.file);
      return;
    }

    if (achievement.attachment?.url) {
      this.fileUtils.previewUrl(achievement.attachment.url, achievement.attachment.resourceName || '', false);
    } else if (achievement.attachment?.file) {
      this.fileUtils.previewBlob(achievement.attachment.file);
    }
  }

  onNext() {
    if (!this.step.valid) {
      this.notify.error(
        `${this.translate.instant('wizard.validationErrorTitle')}: ${this.step.errors
          .map(e => `* ${this.translate.instant(e.i18nKey)}`)
          .join('\n')}`,
      );
      return;
    }

    const achievements = this.ds.state().achievements || [];
    const signature = this.buildSignature(achievements);

    if (signature && signature === this.lastSubmittedSignature) {
      if (this.requireChanges) {
        this.notify.error(this.translate.instant('profileView.notifications.noChanges'));
        return;
      }
      this.notify.info(this.translate.instant('profileView.notifications.noChanges'));
      this.next.emit();
      return;
    }

    if (!achievements.length) {
      this.notify.error(
        `${this.translate.instant('wizard.validationErrorTitle')}: ${this.translate.instant('wizard.achievements.validation.noRows')}`,
      );
      return;
    }

    this.saving = true;
    this.profile.saveAchievementsSection(achievements).subscribe({
      next: () => {
        this.saving = false;
        this.lastSubmittedSignature = signature;
        if (this.profile.isChangeRequestMode()) {
          this.notify.success(this.translate.instant('profileView.notifications.changeRequestSent'));
        }
        this.next.emit();
      },
      error: (err: any) => {
        if(isDevMode())
          console.error(err);
        this.saving = false;
      },
    });
  }

  private buildSignature(achievements: Achievement[]): string {
    return JSON.stringify((achievements ?? []).map(a => ({
      id: a.id ?? null,
      achievementTypeId: a.achievementType?.id ?? null,
      title: a.title ?? '',
      issuingAuthority: a.issuingAuthority ?? '',
      countryId: a.country?.id ?? null,
      issueDate: a.issueDate ?? null,
      description: a.description ?? '',
      attachmentId: a.attachmentId ?? null,
      fileName: a.file?.name ?? a.attachment?.resourceName ?? null,
      relatedToSpecialization: a.relatedToSpecialization ?? null,
    })));
  }
}
