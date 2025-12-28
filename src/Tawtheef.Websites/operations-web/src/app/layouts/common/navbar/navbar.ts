import {Component, inject, OnInit} from '@angular/core';
import {LanguageService} from '../../../core/services/language.service';
import {Router, RouterLink} from '@angular/router';
import {routes} from '../../../routes/routes';
import {AuthService} from '../../../core/auth/auth.service';
import {TranslatePipe} from '@ngx-translate/core';
import {I18nNamespaceDirective} from '../../../shared/directives/i18n-namespace.directive';
import {AvatarUtils} from '../../../core/utils/avatar-utils';

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
  userName: string| null = null;
  userAvatar: string = 'assets/images/default-avatar.png';
  notificationCount: number = 0;
  showUserMenu: boolean = false;

  protected readonly routes = routes;

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
    if (!user) {
      this.resetUserView();
      return;
    }

    this.userName = this.buildDisplayName(user.fullName);
    this.userAvatar = user.profilePictureUrl ?? AvatarUtils.build(this.userName);
    this.notificationCount = user.notifications ?? 0;
  }

  private buildDisplayName(fullName?: string): string | null {
    if (!fullName) return null;

    const parts = fullName.trim().split(/\s+/);
    return parts.length === 1
      ? parts[0]
      : `${parts[0]} ${parts.at(-1)}`;
  }

  private resetUserView(): void {
    this.userName = null;
    this.userAvatar = AvatarUtils.default;
    this.notificationCount = 0;
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
}
