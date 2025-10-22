import { NgModule, Optional, SkipSelf } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

// Services
import { AuthService } from './auth/auth.service';
import { EndpointsService } from './http/endpoints.service';
import { HttpService } from './http/http.service';
import { FileService } from './services/file.service';
import { KeyboardService } from './services/keyboard.service';
import { LanguageService } from './services/language.service';
import { LoadingService } from './services/loading.service';
import { LoggerService } from './services/logger.service';
import { NotificationService } from './services/notification.service';
import { TranslateService } from './services/translate.service';

// Interceptors
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { ErrorInterceptor } from './interceptors/error.interceptor';
import { LoadingInterceptor } from './interceptors/loading.interceptor';
import { RefreshInterceptor } from './interceptors/refresh.interceptor';

@NgModule({
  providers: [
    AuthService,
    EndpointsService,
    HttpService,
    FileService,
    KeyboardService,
    LanguageService,
    LoadingService,
    LoggerService,
    NotificationService,
    TranslateService,

    // Interceptors
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: RefreshInterceptor, multi: true }
  ]
})
export class CoreModule {
  // Guard against multiple imports
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    if (parentModule) {
      throw new Error(
        'CoreModule is already loaded. Import it in the AppModule only.'
      );
    }
  }
}
