import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { Tooltip } from 'primeng/tooltip';
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
  imports: [CommonModule, TranslatePipe, Tooltip, FaDirArrowDirective, HasPermissionDirective]
})
export class SidebarComponent implements OnInit {
  private authService = inject(AuthService);
  @Output() toggleSidebar = new EventEmitter<void>();

  isCollapsed = true;
  activeItem = '';

  menuItems: MenuItem[] = [
    { key: 'home', label: 'admin.sidebar.home', icon: 'hgi-home-05', route: routes.dashboard('admin'), permission: Permissions.Dashboard.View },
    { key: 'roles', label: 'admin.sidebar.roles', icon: 'hgi-security-validation', route: routes.admin.roleManagement, permission: Permissions.Roles.Manage },
    { key: 'profileLogs', label: 'admin.sidebar.profileLogs', icon: 'hgi-mail-send-02', route: routes.admin.profileLogs, permission: Permissions.ProfileLogs.View },
    { key: 'systemAdminLogs', label: 'admin.sidebar.systemAdminLogs', icon: 'hgi-audit-01', route: routes.admin.systemAdminLogs, permission: Permissions.ProfileLogs.View },
    { key: 'users', label: 'admin.sidebar.users', icon: ' hgi-file-star', route: routes.admin.usersManagement, permission: Permissions.Users.Manage },
    { key: 'offices', label: 'admin.sidebar.offices', icon: 'hgi-mail-send-02', route: routes.admin.officesManagement, permission: Permissions.Offices.Manage },
    { key: 'countries', label: 'admin.sidebar.countries', icon: 'hgi-globe-02', route: routes.admin.countriesManagement, permission: Permissions.Countries.Manage },
    { key: 'languages', label: 'admin.sidebar.languages', icon: 'hgi-arrow-data-transfer-horizontal', route: routes.admin.languagesManagement, permission: Permissions.Languages.Manage },
    { key: 'target-entities', label: 'admin.sidebar.targetEntities', icon: 'hgi-arrow-data-transfer-horizontal', route: routes.admin.targetEntitiesManagement, permission: Permissions.TargetEntities.Manage },
    { key: 'religions', label: 'admin.sidebar.religions', icon: 'hgi-structure-03', route: routes.admin.religionsManagement, permission: Permissions.Religions.Manage },
    { key: 'universities', label: 'admin.sidebar.universities', icon: ' hgi-university', route: routes.admin.universitiesManagement, permission: Permissions.Universities.Manage },
    { key: 'job-points-configuration', label: 'admin.sidebar.jobPointsConfig', icon: 'hgi-ai-beautify', route: routes.admin.jobPointsConfiguration, permission: Permissions.JobPoints.Manage },
    { key: 'job-category-candidate-settings', label: 'admin.sidebar.jobCategoryCandidateSettings', icon: 'hgi-settings-02', route: routes.admin.jobCategoryCandidateSettings, permission: Permissions.Jobs.Manage },
    { key: 'job-titles', label: 'admin.sidebar.jobTitles', icon: 'hgi-license-draft', route: routes.admin.jobTitlesManagement, permission: Permissions.Jobs.Manage },
    { key: 'home-content', label: 'admin.sidebar.homeContent', icon: 'hgi-settings-02', route: routes.admin.homeContentManagement, permission: Permissions.HomeContent.Manage },
  ];

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
