import {
  ApplicationConfig,
  importProvidersFrom, inject, provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import {
  HttpBackend, HttpClient,
  provideHttpClient,
  withInterceptors,
} from '@angular/common/http';
import {provideTranslateService, TranslateLoader} from '@ngx-translate/core';
import {MessageService} from 'primeng/api';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import {provideAnimationsAsync} from '@angular/platform-browser/animations/async';
import {providePrimeNG} from 'primeng/config';
import {TawtheefPreset} from './shared/themes/twatheef-preset';
import {LanguageService} from './core/services/language.service';
import {authInterceptor,} from './core/interceptors/auth.interceptor';
import {loadingInterceptor} from './core/interceptors/loading.interceptor';
import {refreshInterceptor} from './core/interceptors/refresh.interceptor';
import {errorInterceptor} from './core/interceptors/error.interceptor';
import {customHttpInterceptor} from './core/interceptors/http.interceptor';
import {VersionedMultiTranslateLoader} from './core/http/version-translate.loader';
export function rootLoaderFactory(_httpBackend: HttpBackend) {
  const resources = [
    {prefix: '/i18n/primeng/', suffix: '.json'},
    {prefix: '/i18n/common/', suffix: '.json'},
    {prefix: '/i18n/server-error/', suffix: '.json'},
    {prefix: '/i18n/layout/', suffix: '.json'},
  ];
  return new VersionedMultiTranslateLoader(resources);
}
export const appConfig: ApplicationConfig = {
  providers: [
    MessageService,
    importProvidersFrom(NgbModule),
    provideBrowserGlobalErrorListeners(),
    provideAnimationsAsync(),
    providePrimeNG({
      theme: {
        preset: TawtheefPreset,
        options:{
          darkModeSelector: 'none'
        }
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
    ),
  ]
};
