import {Component, inject, OnInit} from '@angular/core';
import {LanguageService} from '../../../core/services/language.service';
import {Router, RouterLink} from '@angular/router';
import {routes} from '../../../routes/routes';
import {AuthService} from '../../../core/auth/auth.service';
import {TranslatePipe} from '@ngx-translate/core';

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
  }

  loadUserData(): void {
    this.userName = 'أحمد محمد المحمود';
    this.notificationCount = 3;
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
