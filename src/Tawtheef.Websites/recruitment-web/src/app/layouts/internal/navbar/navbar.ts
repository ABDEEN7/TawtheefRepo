import {Component, inject, OnDestroy, OnInit} from '@angular/core';
import {LanguageService} from '../../../core/services/language.service';
import {Router, RouterLink, RouterLinkActive} from '@angular/router';
import {DatePipe, NgForOf, NgIf} from '@angular/common';
import {routes} from '../../../routes/routes';
import {AuthService} from '../../../core/auth/auth.service';
import {TranslatePipe} from '@ngx-translate/core';
import {AvatarUtils} from '../../../core/utils/avatar-utils';
import {InAppNotificationService} from '../../../core/services/in-app-notification.service';
import {NotificationModel} from '../../../shared/models/notification.model';
import {Subscription} from 'rxjs';
import {take} from 'rxjs/operators';

@Component({
  selector: 'app-nav',
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
  imports: [
    RouterLink,
    RouterLinkActive,
    NgIf,
    NgForOf,
    TranslatePipe,
    DatePipe
  ]
})
export class Navbar implements OnInit, OnDestroy {
  auth = inject(AuthService);
  language = inject(LanguageService);
  router = inject(Router);
  private readonly notificationsApi = inject(InAppNotificationService);
  isLoggedIn: boolean = false;
  isProfileCompleted: boolean = false;
  userName: string | null = null;
  userAvatar: string = 'assets/images/default-avatar.png';
  notificationCount: number = 0;
  showUserMenu: boolean = false;
  showNotificationMenu: boolean = false;
  notifications: NotificationModel[] = [];
  isLoadingNotifications: boolean = false;
  private readonly subscriptions = new Subscription();

  protected readonly routes = routes;

  ngOnInit(): void {
    this.checkAuthStatus();
    if (this.isLoggedIn) {
      this.loadUserData();
      this.refreshNotifications(false);
    }

    this.subscriptions.add(
      this.notificationsApi.notifications$.subscribe(list => {
        this.notifications = list;
        this.notificationCount = list.length;
      })
    );

    this.subscriptions.add(
      this.notificationsApi.isLoading$.subscribe(state => this.isLoadingNotifications = state)
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
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

  toggleNotificationMenu(): void {
    this.showNotificationMenu = !this.showNotificationMenu;
    if (this.showNotificationMenu && this.notifications.length === 0) {
      this.refreshNotifications();
    }
  }

  refreshNotifications(notifyOnError: boolean = true): void {
    if (!this.isLoggedIn) return;

    this.notificationsApi
      .refresh(10, notifyOnError)
      .pipe(take(1))
      .subscribe();
  }

  notificationTrackBy(index: number, notification: NotificationModel): string {
    return notification.id;
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
