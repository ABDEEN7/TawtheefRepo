import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { TranslatePipe} from '@ngx-translate/core';
import {Tooltip} from 'primeng/tooltip';
import {routes} from '../../../routes/routes';
import {AuthService} from '../../../core/auth/auth.service';
import {FaDirArrowDirective} from '../../../shared/directives/dir-arrow.directive';
import {MenuItem} from '../../admin/sidebar/sidebar.models';
import {Permissions} from '../../../core/constants/permissions';
import {HasPermissionDirective} from '../../../shared/directives/has-permission.directive';

@Component({
  selector: 'app-sidebar',
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
    { key: 'home', label: 'internal.sidebar.home', icon: 'hgi-home-05', route: routes.employee.dashboard, permission: Permissions.Dashboard.View },
    { key: 'distribution', label: 'internal.sidebar.distribution', icon: 'hgi-mail-send-02', route: routes.employee.profileDistribution, permission: Permissions.ProfileDistribution.View },
    { key: 'approve-profile', label: 'internal.sidebar.approve-profile', icon: 'hgi-task-done-01', route: routes.employee.approvalProfile, permission: Permissions.ProfileApproval.View },
    { key: 'job', label: 'internal.sidebar.job', icon: 'hgi-ai-beautify', route: routes.employee.JobList, permission: Permissions.Jobs.View },
    { key: 'invitation-summary', label: 'internal.sidebar.invitation-summary', icon: 'hgi-ai-setting', route: routes.employee.jobInvitationSummary, permission: Permissions.Nominations.View },
    { key: 'candidate-users', label: 'internal.sidebar.candidate-users', icon: 'hgi-user-switch', route: routes.employee.candidateUsersManagement, permission: Permissions.CandidateUsers.View },
    { key: 'office-users', label: 'internal.sidebar.office-users', icon: 'hgi-file-star', route: routes.employee.officeUsersManagement, permission: Permissions.OfficeUsers.View },
    { key: 'organization-structures', label: 'internal.sidebar.organization-structures', icon: 'hgi-structure-03', route: routes.employee.organizationStructures, permission: Permissions.OrganizationStructures.Manage },
    { key: 'major-skill', label: 'internal.sidebar.major-skill', icon: 'hgi-list-setting', route: routes.employee.majorsSkillsManagement, permission: Permissions.Nominations.View },
    { key: 'kawader', label: 'internal.sidebar.kawader', icon: ' hgi-user-multiple', route: routes.employee.kawader, permission: Permissions.Kawader.Manage }
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
    const matched = this.menuItems.find(i => url.includes(i.route));
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
    this.router.navigate([routes.settings('employee')]);
  }

  logout() {
    this.authService.logout();
  }
}
