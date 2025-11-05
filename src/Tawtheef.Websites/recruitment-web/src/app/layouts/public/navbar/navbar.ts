import {Component, input, Input, TemplateRef} from '@angular/core';
import {TranslateModule, TranslateService} from '@ngx-translate/core';
import {CommonModule} from '@angular/common';
import {routes} from '../../../routes/routes';
import {LanguageService} from '../../../core/services/language.service';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, TranslateModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
})
export class Navbar {
  routes = routes;
  @Input() menuTemplate: TemplateRef<any> | null | undefined;
  constructor(public language: LanguageService) {}

  switchLang(lang: 'ar' | 'en') {
    this.language.set(lang);
  }
}
