import {Component, Input, TemplateRef, ChangeDetectionStrategy, inject, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { routes } from '../../../routes/routes';
import { LanguageService } from '../../../core/services/language.service';
import { AuthService } from '../../../core/auth/auth.service';
import { map, distinctUntilChanged, shareReplay } from 'rxjs/operators';
import { AvatarUtils } from '../../../core/utils/avatar-utils';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, TranslatePipe, RouterLink],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Navbar implements OnInit {
  routes = routes;
  @Input() menuTemplate: TemplateRef<any> | null | undefined;
  auth = inject(AuthService);
  public language = inject(LanguageService);
  protected authService = inject(AuthService);
  private router = inject(Router);
  userName: string | null = null;
  userAvatar: string = 'assets/images/default-avatar.png';
  defaultAvatar = AvatarUtils.default;
  showUserMenu: boolean = false;
  // expose observable for template
  readonly isLoggedIn$ = this.authService.isAuthenticated$.pipe(
    distinctUntilChanged(),
    shareReplay({ bufferSize: 1, refCount: true })
  );

  ngOnInit(): void {
    this.loadUserData();
  }

  loadUserData(): void {
    const user = this.auth.getCurrentUser();
    if (!user) {
      this.resetUserView();
      return;
    }

    this.userName = this.buildDisplayName(user.fullName);
    this.userAvatar = user.profilePictureUrl ?? AvatarUtils.build(this.userName);
  }

  private buildDisplayName(fullName?: string): string | null {
    if (!fullName) return null;
    const parts = fullName.trim().split(/\s+/);
    return parts.length === 1 ? parts[0] : `${parts[0]} ${parts.at(-1)}`;
  }

  private resetUserView(): void {
    this.userName = null;
    this.userAvatar = AvatarUtils.default;
  }

  toggleLanguage() {
    this.language.toggle();
  }

  toggleUserMenu(): void {
    this.showUserMenu = !this.showUserMenu;
  }

  logout() {
    this.authService.logout();
  }

  navigateToHome(event?: Event) {
    event?.preventDefault();
    this.router.navigateByUrl(this.routes.home).finally(() => window.location.reload());
  }
}
