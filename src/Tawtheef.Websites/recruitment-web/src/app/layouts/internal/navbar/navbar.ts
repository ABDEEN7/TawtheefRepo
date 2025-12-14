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
    this.isProfileCompleted = this.auth.isProfileCompleted;
  }

  loadUserData(): void {
    const user = this.auth.getCurrentUser();
    this.userName = user!.firstName + ' ' + user!.lastName;
    this.userAvatar = user?.profilePictureUrl || this.userAvatar;
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
