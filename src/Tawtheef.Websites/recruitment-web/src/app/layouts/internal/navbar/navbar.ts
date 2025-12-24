import {Component, inject, OnInit} from '@angular/core';
import {LanguageService} from '../../../core/services/language.service';
import {Router, RouterLink, RouterLinkActive} from '@angular/router';
import {NgIf} from '@angular/common';
import {routes} from '../../../routes/routes';
import {AuthService} from '../../../core/auth/auth.service';
import {TranslatePipe} from '@ngx-translate/core';

@Component({
  selector: 'app-nav',
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
  imports: [
    RouterLink,
    RouterLinkActive,
    NgIf,
    TranslatePipe
  ]
})
export class Navbar implements OnInit{
  auth = inject(AuthService);
  language = inject(LanguageService);
  router = inject(Router);
  isLoggedIn: boolean = false;
  isProfileCompleted: boolean = false;
  userName: string | null = null;
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
    this.isProfileCompleted = this.auth.isProfileCompleted;
  }

  loadUserData(): void {
    const user = this.auth.getCurrentUser();
    if (!user) {
      this.resetUserView();
      return;
    }

    this.userName = this.buildDisplayName(user.fullName);
    this.userAvatar = user.profilePictureUrl ?? this.buildAvatar(this.userName);
    this.notificationCount = user.notifications ?? 0;
  }

  private buildDisplayName(fullName?: string): string | null {
    if (!fullName) return null;

    const parts = fullName.trim().split(/\s+/);
    return parts.length === 1
      ? parts[0]
      : `${parts[0]} ${parts.at(-1)}`;
  }

  private buildAvatar(userName: string | null): string {
    return userName
      ? `https://api.dicebear.com/7.x/initials/svg?seed=${encodeURIComponent(userName)}`
      : 'https://placehold.co/30';
  }

  private resetUserView(): void {
    this.userName = null;
    this.userAvatar = 'https://placehold.co/30';
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
