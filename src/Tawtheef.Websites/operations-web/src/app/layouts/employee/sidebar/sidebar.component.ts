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
import { MenuItem, Sidebar } from '../../admin/sidebar/sidebar.models';
import { Permissions } from '../../../core/constants/permissions';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';

@Component({
  selector: 'app-sidebar',
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

  menuItems: MenuItem[] = Sidebar.menuItems;
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
    const matched = this.menuItems.find(i => url.includes(i.route));
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
}
