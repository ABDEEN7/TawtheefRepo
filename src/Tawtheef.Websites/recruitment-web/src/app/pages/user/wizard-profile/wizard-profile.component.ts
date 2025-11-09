import { Component, computed, inject } from '@angular/core';
import {DataService} from './services/data.service';
import {TranslateService} from '@ngx-translate/core';
import {LanguageService} from '../../../core/services/language.service';

@Component({
  selector: 'app-wizard-profile',
  templateUrl: './wizard-profile.component.html',
  styleUrls: ['./wizard-profile.component.scss'],
  standalone: false,
})
export class WizardProfileComponent {
  ds = inject(DataService);
  language = inject(LanguageService);

  step = 1;
  total = 7;

  progress = this.ds.progress;
  progressText = computed(() => this.progress() + '%');

  go(n: number){ if(n>=1 && n<=this.total) this.step = n; }
  next(){ if(this.step < this.total) this.step++; }
  prev(){ if(this.step > 1) this.step--; }

  toggleLang(){
    this.language.toggle();
  }
}
