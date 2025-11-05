import {Component, inject, signal} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {TranslateService} from '@ngx-translate/core';
import {LanguageService} from './core/services/language.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  currentLang: 'ar' | 'en' = 'ar';
  protected readonly title = signal('recruitment-web');
  private translate = inject(TranslateService);
  private language = inject(LanguageService);

  constructor() {
    this.translate.addLangs(['ar', 'en']);
    this.translate.setFallbackLang('ar');
    this.translate.use('ar');
    this.language.set(this.translate.getCurrentLang() as 'ar' | 'en');
  }

  toggleLanguage(): void {
    this.currentLang = this.currentLang === 'ar' ? 'en' : 'ar';
    document.documentElement.lang = this.currentLang;
    document.documentElement.dir = this.currentLang === 'ar' ? 'rtl' : 'ltr';
  }
}
