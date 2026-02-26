import {Component, Input, TemplateRef, ChangeDetectionStrategy, inject} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { routes } from '../../../routes/routes';
import { LanguageService } from '../../../core/services/language.service';
import { AuthService } from '../../../core/auth/auth.service';
import { map, distinctUntilChanged, shareReplay } from 'rxjs/operators';

@Component({
  selector: 'app-navbar',
  imports: [CommonModule, TranslatePipe, RouterLink],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Navbar {
  routes = routes;
  @Input() menuTemplate: TemplateRef<any> | null | undefined;
  public language = inject(LanguageService);
  protected authService = inject(AuthService);
  private router = inject(Router);
  // expose observable for template
  readonly isLoggedIn$ = this.authService.isAuthenticated$.pipe(
    distinctUntilChanged(),
    shareReplay({ bufferSize: 1, refCount: true })
  );

  toggleLanguage() {
    this.language.toggle();
  }

  logout() {
    this.authService.logout();
  }

  navigateToHome(event?: Event) {
    event?.preventDefault();
    this.router.navigateByUrl(this.routes.home).finally(() => window.location.reload());
  }
}
