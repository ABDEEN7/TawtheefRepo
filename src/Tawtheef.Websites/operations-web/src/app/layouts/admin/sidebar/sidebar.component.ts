import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Tooltip } from 'primeng/tooltip';
import { FormsModule } from '@angular/forms';
import { routes } from '../../../routes/routes';
import { AuthService } from '../../../core/auth/auth.service';
import { FaDirArrowDirective } from '../../../shared/directives/dir-arrow.directive';
import { Permissions } from '../../../core/constants/permissions';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { MenuItem } from './sidebar.models';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  host: { 'data-test': 'sidebar-main' },
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  imports: [CommonModule, TranslatePipe, Tooltip, FaDirArrowDirective, HasPermissionDirective, FormsModule]
})
export class SidebarComponent implements OnInit {
  private authService = inject(AuthService);
  private translate = inject(TranslateService);
  @Output() toggleSidebar = new EventEmitter<void>();

  isCollapsed = false;
  activeItem = '';

  menuItems: MenuItem[] = [
    { key: 'home', label: 'admin.sidebar.home', icon: 'hgi-home-05', route: routes.dashboard('admin'), permission: Permissions.Dashboard.View },
    { key: 'users', label: 'admin.sidebar.users', icon: ' hgi-file-star', route: routes.portal.usersManagement, permission: Permissions.Users.Manage },
    { key: 'roles', label: 'admin.sidebar.roles', icon: 'hgi-security-validation', route: routes.portal.roleManagement, permission: Permissions.Roles.Manage },
    { key: 'home-content', label: 'admin.sidebar.homeContent', icon: 'hgi-settings-02', route: routes.portal.homeContentManagement, permission: Permissions.HomeContent.Manage },
    { key: 'systemAdminLogs', label: 'admin.sidebar.systemAdminLogs', icon: 'hgi-audit-01', route: routes.portal.systemAdminLogs, permission: Permissions.ProfileLogs.View },
  ];

  searchTerm: string = '';

  get filteredMenuItems(): MenuItem[] {
    if (!this.searchTerm) return this.menuItems;
    const term = this.searchTerm.toLowerCase();
    return this.menuItems.filter(item =>
      this.translate.instant(item.label).toLowerCase().includes(term)
    );
  }

  constructor(private router: Router) { }

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
    this.searchTerm = '';
    this.toggleSidebar.emit();
  }

  navigateTo(item: any) {
    this.activeItem = item.key;
    this.router.navigate([item.route]);
  }

  logout() {
    this.authService.logout();
  }

  private isMatchingRoute(url: string, route: string) {
    const normalizedUrl = url.split('?')[0];
    return normalizedUrl === route || normalizedUrl.startsWith(`${route}/`);
  }
}
