import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';


@Injectable({ providedIn: 'root' })
export class LanguageService {
  constructor(private translate: TranslateService) {
    translate.addLangs(['en','ar']);
    translate.setDefaultLang('en');
    const browserLang = translate.getBrowserLang();
    translate.use(browserLang.match(/en|ar/) ? browserLang : 'en');
  }
  get current() { return this.translate.currentLang || this.translate.defaultLang; }
  use(lang: string) { this.translate.use(lang); }
}
