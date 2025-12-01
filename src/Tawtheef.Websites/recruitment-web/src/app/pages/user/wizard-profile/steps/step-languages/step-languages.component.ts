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

  get step(){
    const stepValidity = createStepValiditySignal(this.ds.state);
    const validity = stepValidity();
    return validity['languages'];
  }

  newLanguage?: dropdownOptionsModel;
  newLevel?: dropdownOptionsModel;

  ngOnInit(): void {}

  addLang(): void {
    if (this.newLanguage && this.newLevel) {
      this.ds.addLang({
        langId: this.newLanguage.id,
        langName: this.newLanguage.name,
        levelId: this.newLevel.id,
        levelName: this.newLevel.name
      });
      this.newLanguage = this.newLevel = undefined;
    }
  }

  removeLang(index: number){
    this.ds.delLang(index);
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
    const skills = state.skills || [];
    const languages = state.languages || [];

    this.saving = true;
    this.profile.saveSkillsSection(skills, languages).subscribe({
      next: () => {
        this.saving = false;
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

  ngOnDestroy(): void {}
}
