import {Component, input, Input, TemplateRef} from '@angular/core';
import {TranslateModule, TranslateService} from '@ngx-translate/core';
import {CommonModule} from '@angular/common';
import {routes} from '../../../routes/routes';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, TranslateModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
})
export class Navbar {
  routes = routes;
  @Input() menuTemplate: TemplateRef<any> | null | undefined;
  constructor(public translate: TranslateService) {}

  switchLang(lang: 'ar' | 'en') {
    this.translate.use(lang);
    localStorage.setItem('lang', lang);
    document.dir = lang === 'ar' ? 'rtl' : 'ltr';
  }
}
