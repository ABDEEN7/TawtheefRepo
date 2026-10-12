import { Component, EventEmitter, DestroyRef, inject, OnInit, Output } from '@angular/core';
import { Router, NavigationEnd, RouterLink } from '@angular/router';
import { filter } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Tooltip } from 'primeng/tooltip';
import { FormsModule } from '@angular/forms';
import { routes } from '../../../routes/routes';
import { AuthService } from '../../../core/auth/auth.service';
import { FaDirArrowDirective } from '../../../shared/directives/dir-arrow.directive';
import { isMenuGroup, MenuGroup, MenuItem, Sidebar, SidebarItem } from '../../admin/sidebar/sidebar.models';
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
  private readonly destroyRef = inject(DestroyRef);
  readonly isGroup = isMenuGroup;
  readonly expandedGroups = new Set<string>();
  @Output() toggleSidebar = new EventEmitter<void>();

  isCollapsed = false;
  activeItem = '';

  menuItems: SidebarItem[] = [...Sidebar.groupedMenuItems];
  searchTerm: string = '';

  get filteredMenuItems(): SidebarItem[] {
    const term = this.searchTerm.trim().toLowerCase();
    const matches = (item: SidebarItem) =>
      this.translate.instant(item.label).toLowerCase().includes(term);
    return this.menuItems.flatMap((item): SidebarItem[] => {
      if (!isMenuGroup(item)) {
        return this.authService.hasPermission(item.permission) && (!term || matches(item))
          ? [item] : [];
      }
      const children = item.children.filter((child) =>
        this.authService.hasPermission(child.permission) &&
        (!term || matches(item) || matches(child)),
      );
      return children.length ? [{ ...item, children }] : [];
    });
  }

  groupExpanded(group: MenuGroup): boolean {
    return !!this.searchTerm.trim() || this.expandedGroups.has(group.key);
  }

  toggleGroup(group: MenuGroup): void {
    if (this.isCollapsed) {
      this.toggle();
      this.expandedGroups.add(group.key);
    } else if (this.expandedGroups.has(group.key)) {
      this.expandedGroups.delete(group.key);
    } else {
      this.expandedGroups.add(group.key);
    }
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
      .pipe(filter(e => e instanceof NavigationEnd), takeUntilDestroyed(this.destroyRef))
      .subscribe((event: NavigationEnd) => {
        this.highlightActive(event.urlAfterRedirects);
      });
  }

  highlightActive(url: string): void {
    const path = url.split(/[?#]/)[0];
    let matched: MenuItem | undefined;
    let parent: MenuGroup | undefined;
    for (const item of this.menuItems) {
      for (const child of isMenuGroup(item) ? item.children : [item]) {
        if (this.authService.hasPermission(child.permission) &&
            (path === child.route || path.startsWith(`${child.route}/`)) &&
            (!matched || child.route.length > matched.route.length)) {
          matched = child;
          parent = isMenuGroup(item) ? item : undefined;
        }
      }
    }
    this.activeItem = matched?.key ?? '';
    if (parent) this.expandedGroups.add(parent.key);
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
