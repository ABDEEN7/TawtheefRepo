import {Component, inject} from '@angular/core';
import {TranslatePipe} from '@ngx-translate/core';
import {LanguageService} from '../../../core/services/language.service';

@Component({
  selector: 'app-side-nav',
  templateUrl: './side-nav.html',
  styleUrl: './side-nav.scss',
})
export class SideNav {
  language = inject(LanguageService);
  switchLang(lang: 'ar'|'en'){
    this.language.set(lang);
  }

}
