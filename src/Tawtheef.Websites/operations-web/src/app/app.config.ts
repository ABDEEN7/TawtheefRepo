import {
  ApplicationConfig,
  importProvidersFrom,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import {HttpBackend, provideHttpClient} from '@angular/common/http';
import {provideTranslateService, TranslateLoader} from '@ngx-translate/core';
import {MessageService} from 'primeng/api';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {MultiTranslateHttpLoader} from 'ngx-translate-multi-http-loader';

export function HttpLoaderFactory(_httpBackend: HttpBackend) {
  return new MultiTranslateHttpLoader(_httpBackend, [
    {prefix: '/i18n/common/', suffix: '.json'},
    {prefix: '/i18n/pages/auth/', suffix: '.json'},
    {prefix: '/i18n/pages/profile-list/', suffix: '.json'}
  ]);
}

export const appConfig: ApplicationConfig = {
  providers: [
    importProvidersFrom(NgbModule),
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    provideTranslateService({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpBackend]
      },
      fallbackLang: 'ar',
      lang: 'ar'
    }),
    MessageService,
  ]
};
