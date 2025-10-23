import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import {provideTranslateHttpLoader} from '@ngx-translate/http-loader';
import { routes } from './app.routes';
import {provideHttpClient} from '@angular/common/http';
import {provideTranslateService} from '@ngx-translate/core';
import {MessageService} from 'primeng/api';
import {MSAL_INSTANCE, MsalService} from '@azure/msal-angular';
import {msalInstance} from './core/config/msal-config';


export const appConfig: ApplicationConfig = {
  providers: [
    { provide: MSAL_INSTANCE, useValue: msalInstance },
    MsalService,
    MessageService,
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    provideTranslateService({
      loader: provideTranslateHttpLoader({
        prefix: './i18n/',
        suffix: '.json'
      }),
      fallbackLang: 'en',
      lang: 'en'
    })
  ]
};
