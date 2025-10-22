import { BrowserModule } from '@angular/platform-browser';
import { APP_ID, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClient, provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';
import { ModalModule } from 'ngx-bootstrap/modal';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {AuthInterceptor} from "./core/interceptors/auth.interceptor";
import {provideTranslateService} from "@ngx-translate/core";
import {provideTranslateHttpLoader} from "@ngx-translate/http-loader";
import {AppRoutingModule} from "./app-routing.module";
import {AuthModule} from "./features/auth/auth.module";

@NgModule({
    declarations: [
      AppComponent,
    ],
    bootstrap: [AppComponent],
    imports: [
      BrowserModule,
      FormsModule,
      BrowserAnimationsModule,
      ModalModule.forRoot(),
      AuthModule,
      AppRoutingModule
    ],
    providers: [
      { provide: APP_ID, useValue: 'ng-cli-universal' },
      { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
      provideHttpClient(withInterceptorsFromDi()),
      provideHttpClient(),
      provideTranslateService({
        loader: provideTranslateHttpLoader({
          prefix: '/assets/i18n/',
          suffix: '.json'
        }),
        fallbackLang: 'ar',
        lang: 'ar'
      })
    ]
})
export class AppModule { }
