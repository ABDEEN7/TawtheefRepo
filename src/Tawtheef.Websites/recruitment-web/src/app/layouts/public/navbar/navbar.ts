import {Component, Input, TemplateRef} from '@angular/core';
import {CommonModule} from '@angular/common';
import {routes} from '../../../routes/routes';
import {LanguageService} from '../../../core/services/language.service';
import {TranslatePipe} from '@ngx-translate/core';
import {AuthService} from '../../../core/auth/auth.service';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, TranslatePipe, RouterLink],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
})
export class Navbar {
  routes = routes;
  isLoggedIn: boolean = false;
  @Input() menuTemplate: TemplateRef<any> | null | undefined;
  constructor(public language: LanguageService,
              protected authService: AuthService) {
    this.authService.isAuthenticated$.subscribe(loggedIn => {
      this.isLoggedIn = loggedIn;
    });
  }

  toggleLanguage() {
    this.language.toggle();
  }
  logout() {
    this.authService.logout();
  }
}
