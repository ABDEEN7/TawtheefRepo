import { BrowserModule } from '@angular/platform-browser';
import {APP_ID, APP_INITIALIZER, NgModule, provideZoneChangeDetection} from '@angular/core';
import { FormsModule } from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClient, provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';
import { ModalModule } from 'ngx-bootstrap/modal';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import {provideTranslateService, TranslateLoader, TranslateModule} from '@ngx-translate/core';
import {provideTranslateHttpLoader, TranslateHttpLoader} from '@ngx-translate/http-loader';
import { AppRoutingModule } from './app-routing.module';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    BrowserAnimationsModule,
    ModalModule.forRoot(),
    CoreModule,
    SharedModule,
    AppRoutingModule
  ],
  bootstrap: [AppComponent],
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    { provide: APP_ID, useValue: 'ng-cli-universal' },
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    provideHttpClient(withInterceptorsFromDi()),
    provideTranslateService({
      loader: provideTranslateHttpLoader({
        prefix: '/i18n/',
        suffix: '.json'
      }),
      fallbackLang: 'ar',
      lang: 'ar'
    })
  ]
})
export class AppModule { }
