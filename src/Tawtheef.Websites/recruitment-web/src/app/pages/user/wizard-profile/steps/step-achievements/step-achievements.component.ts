import {Component, EventEmitter, OnInit, Output, inject} from '@angular/core';
import {DataService} from '../../services/data.service';
import {DialogService} from 'primeng/dynamicdialog';
import {TranslateService} from '@ngx-translate/core';
import {ProfileService} from '../../services/profile.service';
import {FileUtilsService} from '../../../../../core/utils/file-utils';
import {createStepValiditySignal} from '../../state/profile-step-validity.signal';
import {Achievement} from '../../models/achievement.model';
import {AchievementModal, ACHIEVEMENT_DIALOG_LIMITS} from './dialogs/achievement.modal';
import {NotificationService} from '../../../../../core/services/notification.service';

@Component({
  selector: 'app-step-achievements',
  templateUrl: './step-achievements.component.html',
  styleUrl: './step-achievements.component.scss',
  standalone: false,
})
export class StepAchievementsComponent implements OnInit {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  ds = inject(DataService);
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
        error: err => {
          console.error(err);
          this.notify.error(
            `${this.translate.instant('wizard.errorTitle')}: ${this.translate.instant('wizard.achievements.deleteError')}`,
          );
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
        this.next.emit();
      },
      error: err => {
        console.error(err);
        this.saving = false;
        this.notify.error(
          `${this.translate.instant('wizard.errorTitle')}: ${this.translate.instant('wizard.achievements.saveError')}`,
        );
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
