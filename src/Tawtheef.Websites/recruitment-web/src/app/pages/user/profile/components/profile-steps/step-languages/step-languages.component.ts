import {Component, EventEmitter, inject, Input, isDevMode, OnDestroy, OnInit, Output} from '@angular/core';
import {TranslateService} from '@ngx-translate/core';
import {ProfileDataService} from '../../../wizard-profile/services/profile-data.service';
import {ProfileLookupsService} from '../../../wizard-profile/services/profile-lookups.service';
import {ProfileService} from '../../../wizard-profile/services/profile.service';
import {createStepValiditySignal} from '../../../wizard-profile/state/profile-step-validity.signal';
import {dropdownOptionsModel} from '../../../../../../shared/models/dropdown-options.model';
import {NotificationService} from '../../../../../../core/services/notification.service';

@Component({
  selector: 'app-step-languages',
  templateUrl: './step-languages.component.html',
  styleUrls: ['./step-languages.component.scss'],
  standalone: false
})
export class StepLanguagesComponent implements OnInit, OnDestroy {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();
  @Input() submitLabelKey = 'wizard.buttons.next';
  @Input() showBack = true;
  @Input() requireChanges = false;

  ds = inject(ProfileDataService);
  lookups = inject(ProfileLookupsService);
  notificationService = inject(NotificationService);
  translate = inject(TranslateService);
  profile = inject(ProfileService);

  saving = false;
  private lastSubmittedSignature: string | null = null;

  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['languages'];
  }

  newLanguage?: dropdownOptionsModel;
  newSpeakingLevel?: dropdownOptionsModel;
  newWritingLevel?: dropdownOptionsModel;
  newReadingLevel?: dropdownOptionsModel;

  ngOnInit(): void {
    const state = this.ds.state();
    const signature = this.buildSignature(state.languages);
    this.lastSubmittedSignature = null;
  }

  addLang(): void {
    if (this.newLanguage && this.newSpeakingLevel && this.newWritingLevel && this.newReadingLevel) {
      if(this.ds.state().languages.some(s=> s.lang?.backendName == this.newLanguage?.backendName)) {
        this.notificationService.error(this.translate.instant('wizard.profile.languages.duplicate'));
        return;
      }

      this.ds.addLang({
        langId: this.newLanguage.id,
        lang: this.newLanguage,
        speakingLevelId: this.newSpeakingLevel.id,
        speakingLevel: this.newSpeakingLevel,
        writingLevelId: this.newWritingLevel.id,
        writingLevel: this.newWritingLevel,
        readingLevelId: this.newReadingLevel.id,
        readingLevel: this.newReadingLevel,
      });
      this.newLanguage = this.newSpeakingLevel = this.newWritingLevel = this.newReadingLevel = undefined;
    }
  }

  removeLang(index: number){
    const lang = this.ds.state().languages[index];
    if(lang.id){
      this.profile.deleteLanguage(lang.id).subscribe({
        next: () => {
          this.ds.delLang(index);
        }
      });
    } else {
      this.ds.delLang(index);
    }
  }

  onNext() {
    if (!this.step.valid) {
      this.notificationService.error(this.step.errors.map(e => `* ${this.translate.instant(e.i18nKey)}`).join('\n'));
      return;
    }

    const state = this.ds.state();
    const languages = state.languages || [];
    const signature = this.buildSignature(languages);

    if (signature && signature === this.lastSubmittedSignature) {
      if (this.requireChanges) {
        this.notificationService.error(this.translate.instant('profileView.notifications.noChanges'));
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

    this.saving = true;
    this.profile.saveLanguagesSection(languages).subscribe({
      next: () => {
        this.saving = false;
        this.lastSubmittedSignature = signature;
        if (this.profile.isChangeRequestMode()) {
          this.notificationService.success(this.translate.instant('profileView.notifications.changeRequestSent'));
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

  ngOnDestroy(): void {}
}
