import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService} from '@ngx-translate/core';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  imports: [CommonModule, TranslatePipe]
})
export class SidebarComponent implements OnInit {
  private translationService = inject(TranslateService);
  @Output() toggleSidebar = new EventEmitter<void>();
  
  isCollapsed = true;
  activeItem = '';
  
  menuItems = [
    { key: 'home', label: 'internal.sidebar.home', icon: 'assets/img/icons/home.svg', route: '/dashboard' },
    { key: 'files', label: 'internal.sidebar.files', icon: 'assets/img/icons/files.svg', route: '/facilities' },
    { key: 'approve', label: 'internal.sidebar.approve', icon: 'assets/img/icons/approve.svg', route: '/approval' },
    { key: 'job', label: 'internal.sidebar.job', icon: 'assets/img/icons/job.svg', route: '/jobs/create' },
    { key: 'transfer', label: 'internal.sidebar.transfer', icon: 'assets/img/icons/transfer.svg', route: '/nominations' }
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
    this.router.navigate(['/settings']);
  }

  logout() {
    if (confirm(this.translationService.instant('internal.sidebar.logout.confirm'))) {
      localStorage.clear();
      this.router.navigate(['/auth/login']);
    }
  }
}