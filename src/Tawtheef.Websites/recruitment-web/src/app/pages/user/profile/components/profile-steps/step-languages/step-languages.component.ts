import {
  Component,
  EventEmitter,
  inject,
  Input,
  isDevMode,
  OnDestroy,
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
import { SelectModule } from 'primeng/select';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { FaDirArrowDirective } from '../../../../../../shared/directives/dir-arrow.directive';
import { ProfileDataService } from '../../../wizard-profile/services/profile-data.service';
import { ProfileLookupsService } from '../../../wizard-profile/services/profile-lookups.service';
import { ProfileService } from '../../../wizard-profile/services/profile.service';
import { dropdownOptionsModel, DropdownOptionVM } from '../../../../../../shared/models/dropdown-options.model';
import { NotificationService } from '../../../../../../core/services/notification.service';
import { Tooltip } from 'primeng/tooltip';

@Component({
  selector: 'app-step-languages',
  templateUrl: './step-languages.component.html',
  styleUrls: ['./step-languages.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    SelectModule,
    ButtonModule,
    TableModule,
    FaDirArrowDirective,
    Tooltip,
    ConfirmDialogModule
  ]
})
export class StepLanguagesComponent implements OnInit, OnDestroy {
  back = output<void>();
  next = output<void>();
  submitLabelKey = input<string>('wizard.buttons.next');
  showBack = input<boolean>(true);
  requireChanges = input<boolean>(false);

  ds = inject(ProfileDataService);
  lookups = inject(ProfileLookupsService);
  notificationService = inject(NotificationService);
  translate = inject(TranslateService);
  profile = inject(ProfileService);
  confirmationService = inject(ConfirmationService);

  saving = signal(false);
  private lastSubmittedSignature: string | null = null;

  step = computed(() => this.ds.stepValidationDetailed().languages);

  newLanguage = signal<DropdownOptionVM | undefined>(undefined);
  newSpeakingLevel = signal<DropdownOptionVM | undefined>(undefined);
  newWritingLevel = signal<DropdownOptionVM | undefined>(undefined);
  newReadingLevel = signal<DropdownOptionVM | undefined>(undefined);

  ngOnInit(): void {
    const state = this.ds.state();
    this.lastSubmittedSignature = this.buildSignature(state.languages);
  }

  addLang(): void {
    const langVal = this.newLanguage();
    const speakingVal = this.newSpeakingLevel();
    const writingVal = this.newWritingLevel();
    const readingVal = this.newReadingLevel();

    if (langVal && speakingVal && writingVal && readingVal) {
      if (this.ds.state().languages.some(s => s.lang?.backendName == langVal.backendName)) {
        this.notificationService.error(this.translate.instant('wizard.profile.languages.duplicate'));
        return;
      }

      this.ds.addLang({
        langId: langVal.id,
        lang: langVal,
        speakingLevelId: speakingVal.id,
        speakingLevel: speakingVal,
        writingLevelId: writingVal.id,
        writingLevel: writingVal,
        readingLevelId: readingVal.id,
        readingLevel: readingVal,
      });
      this.newLanguage.set(undefined);
      this.newSpeakingLevel.set(undefined);
      this.newWritingLevel.set(undefined);
      this.newReadingLevel.set(undefined);
    }
  }

  removeLang(index: number) {
    this.confirmDelete(() => this.deleteLang(index));
  }

  private deleteLang(index: number) {
    const lang = this.ds.state().languages[index];
    if (lang.id) {
      this.profile.deleteLanguage(lang.id).subscribe({
        next: () => {
          this.ds.delLang(index);
        }
      });
    } else {
      this.ds.delLang(index);
    }
  }

  private confirmDelete(accept: () => void): void {
    this.confirmationService.confirm({
      header: this.translate.instant('wizard.buttons.delete'),
      message: this.translate.instant('profileView.confirmDeleteRow'),
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: this.translate.instant('common.yes'),
      rejectLabel: this.translate.instant('common.no'),
      acceptButtonStyleClass: 'p-button-danger',
      rejectButtonStyleClass: 'p-button-text',
      accept,
    });
  }

  onNext() {
    if (this.saving()) return;

    if (!this.step().valid) {
      this.notificationService.error(this.step().errors.map(e => `* ${this.translate.instant(e.i18nKey)}`).join('\n'));
      return;
    }

    const state = this.ds.state();
    const languages = state.languages || [];
    const signature = this.buildSignature(languages);

    if (signature && signature === this.lastSubmittedSignature && this.ds.isStepSubmitted('languages')) {
      if (this.requireChanges() || this.ds.hasUnsolvedCorrections(9)) {
        const msg = this.ds.hasUnsolvedCorrections(9)
          ? 'يجب عمل التعديلات المذكورة في ملاحظات المراجع'
          : this.translate.instant('profileView.notifications.noChanges');
        this.notificationService.error(msg);
        return;
      }
      this.notificationService.info(this.translate.instant('profileView.notifications.noChanges'));
      this.next.emit();
      return;
    }

    if (!languages.length) {
      this.notificationService.error(this.translate.instant('wizard.profile.languages.required'));
      return;
    }

    this.saving.set(true);
    this.profile.saveLanguagesSection(languages).subscribe({
      next: () => {
        this.saving.set(false);
        this.lastSubmittedSignature = signature;
        this.ds.markStepSubmitted('languages');
        if (this.profile.isChangeRequestMode()) {
          this.notificationService.success(this.translate.instant('profileView.notifications.changeRequestSent'));
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

  private buildSignature(languages: any[]): string {
    return JSON.stringify(
      (languages ?? []).map(l => ({
        id: l.id ?? null,
        languageId: l.langId ?? l.languageId ?? l.id ?? null,
        speakingLevelId: l.speakingLevelId ?? l.speakingLevel?.id ?? l.levelId ?? l.level?.id ?? l.level ?? null,
        writingLevelId: l.writingLevelId ?? l.writingLevel?.id ?? l.levelId ?? l.level?.id ?? l.level ?? null,
        readingLevelId: l.readingLevelId ?? l.readingLevel?.id ?? l.levelId ?? l.level?.id ?? l.level ?? null,
      }))
    );
  }

  ngOnDestroy(): void { }
}
