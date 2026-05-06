import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { Router, NavigationEnd, RouterLink } from '@angular/router';
import { filter } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Tooltip } from 'primeng/tooltip';
import { FormsModule } from '@angular/forms';
import { routes } from '../../../routes/routes';
import { AuthService } from '../../../core/auth/auth.service';
import { FaDirArrowDirective } from '../../../shared/directives/dir-arrow.directive';
import { MenuItem, Sidebar } from '../../admin/sidebar/sidebar.models';
import { Permissions } from '../../../core/constants/permissions';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { environment } from '../../../../environments/environment';
import { NavigationAuditService } from '../../../core/services/navigation-audit.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  imports: [CommonModule, TranslatePipe, Tooltip, FaDirArrowDirective, HasPermissionDirective, FormsModule, RouterLink]
})
export class SidebarComponent implements OnInit {
  private authService = inject(AuthService);
  private translate = inject(TranslateService);
  private navigationAudit = inject(NavigationAuditService);
  @Output() toggleSidebar = new EventEmitter<void>();

  isCollapsed = false;
  activeItem = '';

  menuItems: MenuItem[] = [...Sidebar.menuItems];
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
    if (!environment.production && !this.menuItems.some(i => i.key === 'notificationTester')) {
      this.menuItems.push({
        key: 'notificationTester',
        label: 'admin.sidebar.notificationTester',
        icon: 'hgi-notification-03',
        route: routes.portal.notificationTester,
        permission: Permissions.Roles.Manage
      });
    }

    this.highlightActive(this.router.url);

    this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.highlightActive(event.urlAfterRedirects);
      });
  }

  highlightActive(url: string) {
    const matched = this.menuItems.find(i => url.includes(i.route));
    this.activeItem = matched ? matched.key : '';
  }

  toggle() {
    this.isCollapsed = !this.isCollapsed;
    this.searchTerm = '';
    this.toggleSidebar.emit();
  }

  onSidebarItemClick(event: MouseEvent, item: MenuItem) {
    if (event.button !== 0 || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey) {
      return;
    }

    if (this.router.url !== item.route) {
      this.navigationAudit.logSidebarNavigation({
        menuKey: item.key,
        menuLabel: item.label,
        targetUrl: item.route,
        previousUrl: this.router.url
      });
    }

    this.activeItem = item.key;
  }

  logout() {
    this.navigationAudit.logSidebarNavigation({
      menuKey: 'logout',
      menuLabel: 'common.sidebar.logout',
      targetUrl: '/logout',
      previousUrl: this.router.url
    });

    this.authService.logout();
  }
}
