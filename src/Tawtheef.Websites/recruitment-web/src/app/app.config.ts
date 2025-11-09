import {
  ApplicationConfig,
  importProvidersFrom,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import {HttpBackend, provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';
import {provideTranslateService, TranslateLoader, TranslateModule} from '@ngx-translate/core';
import {MessageService} from 'primeng/api';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import {provideAnimationsAsync} from '@angular/platform-browser/animations/async';
import {providePrimeNG} from 'primeng/config';
import {TawtheefPreset} from './shared/themes/twatheef-preset';
import {MultiTranslateHttpLoader} from 'ngx-translate-multi-http-loader';
export function HttpLoaderFactory(_httpBackend: HttpBackend) {
  return new MultiTranslateHttpLoader(_httpBackend, [
    {prefix: '/i18n/common/', suffix: '.json'},
    {prefix: '/i18n/layout/', suffix: '.json'},
    {prefix: '/i18n/pages/auth/', suffix: '.json'},
    {prefix: '/i18n/pages/error/', suffix: '.json'},
    {prefix: '/i18n/pages/home/', suffix: '.json'},
    {prefix: '/i18n/pages/user/wizard-profile/', suffix: '.json'},
    {prefix: '/i18n/pages/user/candidate-dashboard/', suffix: '.json'},
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
