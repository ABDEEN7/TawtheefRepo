import {Component, EventEmitter, OnDestroy, OnInit, Output, inject} from '@angular/core';
import {MessageService} from 'primeng/api';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ProfileDataService} from '../../../wizard-profile/services/profile-data.service';
import {ProfileLookupsService} from '../../../wizard-profile/services/profile-lookups.service';
import {ProfileService} from '../../../wizard-profile/services/profile.service';
import {createStepValiditySignal} from '../../../wizard-profile/state/profile-step-validity.signal';
import {dropdownOptionsModel} from '../../../../../../shared/models/dropdown-options.model';

@Component({
  selector: 'app-step-languages',
  templateUrl: './step-languages.component.html',
  styleUrls: ['./step-languages.component.scss'],
  standalone: false
})
export class StepLanguagesComponent implements OnInit, OnDestroy {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  ds = inject(ProfileDataService);
  lookups = inject(ProfileLookupsService);
  messageService = inject(MessageService);
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
        },
        error: (err: any) => {
          console.error(err);
          this.messageService.add({
            severity: 'error',
            summary: this.translate.instant('wizard.errorTitle'),
            detail: this.translate.instant('wizard.language.deleteError'),
            life: 5000,
          });
        },
      });
    } else {
      this.ds.delLang(index);
    }
  }

  onNext() {
    if (!this.step.valid) {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('wizard.validationErrorTitle'),
        detail: this.step.errors.map(e => `* ${this.translate.instant(e.i18nKey)}`).join('\n'),
        life: 5000,
      });
      return;
    }

    const state = this.ds.state();
    const languages = state.languages || [];
    const signature = this.buildSignature(languages);

    if (signature && signature === this.lastSubmittedSignature) {
      this.next.emit();
      return;
    }

    if (!languages.length) {
      this.messageService.add({
        severity: 'error',
        summary: this.translate.instant('wizard.validationErrorTitle'),
        detail: this.translate.instant('wizard.profile.languages.required'),
        life: 5000,
      });
      return;
    }

    this.saving = true;
    this.profile.saveLanguagesSection(languages).subscribe({
      next: () => {
        this.saving = false;
        this.lastSubmittedSignature = signature;
        this.next.emit();
      },
      error: (err: any) => {
        console.error(err);
        this.saving = false;
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('wizard.errorTitle'),
          detail: this.translate.instant('wizard.languages.saveError'),
          life: 5000,
        });
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
