import {Component, EventEmitter, OnDestroy, OnInit, Output, inject} from '@angular/core';
import {MessageService} from 'primeng/api';
import {TranslateService} from '@ngx-translate/core';
import {ProfileService} from '../../services/profile.service';
import {DataService} from '../../services/data.service';
import {ProfileLookupsService} from '../../services/profile-lookups.service';
import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';
import {createStepValiditySignal} from '../../state/profile-step-validity.signal';

@Component({
  selector: 'app-step-languages',
  templateUrl: './step-languages.component.html',
  styleUrls: ['./step-languages.component.scss'],
  standalone: false,
})
export class StepLanguagesComponent implements OnInit, OnDestroy {
  @Output() back = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  ds = inject(DataService);
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
  newLevel?: dropdownOptionsModel;

  ngOnInit(): void {
    const state = this.ds.state();
    const signature = this.buildSignature(state.languages);
    this.lastSubmittedSignature = null;
  }

  addLang(): void {
    if (this.newLanguage && this.newLevel) {
      this.ds.addLang({
        langId: this.newLanguage.id,
        lang: this.newLanguage,
        levelId: this.newLevel.id,
        level: this.newLevel
      });
      this.newLanguage = this.newLevel = undefined;
    }
  }

  removeLang(index: number){
    const lang = this.ds.state().languages[index];
    if(lang.id){
      this.profile.deleteLanguage(lang.id).subscribe({
        next: () => {
          this.ds.delLang(index);
        },
        error: err => {
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

    this.saving = true;
    this.profile.saveLanguagesSection(languages).subscribe({
      next: () => {
        this.saving = false;
        this.lastSubmittedSignature = signature;
        this.next.emit();
      },
      error: err => {
        console.error(err);
        this.saving = false;
        this.messageService.add({
          severity: 'error',
          summary: this.translate.instant('wizard.errorTitle'),
          detail: this.translate.instant('wizard.skills.saveError'),
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
        levelId: l.levelId ?? l.level?.id ?? l.level ?? null,
      }))
    );
  }

  ngOnDestroy(): void {}
}
