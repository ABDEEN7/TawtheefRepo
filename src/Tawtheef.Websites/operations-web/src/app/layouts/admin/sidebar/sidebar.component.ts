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
    { key: 'home', label: 'admin.sidebar.home', icon: 'assets/img/icons/home.svg', route: routes.dashboard('admin') },
    { key: 'roles', label: 'admin.sidebar.roles', icon: 'assets/img/icons/shield.svg', route: routes.admin.roleManagement },
    { key: 'users', label: 'admin.sidebar.users', icon: 'assets/img/icons/users.svg', route: routes.admin.usersManagement },
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
    this.router.navigate([routes.settings('admin')]);
  }

  logout() {
    this.authService.logout();
  }
}
