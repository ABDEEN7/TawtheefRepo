import {Component, inject, OnInit} from '@angular/core';
import {LanguageService} from '../../../core/services/language.service';
import {Router, RouterLink} from '@angular/router';
import {routes} from '../../../routes/routes';
import {AuthService} from '../../../core/auth/auth.service';
import {TranslatePipe} from '@ngx-translate/core';
import {I18nNamespaceDirective} from '../../../shared/directives/i18n-namespace.directive';

@Component({
  selector: 'app-nav',
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
  imports: [
    RouterLink,
    TranslatePipe
  ]
})
export class Navbar implements OnInit{
  auth = inject(AuthService);
  language = inject(LanguageService);
  router = inject(Router);
  isLoggedIn: boolean = false;
  userName: string = '';
  userAvatar: string = 'assets/images/default-avatar.png';
  notificationCount: number = 0;
  showUserMenu: boolean = false;

  ngOnInit(): void {
    this.checkAuthStatus();
    if (this.isLoggedIn) {
      this.loadUserData();
    }
  }

  checkAuthStatus(): void {
    this.isLoggedIn = this.auth.isAuthenticated;
  }

  loadUserData(): void {
    const user = this.auth.getCurrentUser();
    const nameParts = user?.fullName.split(' ');
    this.userName =  nameParts? nameParts[0] + ' ' + (nameParts.length > 1 ? nameParts[nameParts.length - 1] : '') : '';
    this.userAvatar = user?.profilePictureUrl || `https://api.dicebear.com/7.x/initials/svg?seed=${this.userName}`;
    this.notificationCount = user?.notifications || 0;
  }

  toggleLanguage(): void {
    this.language.toggle();
  }

  toggleUserMenu(): void {
    this.showUserMenu = !this.showUserMenu;
  }

  login(): void {
    this.router.navigate([routes.auth.login]);
  }

  logout(): void {
    this.auth.logout();
  }

  protected readonly routes = routes;
}
