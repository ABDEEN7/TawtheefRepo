import {
  ApplicationConfig,
  importProvidersFrom,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection
} from '@angular/core';
import {provideRouter} from '@angular/router';
import {routes} from './app.routes';
import {HttpBackend, provideHttpClient, withInterceptors} from '@angular/common/http';
import {provideTranslateService, TranslateLoader} from '@ngx-translate/core';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {MultiTranslateHttpLoader} from 'ngx-translate-multi-http-loader';
import {LanguageService} from './core/services/language.service';
import {authInterceptor} from './core/interceptors/auth.interceptor';
import {errorInterceptor} from './core/interceptors/error.interceptor';
import {loadingInterceptor} from './core/interceptors/loading.interceptor';
import {refreshInterceptor} from './core/interceptors/refresh.interceptor';
import {provideAnimationsAsync} from '@angular/platform-browser/animations/async';
import {providePrimeNG} from 'primeng/config';
import {TawtheefPreset} from './shared/themes/twatheef-preset';
import {customHttpInterceptor} from './core/interceptors/http.interceptor';
import {NotificationService} from './core/services/notification.service';

export function rootLoaderFactory(_httpBackend: HttpBackend) {
  return new MultiTranslateHttpLoader(_httpBackend, [
    {prefix: '/i18n/common/', suffix: '.json'},
    {prefix: '/i18n/server-error/', suffix: '.json'},
  ]);
}

export const appConfig: ApplicationConfig = {
  providers: [
    NotificationService,
    importProvidersFrom(NgbModule),
    provideBrowserGlobalErrorListeners(),
    provideAnimationsAsync(),
    providePrimeNG({
      theme: {
        preset: TawtheefPreset
      }
    }),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideTranslateService(),
    { provide: TranslateLoader, useFactory: rootLoaderFactory, deps: [HttpBackend] },
    provideAppInitializer(() => {
      const langSvc = inject(LanguageService);
      return langSvc.init();
    }),
    provideHttpClient(
      withInterceptors([
        errorInterceptor,
        refreshInterceptor,
        authInterceptor,
        customHttpInterceptor,
        loadingInterceptor,
      ])
    )
  ]
};
