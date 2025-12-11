import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService} from '@ngx-translate/core';
import {Tooltip} from 'primeng/tooltip';
import {routes} from '../../../routes/routes';
import {AuthService} from '../../../core/auth/auth.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  imports: [CommonModule, TranslatePipe, Tooltip]
})
export class SidebarComponent implements OnInit {
  private authService = inject(AuthService);
  @Output() toggleSidebar = new EventEmitter<void>();

  isCollapsed = true;
  activeItem = '';

  menuItems = [
    { key: 'home', label: 'internal.sidebar.home', icon: 'assets/img/icons/home.svg', route: routes.employee.dashboard },
    { key: 'distribution', label: 'internal.sidebar.distribution', icon: 'assets/img/icons/files.svg', route: routes.employee.profileDistribution },
    { key: 'approve-job', label: 'internal.sidebar.approve-job', icon: 'assets/img/icons/approve.svg', route: routes.employee.approvalJob },
    { key: 'approve-profile', label: 'internal.sidebar.approve-profile', icon: 'assets/img/icons/approve.svg', route: routes.employee.approvalProfile },
    { key: 'job', label: 'internal.sidebar.job', icon: 'assets/img/icons/job.svg', route: routes.employee.jobCreate },
    { key: 'transfer', label: 'internal.sidebar.transfer', icon: 'assets/img/icons/transfer.svg', route: routes.employee.nominations }
  ];

  constructor(private router: Router) {}

  ngOnInit(): void {
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
