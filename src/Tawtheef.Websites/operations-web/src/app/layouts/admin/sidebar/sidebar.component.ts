import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import {Tooltip} from 'primeng/tooltip';
import {routes} from '../../../routes/routes';
import {AuthService} from '../../../core/auth/auth.service';
import {FaDirArrowDirective} from '../../../shared/directives/dir-arrow.directive';
import {Permissions} from '../../../core/constants/permissions';
import {HasPermissionDirective} from '../../../shared/directives/has-permission.directive';
import {MenuItem} from './sidebar.models';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  host: { 'data-test': 'sidebar-main' },
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  imports: [CommonModule, TranslatePipe, Tooltip, FaDirArrowDirective, HasPermissionDirective]
})
export class SidebarComponent implements OnInit {
  private authService = inject(AuthService);
  @Output() toggleSidebar = new EventEmitter<void>();

  isCollapsed = true;
  activeItem = '';

  menuItems: MenuItem[] = [
    { key: 'home', label: 'admin.sidebar.home', icon: 'assets/img/icons/home.svg', route: routes.dashboard('admin'), permission: Permissions.Dashboard.View },
    { key: 'roles', label: 'admin.sidebar.roles', icon: 'assets/img/icons/shield.svg', route: routes.admin.roleManagement, permission: Permissions.Roles.Manage },
    { key: 'profileLogs', label: 'admin.sidebar.profileLogs', icon: 'assets/img/icons/file-eye.svg', route: routes.admin.profileLogs, permission: Permissions.ProfileLogs.View },
    { key: 'users', label: 'admin.sidebar.users', icon: 'assets/img/icons/users.svg', route: routes.admin.usersManagement, permission: Permissions.Users.Manage },
    { key: 'offices', label: 'admin.sidebar.offices', icon: 'assets/img/icons/files.svg', route: routes.admin.officesManagement, permission: Permissions.Offices.Manage },
    { key: 'countries', label: 'admin.sidebar.countries', icon: 'assets/img/icons/globe.svg', route: routes.admin.countriesManagement, permission: Permissions.Countries.Manage },
    { key: 'languages', label: 'admin.sidebar.languages', icon: 'assets/img/icons/transfer.svg', route: routes.admin.languagesManagement, permission: Permissions.Languages.Manage },
    { key: 'universities', label: 'admin.sidebar.universities', icon: 'assets/img/icons/university.svg', route: routes.admin.universitiesManagement, permission: Permissions.Universities.Manage },
  ];

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.highlightActive(this.router.url);

    this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.highlightActive(event.urlAfterRedirects);
      });
  }

  highlightActive(url: string) {
    const matched = this.menuItems.reduce<MenuItem | null>((best, item) => {
      if (!this.isMatchingRoute(url, item.route)) return best;

      if (!best || item.route.length > best.route.length) {
        return item;
      }

      return best;
    }, null);

    this.activeItem = matched ? matched.key : '';
  }

  toggle() {
    this.isCollapsed = !this.isCollapsed;
    this.toggleSidebar.emit();
  }

  navigateTo(item: any) {
    this.activeItem = item.key;
    this.router.navigate([item.route]);
  }

  openSettings() {
    this.activeItem = '';
    this.router.navigate([routes.settings('admin')]);
  }

  logout() {
    this.authService.logout();
  }

  private isMatchingRoute(url: string, route: string) {
    const normalizedUrl = url.split('?')[0];
    return normalizedUrl === route || normalizedUrl.startsWith(`${route}/`);
  }
}
