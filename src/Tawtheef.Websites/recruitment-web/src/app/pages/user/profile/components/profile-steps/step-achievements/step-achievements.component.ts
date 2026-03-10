import {
  Component,
  EventEmitter,
  inject,
  Input,
  isDevMode,
  OnInit,
  Output,
  computed,
  input,
  output,
  ChangeDetectionStrategy,
  signal
} from '@angular/core';
import { CommonModule, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DialogService } from 'primeng/dynamicdialog';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { AchievementModal } from './dialogs/achievement.modal';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { ProfileDataService } from '../../../wizard-profile/services/profile-data.service';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { FileUtilsService } from '../../../../../../core/utils/file-utils';
import { Achievement } from '../../../wizard-profile/models/achievement.model';
import { FaDirArrowDirective } from '../../../../../../shared/directives/dir-arrow.directive';

@Component({
  selector: 'app-step-achievements',
  templateUrl: './step-achievements.component.html',
  styleUrl: './step-achievements.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    TableModule,
    ButtonModule,
    FaDirArrowDirective
  ]
})
export class StepAchievementsComponent implements OnInit {
  back = output<void>();
  next = output<void>();
  submitLabelKey = input<string>('wizard.buttons.next');
  showBack = input<boolean>(true);
  requireChanges = input<boolean>(false);

  ds = inject(ProfileDataService);
  dialog = inject(DialogService);
  translate = inject(TranslateService);
  notify = inject(NotificationService);
  profile = inject(ProfileService);
  fileUtils = inject(FileUtilsService);

  saving = signal(false);
  private lastSubmittedSignature: string | null = null;

  step = computed(() => this.ds.stepValidationDetailed().achievements);

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
      draggable: false,
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
      draggable: false,
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
          if (isDevMode())
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
    if (!this.step().valid) {
      this.notify.error(
        `${this.translate.instant('wizard.validationErrorTitle')}: ${this.step().errors
          .map(e => `* ${this.translate.instant(e.i18nKey)}`)
          .join('\n')}`,
      );
      return;
    }

    const achievements = this.ds.state().achievements || [];
    const signature = this.buildSignature(achievements);

    if (signature && signature === this.lastSubmittedSignature) {
      if (this.requireChanges() || this.ds.hasUnsolvedCorrections(7)) {
        const msg = this.ds.hasUnsolvedCorrections(7)
          ? 'يجب عمل التعديلات المذكورة في ملاحظات المراجع'
          : this.translate.instant('profileView.notifications.noChanges');
        this.notify.error(msg);
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

    this.saving.set(true);
    this.profile.saveAchievementsSection(achievements).subscribe({
      next: () => {
        this.saving.set(false);
        this.lastSubmittedSignature = signature;
        this.ds.markStepSubmitted('achievements');
        if (this.profile.isChangeRequestMode()) {
          this.notify.success(this.translate.instant('profileView.notifications.changeRequestSent'));
        }
        this.next.emit();
      },
      error: (err: any) => {
        if (isDevMode())
          console.error(err);
        this.saving.set(false);
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
