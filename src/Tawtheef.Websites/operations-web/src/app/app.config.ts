import {
  ApplicationConfig,
  importProvidersFrom, inject, provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import {HTTP_INTERCEPTORS, HttpBackend, provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';
import {provideTranslateService, TranslateLoader} from '@ngx-translate/core';
import {MessageService} from 'primeng/api';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {MultiTranslateHttpLoader} from 'ngx-translate-multi-http-loader';
import {LanguageService} from './core/services/language.service';
import {AuthInterceptor} from './core/interceptors/auth.interceptor';
import {ErrorInterceptor} from './core/interceptors/error.interceptor';
import {LoadingInterceptor} from './core/interceptors/loading.interceptor';
import {RefreshInterceptor} from './core/interceptors/refresh.interceptor';
import {provideAnimationsAsync} from '@angular/platform-browser/animations/async';
import {providePrimeNG} from 'primeng/config';
import {TawtheefPreset} from './shared/themes/twatheef-preset';

export function rootLoaderFactory(_httpBackend: HttpBackend) {
  return new MultiTranslateHttpLoader(_httpBackend, [
    {prefix: '/i18n/common/', suffix: '.json'},
    {prefix: '/i18n/layout/', suffix: '.json'},
  ]);
}

export const appConfig: ApplicationConfig = {
  providers: [
    importProvidersFrom(NgbModule),
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptorsFromDi()),
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
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: RefreshInterceptor, multi: true },
    MessageService,
  ]
};
