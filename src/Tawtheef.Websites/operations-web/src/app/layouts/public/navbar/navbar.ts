import {Component, inject, Input, TemplateRef} from '@angular/core';
import {CommonModule} from '@angular/common';
import {routes} from '../../../routes/routes';
import {LanguageService} from '../../../core/services/language.service';
import {TranslatePipe} from '@ngx-translate/core';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, TranslatePipe],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
})
export class Navbar {
  routes = routes;
  @Input() menuTemplate: TemplateRef<any> | null | undefined;
  public language = inject(LanguageService);

  switchLang(lang: 'ar' | 'en') {
    this.language.set(lang);
  }
}
